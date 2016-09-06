// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the PaymentsController class.
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

namespace Sitecore.Commerce.PaymentService
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using Newtonsoft.Json;
    using Sitecore.Commerce.PaymentService.Connector;
    using System.Globalization;

    /// <summary>
    /// The web API controller for payments.
    /// </summary>
    public class PaymentsController : ApiController
    {
        /// <summary>
        /// Gets the accepting URL of the card payment page.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        [HttpPost]
        [ActionName("GetPaymentAcceptPoint")]
        public Response GetPaymentAcceptPoint([FromBody] Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.Properties == null)
            {
                throw new ArgumentException("request.Properties cannot be null.");
            }

            // Initialize response
            var errorList = new List<PaymentError>();
            var response = new Response()
            {
                Locale = request.Locale,
            };

            // Validate merchant information
            ValidateMerchantAccount(request, errorList);

            // Convert the request into a payment entry
            var cardPaymentEntry = ConvertRequestToCardPaymentEntry(request, errorList);

            // Create card payment entry if no errors.
            if (errorList.Count == 0)
            {
                var dataManager = new DataManager();
                dataManager.CreateCardPaymentEntry(cardPaymentEntry);

                // Prepare response properties
                List<PaymentProperty> responseProperties = new List<PaymentProperty>();
                string paymentAcceptUrlLeftPart = this.Url.Request.RequestUri.GetLeftPart(UriPartial.Authority); // e.g. https://localhost
                if (this.Url.Request.RequestUri.Segments != null && this.Url.Request.RequestUri.Segments.Length > 2)
                {
                    // Do not append the last two segments because they are controller name and action name e.g. Payments/GetPaymentAcceptPoint
                    for (int i = 0; i < this.Url.Request.RequestUri.Segments.Length - 2; i++)
                    {
                        paymentAcceptUrlLeftPart += this.Url.Request.RequestUri.Segments[i]; // e.g. https://localhost/PaymentAcceptSample/
                    }
                }

                string paymentAcceptUrl = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}CardPage.aspx?id={1}",
                        paymentAcceptUrlLeftPart,
                        cardPaymentEntry.EntryId);
                responseProperties.Add(
                    new PaymentProperty(
                                    GenericNamespace.TransactionData,
                                    TransactionDataProperties.PaymentAcceptUrl,
                                    paymentAcceptUrl));

                response.Properties = responseProperties.ToArray();
            }
            else
            {
                response.Errors = errorList.ToArray();
            }

            return response;
        }

        /// <summary>
        /// Retrieves the card payment accepting result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        [HttpPost]
        [ActionName("RetrievePaymentAcceptResult")]
        public Response RetrievePaymentAcceptResult([FromBody] Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.Properties == null)
            {
                throw new ArgumentException("request.Properties cannot be null.");
            }

            // Initialize response
            var errorList = new List<PaymentError>();
            var response = new Response()
            {
                Locale = request.Locale,
            };

            // Validate merchant information
            ValidateMerchantAccount(request, errorList);

            // Get result access code from request
            var requestProperties = PaymentProperty.ConvertToHashtable(request.Properties);
            string serviceAccountId = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.ServiceAccountId,
                required: true,
                errors: errorList);
            string resultAccessCode = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.TransactionData,
                TransactionDataProperties.PaymentAcceptResultAccessCode,
                required: true,
                errors: errorList);

            if (errorList.Count > 0)
            {
                response.Errors = errorList.ToArray();
                return response;
            }

            // Retrieve payment result if no errors
            var dataManager = new DataManager();
            CardPaymentResult cardPaymentResult = dataManager.GetCardPaymentResultByResultAccessCode(serviceAccountId, resultAccessCode);

            if (cardPaymentResult == null)
            {
                // Result does not exist.
                errorList.Add(new PaymentError(ErrorCode.InvalidRequest, "Invalid payment result access code."));
                response.Errors = errorList.ToArray();
                return response;
            }

            // Success
            response = JsonConvert.DeserializeObject<Response>(cardPaymentResult.ResultData);
            return response;
        }

        /// <summary>
        /// Converts a payment request to a card payment entry.
        /// </summary>
        /// <param name="request">The payment request.</param>
        /// <param name="errorList">The errors during conversion.</param>
        /// <returns>The card payment entry.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)", Justification = "By Design.")]
        private static CardPaymentEntry ConvertRequestToCardPaymentEntry(Request request, List<PaymentError> errorList)
        {
            string locale = request.Locale;
            if (string.IsNullOrWhiteSpace(locale))
            {
                errorList.Add(new PaymentError(ErrorCode.LocaleNotSupported, "Locale is not specified."));
            }

            var requestProperties = PaymentProperty.ConvertToHashtable(request.Properties);

            string serviceAccountId = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.ServiceAccountId,
                required: true,
                errors: errorList);

            string industryType = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.TransactionData,
                TransactionDataProperties.IndustryType,
                required: false,
                errors: errorList);
            IndustryType industryTypeEnum;
            if (string.IsNullOrEmpty(industryType) || !Enum.TryParse(industryType, true, out industryTypeEnum))
            {
                industryTypeEnum = IndustryType.Ecommerce;
            }

            string transactionType = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.TransactionData,
                TransactionDataProperties.TransactionType,
                required: true,
                errors: errorList);
            TransactionType transactionTypeEnum = TransactionType.None;
            if (!Enum.TryParse(transactionType, true, out transactionTypeEnum)
                || (transactionTypeEnum != TransactionType.None && transactionTypeEnum != TransactionType.Authorize && transactionTypeEnum != TransactionType.Capture))
            {
                errorList.Add(new PaymentError(ErrorCode.InvalidRequest, "The transaction type is not suppoted."));
            }

            string supportCardSwipeString = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.TransactionData,
                TransactionDataProperties.SupportCardSwipe,
                required: false,
                errors: errorList);
            bool supportCardSwipe = false;
            if (!string.IsNullOrWhiteSpace(supportCardSwipeString))
            {
                bool.TryParse(supportCardSwipeString, out supportCardSwipe);
            }

            string supportCardTokenizationString = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.TransactionData,
                TransactionDataProperties.SupportCardTokenization,
                required: false,
                errors: errorList);
            bool supportCardTokenization = false;
            if (!string.IsNullOrWhiteSpace(supportCardTokenizationString))
            {
                bool.TryParse(supportCardTokenizationString, out supportCardTokenization);
            }

            // When transaction type is None, support card tokenization must be enabled.
            if (transactionTypeEnum == TransactionType.None && !supportCardTokenization)
            {
                errorList.Add(new PaymentError(ErrorCode.InvalidRequest, "When transaction type is None, support card tokenization must be enabled."));
            }

            string allowVoiceAuthorizationString = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.TransactionData,
                TransactionDataProperties.AllowVoiceAuthorization,
                required: false,
                errors: errorList);
            bool allowVoiceAuthorization = false;
            if (!string.IsNullOrWhiteSpace(allowVoiceAuthorizationString))
            {
                bool.TryParse(allowVoiceAuthorizationString, out allowVoiceAuthorization);
            }

            string hostPageOrigin = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.TransactionData,
                TransactionDataProperties.HostPageOrigin,
                required: true,
                errors: errorList);
            if (string.IsNullOrWhiteSpace(hostPageOrigin))
            { 
                errorList.Add(new PaymentError(ErrorCode.InvalidRequest, "The host page origin is not specified."));
            }

            string cardTypes = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.CardType,
                required: false,
                errors: errorList);

            string defaultCardHolderName = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.Name,
                required: false,
                errors: errorList);

            string defaultStreet1 = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.StreetAddress,
                required: false,
                errors: errorList);

            string defaultStreet2 = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.StreetAddress2,
                required: false,
                errors: errorList);

            string defaultCity = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.City,
                required: false,
                errors: errorList);

            string defaultStateOrProvince = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.State,
                required: false,
                errors: errorList);

            string defaultPostalCode = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.PostalCode,
                required: false,
                errors: errorList);

            string defaultCountryCode = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.Country,
                required: false,
                errors: errorList);

            string showSameAsShippingAddressString = GetPropertyStringValue(
                requestProperties,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.ShowSameAsShippingAddress,
                required: false,
                errors: errorList);

            bool showSameAsShippingAddress = false;
            if (!string.IsNullOrWhiteSpace(showSameAsShippingAddressString))
            {
                bool.TryParse(showSameAsShippingAddressString, out showSameAsShippingAddress);
            }

            string entryData = JsonConvert.SerializeObject(request);

            // Create the request in database with an unique entry ID
            var cardPaymentEntry = new CardPaymentEntry()
            {
                AllowVoiceAuthorization = allowVoiceAuthorization,
                CardTypes = CardTypes.GetSupportedCardTypes(cardTypes),
                DefaultCardHolderName = defaultCardHolderName,
                DefaultCity = defaultCity,
                DefaultCountryCode = defaultCountryCode,
                DefaultPostalCode = defaultPostalCode,
                DefaultStateOrProvince = defaultStateOrProvince,
                DefaultStreet1 = defaultStreet1,
                DefaultStreet2 = defaultStreet2,
                EntryData = entryData,
                EntryId = CommonUtility.NewGuid().ToString(),
                EntryLocale = locale,
                EntryUtcTime = DateTime.UtcNow,
                HostPageOrigin = hostPageOrigin,
                IndustryType = industryTypeEnum.ToString(),
                ServiceAccountId = serviceAccountId,
                ShowSameAsShippingAddress = showSameAsShippingAddress,
                SupportCardSwipe = supportCardSwipe,
                SupportCardTokenization = supportCardTokenization,
                TransactionType = transactionType,
                Used = false,
            };
            return cardPaymentEntry;
        }

        /// <summary>
        /// Validates the merchant account.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="errors">The error list to add any validation errors.</param>
        private static void ValidateMerchantAccount(Request request, List<PaymentError> errors)
        {
            var processor = new SampleConnector();

            // Prepare a request for validating merchant account
            var validateMerchantAccountRequest = new Request();
            validateMerchantAccountRequest.Locale = request.Locale;
            var validateMerchantAccountRequestPropertyList = new List<PaymentProperty>();
            foreach (var paymentProperty in request.Properties)
            {
                if (paymentProperty.Namespace == GenericNamespace.MerchantAccount)
                {
                    validateMerchantAccountRequestPropertyList.Add(paymentProperty);
                }
            }

            validateMerchantAccountRequest.Properties = validateMerchantAccountRequestPropertyList.ToArray();

            // Validates the merchant account by calling the payment processor
            Response validateMerchantAccountResponse = processor.ValidateMerchantAccount(validateMerchantAccountRequest);

            if (validateMerchantAccountResponse != null)
            {
                if (validateMerchantAccountResponse.Errors != null)
                {
                    errors.AddRange(validateMerchantAccountResponse.Errors);
                }
            }
            else
            {
                var error = new PaymentError(ErrorCode.InvalidMerchantConfiguration, "Merchant configuraiton is invalid.");
                errors.Add(error);
            }
        }

        /// <summary>
        /// Gets the string value of a payment property.
        /// </summary>
        /// <param name="propertyHashtable">The property hashtable.</param>
        /// <param name="propertyNamespace">The namespace.</param>
        /// <param name="propertyName">The name.</param>
        /// <param name="required">The flag indicating whether the property is required.</param>
        /// <param name="errors">The error list in case the property is required but not found.</param>
        /// <returns>The string value.</returns>
        private static string GetPropertyStringValue(Dictionary<string, object> propertyHashtable, string propertyNamespace, string propertyName, bool required, List<PaymentError> errors)
        {
            string propertyValue;
            bool found = PaymentProperty.GetPropertyValue(
                            propertyHashtable,
                            propertyNamespace,
                            propertyName,
                            out propertyValue);
            if (!found && required)
            {
                var error = new PaymentError(ErrorCode.InvalidRequest, string.Format(CultureInfo.InvariantCulture, "Property '{0}' is null or not set", propertyName));
                errors.Add(error);
            }

            return propertyValue;
        }
    }
}
