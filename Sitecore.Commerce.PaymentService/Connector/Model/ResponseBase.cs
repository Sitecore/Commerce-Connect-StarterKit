// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseBase.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ResponseBase class.
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

    internal abstract class ResponseBase
    {
        internal ResponseBase(string locale, string serviceAccountId, string connectorName)
        {
            this.Locale = locale;
            this.ServiceAccountId = serviceAccountId;
            this.ConnectorName = connectorName;
        }

        internal string Locale { get; set; }
        
        internal string ServiceAccountId { get; set; }

        internal string ConnectorName { get; set; }
        
        protected void WriteBaseProperties(Response response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            response.Locale = this.Locale;

            var properties = new List<PaymentProperty>();
            if (response.Properties != null)
            {
                properties.AddRange(response.Properties);
            }

            properties.Add(new PaymentProperty(GenericNamespace.MerchantAccount, MerchantAccountProperties.ServiceAccountId, this.ServiceAccountId));
            properties.Add(new PaymentProperty(GenericNamespace.Connector, ConnectorProperties.ConnectorName, this.ConnectorName));
            response.Properties = properties.ToArray();
        }
    }
}
