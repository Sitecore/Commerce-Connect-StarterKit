// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the OrderService class.</summary>
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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Sitecore.Commerce.Contacts;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Commerce.Services.Orders;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The orders service.
    /// </summary>
    public class OrderService : IOrderService
    {
        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly OrderServiceProvider serviceProvider;

        private readonly ContactFactory contactFactory;

        /// <summary>
        /// The cart service
        /// </summary>
        private readonly ICartService cartService;

        private readonly string shopName;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The orders service provider.</param>
        /// <param name="contactFactory">The contact factory.</param>
        /// <param name="cartService">The carts service.</param>
        /// <param name="shopName">The shop name.</param>
        public OrderService(
            [NotNull] OrderServiceProvider serviceProvider,
            [NotNull] ContactFactory contactFactory,
            [NotNull] ICartService cartService,
            [NotNull] string shopName)
        {
            Assert.ArgumentNotNull(serviceProvider, "serviceProvider");
            Assert.ArgumentNotNull(contactFactory, "contactFactory");
            Assert.ArgumentNotNull(cartService, "cartService");
            Assert.ArgumentNotNullOrEmpty(shopName, "shopName");

            this.serviceProvider = serviceProvider;
            this.contactFactory = contactFactory;
            this.cartService = cartService;
            this.shopName = shopName;
        }

        /// <summary>
        /// Sets The Payment Information to the cart and submits an order.
        /// </summary>
        /// <param name="paymentInformation">The payment information.</param>
        /// <param name="paymentMethod">The payment method.</param>
        /// <returns>The order ID.</returns>
        public string SetPaymentInformationAndSubmit(PropertyCollection paymentInformation, string paymentMethod)
        {
            var currentCart = this.cartService.GetCart();

            currentCart.Payment = new ReadOnlyCollection<PaymentInfo>(new PaymentInfo[]
            {
                new PaymentInfo()
                {
                    ExternalId = currentCart.ExternalId, 
                    PaymentMethodID = paymentMethod, 
                    Properties = paymentInformation
                }
            });

            var result = this.serviceProvider.SubmitVisitorOrder(new SubmitVisitorOrderRequest(currentCart));

            if (result != null)
            {
                return result.Order.OrderID;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the visitor orders.
        /// </summary>
        /// <returns>The visitor orders.</returns>
        public IReadOnlyCollection<OrderHeader> GetOrders()
        {
            var customerId = this.cartService.GetCart().ExternalId;

            var ordersResult = this.serviceProvider.GetVisitorOrders(new GetVisitorOrdersRequest(customerId, this.shopName));

            return ordersResult.OrderHeaders;
        }
    }
}