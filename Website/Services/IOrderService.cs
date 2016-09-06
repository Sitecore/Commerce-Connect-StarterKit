// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrderService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the IOrderService class.</summary>
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
    using Sitecore.Commerce.Entities.Orders;

    /// <summary>
    /// The orders service interface.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Sets The Payment Information to the cart and submits an order.
        /// </summary>
        /// <param name="paymentInformation">The payment information.</param>
        /// <param name="paymentMethod">The payment method.</param>
        /// <returns>The order ID.</returns>
        string SetPaymentInformationAndSubmit(PropertyCollection paymentInformation, string paymentMethod);

        /// <summary>
        /// Gets the visitor orders.
        /// </summary>
        /// <returns>The visitor orders.</returns>
        IReadOnlyCollection<OrderHeader> GetOrders();
    }
}
