// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureRequest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CaptureRequest class.
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
    using System.Globalization;

    internal class CaptureRequest : RequestBase
    {
        internal CaptureRequest()
            : base()
        {
            this.Amount = PaymentUtilities.DecimalValueNotPresent;
            this.AuthorizationApprovedAmount = PaymentUtilities.DecimalValueNotPresent;
            this.AuthorizationCashbackAmount = PaymentUtilities.DecimalValueNotPresent;
            this.AuthorizationTransactionDateTime = PaymentUtilities.DateTimeValueNotPresent;
        }

        internal string CardType { get; set; }

        internal bool IsSwipe { get; set; }

        internal string CardNumber { get; set; }

        internal string Track1 { get; set; }

        internal string Track2 { get; set; }

        internal string CardToken { get; set; }

        internal string Last4Digit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string AccountType { get; set; }

        internal string UniqueCardId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string VoiceAuthorizationCode { get; set; }

        internal decimal Amount { get; set; }

        internal string CurrencyCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string PurchaseLevel { get; set; }

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
        
        internal static CaptureRequest ConvertFrom(Request request)
        {
            var captureRequest = new CaptureRequest();
            var errors = new List<PaymentError>();
            captureRequest.ReadBaseProperties(request, errors);

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
            captureRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.CardType,
                errors,
                ErrorCode.InvalidRequest);

            captureRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.IsSwiped,
                false);
            if (captureRequest.IsSwipe)
            {
                captureRequest.Track1 = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Track1);
                captureRequest.Track2 = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Track2);
                captureRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardNumber);

                if (string.IsNullOrEmpty(captureRequest.CardNumber))
                {
                    captureRequest.CardNumber = PaymentUtilities.ParseTrack1ForCardNumber(captureRequest.Track1);
                    if (captureRequest.CardNumber == null)
                    {
                        captureRequest.CardNumber = PaymentUtilities.ParseTrack2ForCardNumber(captureRequest.Track2);
                    }
                }

                if (string.IsNullOrEmpty(captureRequest.CardNumber))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidCardTrackData, "Invalid card track data."));
                }
            }
            else
            {
                captureRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.CardToken);
                if (captureRequest.CardToken == null)
                {
                    captureRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardToken);
                    if (captureRequest.CardToken == null)
                    {
                        captureRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.CardNumber,
                            errors,
                            ErrorCode.InvalidCardNumber);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(captureRequest.CardNumber)
                && string.IsNullOrWhiteSpace(captureRequest.CardToken))
            {
                errors.Add(new PaymentError(ErrorCode.InvalidRequest, string.Format(CultureInfo.InvariantCulture, "Neither card number nor card token is provided.")));
            }

            captureRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.Last4Digits);
            captureRequest.AccountType = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.AccountType);
            captureRequest.UniqueCardId = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.UniqueCardId);
            captureRequest.VoiceAuthorizationCode = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.VoiceAuthorizationCode);

            // Read transaction data
            captureRequest.Amount = PaymentUtilities.GetPropertyDecimalValue(
                hashtable, 
                GenericNamespace.TransactionData, 
                TransactionDataProperties.Amount, 
                errors,
                ErrorCode.InvalidAmount);
            captureRequest.CurrencyCode = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.TransactionData,
                TransactionDataProperties.CurrencyCode,
                errors,
                ErrorCode.InvalidRequest);
            captureRequest.PurchaseLevel = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.TransactionData,
                TransactionDataProperties.PurchaseLevel);

            // Read authorization data
            captureRequest.AuthorizationTransactionType = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.TransactionType,
                errors,
                ErrorCode.InvalidRequest);
            if (captureRequest.AuthorizationTransactionType != null
                && !TransactionType.Authorize.ToString().Equals(captureRequest.AuthorizationTransactionType, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new PaymentError(ErrorCode.InvalidTransaction, "Capture does not support this type of transaction"));
            }

            captureRequest.AuthorizationApprovalCode = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ApprovalCode);
            captureRequest.AuthorizationApprovedAmount = PaymentUtilities.GetPropertyDecimalValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ApprovedAmount,
                errors, 
                ErrorCode.InvalidRequest);
            captureRequest.AuthorizationCashbackAmount = PaymentUtilities.GetPropertyDecimalValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.CashBackAmount);
            captureRequest.AuthorizationProviderMessage = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ProviderMessage);
            captureRequest.AuthorizationProviderTransactionId = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ProviderTransactionId,
                errors,
                ErrorCode.InvalidRequest);
            captureRequest.AuthorizationResponseCode = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.ResponseCode);
            captureRequest.AuthorizationResult = PaymentUtilities.GetPropertyStringValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.AuthorizationResult,
                errors,
                ErrorCode.InvalidRequest);
            captureRequest.AuthorizationTransactionDateTime = PaymentUtilities.GetPropertyDateTimeValue(
                authorizationHashtable,
                GenericNamespace.AuthorizationResponse,
                AuthorizationResponseProperties.TransactionDateTime);
            
            if (errors.Count > 0)
            {
                throw new SampleException(errors);
            }

            return captureRequest;
        }
    }
}
