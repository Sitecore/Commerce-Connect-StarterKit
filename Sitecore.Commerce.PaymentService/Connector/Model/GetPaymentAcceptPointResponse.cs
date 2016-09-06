// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetPaymentAcceptPointResponse.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetPaymentAcceptPointResponse class.
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
    using System.Collections.Generic;

    internal class GetPaymentAcceptPointResponse : ResponseBase
    {
        internal GetPaymentAcceptPointResponse(string locale, string serviceAccountId, string connectorName)
            : base(locale, serviceAccountId, connectorName)
        {
        }

        internal string PaymentAcceptUrl { get; set; }

        internal static Response ConvertTo(GetPaymentAcceptPointResponse acceptPointResponse)
        {
            var response = new Response();
            acceptPointResponse.WriteBaseProperties(response);

            var properties = new List<PaymentProperty>();
            if (response.Properties != null)
            {
                properties.AddRange(response.Properties);
            }

            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.TransactionData, TransactionDataProperties.PaymentAcceptUrl, acceptPointResponse.PaymentAcceptUrl);

            response.Properties = properties.ToArray();
            return response;
        }
    }
}
