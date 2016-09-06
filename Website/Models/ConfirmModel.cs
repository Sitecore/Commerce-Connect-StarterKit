// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfirmModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the ConfirmModel class.</summary>
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
    /// <summary>
    /// specifies order confirmation information.
    /// </summary>
    public class ConfirmModel
    {
        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public AddressModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        public AddressModel ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the payment method.
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the shipping method.
        /// </summary>
        public string ShippingMethod { get; set; }
    }
}