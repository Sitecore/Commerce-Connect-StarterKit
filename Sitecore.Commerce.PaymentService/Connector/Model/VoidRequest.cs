// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoidRequest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the VoidRequest class.
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

    internal class VoidRequest : RequestBase
    {
        internal VoidRequest()
            : base()
        {
            this.AuthorizationApprovedAmount = PaymentUtilities.DecimalValueNotPresent;
            this.AuthorizationCashbackAmount = PaymentUtilities.DecimalValueNotPresent;
            this.AuthorizationTransactionDateTime = PaymentUtilities.DateTimeValueNotPresent;
        }

        internal string CardType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal bool IsSwipe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string CardToken { get; set; }

        internal string Last4Digit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string AccountType { get; set; }

        internal string UniqueCardId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string VoiceAuthorizationCode { get; set; }
        
        internal string CurrencyCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string AuthorizationProviderTransactionId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string AuthorizationApprovalCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string AuthorizationResponseCode { get; set; }

        internal decimal AuthorizationApprovedAmount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal decimal AuthorizationCashbackAmount { get; set; }
                
        internal string AuthorizationResult { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string AuthorizationProviderMessage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal DateTime AuthorizationTransactionDateTime { get; set; }

        internal string AuthorizationTransactionType { get; set; }

        internal static VoidRequest ConvertFrom(Request request)
        {
            var voidRequest = new VoidRequest();
            var errors = new List<PaymentError>();
            voidRequest.ReadBaseProperties(request, errors);

            // Check authorization response
            var hashtable = PaymentProperty.ConvertToHashtable(request.Properties);
            PaymentProperty authorizationResponsePropertyList = PaymentProperty.GetPropertyFromHashtable(hashtable, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.Properties);
            Dictionary<string, object> authorizationHashtable = null;
            if (authorizationResponsePropertyList == null)
            {
                errors.Add(new PaymentError(ErrorCode.InvalidRequest, "Authorization response is missing."));
                throw new SampleException(errors);
            }
            else
            {
                authorizationHashtable = PaymentProperty.ConvertToHashtable(authorizationResponsePropertyList.PropertyList);
            }

            // Read card data
            voidRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.CardType,
                errors,
                ErrorCode.InvalidRequest);
            voidRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.IsSwiped,
                false);
            voidRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.CardToken);
            voidRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.Last4Digits);
            voidRequest.AccountType = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.AccountType);
            voidRequest.UniqueCardId = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.UniqueCardId);
            voidRequest.VoiceAuthorizationCode = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.VoiceAuthorizationCode);

            // Read authorization data
            voidRequest.AuthorizationTransactionType = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.TransactionType,
                errors,
                ErrorCode.InvalidRequest);
            if (voidRequest.AuthorizationTransactionType != null
                && !TransactionType.Authorize.ToString().Equals(voidRequest.AuthorizationTransactionType, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new PaymentError(ErrorCode.InvalidTransaction, "Void does not support this type of transaction"));
            }

            voidRequest.AuthorizationApprovalCode = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ApprovalCode);
            voidRequest.AuthorizationApprovedAmount = PaymentUtilities.GetPropertyDecimalValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ApprovedAmount,
                errors, 
                ErrorCode.InvalidRequest);
            voidRequest.CurrencyCode = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.CurrencyCode,
                errors,
                ErrorCode.InvalidRequest);
            voidRequest.AuthorizationCashbackAmount = PaymentUtilities.GetPropertyDecimalValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.CashBackAmount);
            voidRequest.AuthorizationProviderMessage = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ProviderMessage);
            voidRequest.AuthorizationProviderTransactionId = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ProviderTransactionId,
                errors,
                ErrorCode.InvalidRequest);
            voidRequest.AuthorizationResponseCode = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ResponseCode);
            voidRequest.AuthorizationResult = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.AuthorizationResult,
                errors,
                ErrorCode.InvalidRequest);
            voidRequest.AuthorizationTransactionDateTime = PaymentUtilities.GetPropertyDateTimeValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.TransactionDateTime);
            
            if (errors.Count > 0)
            {
                throw new SampleException(errors);
            }

            return voidRequest;
        }
    }
}
