// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrdersService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the IOrdersService interface.
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
// -----------------------------------------------------------------
namespace Sitecore.Commerce.Nop.Orders
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using Sitecore.Commerce.Nop.Orders.Models;

    /// <summary>
    /// The orders service interface.
    /// </summary>
    [ServiceContract]
    public interface IOrdersService
    {
        /// <summary>
        /// Submits an order.
        /// </summary>
        /// <param name="customertGuid">The customer ID.</param>
        /// <param name="shippingMethod">The shipping method.</param>
        /// <param name="paymentMethod">The payment method.</param>
        /// <param name="properties">The order properties.</param>
        /// <returns>The submitted order model.</returns>
        [OperationContract]
        OrderModel SubmitOrder(Guid customertGuid, string shippingMethod, string paymentMethod, Dictionary<string, string> properties);

        /// <summary>
        /// Gets an order.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>The order model.</returns>
        [OperationContract]
        OrderModel GetOrder(int orderId, string storeName);

        /// <summary>
        /// Gets a list of customer orders.
        /// </summary>
        /// <param name="customerGuid">The customer ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>The customer orders.</returns>
        [OperationContract]
        IEnumerable<OrderModel> GetOrders(Guid customerGuid, string storeName);

        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>The cancelled order.</returns>
        [OperationContract]
        OrderModel CancelOrder(int orderId, string storeName);
    }
}