// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoidResponse.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the VoidResponse class.
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

    internal class VoidResponse : ResponseBase
    {
        internal VoidResponse(string locale, string serviceAccountId, string connectorName)
            : base(locale, serviceAccountId, connectorName)
        {
            this.TransactionDateTime = PaymentUtilities.DateTimeValueNotPresent;
        }

        internal string CardType { get; set; }

        internal string Last4Digit { get; set; }

        internal string UniqueCardId { get; set; }

        internal string ProviderTransactionId { get; set; }

        internal string ResponseCode { get; set; }

        internal string CurrencyCode { get; set; }

        internal string VoidResult { get; set; }

        internal string ProviderMessage { get; set; }

        internal string TransactionType { get; set; }

        internal DateTime TransactionDateTime { get; set; }

        internal static Response ConvertTo(VoidResponse voidResponse)
        {
            var response = new Response();
            voidResponse.WriteBaseProperties(response);

            var properties = new List<PaymentProperty>();
            if (response.Properties != null)
            {
                properties.AddRange(response.Properties);
            }

            var voidRespnseProperties = new List<PaymentProperty>();
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.CardType, voidResponse.CardType);
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.Last4Digits, voidResponse.Last4Digit);
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.UniqueCardId, voidResponse.UniqueCardId);
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.ProviderTransactionId, voidResponse.ProviderTransactionId);
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.ResponseCode, voidResponse.ResponseCode);
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.CurrencyCode, voidResponse.CurrencyCode);
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.VoidResult, voidResponse.VoidResult);
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.ProviderMessage, voidResponse.ProviderMessage);
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.TransactionType, voidResponse.TransactionType);
            PaymentUtilities.AddPropertyIfPresent(voidRespnseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.TransactionDateTime, voidResponse.TransactionDateTime);
            properties.Add(new PaymentProperty(GenericNamespace.VoidResponse, VoidResponseProperties.Properties, voidRespnseProperties.ToArray()));

            response.Properties = properties.ToArray();
            return response;
        }
    }
}
