// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureResponse.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CaptureResponse class.
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

    internal class CaptureResponse : ResponseBase
    {
        internal CaptureResponse(string locale, string serviceAccountId, string connectorName)
            : base(locale, serviceAccountId, connectorName)
        {
            this.TransactionDateTime = PaymentUtilities.DateTimeValueNotPresent;
        }

        internal string CardType { get; set; }

        internal string Last4Digit { get; set; }

        internal string CardToken { get; set; }

        internal string UniqueCardId { get; set; }

        internal string ProviderTransactionId { get; set; }

        internal string ApprovalCode { get; set; }

        internal string ResponseCode { get; set; }

        internal string CurrencyCode { get; set; }

        internal string CaptureResult { get; set; }

        internal string ProviderMessage { get; set; }

        internal string TransactionType { get; set; }

        internal DateTime TransactionDateTime { get; set; }

        internal static Response ConvertTo(CaptureResponse captureResponse)
        {
            var response = new Response();
            captureResponse.WriteBaseProperties(response);

            var properties = new List<PaymentProperty>();
            if (response.Properties != null)
            {
                properties.AddRange(response.Properties);
            }

            var captureRespnseProperties = new List<PaymentProperty>();
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CardType, captureResponse.CardType);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.Last4Digits, captureResponse.Last4Digit);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CardToken, captureResponse.CardToken);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.UniqueCardId, captureResponse.UniqueCardId);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.ProviderTransactionId, captureResponse.ProviderTransactionId);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.ApprovalCode, captureResponse.ApprovalCode);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.ResponseCode, captureResponse.ResponseCode);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CurrencyCode, captureResponse.CurrencyCode);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CaptureResult, captureResponse.CaptureResult);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.ProviderMessage, captureResponse.ProviderMessage);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.TransactionType, captureResponse.TransactionType);
            PaymentUtilities.AddPropertyIfPresent(captureRespnseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.TransactionDateTime, captureResponse.TransactionDateTime);
            properties.Add(new PaymentProperty(GenericNamespace.CaptureResponse, CaptureResponseProperties.Properties, captureRespnseProperties.ToArray()));

            response.Properties = properties.ToArray();
            return response;
        }
    }
}
