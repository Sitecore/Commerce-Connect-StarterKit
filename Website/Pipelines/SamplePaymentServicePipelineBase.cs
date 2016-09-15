// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplePaymentServicePipelineBase.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the SamplePaymentServicePipelineBase class.
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
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Payments;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;

    public abstract class SamplePaymentServicePipelineBase : PipelineProcessor<ServicePipelineArgs>
    {
        /// <summary>
        /// Gets the merchant properties for the current site.
        /// </summary>
        /// <returns>The merchant properties for the current site.</returns>
        protected virtual IEnumerable<PaymentProperty> GetMerchantProperties()
        {
            var providerItem = this.GetProviderItem();

            var merchantProperties = new List<PaymentProperty>();
            merchantProperties.Add(new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.ServiceAccountId,
                providerItem[Constants.KnownFieldNames.ServiceAccountId]));
            merchantProperties.Add(new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.MerchantId,
                providerItem[Constants.KnownFieldNames.MerchantId]));
            merchantProperties.Add(new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.ProviderId,
                providerItem[Constants.KnownFieldNames.ProviderId]));
            merchantProperties.Add(new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.Environment,
                providerItem[Constants.KnownFieldNames.Environment]));
            merchantProperties.Add(new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.SupportedCurrencies,
                providerItem[Constants.KnownFieldNames.SupportedCurrencies]));
            merchantProperties.Add(new PaymentProperty(
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.SupportedTenderTypes,
                providerItem[Constants.KnownFieldNames.SupportedTenderTypes]));

            return merchantProperties;
        }

        /// <summary>
        /// Gets the Sitecore item that contains the federated service provider merchant properties.
        /// </summary>
        /// <returns>The Sitecore item that contains the federated service provider merchant properties.</returns>
        protected virtual Item GetProviderItem()
        {
            return Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);
        }

        /// <summary>
        /// Executes a REST service request against the payment service.
        /// </summary>
        /// <param name="serviceRequestProperties">The service request properties.</param>
        /// <param name="relativeUri">The relative URI to the rest method.</param>
        /// <param name="locale">The service locale.</param>
        /// <param name="systemMessages">The collection that will be populated with error information if anny errors occur.</param>
        /// <returns>The REST service response.</returns>
        protected virtual Response ExecutePaymentServiceRequest(List<PaymentProperty> serviceRequestProperties, string relativeUri, string locale, IList<SystemMessage> systemMessages)
        {
            Assert.ArgumentNotNull(serviceRequestProperties, "serviceRequestProperties");
            Assert.ArgumentNotNullOrEmpty(relativeUri, "relativeUri");
            Assert.ArgumentNotNullOrEmpty(locale, "locale");

            var serviceRequest = new Request();
            serviceRequest.Properties = serviceRequestProperties.ToArray();
            serviceRequest.Locale = locale;

            var providerItem = this.GetProviderItem();
            var errors = new List<PaymentError>();

            using (HttpClient client = new HttpClient())
            {
                var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(serviceRequest);
                var content = new StringContent(requestJson);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage httpResponse = null;
                client.BaseAddress = new Uri(providerItem[Constants.KnownFieldNames.PaymentServiceUrl]);
                var serviceTask = Task.Run(() => httpResponse = client.PostAsync(relativeUri, content).Result);
                serviceTask.Wait();

                if (httpResponse.IsSuccessStatusCode)
                {
                    // Read and deserialize response content
                    string responseJson = null;
                    var readAsStringAsyncTask = Task.Run(() => responseJson = httpResponse.Content.ReadAsStringAsync().Result);
                    readAsStringAsyncTask.Wait();

                    var serviceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(responseJson);

                    // Prepare response properties
                    if (serviceResponse.Errors == null || serviceResponse.Errors.Length == 0)
                    {
                        return serviceResponse;
                    }
                    else
                    {
                        foreach (var error in serviceResponse.Errors)
                        {
                            var message = string.Format("Payment Service Error {0}: {1}", error.Code, error.Message);
                            Log.Error(message, this);
                            if (systemMessages != null)
                            {
                                systemMessages.Add(new SystemMessage { Message = message });
                            }
                        }
                    }
                }
                else
                {
                    var message = string.Format("Payment Service Error {0}: GetPaymentAcceptPoint failure: Internal server error:", ErrorCode.ApplicationError);
                    var detail = httpResponse.Content.ReadAsStringAsync();
                    detail.Wait();

                    Log.Error(message, this);
                    Log.Error(detail.Result, this);
                    if (systemMessages != null)
                    {
                        systemMessages.Add(new SystemMessage { Message = message });
                    }
                }
            }

            return null;
        }
    }
}