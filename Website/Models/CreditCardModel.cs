//-----------------------------------------------------------------------
// <copyright file="CreditCardModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The CreditCardModel class.</summary>
//-----------------------------------------------------------------------
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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.Commerce.StarterKit.Models
{
  public class CreditCardModel
  {
    public CreditCardModel()
        {
            CreditCardTypes = new List<SelectListItem>();
        }

    [Display(Name = "Select credit card")]
    public string CreditCardType { get; set; }
    public IList<SelectListItem> CreditCardTypes { get; set; }

    [Required]
    [Display(Name = "Cardholder name")]
    public string CardholderName { get; set; }

    [Required]
    [Display(Name = "Card number")]
    [CreditCard]
    [Range(100000000000, 9999999999999999999, ErrorMessage = "Card number must be between 12 and 19 digits")]
    public string CardNumber { get; set; }

    [Display(Name = "Expiration month")]
    public string ExpireMonth { get; set; }

    [Display(Name = "Expiration year")]
    public string ExpireYear { get; set; }

    [Required]
    [Range(100, 999, ErrorMessage = "Card code must have 3 digits")]
    [Display(Name = "Card code")]
    public string CardCode { get; set; }

  }
}