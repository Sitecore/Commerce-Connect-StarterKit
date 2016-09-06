// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSamplePaymentServiceUrl.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetSamplePaymentServiceUrl class.
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

namespace Sitecore.Commerce.StarterKit.Pipelines
{
    using Sitecore.Commerce.PaymentService;
    using Sitecore.Commerce.PaymentService.Connector;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Payments;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Gets the URL to a payment service.
    /// </summary>
    public class GetSamplePaymentServiceUrl : SamplePaymentServicePipelineBase
    {
        private const string GetPaymentAcceptPointRelativeUri = "/Payments/GetPaymentAcceptPoint";

        /// <summary>
        /// Processes the service pipeline arguments.
        /// </summary>
        /// <param name="args">The service pipeline arguments.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentCondition(args.Request is GetPaymentServiceUrlRequest, "args.Request", "Request must be of type GetPaymentServiceUrlRequest.");
            Assert.ArgumentCondition(args.Result is GetPaymentServiceUrlResult, "args.Result", "Result must be of type GetPaymentServiceUrlResult.");
            var request = (GetPaymentServiceUrlRequest)args.Request;
            var result = (GetPaymentServiceUrlResult)args.Result;

            Assert.ArgumentNotNull(request.CardType, "request.CardType");
            Assert.ArgumentNotNull(request.TransactionType, "request.TransactionType");

            var serviceRequestProperties = new List<PaymentProperty>();
            serviceRequestProperties.AddRange(this.GetMerchantProperties() ?? Enumerable.Empty<PaymentProperty>());

            serviceRequestProperties.Add(new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.CardType,
                request.CardType.Name));
            serviceRequestProperties.Add(new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.ShowSameAsShippingAddress,
                request.ShowSameAsShippingAddress.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)));
            
            if (!string.IsNullOrWhiteSpace(request.StreetAddress))
            {
                serviceRequestProperties.Add(new PaymentProperty(
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.StreetAddress,
                    request.StreetAddress));
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                serviceRequestProperties.Add(new PaymentProperty(
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.City,
                    request.City));
            }

            if (!string.IsNullOrWhiteSpace(request.State))
            {
                serviceRequestProperties.Add(new PaymentProperty(
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.State,
                    request.State));
            }

            if (!string.IsNullOrWhiteSpace(request.PostalCode))
            {
                serviceRequestProperties.Add(new PaymentProperty(
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.PostalCode,
                    request.PostalCode));
            }

            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                serviceRequestProperties.Add(new PaymentProperty(
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Country,
                    request.Country));
            }

            serviceRequestProperties.Add(new PaymentProperty(
                GenericNamespace.TransactionData,
                TransactionDataProperties.TransactionType,
                request.TransactionType.Name));
            serviceRequestProperties.Add(new PaymentProperty(
                GenericNamespace.TransactionData,
                TransactionDataProperties.SupportCardTokenization,
                request.SupportCardTokenization.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)));
            serviceRequestProperties.Add(new PaymentProperty(
                GenericNamespace.TransactionData,
                TransactionDataProperties.HostPageOrigin,
                request.HostPageOrigin));
            serviceRequestProperties.Add(new PaymentProperty(
                GenericNamespace.TransactionData,
                TransactionDataProperties.IndustryType,
                request.IndustryType));

            if (request.TransactionType == Sitecore.Commerce.Entities.Payments.TransactionType.Authorize ||
                request.TransactionType == Sitecore.Commerce.Entities.Payments.TransactionType.Capture)
            {
                Assert.ArgumentNotNull(request.PurchaseLevel, "request.PurchaseLevel");
                Assert.ArgumentNotNull(request.AllowVoiceAuthorization, "request.AllowVoiceAuthorization");
                Assert.ArgumentNotNullOrEmpty(request.CurrencyCode, "request.CurrencyCode");

                serviceRequestProperties.Add(new PaymentProperty(
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.PurchaseLevel,
                    request.PurchaseLevel.Name));
                serviceRequestProperties.Add(new PaymentProperty(
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.AllowPartialAuthorization,
                    request.AllowPartialAuthorization.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)));
                serviceRequestProperties.Add(new PaymentProperty(
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.AllowVoiceAuthorization,
                    request.AllowVoiceAuthorization.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)));
                serviceRequestProperties.Add(new PaymentProperty(
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.CurrencyCode,
                    request.CurrencyCode));
            }

            var paymentAcceptUrl = this.GetBasePaymentServiceUrl(request, result, serviceRequestProperties);
            if (!string.IsNullOrWhiteSpace(paymentAcceptUrl))
            {
                result.Success = true;
                result.Url = string.Format(
                    "{0}&pagewidth={1}&fontsize={2}&fontfamily={3}&pagebackgroundcolor={4}&labelcolor={5}&textbackgroundcolor={6}&textcolor={7}&disabledtextbackgroundcolor={8}&columnnumber={9}",
                    paymentAcceptUrl,
                    HttpUtility.UrlEncode(request.PageWidth),
                    HttpUtility.UrlEncode(request.FontSize),
                    HttpUtility.UrlEncode(request.FontFamily),
                    HttpUtility.UrlEncode(request.PageBackgroundColor),
                    HttpUtility.UrlEncode(request.LabelColor),
                    HttpUtility.UrlEncode(request.TextBackgroundColor),
                    HttpUtility.UrlEncode(request.TextColor),
                    HttpUtility.UrlEncode(request.DisabledTextBackgroundColor),
                    HttpUtility.UrlEncode(request.ColumnNumber.GetValueOrDefault(1).ToString(CultureInfo.InvariantCulture)));
            }
            else
            {
                result.Success = false;
            }
        }

        private string GetBasePaymentServiceUrl(GetPaymentServiceUrlRequest request, GetPaymentServiceUrlResult result, List<PaymentProperty> serviceRequestProperties)
        {
            var getPaymentAcceptPointResponse = this.ExecutePaymentServiceRequest(
                serviceRequestProperties,
                GetPaymentAcceptPointRelativeUri,
                request.Locale,
                result.SystemMessages);
            if (getPaymentAcceptPointResponse != null)
            {
                var getPaymentAcceptPointResponseProperties = PaymentProperty.ConvertToHashtable(getPaymentAcceptPointResponse.Properties);

                var paymentAcceptUrl = string.Empty;
                PaymentProperty.GetPropertyValue(
                    getPaymentAcceptPointResponseProperties,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.PaymentAcceptUrl,
                    out paymentAcceptUrl);
                
                return paymentAcceptUrl;
            }

            return string.Empty;
        }
    }
}