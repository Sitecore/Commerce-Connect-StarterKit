// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NopPaymentOptionType.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the NopPaymentOptionType class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Entities.Payments
{
    using System;

    /// <summary>
    /// Payment option type enum
    /// </summary>
    [Serializable]
    public class NopPaymentOptionType : Commerce.Entities.Shipping.ShippingOptionType
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="NopPaymentOptionType" /> class from being created.
        /// </summary>
        private NopPaymentOptionType()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NopPaymentOptionType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        private NopPaymentOptionType(int value, string name)
            : base(value, name)
        {
        }

        /// <summary>
        /// Gets the online payment.
        /// </summary>
        /// <value>
        /// The online payment.
        /// </value>
        public static Commerce.Entities.Payments.PaymentOptionType OnlinePayment
        {
            get
            {
                return new Commerce.Entities.Payments.PaymentOptionType(4, "OnlinePayment");
            }
        }
    }
}
