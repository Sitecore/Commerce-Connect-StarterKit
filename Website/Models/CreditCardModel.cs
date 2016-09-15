// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreditCardModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the CreditCardModel class.</summary>
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

namespace Sitecore.Commerce.StarterKit.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Represents credit card information.
    /// </summary>
    public class CreditCardModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardModel"/> class.
        /// </summary>
        public CreditCardModel()
        {
            this.CreditCardTypes = new List<SelectListItem>();
        }

        /// <summary>
        /// Gets or sets the credit card type.
        /// </summary>
        [Display(Name = "Select credit card")]
        public string CreditCardType { get; set; }

        /// <summary>
        /// Gets or sets the selected credit card types.
        /// </summary>
        public IList<SelectListItem> CreditCardTypes { get; set; }

        /// <summary>
        /// Gets or sets the cardholder name.
        /// </summary>
        [Required]
        [Display(Name = "Cardholder name")]
        public string CardholderName { get; set; }

        /// <summary>
        /// Gets or sets the card number.
        /// </summary>
        [Required]
        [Display(Name = "Card number")]
        [CreditCard]
        [Range(100000000000, 9999999999999999999, ErrorMessage = "Card number must be between 12 and 19 digits")]
        public string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the expriation month.
        /// </summary>
        [Display(Name = "Expiration month")]
        public string ExpireMonth { get; set; }

        /// <summary>
        /// Gets or sets the expiration year.
        /// </summary>
        [Display(Name = "Expiration year")]
        public string ExpireYear { get; set; }

        /// <summary>
        /// Gets or sets the card verification code
        /// </summary>
        [Required]
        [Range(100, 999, ErrorMessage = "Card code must have 3 digits")]
        [Display(Name = "Card code")]
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the credit card number prefix.
        /// </summary>
        public string CreditCardPrefix { get; set; }

        /// <summary>
        /// Gets or sets the card authorization result access code.
        /// </summary>
        public string ResultAccessCode { get; set; }

        /// <summary>
        /// Gets or sets the credit card federated payment token.
        /// </summary>
        public string CardToken { get; set; }
    }
}