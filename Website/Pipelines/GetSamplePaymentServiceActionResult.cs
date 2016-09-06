// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSamplePaymentServiceActionResult.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetSamplePaymentServiceActionResult class.
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
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Gets the action result from the sample payment service.
    /// </summary>
    public class GetSamplePaymentServiceActionResult : SamplePaymentServicePipelineBase
    {
        private const string RetrievePaymentAcceptResultRelativeUri = "/Payments/RetrievePaymentAcceptResult";

        /// <summary>
        /// Processes the service pipeline arguments.
        /// </summary>
        /// <param name="args">The service pipeline arguments.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentCondition(args.Request is GetPaymentServiceActionResultRequest, "args.Request", "Request must be of type GetPaymentServiceActionResultRequest.");
            Assert.ArgumentCondition(args.Result is GetPaymentServiceActionResultResult, "args.Result", "Result must be of type GetPaymentServiceActionResultResult.");
            var request = (GetPaymentServiceActionResultRequest)args.Request;
            var result = (GetPaymentServiceActionResultResult)args.Result;

            var serviceRequestProperties = new List<PaymentProperty>();
            serviceRequestProperties.AddRange(this.GetMerchantProperties() ?? Enumerable.Empty<PaymentProperty>());

            serviceRequestProperties.Add(new PaymentProperty(
                GenericNamespace.TransactionData,
                TransactionDataProperties.PaymentAcceptResultAccessCode,
                request.PaymentAcceptResultAccessCode));

            var serviceResponse = this.ExecutePaymentServiceRequest(
                serviceRequestProperties,
                RetrievePaymentAcceptResultRelativeUri,
                request.Locale,
                result.SystemMessages);
            if (serviceResponse != null)
            {
                var serviceResponseProperties = PaymentProperty.ConvertToHashtable(serviceResponse.Properties);

                string token;
                PaymentProperty.GetPropertyValue(
                    serviceResponseProperties,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardToken,
                    out token);

                var innerAuthorizeResponseProperty = PaymentProperty.GetPropertyFromHashtable(
                    serviceResponseProperties,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.Properties);
                var innerAuthorizeResponseProperties = PaymentProperty.ConvertToHashtable(innerAuthorizeResponseProperty.PropertyList);

                string authorizationResult = null;
                PaymentProperty.GetPropertyValue(
                    innerAuthorizeResponseProperties,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.AuthorizationResult,
                    out authorizationResult);

                result.AuthorizationResult = authorizationResult;
                result.CardToken = token;
                result.Success = true;
            }
            else
            {
                result.Success = false;
            }
        }
    }
}