// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICheckoutService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the ICheckoutService class.</summary>
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
namespace Sitecore.Commerce.StarterKit.Services
{
    using System.Collections.Generic;
    using Entities;
    using Entities.Payments;
    using Entities.Shipping;
	using Sitecore.Commerce.Services.Payments;

    /// <summary>
    /// The CheckoutService interface.
    /// </summary>
    public interface ICheckoutService
    {
        /// <summary>
        /// Get shipping options
        /// </summary>
        /// <returns>
        /// The shipping optins
        /// </returns>
        IEnumerable<ShippingOption> GetShippingOptions();

        /// <summary>
        /// Get payment options
        /// </summary>
        /// <returns>
        /// The payment options
        /// </returns>
        IEnumerable<PaymentOption> GetPaymentOptions();

        /// <summary>
        /// Get shipping methods
        /// </summary>
        /// <param name="shippingOption">The shipping option.</param>
        /// <returns>The shipping methods.</returns>
        IEnumerable<ShippingMethod> GetShippingMethods(ShippingOption shippingOption);

        /// <summary>
        /// Get payment methods
        /// </summary>
        /// <param name="paymentOption">The payment option.</param>
        /// <returns>The payment methods.</returns>
        IEnumerable<PaymentMethod> GetPaymentMethods(PaymentOption paymentOption);

        /// <summary>
        /// Gets the federated payment service URL.
        /// </summary>
        /// <returns>The federated payment service URL.</returns>
        string GetPaymentServiceUrl();

        /// <summary>
        /// Gets the result of a payment service action.
        /// </summary>
        /// <param name="accessCode">The payment service access code.</param>
        /// <returns>The result of the payment service acction.</returns>
        GetPaymentServiceActionResultResult GetPaymentServiceActionResult(string accessCode);
    }
}