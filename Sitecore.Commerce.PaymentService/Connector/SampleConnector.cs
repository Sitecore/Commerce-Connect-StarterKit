// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleConnector.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the SampleConnector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file  
// except in compliance with the License. You may obtain a copy of the License at 
//       http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed under the  
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,  
// either express or implied. See the License for the specific language governing permissions  
// and limitations under the License. 
// --------------------------------------------------------------------- 

namespace Sitecore.Commerce.PaymentService.Connector
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// SampleConnector class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "SAMPLE CODE ONLY")]
    public class SampleConnector : SampleProcessorIdentifier
    {
        #region Contants

        private const string Platform = "Portable";

        // Supported environments
        private const string EnvironmentConfigurable = "Configurable";

        // Default environment
        private const string DefaultEnvironment = EnvironmentConfigurable;

        /// <summary>
        /// The relative URI of the web service for getting payment accepting point.
        /// </summary>
        private const string GetPaymentAcceptPointRelativeUri = "/Payments/GetPaymentAcceptPoint";

        /// <summary>
        /// The relative URI of the web service for retrieving payment accepting result.
        /// </summary>
        private const string RetrievePaymentAcceptResultRelativeUri = "/Payments/RetrievePaymentAcceptResult";

        /// <summary>
        /// The padding character.
        /// </summary>
        private const char PaddingCharacter = '*';

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleConnector"/> class.
        /// </summary>
        public SampleConnector()
        {
        }

        #endregion

        #region IPaymentProcessor methods

        /// <summary>
        /// Authorize the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the authorize transaction.</param>
        /// <param name="requiredInteractionProperties">Properties required by authorization process.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        public Response Authorize(Request request, PaymentProperty[] requiredInteractionProperties)
        {
            string methodName = "Authorize";

            // Convert request
            AuthorizeRequest authorizeRequest = null;
            try
            {
                authorizeRequest = AuthorizeRequest.ConvertFrom(request);
            }
            catch (SampleException ex)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: null, properties: null, errors: ex.Errors);
            }

            // Validate merchant account
            List<PaymentError> errors = new List<PaymentError>();
            ValidateMerchantProperties(authorizeRequest, errors);
            if (errors.Count > 0)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, authorizeRequest.Locale, errors);
            }

            // Authorize and create response
            var authorizeResponse = new AuthorizeResponse(authorizeRequest.Locale, authorizeRequest.ServiceAccountId, this.Name);
            authorizeResponse.IsSwipe = authorizeRequest.IsSwipe;
            authorizeResponse.CardType = authorizeRequest.CardType;
            authorizeResponse.CardToken = authorizeRequest.CardToken;
            authorizeResponse.UniqueCardId = authorizeRequest.UniqueCardId;
            authorizeResponse.VoiceAuthorizationCode = authorizeRequest.VoiceAuthorizationCode;
            authorizeResponse.AccountType = authorizeRequest.AccountType;
            authorizeResponse.CurrencyCode = authorizeRequest.CurrencyCode;
            authorizeResponse.TransactionType = TransactionType.Authorize.ToString();
            authorizeResponse.TransactionDateTime = DateTime.UtcNow;

            if (authorizeRequest.CardNumber != null)
            {
                authorizeResponse.Last4Digit = GetLastFourDigits(authorizeRequest.CardNumber);
            }
            else if (authorizeRequest.Last4Digit != null)
            {
                authorizeResponse.Last4Digit = authorizeRequest.Last4Digit;
            }

            if (authorizeRequest.SupportCardTokenization && string.IsNullOrWhiteSpace(authorizeRequest.CardToken))
            {
                // Tokenize card
                authorizeResponse.CardToken = GetToken(authorizeRequest.CardNumber);
                authorizeResponse.UniqueCardId = Guid.NewGuid().ToString();
            }

            if (authorizeRequest.AuthorizationProviderTransactionId != null)
            {
                // Check for ReAuth
                authorizeResponse.ProviderTransactionId = authorizeRequest.AuthorizationProviderTransactionId;
            }
            else
            {
                authorizeResponse.ProviderTransactionId = Guid.NewGuid().ToString();
            }

            bool isApproved = ProcessAuthorizationResult(authorizeResponse, authorizeRequest);
            if (!isApproved)
            {
                errors.Add(new PaymentError(ErrorCode.Decline, "Declined"));
            }

            // Convert response and return
            Response response = AuthorizeResponse.ConvertTo(authorizeResponse);
            if (errors.Count > 0)
            {
                response.Errors = errors.ToArray();
            }

            PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
            return response;
        }

        /// <summary>
        /// Capture the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the Capture transaction.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        public Response Capture(Request request)
        {
            string methodName = "Capture";

            // Convert request
            CaptureRequest captureRequest = null;
            try
            {
                captureRequest = CaptureRequest.ConvertFrom(request);
            }
            catch (SampleException ex)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: null, properties: null, errors: ex.Errors);
            }

            // Validate merchant account
            List<PaymentError> errors = new List<PaymentError>();
            ValidateMerchantProperties(captureRequest, errors);
            if (errors.Count > 0)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, captureRequest.Locale, errors);
            }

            // Capture and create response
            var captureResponse = new CaptureResponse(captureRequest.Locale, captureRequest.ServiceAccountId, this.Name);
            captureResponse.CardType = captureRequest.CardType;
            captureResponse.CardToken = captureRequest.CardToken;
            captureResponse.Last4Digit = captureRequest.Last4Digit;
            captureResponse.UniqueCardId = captureRequest.UniqueCardId;
            captureResponse.CurrencyCode = captureRequest.CurrencyCode;
            captureResponse.TransactionType = TransactionType.Capture.ToString();
            captureResponse.TransactionDateTime = DateTime.UtcNow;
            captureResponse.ProviderTransactionId = Guid.NewGuid().ToString();

            bool isApproved = ProcessCaptureResult(captureResponse, captureRequest);
            if (!isApproved)
            {
                errors.Add(new PaymentError(ErrorCode.Decline, "Declined"));
            }

            // Convert response and return
            Response response = CaptureResponse.ConvertTo(captureResponse);
            if (errors.Count > 0)
            {
                response.Errors = errors.ToArray();
            }

            PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
            return response;
        }

        /// <summary>
        /// ImmediateCapture the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the ImmediateCapture transaction.</param>
        /// <param name="requiredInteractionProperties">Properties required by ImmediateCapture process.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        public Response ImmediateCapture(Request request, PaymentProperty[] requiredInteractionProperties)
        {
            var ex = new NotImplementedException("ImmediateCapture Not Implemented");
            throw ex;
        }

        /// <summary>
        /// Void the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the Void transaction.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        public Response Void(Request request)
        {
            string methodName = "Void";

            // Convert request
            VoidRequest voidRequest = null;
            try
            {
                voidRequest = VoidRequest.ConvertFrom(request);
            }
            catch (SampleException ex)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: null, properties: null, errors: ex.Errors);
            }

            // Validate merchant account
            List<PaymentError> errors = new List<PaymentError>();
            ValidateMerchantProperties(voidRequest, errors);
            if (errors.Count > 0)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, voidRequest.Locale, errors);
            }

            // Void and create response
            var voidResponse = new VoidResponse(voidRequest.Locale, voidRequest.ServiceAccountId, this.Name);
            voidResponse.CardType = voidRequest.CardType;
            voidResponse.Last4Digit = voidRequest.Last4Digit;
            voidResponse.UniqueCardId = voidRequest.UniqueCardId;
            voidResponse.CurrencyCode = voidRequest.CurrencyCode;
            voidResponse.TransactionType = TransactionType.Void.ToString();
            voidResponse.TransactionDateTime = DateTime.UtcNow;
            voidResponse.ProviderTransactionId = Guid.NewGuid().ToString();

            bool isApproved = ProcessVoidResult(voidResponse, voidRequest);
            if (!isApproved)
            {
                errors.Add(new PaymentError(ErrorCode.Decline, "Declined"));
            }

            // Convert response and return
            Response response = VoidResponse.ConvertTo(voidResponse);
            if (errors.Count > 0)
            {
                response.Errors = errors.ToArray();
            }

            PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
            return response;
        }

        /// <summary>
        /// Refund the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the Refund transaction.</param>
        /// <param name="requiredInteractionProperties">Properties required by Refund process.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Test code only")]
        public Response Refund(Request request, PaymentProperty[] requiredInteractionProperties)
        {
            string methodName = "Refund";

            // Convert request
            RefundRequest refundRequest = null;
            try
            {
                refundRequest = RefundRequest.ConvertFrom(request);
            }
            catch (SampleException ex)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, locale: null, properties: null, errors: ex.Errors);
            }

            // Validate merchant account
            List<PaymentError> errors = new List<PaymentError>();
            ValidateMerchantProperties(refundRequest, errors);
            if (errors.Count > 0)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, refundRequest.Locale, errors);
            }

            // Refund and create response
            var refundResponse = new RefundResponse(refundRequest.Locale, refundRequest.ServiceAccountId, this.Name);
            refundResponse.CardType = refundRequest.CardType;
            refundResponse.CardToken = refundRequest.CardToken;
            refundResponse.UniqueCardId = refundRequest.UniqueCardId;
            refundResponse.CurrencyCode = refundRequest.CurrencyCode;
            refundResponse.TransactionType = TransactionType.Refund.ToString();
            refundResponse.TransactionDateTime = DateTime.UtcNow;
            refundResponse.ProviderTransactionId = Guid.NewGuid().ToString();

            if (refundRequest.CardNumber != null)
            {
                refundResponse.Last4Digit = GetLastFourDigits(refundRequest.CardNumber);
            }
            else if (refundRequest.Last4Digit != null)
            {
                refundResponse.Last4Digit = refundRequest.Last4Digit;
            }

            if (refundRequest.SupportCardTokenization && string.IsNullOrWhiteSpace(refundRequest.CardToken))
            {
                // Tokenize card
                refundResponse.CardToken = GetToken(refundRequest.CardNumber);
                refundResponse.UniqueCardId = Guid.NewGuid().ToString();
            }

            bool isApproved = ProcessRefundResult(refundResponse, refundRequest);
            if (!isApproved)
            {
                errors.Add(new PaymentError(ErrorCode.Decline, "Declined"));
            }

            // Convert response and return
            Response response = RefundResponse.ConvertTo(refundResponse);
            if (errors.Count > 0)
            {
                response.Errors = errors.ToArray();
            }

            PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
            return response;
        }

        /// <summary>
        /// Reversal the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the Reversal transaction.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        public Response Reversal(Request request)
        {
            var ex = new NotImplementedException("Reversal Not Implemented");
            throw ex;
        }

        /// <summary>
        /// ReAuthorize the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the ReAuthorize transaction.</param>
        /// <param name="requiredInteractionProperties">Properties required by ReAuthorize process.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        public Response Reauthorize(Request request, PaymentProperty[] requiredInteractionProperties)
        {
            var ex = new NotImplementedException("Reauthorize Not Implemented");
            throw ex;
        }

        /// <summary>
        /// GenerateCardToken get the token for the requested credit card from the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the GenerateCardToken transaction.</param>
        /// <param name="requiredInteractionProperties">Properties required by GenerateCardToken process.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        public Response GenerateCardToken(Request request, PaymentProperty[] requiredInteractionProperties)
        {
            string methodName = "GenerateCardToken";

            // Convert request
            GenerateCardTokenRequest tokenizeRequest = null;
            try
            {
                tokenizeRequest = GenerateCardTokenRequest.ConvertFrom(request, requiredInteractionProperties);
            }
            catch (SampleException ex)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, ex.Errors);
            }

            // Validate merchant account
            List<PaymentError> errors = new List<PaymentError>();
            ValidateMerchantProperties(tokenizeRequest, errors);
            if (errors.Count > 0)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, tokenizeRequest.Locale, errors);
            }

            // Create response
            var tokenizeResponse = new GenerateCardTokenResponse(tokenizeRequest.Locale, tokenizeRequest.ServiceAccountId, this.Name);
            tokenizeResponse.CardType = tokenizeRequest.CardType;
            tokenizeResponse.Last4Digit = GetLastFourDigits(tokenizeRequest.CardNumber);
            tokenizeResponse.CardToken = GetToken(tokenizeRequest.CardNumber);
            tokenizeResponse.UniqueCardId = Guid.NewGuid().ToString();
            tokenizeResponse.ExpirationMonth = tokenizeRequest.ExpirationMonth;
            tokenizeResponse.ExpirationYear = tokenizeRequest.ExpirationYear;
            tokenizeResponse.Name = tokenizeRequest.Name;
            tokenizeResponse.StreetAddress = tokenizeRequest.StreetAddress;
            tokenizeResponse.StreetAddress2 = tokenizeRequest.StreetAddress2;
            tokenizeResponse.City = tokenizeRequest.City;
            tokenizeResponse.State = tokenizeRequest.State;
            tokenizeResponse.PostalCode = tokenizeRequest.PostalCode;
            tokenizeResponse.Country = tokenizeRequest.Country;
            tokenizeResponse.Phone = tokenizeRequest.Phone;
            tokenizeResponse.AccountType = tokenizeRequest.AccountType;
            tokenizeResponse.OtherCardProperties = tokenizeRequest.OtherCardProperties;

            // Convert response and return
            Response response = GenerateCardTokenResponse.ConvertTo(tokenizeResponse);
            PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
            return response;
        }

        /// <summary>
        /// GetPaymentAcceptPoint gets the payment accepting point from the payment provider, e.g. a payment page URL.
        /// </summary>
        /// <param name="request">Request object needed to process the GetPaymentAcceptPoint transaction.</param>
        /// <returns>Response object.</returns>
        public Response GetPaymentAcceptPoint(Request request)
        {
            string methodName = "GetPaymentAcceptPoint";

            // Convert request to validate
            GetPaymentAcceptPointRequest acceptPointRequest = null;
            Uri baseAddress = null;
            try
            {
                acceptPointRequest = GetPaymentAcceptPointRequest.ConvertFrom(request);
                baseAddress = GetPaymentAcceptBaseAddress(acceptPointRequest.Environment);
            }
            catch (SampleException ex)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, ex.Errors);
            }

            // Process and create response
            List<PaymentError> errors = new List<PaymentError>();
            var acceptPointResponse = new GetPaymentAcceptPointResponse(acceptPointRequest.Locale, acceptPointRequest.ServiceAccountId, this.Name);

            // Do not validate merchant account here, because the REST service will validate it.
            using (HttpClient client = new HttpClient())
            {
                // Serialize request content
                string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                HttpContent content = new StringContent(requestJson);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // Call REST service
                HttpResponseMessage httpResponse = null;
                client.BaseAddress = baseAddress;
                Task getPaymentAcceptPointTask = Task.Run(() => httpResponse = client.PostAsync(GetPaymentAcceptPointRelativeUri, content).Result);
                getPaymentAcceptPointTask.Wait();

                // Handle REST response
                if (httpResponse.IsSuccessStatusCode)
                {
                    // Read and deserialize response content
                    string responseJson = null;
                    Task readAsStringAsyncTask = Task.Run(() => responseJson = httpResponse.Content.ReadAsStringAsync().Result);
                    readAsStringAsyncTask.Wait();

                    Response getPaymentAcceptPointResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(responseJson);

                    // Prepare response properties
                    if (getPaymentAcceptPointResponse.Errors == null || getPaymentAcceptPointResponse.Errors.Length == 0)
                    {
                        // Read PaymentEntryUrl from reponse
                        var getPaymentAcceptPointResponseProperties = PaymentProperty.ConvertToHashtable(getPaymentAcceptPointResponse.Properties);
                        acceptPointResponse.PaymentAcceptUrl = GetPropertyStringValue(
                            getPaymentAcceptPointResponseProperties,
                            GenericNamespace.TransactionData,
                            TransactionDataProperties.PaymentAcceptUrl,
                            true);
                    }
                    else
                    {
                        errors.AddRange(getPaymentAcceptPointResponse.Errors);
                    }
                }
                else
                {
                    // GetPaymentAcceptPoint service failure
                    errors.Add(new PaymentError(ErrorCode.ApplicationError, "GetPaymentAcceptPoint failure: Internal server error."));
                }
            }

            // Convert response and return
            Response response = GetPaymentAcceptPointResponse.ConvertTo(acceptPointResponse);
            if (errors.Count > 0)
            {
                response.Errors = errors.ToArray();
            }

            PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
            return response;
        }

        /// <summary>
        /// RetrievePaymentAcceptResult retrieves the payment accepting result from the payment provider after the payment is processed externally.
        /// This method pairs with GetPaymentAcceptPoint.
        /// </summary>
        /// <param name="request">Request object needed to process the RetrievePaymentAcceptResult transaction.</param>
        /// <returns>Response object.</returns>
        public Response RetrievePaymentAcceptResult(Request request)
        {
            string methodName = "RetrievePaymentAcceptResult";

            // Convert request to validate
            RetrievePaymentAcceptResultRequest acceptResultRequest = null;
            Uri baseAddress = null;
            try
            {
                acceptResultRequest = RetrievePaymentAcceptResultRequest.ConvertFrom(request);
                baseAddress = GetPaymentAcceptBaseAddress(acceptResultRequest.Environment);
            }
            catch (SampleException ex)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, ex.Errors);
            }

            // Process and create response
            List<PaymentError> errors = new List<PaymentError>();
            var acceptResultResponse = new RetrievePaymentAcceptResultResponse(acceptResultRequest.Locale, acceptResultRequest.ServiceAccountId, this.Name);

            // Do not validate merchant account here, because the REST service will validate it.
            Response response = null;
            using (HttpClient client = new HttpClient())
            {
                // Serialize request content
                string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                HttpContent content = new StringContent(requestJson);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // Call REST service
                HttpResponseMessage httpResponse = null;
                client.BaseAddress = baseAddress;
                Task retrievePaymentAcceptResultTask = Task.Run(() => httpResponse = client.PostAsync(RetrievePaymentAcceptResultRelativeUri, content).Result);
                retrievePaymentAcceptResultTask.Wait();

                // Handle REST response
                if (httpResponse.IsSuccessStatusCode)
                {
                    // Read and deserialize response content
                    string responseJson = null;
                    Task readAsStringAsyncTask = Task.Run(() => responseJson = httpResponse.Content.ReadAsStringAsync().Result);
                    readAsStringAsyncTask.Wait();

                    Response retrievePaymentAcceptResultResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(responseJson);

                    // Return the retrieved response as is.
                    response = retrievePaymentAcceptResultResponse;
                }
                else
                {
                    // RetrievePaymentAcceptResult service failure
                    errors.Add(new PaymentError(ErrorCode.ApplicationError, "RetrievePaymentAcceptResult failure: Internal server error."));
                    response = RetrievePaymentAcceptResultResponse.ConvertTo(acceptResultResponse);
                }
            }

            // Return repsonse
            if (errors.Count > 0)
            {
                response.Errors = errors.ToArray();
            }

            PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
            return response;
        }

        /// <summary>
        /// ActivateGiftCard the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the ActivateGiftCard transaction.</param>
        /// <param name="requiredInteractionProperties">Properties required by ActivateGiftCard process.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented exception.</exception>
        public Response ActivateGiftCard(Request request, PaymentProperty[] requiredInteractionProperties)
        {
            var ex = new NotImplementedException("ActivateGiftCard NotImplemented");
            throw ex;
        }

        /// <summary>
        /// LoadGiftCard the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the LoadGiftCard transaction.</param>
        /// <param name="requiredInteractionProperties">Properties required by LoadGiftCard process.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented exception.</exception>
        public Response LoadGiftCard(Request request, PaymentProperty[] requiredInteractionProperties)
        {
            var ex = new NotImplementedException("LoadGiftCard NotImplemented");
            throw ex;
        }

        /// <summary>
        /// BalanceOnGiftCard the request with the payment provider.
        /// </summary>
        /// <param name="request">Request object needed to process the BalanceOnGiftCard transaction.</param>
        /// <param name="requiredInteractionProperties">Properties required by BalanceOnGiftCard process.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        /// <exception cref="System.NotImplementedException">Not implemented exception.</exception>
        public Response BalanceOnGiftCard(Request request, PaymentProperty[] requiredInteractionProperties)
        {
            var ex = new NotImplementedException("BalanceOnGiftCard NotImplemented");
            throw ex;
        }

        /// <summary>
        /// GetMerchantAccountPropertyMetadata returns the merchant account properties need by the payment provider.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        public Response GetMerchantAccountPropertyMetadata(Request request)
        {
            string methodName = "GetMerchantAccountPropertyMetadata";

            // Check null request
            List<PaymentError> errors = new List<PaymentError>();
            if (request == null)
            {
                errors.Add(new PaymentError(ErrorCode.InvalidRequest, "Request is null."));
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, errors);
            }

            // Prepare response
            List<PaymentProperty> properties = new List<PaymentProperty>();
            PaymentProperty property;
            property = new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.ServiceAccountId,
                Guid.NewGuid().ToString());
            property.SetMetadata("Service account ID:", "The organization subscription Id for  Microsoft Test Provider", false, true, 1);
            properties.Add(property);
            property = new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.MerchantId,
                TestData.MerchantId.ToString());
            property.SetMetadata("Merchant ID:", "The merchant ID received from Microsoft Test Provider", false, false, 2);
            properties.Add(property);
            property = new PaymentProperty(
                GenericNamespace.MerchantAccount,
                SampleMerchantAccountProperty.ProviderId,
                TestData.ProviderId.ToString());
            property.SetMetadata("Provider ID:", "The provider ID received from Microsoft Test Provider", false, false, 3);
            properties.Add(property);
            property = new PaymentProperty(
                GenericNamespace.MerchantAccount,
                SampleMerchantAccountProperty.Environment,
                DefaultEnvironment);
            property.SetMetadata("Environment:", "Variable that sets which Payment Accepting service it points to i.e. INT, PROD", false, false, 4);
            properties.Add(property);
            property = new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.SupportedCurrencies,
                TestData.SupportedCurrencies);
            property.SetMetadata("Supported Currencies:", "The supported currencies (ISO 4217) for the Microsoft Test Provider", false, false, 5);
            properties.Add(property);
            property = new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.SupportedTenderTypes,
                TestData.SupportedTenderTypes);
            property.SetMetadata("Supported Tender Types:", "The supported tender types for the Microsoft Test Provider", false, false, 6);
            properties.Add(property);

            Response response = new Response();
            response.Locale = request.Locale;
            response.Properties = properties.ToArray();
            if (errors.Count > 0)
            {
                response.Errors = errors.ToArray();
            }

            PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
            return response;
        }

        /// <summary>
        /// ValidateMerchantAccount the passed merchant account properties with the payment provider.
        /// </summary>
        /// <param name="request">Request object to validate.</param>
        /// <returns>
        /// Response object.
        /// </returns>
        public Response ValidateMerchantAccount(Request request)
        {
            string methodName = "ValidateMerchantAccount";

            // Convert request
            ValidateMerchantAccountRequest validateRequest = null;
            try
            {
                validateRequest = ValidateMerchantAccountRequest.ConvertFrom(request);
            }
            catch (SampleException ex)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, ex.Errors);
            }

            // Validate merchant account
            List<PaymentError> errors = new List<PaymentError>();
            ValidateMerchantProperties(validateRequest, errors);
            if (errors.Count > 0)
            {
                return PaymentUtilities.CreateAndLogResponseForReturn(methodName, this.Name, Platform, validateRequest.Locale, errors);
            }

            // Create response
            var validateResponse = new ValidateMerchantAccountResponse(validateRequest.Locale, validateRequest.ServiceAccountId, this.Name);

            // Convert response and return
            Response response = ValidateMerchantAccountResponse.ConvertTo(validateResponse);
            PaymentUtilities.LogResponseBeforeReturn(methodName, this.Name, Platform, response);
            return response;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Validates merchant account properties.
        /// </summary>
        /// <param name="requestBase">The request.</param>
        /// <param name="errors">The errors.</param>
        private static void ValidateMerchantProperties(RequestBase requestBase, List<PaymentError> errors)
        {
            /* 
             IMPORTANT!!!
             THIS IS SAMPLE CODE ONLY!  
             THE CODE SHOULD BE UPDATED TO VALIDATE MERCHANT FROM THE APPROPRIATE PAYMENT PROVIDERS.
            */
            if (!TestData.MerchantId.ToString("D").Equals(requestBase.MerchantId))
            {
                errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format(CultureInfo.InvariantCulture, "Invalid property value for {0}", MerchantAccountProperties.MerchantId)));
            }

            if (!TestData.ProviderId.ToString("D").Equals(requestBase.ProviderId))
            {
                errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format(CultureInfo.InvariantCulture, "Invalid property value for {0}", SampleMerchantAccountProperty.ProviderId)));
            }
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <returns>The token.</returns>
        private static string GetToken(string cardNumber)
        {
            /* 
             IMPORTANT!!!
             THIS IS SAMPLE CODE ONLY!  
             THE CODE SHOULD BE UPDATED TO OBTAIN THE TOKEN FROM THE APPROPRIATE PAYMENT PROVIDERS.
            */
            var encoder = new UTF8Encoding();
            byte[] bytes = encoder.GetBytes(cardNumber);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Sets the authorization result to the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="authorizeAmount">The authorize amount.</param>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        /// <param name="avsResult">The AVS result.</param>
        /// <param name="avsDetail">The AVS detail.</param>
        private static void SetAuthorizationResult(AuthorizeResponse response, decimal authorizeAmount, AuthorizationResult result, string message, AVSResult avsResult, AVSDetail avsDetail)
        {
            response.ApprovedAmount = authorizeAmount;
            response.AuthorizationResult = result.ToString();
            response.ProviderMessage = message;
            response.AVSResult = avsResult.ToString();
            response.AVSDetail = avsDetail.ToString();
        }

        /// <summary>
        /// Processes and gets the authorization result.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="request">The request.</param>
        /// <returns>The result indicating whether the authorization is approved.</returns>
        private static bool ProcessAuthorizationResult(AuthorizeResponse response, AuthorizeRequest request)
        {
            /* 
             IMPORTANT!!!
             THIS IS SAMPLE CODE ONLY!  
             THE CODE SHOULD BE UPDATED TO OBTAIN AUTHORIZATION RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
            */
            switch (request.Amount.ToString(CultureInfo.InvariantCulture))
            {
                // Authorization result
                case TestData.AuthorizationDeclined:
                    SetAuthorizationResult(response, 0M, AuthorizationResult.Failure, "Declined", AVSResult.None, AVSDetail.None);
                    break;
                case TestData.AuthorizationNone:
                    SetAuthorizationResult(response, 0M, AuthorizationResult.None, "Declined", AVSResult.Returned, AVSDetail.BillingAddress);
                    break;
                case TestData.AuthorizationReferral:
                    SetAuthorizationResult(response, 0M, AuthorizationResult.Referral, "Declined", AVSResult.Returned, AVSDetail.BillingAddress);
                    break;
                case TestData.AuthorizationPartialAuthorization:
                    SetAuthorizationResult(response, request.Amount - 0.05M, AuthorizationResult.PartialAuthorization, "Partial Approval", AVSResult.Returned, AVSDetail.BillingAddress);
                    break;
                case TestData.AuthorizationImmediateCaptureRequired:
                    SetAuthorizationResult(response, 0M, AuthorizationResult.ImmediateCaptureFailed, "Declined", AVSResult.None, AVSDetail.None);
                    break;

                // AVS Result & Detail
                case TestData.AVSReturnedBillingName:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.AccountholderName);
                    break;
                case TestData.AVSReturnedBillingAddress:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.BillingAddress);
                    break;
                case TestData.AVSReturnedBillingAndPostalCode:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.BillingAndPostalCode);
                    break;
                case TestData.AVSReturnedBillingPostalCode:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.BillingPostalCode);
                    break;
                case TestData.AVSReturnedNone:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.None);
                    break;
                case TestData.AVSNotReturned:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.NotReturned, AVSDetail.None);
                    break;
                case TestData.AVSNone:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.None, AVSDetail.None);
                    break;
                case TestData.AVSVerificationNotSupported:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.VerificationNotSupported, AVSDetail.None);
                    break;
                case TestData.AVSSystemUnavailable:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.SystemUnavailable, AVSDetail.None);
                    break;
                default:
                    SetAuthorizationResult(response, request.Amount, AuthorizationResult.Success, "Approved", AVSResult.Returned, AVSDetail.BillingAddress);
                    break;
            }

            bool isApproved = response.ApprovedAmount != 0M;
            if (isApproved)
            {
                ProcessCVV(response, request);
                response.ApprovalCode = "AB123456";
                response.ResponseCode = "00";
                if (request.CardType.Equals(CardType.Debit.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    response.AvailableBalance = 100M;
                    response.CashBackAmount = request.CashBackAmount;
                }
                else if (TestData.AuthorizationAvailableBalance.Equals(request.Amount.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase))
                {
                    response.AvailableBalance = 10M;
                }
                else
                {
                    response.AvailableBalance = 0M;
                }
            }
            else
            {
                response.CVV2Result = CVV2Result.NotProcessed.ToString();
                response.ResponseCode = "51";
                response.CashBackAmount = 0M;
            }

            return isApproved;
        }

        /// <summary>
        /// Processes and gets the capture result.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="request">The request.</param>
        /// <returns>The result indicating whether the capture is approved.</returns>
        private static bool ProcessCaptureResult(CaptureResponse response, CaptureRequest request)
        {
            /* 
             IMPORTANT!!!
             THIS IS SAMPLE CODE ONLY!  
             THE CODE SHOULD BE UPDATED TO OBTAIN CAPTURE RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
            */
            if (!AuthorizationResult.Success.ToString().Equals(request.AuthorizationResult, StringComparison.OrdinalIgnoreCase)
                && !AuthorizationResult.PartialAuthorization.ToString().Equals(request.AuthorizationResult, StringComparison.OrdinalIgnoreCase))
            {
                response.CaptureResult = CaptureResult.Failure.ToString();
                response.ProviderMessage = "Authorization result is not expected.";
                return false;
            }

            if (request.Amount > request.AuthorizationApprovedAmount)
            {
                response.CaptureResult = CaptureResult.Failure.ToString();
                response.ProviderMessage = "Capture amount cannot be greater than approved amount.";
                return false;
            }

            switch (request.Amount.ToString(CultureInfo.InvariantCulture))
            {
                case TestData.CaptureFailure:
                case TestData.CaptureFailureVoidFailure:
                    response.CaptureResult = CaptureResult.Failure.ToString();
                    response.ProviderMessage = "Declined";
                    break;
                case TestData.CaptureNone:
                    response.CaptureResult = CaptureResult.None.ToString();
                    response.ProviderMessage = "Unknown error has occurred";
                    break;
                case TestData.CaptureQueuedForBatch:
                    response.CaptureResult = CaptureResult.Success.ToString();
                    response.ProviderMessage = "Capture queued for batch";
                    break;
                default:
                    response.CaptureResult = CaptureResult.Success.ToString();
                    response.ProviderMessage = "Approved";
                    break;
            }

            bool isApproved = CaptureResult.Success.ToString().Equals(response.CaptureResult);
            if (isApproved)
            {
                response.ApprovalCode = "CC222222";
                response.ResponseCode = "00";
            }
            else
            {
                response.ResponseCode = "61";
            }

            return isApproved;
        }

        /// <summary>
        /// Processes and gets the void result.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="request">The request.</param>
        /// <returns>The result indicating whether the void is approved.</returns>
        private static bool ProcessVoidResult(VoidResponse response, VoidRequest request)
        {
            /* 
             IMPORTANT!!!
             THIS IS SAMPLE CODE ONLY!  
             THE CODE SHOULD BE UPDATED TO OBTAIN VOID RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
            */
            if (!AuthorizationResult.Success.ToString().Equals(request.AuthorizationResult, StringComparison.OrdinalIgnoreCase)
                && !AuthorizationResult.PartialAuthorization.ToString().Equals(request.AuthorizationResult, StringComparison.OrdinalIgnoreCase))
            {
                response.VoidResult = CaptureResult.Failure.ToString();
                response.ProviderMessage = "Authorization result is not expected.";
                return false;
            }

            switch (request.AuthorizationApprovedAmount.ToString(CultureInfo.InvariantCulture))
            {
                case TestData.VoidFailure:
                case TestData.CaptureFailureVoidFailure:
                    response.VoidResult = VoidResult.Failure.ToString();
                    response.ProviderMessage = "Declined";
                    break;
                case TestData.VoidNone:
                    response.VoidResult = VoidResult.None.ToString();
                    response.ProviderMessage = "Unknown error has occurred";
                    break;
                default:
                    response.VoidResult = VoidResult.Success.ToString();
                    response.ProviderMessage = "Approved";
                    break;
            }

            bool isApproved = VoidResult.Success.ToString().Equals(response.VoidResult);
            if (isApproved)
            {
                response.ResponseCode = "00";
            }
            else
            {
                response.ResponseCode = "81";
            }

            return isApproved;
        }

        /// <summary>
        /// Processes and gets the refund result.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="request">The request.</param>
        /// <returns>The result indicating whether the refund is approved.</returns>
        private static bool ProcessRefundResult(RefundResponse response, RefundRequest request)
        {
            /* 
             IMPORTANT!!!
             THIS IS SAMPLE CODE ONLY!  
             THE CODE SHOULD BE UPDATED TO OBTAIN REFUND RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
            */
            switch (request.Amount.ToString(CultureInfo.InvariantCulture))
            {
                case TestData.RefundFailure:
                    response.RefundResult = RefundResult.Failure.ToString();
                    response.ProviderMessage = "Declined";
                    response.ApprovedAmount = 0M;
                    break;
                case TestData.RefundQueueForBatch:
                    response.RefundResult = RefundResult.Success.ToString();
                    response.ProviderMessage = "Approved";
                    response.ApprovedAmount = request.Amount;
                    break;
                case TestData.RefundNone:
                    response.RefundResult = RefundResult.None.ToString();
                    response.ProviderMessage = "Unknown error has occurred";
                    response.ApprovedAmount = 0M;
                    break;
                default:
                    response.RefundResult = RefundResult.Success.ToString();
                    response.ProviderMessage = "Approved";
                    response.ApprovedAmount = request.Amount;
                    break;
            }

            bool isApproved = RefundResult.Success.ToString().Equals(response.RefundResult);
            if (isApproved)
            {
                response.ApprovalCode = "RR654321";
                response.ResponseCode = "00";
            }
            else
            {
                response.ResponseCode = "71";
            }

            return isApproved;
        }

        /// <summary>
        /// Processes the CVV result.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="request">The request.</param>
        /// <returns>The result indicating whether CVV result is success.</returns>
        private static bool ProcessCVV(AuthorizeResponse response, AuthorizeRequest request)
        {
            /* 
             IMPORTANT!!!
             THIS IS SAMPLE CODE ONLY!  
             THE CODE SHOULD BE UPDATED TO OBTAIN CVV RESULT FROM THE APPROPRIATE PAYMENT PROVIDERS.
            */
            string cvvValue = request.CardVerificationValue;
            bool isSuccess = false;
            switch (cvvValue)
            {
                // CVV2 result
                case TestData.CVV2Failure:
                    response.CVV2Result = CVV2Result.Failure.ToString();
                    break;
                case TestData.CVV2IssuerNotRegistered:
                    response.CVV2Result = CVV2Result.IssuerNotRegistered.ToString();
                    break;
                case TestData.CVV2NotProcessed:
                    response.CVV2Result = CVV2Result.NotProcessed.ToString();
                    break;
                case TestData.CVV2Unknown:
                    response.CVV2Result = CVV2Result.Unknown.ToString();
                    break;
                default:
                    response.CVV2Result = CVV2Result.Success.ToString();
                    isSuccess = true;
                    break;
            }

            return isSuccess;
        }

        private static string GetLastFourDigits(string cardNumber)
        {
            return cardNumber.Length < 4 ? cardNumber.PadLeft(4, PaddingCharacter) : cardNumber.Substring(cardNumber.Length - 4);
        }

        private static string GetPropertyStringValue(Dictionary<string, object> propertyHashtable, string propertyNamespace, string propertyName, bool throwExceptionIfNotFound)
        {
            string propertyValue;
            bool found = PaymentProperty.GetPropertyValue(
                            propertyHashtable,
                            propertyNamespace,
                            propertyName,
                            out propertyValue);
            if (!found && throwExceptionIfNotFound)
            {
                ThrowPropertyNotSetException(propertyName);
            }

            return propertyValue;
        }

        private static void ThrowPropertyNotSetException(string propertyName)
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Property '{0}' is null or not set", propertyName));
        }

        private static Uri GetPaymentAcceptBaseAddress(string requestEnvironment)
        {
            string paymentAcceptBaseAddress = null;
            if (string.IsNullOrWhiteSpace(requestEnvironment))
            {
                paymentAcceptBaseAddress = AppSettings.PaymentAcceptBaseAddress;
            }
            else
            {
                switch (requestEnvironment)
                {
                    case EnvironmentConfigurable:
                        paymentAcceptBaseAddress = AppSettings.PaymentAcceptBaseAddress;
                        break;
                    default:
                        var errors = new List<PaymentError>();
                        errors.Add(new PaymentError(ErrorCode.ApplicationError, "Environment is not valid."));
                        throw new SampleException(errors);
                }
            }

            if (!string.IsNullOrWhiteSpace(paymentAcceptBaseAddress))
            {
                return new Uri(paymentAcceptBaseAddress);
            }
            else
            {
                return null;
            }
        }

        private string GetAssemblyName()
        {
            string asemblyQualifiedName = this.GetType().AssemblyQualifiedName;
            int commaIndex = asemblyQualifiedName.IndexOf(',');
            return asemblyQualifiedName.Substring(commaIndex + 1).Trim();
        }

        #endregion
    }
}