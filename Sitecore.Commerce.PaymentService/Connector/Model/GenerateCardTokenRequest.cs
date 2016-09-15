// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateCardTokenRequest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GenerateCardTokenRequest class.
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
    using System.Linq;

    internal class GenerateCardTokenRequest : RequestBase
    {
        internal GenerateCardTokenRequest() : base()
        {
            this.ExpirationYear = PaymentUtilities.DecimalValueNotPresent;
            this.ExpirationMonth = PaymentUtilities.DecimalValueNotPresent;
        }

        internal string CardType { get; set; }

        internal string CardNumber { get; set; }

        internal decimal ExpirationYear { get; set; }

        internal decimal ExpirationMonth { get; set; }

        internal string Name { get; set; }

        internal string StreetAddress { get; set; }

        internal string StreetAddress2 { get; set; }

        internal string City { get; set; }

        internal string State { get; set; }

        internal string PostalCode { get; set; }

        internal string Country { get; set; }

        internal string Phone { get; set; }

        internal string AccountType { get; set; }

        internal IList<PaymentProperty> OtherCardProperties { get; set; }

        internal static GenerateCardTokenRequest ConvertFrom(Request request, PaymentProperty[] requiredInteractionProperties)
        {
            var tokenizeRequest = new GenerateCardTokenRequest();
            var errors = new List<PaymentError>();
            tokenizeRequest.ReadBaseProperties(request, errors);

            // Get card data from request.
            var cardProperties = request.Properties;

            // Read card data
            tokenizeRequest.OtherCardProperties = new List<PaymentProperty>();
            if (cardProperties != null)
            {
                var hashtable = PaymentProperty.ConvertToHashtable(cardProperties);
                tokenizeRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardType,
                    errors,
                    ErrorCode.InvalidRequest);
                if (tokenizeRequest.CardType != null
                    && (!PaymentUtilities.ValidateCardType(tokenizeRequest.SupportedTenderTypes, tokenizeRequest.CardType)
                        || tokenizeRequest.CardType.Equals(Sitecore.Commerce.PaymentService.Connector.CardType.Debit.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    errors.Add(new PaymentError(ErrorCode.CardTypeNotSupported, string.Format(CultureInfo.InvariantCulture, "Card type is not supported: {0}.", tokenizeRequest.CardType)));
                }

                tokenizeRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardNumber,
                    errors,
                    ErrorCode.InvalidCardNumber);
                if (tokenizeRequest.CardNumber != null
                    && !HelperUtilities.ValidateBankCardNumber(tokenizeRequest.CardNumber))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidCardNumber, "Invalid card number."));
                }
                
                tokenizeRequest.ExpirationYear = PaymentUtilities.GetPropertyDecimalValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.ExpirationYear,
                    errors,
                    ErrorCode.InvalidExpirationDate);
                tokenizeRequest.ExpirationMonth = PaymentUtilities.GetPropertyDecimalValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.ExpirationMonth,
                    errors,
                    ErrorCode.InvalidExpirationDate);
                if (tokenizeRequest.ExpirationYear >= 0M
                    && tokenizeRequest.ExpirationMonth >= 0M
                    && !PaymentUtilities.ValidateExpirationDate(tokenizeRequest.ExpirationYear, tokenizeRequest.ExpirationMonth))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidExpirationDate, "Invalid expiration date."));
                }

                tokenizeRequest.Name = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Name);
                tokenizeRequest.StreetAddress = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.StreetAddress);
                tokenizeRequest.StreetAddress2 = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.StreetAddress2);
                tokenizeRequest.City = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.City);
                tokenizeRequest.State = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.State);
                tokenizeRequest.PostalCode = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.PostalCode);
                tokenizeRequest.Country = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Country);
                tokenizeRequest.Phone = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Phone);
                tokenizeRequest.AccountType = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.AccountType);

                // Add other custom card properties from interaction properties
                foreach (var cardProperty in cardProperties)
                {
                    if (GenericNamespace.PaymentCard.Equals(cardProperty.Namespace) && IsCustomCardProperty(cardProperty.Name))
                    {
                        tokenizeRequest.OtherCardProperties.Add(cardProperty);
                    }
                }
            }

            // Add other custom card properties from request properties
            foreach (var requestProperty in request.Properties)
            {
                if (GenericNamespace.PaymentCard.Equals(requestProperty.Namespace)
                    && !tokenizeRequest.OtherCardProperties.Any(p => p.Name.Equals(requestProperty.Name, StringComparison.OrdinalIgnoreCase))
                    && IsCustomCardProperty(requestProperty.Name))
                {
                    tokenizeRequest.OtherCardProperties.Add(requestProperty);
                }
            }

            if (errors.Count > 0)
            {
                throw new SampleException(errors);
            }

            return tokenizeRequest;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design.")]
        private static bool IsCustomCardProperty(string propertyName)
        {
            return !PaymentCardProperties.AccountType.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.AdditionalSecurityData.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.CardEntryType.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.CardNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.CardToken.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.CardType.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.CardVerificationValue.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.CashBackAmount.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.City.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Country.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.EncryptedPin.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.ExpirationMonth.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.ExpirationYear.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.IsSwipe.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Last4Digits.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Phone.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.PostalCode.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.State.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.StreetAddress.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.StreetAddress2.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track1.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track1Encrypted.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track1KeySerialNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track2.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track2Encrypted.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track2KeySerialNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track3.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track3Encrypted.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track3KeySerialNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track4.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track4Encrypted.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.Track4KeySerialNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.UniqueCardId.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                && !PaymentCardProperties.VoiceAuthorizationCode.Equals(propertyName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
