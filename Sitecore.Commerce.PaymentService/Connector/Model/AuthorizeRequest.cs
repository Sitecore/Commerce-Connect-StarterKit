// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizeRequest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the AuthorizeRequest class.
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

    internal class AuthorizeRequest : RequestBase
    {
        internal AuthorizeRequest()
            : base()
        {
            this.ExpirationYear = PaymentUtilities.DecimalValueNotPresent;
            this.ExpirationMonth = PaymentUtilities.DecimalValueNotPresent;
            this.CashBackAmount = PaymentUtilities.DecimalValueNotPresent;
            this.Amount = PaymentUtilities.DecimalValueNotPresent;
        }

        internal string CardType { get; set; }

        internal bool IsSwipe { get; set; }

        internal string CardNumber { get; set; }

        internal string Track1 { get; set; }

        internal string Track2 { get; set; }

        internal decimal ExpirationYear { get; set; }

        internal decimal ExpirationMonth { get; set; }

        internal string CardToken { get; set; }

        internal string Last4Digit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string EncryptedPin { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string AdditionalSecurityData { get; set; }

        internal string CardVerificationValue { get; set; }

        internal string VoiceAuthorizationCode { get; set; }

        internal decimal CashBackAmount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string StreetAddress { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string StreetAddress2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string City { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string State { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string PostalCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string Country { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string Phone { get; set; }

        internal string AccountType { get; set; }

        internal string UniqueCardId { get; set; }

        internal decimal Amount { get; set; }

        internal string CurrencyCode { get; set; }

        internal bool SupportCardTokenization { get; set; }

        internal string AuthorizationProviderTransactionId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string PurchaseLevel { get; set; }
        
        internal static AuthorizeRequest ConvertFrom(Request request)
        {
            var authorizeRequest = new AuthorizeRequest();
            var errors = new List<PaymentError>();
            authorizeRequest.ReadBaseProperties(request, errors);

            // Read card data
            Dictionary<string, object> hashtable = PaymentProperty.ConvertToHashtable(request.Properties);
            authorizeRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.CardType,
                errors,
                ErrorCode.InvalidRequest);
            if (authorizeRequest.CardType != null
                && !PaymentUtilities.ValidateCardType(authorizeRequest.SupportedTenderTypes, authorizeRequest.CardType))
            {
                errors.Add(new PaymentError(ErrorCode.CardTypeNotSupported, string.Format(CultureInfo.InvariantCulture, "Card type is not supported: {0}.", authorizeRequest.CardType)));
            }

            authorizeRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.IsSwipe,
                false);
            if (authorizeRequest.IsSwipe)
            {
                authorizeRequest.Track1 = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Track1);
                authorizeRequest.Track2 = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Track2);

                authorizeRequest.CardNumber = PaymentUtilities.ParseTrack1ForCardNumber(authorizeRequest.Track1);
                if (authorizeRequest.CardNumber == null)
                {
                    authorizeRequest.CardNumber = PaymentUtilities.ParseTrack2ForCardNumber(authorizeRequest.Track2);
                }

                if (authorizeRequest.CardNumber == null)
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidCardTrackData, "Invalid card track data."));
                }

                decimal expirationYear, expirationMonth;
                HelperUtilities.ParseTrackDataForExpirationDate(authorizeRequest.Track1 ?? string.Empty, authorizeRequest.Track2 ?? string.Empty, out expirationYear, out expirationMonth);
                authorizeRequest.ExpirationYear = expirationYear;
                authorizeRequest.ExpirationMonth = expirationMonth;
            }
            else
            {
                authorizeRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardToken);

                if (authorizeRequest.CardToken == null)
                {
                    authorizeRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardNumber,
                        errors,
                        ErrorCode.InvalidCardNumber);
                }
                else
                {
                    authorizeRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Last4Digits);
                }

                authorizeRequest.ExpirationYear = PaymentUtilities.GetPropertyDecimalValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.ExpirationYear,
                    errors,
                    ErrorCode.InvalidExpirationDate);
                authorizeRequest.ExpirationMonth = PaymentUtilities.GetPropertyDecimalValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.ExpirationMonth,
                    errors,
                    ErrorCode.InvalidExpirationDate);
            }

            if (authorizeRequest.CardNumber != null
                && !HelperUtilities.ValidateBankCardNumber(authorizeRequest.CardNumber))
            {
                errors.Add(new PaymentError(ErrorCode.InvalidCardNumber, "Invalid card number."));
            }
            
            if (authorizeRequest.ExpirationYear >= 0M
                && authorizeRequest.ExpirationMonth >= 0M
                && !PaymentUtilities.ValidateExpirationDate(authorizeRequest.ExpirationYear, authorizeRequest.ExpirationMonth))
            {
                errors.Add(new PaymentError(ErrorCode.InvalidExpirationDate, "Invalid expiration date."));
            }

            if (Sitecore.Commerce.PaymentService.Connector.CardType.Debit.ToString().Equals(authorizeRequest.CardType, StringComparison.OrdinalIgnoreCase))
            {
                authorizeRequest.EncryptedPin = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.EncryptedPin,
                    errors,
                    ErrorCode.CannotVerifyPin);
                authorizeRequest.AdditionalSecurityData = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.AdditionalSecurityData);
            }

            authorizeRequest.CashBackAmount = PaymentUtilities.GetPropertyDecimalValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.CashBackAmount);
            if (authorizeRequest.CashBackAmount > 0M
                && !Sitecore.Commerce.PaymentService.Connector.CardType.Debit.ToString().Equals(authorizeRequest.CardType, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new PaymentError(ErrorCode.CashBackNotAvailable, "Cashback is not available."));
            }

            authorizeRequest.CardVerificationValue = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.CardVerificationValue);
            authorizeRequest.VoiceAuthorizationCode = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.VoiceAuthorizationCode);
            authorizeRequest.Name = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.Name);
            authorizeRequest.StreetAddress = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.StreetAddress);
            authorizeRequest.StreetAddress2 = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.StreetAddress2);
            authorizeRequest.City = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.City);
            authorizeRequest.State = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.State);
            authorizeRequest.PostalCode = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.PostalCode);
            authorizeRequest.Country = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.Country);
            authorizeRequest.Phone = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.Phone);
            authorizeRequest.AccountType = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.AccountType);
            authorizeRequest.UniqueCardId = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.PaymentCard,
                PaymentCardProperties.UniqueCardId);

            // Read transaction data
            authorizeRequest.Amount = PaymentUtilities.GetPropertyDecimalValue(
                hashtable, 
                GenericNamespace.TransactionData, 
                TransactionDataProperties.Amount, 
                errors,
                ErrorCode.InvalidAmount);
            authorizeRequest.CurrencyCode = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.TransactionData,
                TransactionDataProperties.CurrencyCode,
                errors,
                ErrorCode.InvalidRequest);
            if (authorizeRequest.CurrencyCode != null
                && !PaymentUtilities.ValidateCurrencyCode(authorizeRequest.SupportedCurrencies, authorizeRequest.CurrencyCode))
            {
                errors.Add(new PaymentError(ErrorCode.UnsupportedCurrency, string.Format(CultureInfo.InvariantCulture, "Currency code is not supported: {0}.", authorizeRequest.CurrencyCode)));
            }

            authorizeRequest.SupportCardTokenization = PaymentUtilities.GetPropertyBooleanValue(
                hashtable,
                GenericNamespace.TransactionData,
                TransactionDataProperties.SupportCardTokenization,
                false);

            authorizeRequest.AuthorizationProviderTransactionId = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.TransactionData,
                TransactionDataProperties.AuthorizationProviderTransactionId);

            authorizeRequest.PurchaseLevel = PaymentUtilities.GetPropertyStringValue(
                hashtable,
                GenericNamespace.TransactionData,
                TransactionDataProperties.PurchaseLevel);
            
            if (errors.Count > 0)
            {
                throw new SampleException(errors);
            }

            return authorizeRequest;
        }
    }
}
