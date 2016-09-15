// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CardPaymentEntry.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CardPaymentEntry class.
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

namespace Sitecore.Commerce.PaymentService
{
    using System;
    using System.Configuration;
    using System.Globalization;

    /// <summary>
    /// The data object that maps to the CardPaymentEntry table.
    /// </summary>
    public class CardPaymentEntry
    {
        /// <summary>
        /// Gets or sets the service account ID.
        /// </summary>
        public string ServiceAccountId { get; set; }

        /// <summary>
        /// Gets or sets the unique entry ID.
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// Gets or sets the entry data.
        /// </summary>
        public string EntryData { get; set; }

        /// <summary>
        /// Gets or sets the UTC date time of the payment entry.
        /// </summary>
        public DateTime EntryUtcTime { get; set; }

        /// <summary>
        /// Gets or sets the origin of the host page URI.
        /// </summary>
        public string HostPageOrigin { get; set; }

        /// <summary>
        /// Gets or sets the entry locale.
        /// </summary>
        public string EntryLocale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entry has been used.
        /// </summary>
        public bool Used { get; set; }

        /// <summary>
        /// Gets or sets the industry type, e.g. Retail, DirectMarketing, Ecommerce.
        /// </summary>
        public string IndustryType { get; set; }

        /// <summary>
        /// Gets or sets the transaction type, e.g. None, Authorize, Capture.
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction should support card swipe.
        /// </summary>
        public bool SupportCardSwipe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction should tokenize a card.
        /// </summary>
        public bool SupportCardTokenization { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether voice authorization is allowed.
        /// </summary>
        public bool AllowVoiceAuthorization { get; set; }

        /// <summary>
        /// Gets or sets the card types to display.
        /// </summary>
        public string CardTypes { get; set; }

        /// <summary>
        /// Gets or sets the default value of the card holder name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CardHolder", Justification = "By Design.")]
        public string DefaultCardHolderName { get; set; }

        /// <summary>
        /// Gets or sets the default value of the street 1 of the billing address.
        /// </summary>
        public string DefaultStreet1 { get; set; }

        /// <summary>
        /// Gets or sets the default value of the street 2 of the billing address.
        /// </summary>
        public string DefaultStreet2 { get; set; }

        /// <summary>
        /// Gets or sets the default value of the city of the billing address.
        /// </summary>
        public string DefaultCity { get; set; }

        /// <summary>
        /// Gets or sets the default value of the state or province of the billing address.
        /// </summary>
        public string DefaultStateOrProvince { get; set; }

        /// <summary>
        /// Gets or sets the default value of the zip code or post code of the billing address.
        /// </summary>
        public string DefaultPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the default value of the country code of the billing address.
        /// </summary>
        public string DefaultCountryCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the "Same as shipping address" option should be displayed.
        /// </summary>
        public bool ShowSameAsShippingAddress { get; set; }

        /// <summary>
        /// Gets a value indicating whether the request has been expired.
        /// </summary>
        public bool IsExpired
        {
            get
            {
                var paymentEntryValidPeriod = int.Parse(ConfigurationManager.AppSettings["PaymentEntryValidPeriodInMinutes"], CultureInfo.InvariantCulture);
                return DateTime.UtcNow > this.EntryUtcTime.AddMinutes(paymentEntryValidPeriod);
            }
        }
    }
}