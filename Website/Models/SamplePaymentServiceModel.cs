// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplePaymentServiceModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the SamplePaymentServiceModel class.</summary>
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
    public class SamplePaymentServiceModel
    {
        /// <summary>
        /// Gets or sets the URL of the federated payment service card page.
        /// </summary>
        public string PaymentServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the card page origin.
        /// </summary>
        public string CardPageOrigin { get; set; }

        /// <summary>
        /// Gets or sets the payment amount.
        /// </summary>
        public decimal PaymentAmount { get; set; }
    }
}