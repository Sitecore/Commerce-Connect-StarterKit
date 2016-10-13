// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the OrderModel class.
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
namespace Sitecore.Commerce.Nop.Orders.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Nop.Core.Domain.Orders;
    using Sitecore.Commerce.Nop.Common.Models;
    using System.Runtime.Serialization;
    using global::Nop.Core.Domain.Shipping;

    /// <summary>
    /// Class for Nop shopping cart data
    /// </summary>
    [DataContract]
    public class OrderModel : ShoppingCartModel
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the order GUID
        /// </summary>
        [DataMember]
        public Guid OrderGuid { get; set; }

        /// <summary>
        /// Gets or sets the order name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the order created date.
        /// </summary>
        [DataMember]
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the order items.
        /// </summary>
        [DataMember]
        public IList<ShoppingCartItemModel> OrderItems { get; set; }

        //// Payment fields

        /// <summary>
        /// Gets or sets the card type.
        /// </summary>
        [DataMember(Name = "CardType")]
        public string CardType { get; set; }

        /// <summary>
        /// Gets or sets card name.
        /// </summary>
        [DataMember(Name = "CardName")]
        public string CardName { get; set; }

        /// <summary>
        /// Gets or sets the card number.
        /// </summary>
        [DataMember(Name = "CardNumber")]
        public string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the card expiration month.
        /// </summary>
        [DataMember(Name = "CardExpirationMonth")]
        public string CardExpirationMonth { get; set; }

        /// <summary>
        /// Gets or sets the card expiration year.
        /// </summary>
        [DataMember(Name = "CardExpirationYear")]
        public string CardExpirationYear { get; set; }

        /// <summary>
        /// Gets or sets the card CVV
        /// </summary>
        [DataMember(Name = "CardCvv2")]
        public string CardCvv2 { get; set; }

        /// <summary>
        /// Maps a NOP order into this Connect model.
        /// </summary>
        /// <param name="order">The NOP order.</param>
        /// <param name="storeName">The store name.</param>
        public void Map(Order order, string storeName = null)
        {
            base.Map(order.Customer, ShoppingCartType.ShoppingCart, storeName);

            var shipments = this.GetOrderShipmentModels(order.Shipments);

            this.Id = order.Id;
            this.OrderGuid = order.OrderGuid;
            this.CardType = order.CardType;
            this.IsDeleted = order.Deleted;
            this.Status = order.OrderStatus.ToString();
            this.ShippingAddressId = order.ShippingAddressId;
            this.CreatedDateTime = order.CreatedOnUtc;

            this.OrderItems = this.GetShoppingItemsModels(order.OrderItems);

            if (order.ShippingAddress != null)
            {
                this.ShippingName = order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName;
            }

            this.BillingAddressId = order.BillingAddressId;
            if (order.BillingAddress != null)
            {
                this.BillingName = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;
            }

            this.Shipments = shipments;
            this.Discount = order.OrderDiscount;
            this.PaymentMethod = order.PaymentMethodSystemName;
            this.PaymentStatus = order.PaymentStatus.ToString();
            this.ShippingMethod = order.ShippingMethod;
            this.ShippingStatus = order.ShippingStatus.ToString();
        }

        /// <summary>
        /// Gets the shopping cart item modlels.
        /// </summary>
        /// <param name="items">The order items.</param>
        /// <returns>The cart item models.</returns>
        protected IList<ShoppingCartItemModel> GetShoppingItemsModels(ICollection<OrderItem> items)
        {
            var orderItems = new List<ShoppingCartItemModel>();

            foreach (OrderItem item in items)
            {
                if (item.Product != null)
                {
                    orderItems.Add(new ShoppingCartItemModel
                    {
                        Id = item.Id.ToString(),
                        ProductId = item.ProductId,
                        Price = item.Product.Price,
                        LineTotal = item.Product.Price * (uint)item.Quantity,
                        Sku = item.Product.Sku,
                        Quantity = (uint)item.Quantity
                    });
                }
            }

            return orderItems;
        }

        /// <summary>
        /// gets the order shipments.
        /// </summary>
        /// <param name="shipments">The list of NOP shipments.</param>
        /// <returns>The Connect shipments.</returns>
        protected IList<ShipmentModel> GetOrderShipmentModels(ICollection<Shipment> shipments)
        {
            var models = new List<ShipmentModel>();

            foreach (Shipment shipment in shipments)
            {
                models.Add(new ShipmentModel
                {
                    Id = shipment.Id,
                    ItemsIDs = shipment.ShipmentItems.Select(i => i.OrderItemId).ToList(),
                    ShippingAddressId = shipment.Order.ShippingAddressId,
                    ShippingMethod = shipment.Order.ShippingMethod
                });
            }

            return models;
        }
    }
}