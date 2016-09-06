// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefundResponse.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the RefundResponse class.
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

    internal class RefundResponse : ResponseBase
    {
        internal RefundResponse(string locale, string serviceAccountId, string connectorName)
            : base(locale, serviceAccountId, connectorName)
        {
            this.ApprovedAmount = PaymentUtilities.DecimalValueNotPresent;
            this.TransactionDateTime = PaymentUtilities.DateTimeValueNotPresent;
        }

        internal string CardType { get; set; }

        internal string Last4Digit { get; set; }

        internal string CardToken { get; set; }

        internal string UniqueCardId { get; set; }

        internal string ProviderTransactionId { get; set; }

        internal string ApprovalCode { get; set; }

        internal string ResponseCode { get; set; }
        
        internal decimal ApprovedAmount { get; set; }

        internal string CurrencyCode { get; set; }

        internal string RefundResult { get; set; }

        internal string ProviderMessage { get; set; }

        internal string TransactionType { get; set; }

        internal DateTime TransactionDateTime { get; set; }

        internal static Response ConvertTo(RefundResponse refundResponse)
        {
            var response = new Response();
            refundResponse.WriteBaseProperties(response);

            var properties = new List<PaymentProperty>();
            if (response.Properties != null)
            {
                properties.AddRange(response.Properties);
            }

            var refundRespnseProperties = new List<PaymentProperty>();
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.CardType, refundResponse.CardType);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.Last4Digits, refundResponse.Last4Digit);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.CardToken, refundResponse.CardToken);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.UniqueCardId, refundResponse.UniqueCardId);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.ProviderTransactionId, refundResponse.ProviderTransactionId);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.ApprovalCode, refundResponse.ApprovalCode);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.ResponseCode, refundResponse.ResponseCode);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.ApprovedAmount, refundResponse.ApprovedAmount);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.CurrencyCode, refundResponse.CurrencyCode);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.RefundResult, refundResponse.RefundResult);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.ProviderMessage, refundResponse.ProviderMessage);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.TransactionType, refundResponse.TransactionType);
            PaymentUtilities.AddPropertyIfPresent(refundRespnseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.TransactionDateTime, refundResponse.TransactionDateTime);
            properties.Add(new PaymentProperty(GenericNamespace.RefundResponse, RefundResponseProperties.Properties, refundRespnseProperties.ToArray()));

            response.Properties = properties.ToArray();
            return response;
        }
    }
}
