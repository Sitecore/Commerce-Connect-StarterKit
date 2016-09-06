// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeResponse.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the AuthorizeResponse class.
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

    internal class AuthorizeResponse : ResponseBase
    {
        internal AuthorizeResponse(string locale, string serviceAccountId, string connectorName)
            : base(locale, serviceAccountId, connectorName)
        {
            this.CashBackAmount = PaymentUtilities.DecimalValueNotPresent;
            this.ApprovedAmount = PaymentUtilities.DecimalValueNotPresent;
            this.AvailableBalance = PaymentUtilities.DecimalValueNotPresent;
            this.TransactionDateTime = PaymentUtilities.DateTimeValueNotPresent;
        }

        internal string CardType { get; set; }

        internal bool IsSwipe { get; set; }

        internal string Last4Digit { get; set; }

        internal string CardToken { get; set; }

        internal string UniqueCardId { get; set; }

        internal string VoiceAuthorizationCode { get; set; }

        internal decimal CashBackAmount { get; set; }

        internal string AccountType { get; set; }

        internal string ProviderTransactionId { get; set; }

        internal string ApprovalCode { get; set; }

        internal string ResponseCode { get; set; }
        
        internal decimal ApprovedAmount { get; set; }

        internal string CurrencyCode { get; set; }

        internal string AuthorizationResult { get; set; }

        internal string ProviderMessage { get; set; }

        internal string AVSResult { get; set; }

        internal string AVSDetail { get; set; }

        internal string CVV2Result { get; set; }

        internal decimal AvailableBalance { get; set; }

        internal string TransactionType { get; set; }

        internal DateTime TransactionDateTime { get; set; }

        internal static Response ConvertTo(AuthorizeResponse authorizeResponse)
        {
            var response = new Response();
            authorizeResponse.WriteBaseProperties(response);

            var properties = new List<PaymentProperty>();
            if (response.Properties != null)
            {
                properties.AddRange(response.Properties);
            }

            var authorizationRespnseProperties = new List<PaymentProperty>();
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CardType, authorizeResponse.CardType);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.IsSwiped, authorizeResponse.IsSwipe.ToString());
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.Last4Digits, authorizeResponse.Last4Digit);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CardToken, authorizeResponse.CardToken);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.UniqueCardId, authorizeResponse.UniqueCardId);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.VoiceAuthorizationCode, authorizeResponse.VoiceAuthorizationCode);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CashBackAmount, authorizeResponse.CashBackAmount);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AccountType, authorizeResponse.AccountType);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ProviderTransactionId, authorizeResponse.ProviderTransactionId);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ApprovalCode, authorizeResponse.ApprovalCode);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ResponseCode, authorizeResponse.ResponseCode);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ApprovedAmount, authorizeResponse.ApprovedAmount);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CurrencyCode, authorizeResponse.CurrencyCode);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AuthorizationResult, authorizeResponse.AuthorizationResult);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ProviderMessage, authorizeResponse.ProviderMessage);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AVSResult, authorizeResponse.AVSResult);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AVSDetail, authorizeResponse.AVSDetail);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CVV2Result, authorizeResponse.CVV2Result);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AvailableBalance, authorizeResponse.AvailableBalance);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.TransactionType, authorizeResponse.TransactionType);
            PaymentUtilities.AddPropertyIfPresent(authorizationRespnseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.TransactionDateTime, authorizeResponse.TransactionDateTime);
            properties.Add(new PaymentProperty(GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.Properties, authorizationRespnseProperties.ToArray()));

            response.Properties = properties.ToArray();
            return response;
        }
    }
}
