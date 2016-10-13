// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShoppingCartModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Class for Nop shopping cart data
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
namespace Sitecore.Commerce.Nop.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using global::Nop.Core.Domain.Common;
    using global::Nop.Core.Domain.Customers;
    using global::Nop.Core.Domain.Orders;
    using global::Nop.Core.Domain.Shipping;
    using global::Nop.Core.Infrastructure;
    using global::Nop.Services.Common;
    using global::Nop.Services.Orders;
    using global::Nop.Services.Stores;

    /// <summary>
    /// Class for Nop shopping cart data
    /// </summary>
    [DataContract]
    public class ShoppingCartModel
    {
        /// <summary>
        /// The order total calculation service.
        /// </summary>
        protected readonly IOrderTotalCalculationService OrderTotalCalculationService;

        /// <summary>
        /// The store service
        /// </summary>
        private readonly IStoreService storeService;

        /// <summary>
        /// The generic attribute service
        /// </summary>
        private readonly IGenericAttributeService genericAttributeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartModel"/> class.
        /// </summary>
        public ShoppingCartModel()
        {
            this.OrderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
            this.storeService = EngineContext.Current.Resolve<IStoreService>();
            this.genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        }

        /// <summary>
        /// Gets or sets the store ID.
        /// </summary>
        [DataMember]
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the total item count.
        /// </summary>
        [DataMember]
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the customer ID.
        /// </summary>
        [DataMember]
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer GUID.
        /// </summary>
        [DataMember]
        public Guid CustomerGuid { get; set; }

        /// <summary>
        /// Gets or sets the cart total.
        /// </summary>
        [DataMember]
        public decimal? Total { get; set; }

        /// <summary>
        /// Gets or sets the cart items.
        /// </summary>
        [DataMember]
        public IList<ShoppingCartItemModel> ShoppingItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cart belongs to an anonymous users.
        /// </summary>
        [DataMember]
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// Gets or sets the customer email address.
        /// </summary>
        [DataMember]
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cart is deleted.
        /// </summary>
        [DataMember]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the cart status.
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the shipping address ID.
        /// </summary>
        [DataMember]
        public int? ShippingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        [DataMember]
        public AddressModel ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping name.
        /// </summary>
        [DataMember]
        public string ShippingName { get; set; }

        /// <summary>
        /// Gets or sets the billing address ID.
        /// </summary>
        [DataMember]
        public int? BillingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        [DataMember]
        public AddressModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the billing name.
        /// </summary>
        [DataMember]
        public string BillingName { get; set; }

        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        [DataMember]
        public ICollection<AddressModel> Addresses { get; set; }

        /// <summary>
        /// Gets or sets shipments.
        /// </summary>
        [DataMember]
        public ICollection<ShipmentModel> Shipments { get; set; }

        /// <summary>
        /// Gets or sets the discount amount.
        /// </summary>
        [DataMember]
        public decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets the payment method.
        /// </summary>
        [DataMember]
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the payment status.
        /// </summary>
        [DataMember]
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Gets or sets the shipping method.
        /// </summary>
        [DataMember]
        public string ShippingMethod { get; set; }

        /// <summary>
        /// Gets or sets the shipping status.
        /// </summary>
        [DataMember]
        public string ShippingStatus { get; set; }

        /// <summary>
        /// Gets or sets the payment information.
        /// </summary>
        [DataMember]
        public PaymentInfoModel PaymentInfo { get; set; }

        /// <summary>
        /// Gets or sets the shipping information.
        /// </summary>
        [DataMember]
        public ShippingMethodModel ShippingInfo { get; set; }

        /// <summary>
        /// Maps NOP data insto this model.
        /// </summary>
        /// <param name="customer">The NOP customer.</param>
        /// <param name="cartType">The cart type.</param>
        /// <param name="storeName">The store name.</param>
        public void Map(Customer customer, ShoppingCartType cartType, string storeName = null)
        {
            this.CustomerGuid = customer.CustomerGuid;
            this.CustomerId = customer.Id;
            this.Total = this.OrderTotalCalculationService.GetShoppingCartTotal(customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == cartType).ToList(), true, false);
            this.CustomerEmail = customer.IsGuest() ? string.Empty : customer.Email;
            this.ShoppingItems = this.GetShoppingItemsModels(customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == cartType).ToList());
            this.IsAnonymous = customer.IsGuest();
            this.IsDeleted = customer.Deleted;
            this.ShippingAddressId = (customer.ShippingAddress != null) ? customer.ShippingAddress.Id : (int?)null;
            if (customer.ShippingAddress != null)
            {
                this.ShippingName = customer.ShippingAddress.FirstName + " " + customer.ShippingAddress.LastName;
                var model = new AddressModel();
                model.Map(customer.ShippingAddress);
                this.ShippingAddress = model;
            }

            this.BillingAddressId = (customer.BillingAddress != null) ? customer.BillingAddress.Id : (int?)null;
            if (customer.BillingAddress != null)
            {
                this.BillingName = customer.BillingAddress.FirstName + " " + customer.BillingAddress.LastName;
                var model = new AddressModel();
                model.Map(customer.BillingAddress);
                this.BillingAddress = model;
            }

            this.Addresses = this.GetAddressModels(customer.Addresses);

            /////////////////////
            this.StoreId = this.GetStoreId(storeName);
            this.PaymentInfo = this.GetPaymentInfoModel(customer, this.StoreId);
            this.ShippingInfo = this.GetShippingMethodModel(customer, this.StoreId);
        }

        /// <summary>
        /// Get shopping item models.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The shopping cart item models.</returns>
        protected IList<ShoppingCartItemModel> GetShoppingItemsModels(ICollection<ShoppingCartItem> items)
        {
            var cartItems = new List<ShoppingCartItemModel>();

            foreach (ShoppingCartItem item in items)
            {
                if (item.Product != null)
                {
                    cartItems.Add(new ShoppingCartItemModel
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

            return cartItems;
        }

        /// <summary>
        /// get address models.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <returns>The address models.</returns>
        protected IList<AddressModel> GetAddressModels(ICollection<Address> addresses)
        {
            var models = new List<AddressModel>();

            foreach (Address address in addresses)
            {
                var model = new AddressModel();
                model.Map(address);
                models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// Gets a store ID.
        /// </summary>
        /// <param name="storeName">the store name.</param>
        /// <returns>The store name.</returns>
        protected int GetStoreId(string storeName)
        {
            int storeId;

            if (string.IsNullOrEmpty(storeName))
            {
                storeId = 0;
            }
            else
            {
                var store =
                  this.storeService.GetAllStores()
                    .FirstOrDefault(s => s.Name.Equals(storeName, StringComparison.InvariantCultureIgnoreCase));

                storeId = store == null ? 0 : store.Id;
            }

            return storeId;
        }

        /// <summary>
        /// get payment info model.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="storeId">The store ID.</param>
        /// <returns>The payment info model.</returns>
        protected PaymentInfoModel GetPaymentInfoModel(Customer customer, int storeId)
        {
            string selectedPaymentMethodSystemName = null;

            try
            {
                selectedPaymentMethodSystemName = customer.GetAttribute<string>(SystemCustomerAttributeNames.SelectedPaymentMethod, this.genericAttributeService, storeId);
            }
            catch
            {
                // ignored
            }

            var paymentMethod = !String.IsNullOrEmpty(selectedPaymentMethodSystemName) ? new PaymentInfoModel { SystemName = selectedPaymentMethodSystemName } : null;

            return paymentMethod;
        }

        /// <summary>
        /// get shipping method model.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="storeId">The store ID.</param>
        /// <returns>The shipping method model.</returns>
        protected ShippingMethodModel GetShippingMethodModel(Customer customer, int storeId)
        {
            ShippingOption selectedShippingOption = null;

            try
            {
                selectedShippingOption = customer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, storeId);
            }
            catch
            {
                // ignored
            }

            var shippingOption = selectedShippingOption != null ?
              new ShippingMethodModel
              {
                  SystemName = selectedShippingOption.ShippingRateComputationMethodSystemName,
                  Name = selectedShippingOption.Name,
                  Description = selectedShippingOption.Description
              }
              : null;

            return shippingOption;
        }
    }
}