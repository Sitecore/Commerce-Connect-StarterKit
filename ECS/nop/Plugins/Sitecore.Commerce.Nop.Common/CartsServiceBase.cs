// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartsServiceBase.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the CartsServiceBase class.</summary>
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
namespace Sitecore.Commerce.Nop.Common
{
    using System;
    using System.Linq;
    using global::Nop.Core.Domain.Customers;
    using global::Nop.Core.Domain.Orders;
    using global::Nop.Core.Infrastructure;
    using Sitecore.Commerce.Nop.Common.Exceptions;
    using Sitecore.Commerce.Nop.Common.Helpers;
    using Sitecore.Commerce.Nop.Common.Models;
    using global::Nop.Services.Catalog;
    using global::Nop.Services.Customers;
    using global::Nop.Services.Orders;

    /// <summary>
    /// base class for cart services.
    /// </summary>
    public abstract class CartsServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartsServiceBase"/> class.
        /// </summary>
        public CartsServiceBase()
        {
            this.CustomerService = EngineContext.Current.Resolve<ICustomerService>();
            this.ShoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
            this.OrderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
            this.ProductService = EngineContext.Current.Resolve<IProductService>();
        }

        /// <summary>
        /// Gets the customer service.
        /// </summary>
        protected ICustomerService CustomerService { get; private set; }

        /// <summary>
        /// Gets the order total calculation service.
        /// </summary>
        protected IOrderTotalCalculationService OrderTotalCalculationService { get; private set; }

        /// <summary>
        /// Gets the shopping cart service.
        /// </summary>
        protected IShoppingCartService ShoppingCartService { get; private set; }

        /// <summary>
        /// Gets the product service.
        /// </summary>
        protected IProductService ProductService { get; private set; }

        /// <summary>
        /// Gets or sets the cart type.
        /// </summary>
        protected ShoppingCartType CartType { get; set; }

        /// <summary>
        /// Converts specified customer to shopping cart model.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="storeName">The name of the store.</param>
        /// <returns>
        /// Instance of <see cref="ShoppingCartModel" />
        /// </returns>
        protected ShoppingCartModel GetModel(Customer customer, string storeName = null)
        {
            var model = new ShoppingCartModel();
            model.Map(customer, this.CartType, storeName);

            return model;
        }

        /// <summary>
        /// get carts.
        /// </summary>
        /// <returns>All custommer carts.</returns>
        protected virtual IQueryable<ShoppingCartModel> GetCarts()
        {
            return this.CustomerService.GetAllCustomers(loadOnlyWithShoppingCart: true, sct: this.CartType)
              .Select(x => this.GetModel(x))
              .AsQueryable();
        }

        /// <summary>
        /// get cart.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>The cart model.</returns>
        protected virtual ShoppingCartModel GetCart(Guid customerId, string storeName = null)
        {
            // Gets existing or creates new customer with specified customer id.
            var customer = this.CustomerService.GetCustomerByGuid(customerId) ?? CustomerHelper.CreateCustomer(customerId, this.CustomerService);

            // Returns shopping cart models for specified customer.
            return this.GetModel(customer, storeName);
        }

        /// <summary>
        /// Creates a cart.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>The cart model.</returns>
        protected virtual ShoppingCartModel CreateCart(Guid customerId, string storeName = null)
        {
            // Gets customer by specified customer id.
            Customer customer = this.CustomerService.GetCustomerByGuid(customerId);

            // Creates customer with specified customer id if customer was not found.
            if (customer == null)
            {
                customer = CustomerHelper.CreateCustomer(customerId, this.CustomerService);
            }

            return this.GetModel(customer, storeName);
        }

        /// <summary>
        /// Deletes the cart entity by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        protected void DeleteCart(Guid customerId)
        {
            var customer = this.CustomerService.GetCustomerByGuid(customerId);

            if (customer == null)
            {
                return;
            }

            var cartEntityItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == this.CartType).ToList();

            foreach (var cartItem in cartEntityItems)
            {
                this.ShoppingCartService.DeleteShoppingCartItem(cartItem);
            }
        }

        /// <summary>
        /// Updates a cart product quantity.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="externalProductId">The product external ID.</param>
        /// <param name="newQuantity">The new product quantity.</param>
        /// <returns>The updated cart model.</returns>
        protected ShoppingCartModel UpdateQuantity(Guid customerId, string externalProductId, int newQuantity)
        {
            int productId;
            if (int.TryParse(externalProductId, out productId))
            {
                var product = this.ProductService.GetProductById(productId);
                if (product == null)
                {
                    return this.GetCart(customerId);
                }

                var currentCustomer = this.CustomerService.GetCustomerByGuid(customerId);
                if (currentCustomer == null)
                {
                    throw new CustomerNotExistException(customerId);
                }

                // Gets cart for current customer.
                var cart = currentCustomer
                  .ShoppingCartItems
                  .Where(sci => sci.ShoppingCartType == this.CartType)
                  .ToList();

                // Performs search for cart item that should be changed.
                var shoppingCartItem = this.ShoppingCartService.FindShoppingCartItemInTheCart(cart, this.CartType, product);

                // Updates quantity of specified cart item.
                if (shoppingCartItem != null)
                {
                    this.ShoppingCartService.UpdateShoppingCartItem(currentCustomer, shoppingCartItem.Id, newQuantity, true);
                }
            }

            return this.GetCart(customerId);
        }

        /// <summary>
        /// Adds a product to the cart.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="externalProductId">The product external ID.</param>
        /// <param name="quantity">The product quantity.</param>
        /// <returns>The updated cart model.</returns>
        protected virtual ShoppingCartModel AddProduct(Guid customerId, string externalProductId, uint quantity)
        {
            int productId;

            if (int.TryParse(externalProductId, out productId))
            {
                var product = this.ProductService.GetProductById(productId);
                var currentCustomer = this.CustomerService.GetCustomerByGuid(customerId);

                if (currentCustomer == null)
                {
                    throw new CustomerNotExistException(customerId);
                }

                if (product == null)
                {
                    return this.GetModel(currentCustomer);
                }

                // Gets wishlistt for current customer.
                var cart = currentCustomer
                          .ShoppingCartItems
                          .Where(sci => sci.ShoppingCartType == this.CartType)
                          .ToList();

                // Performs search for wishlist item that should be changed.
                var cartItem = this.ShoppingCartService.FindShoppingCartItemInTheCart(cart, this.CartType, product);

                // Gets overall quantity of wishlist item.
                var quantityToValidate = cartItem != null ? cartItem.Quantity + quantity : quantity;

                var storeId = cartItem != null ? cartItem.StoreId : 0;

                // Checks that it is possible to set specified overall quantity.
                var addToCartWarnings = this.ShoppingCartService.GetShoppingCartItemWarnings(currentCustomer, this.CartType, product, storeId, string.Empty, decimal.Zero, (int)quantityToValidate, false, true, false, false, false);

                // If it is not possible to set specified quantity skip processing.
                if (addToCartWarnings.Count != 0)
                {
                    return this.GetModel(currentCustomer);
                }

                // Adds specified product in specified quantity to wishlist of specified customer.
                this.ShoppingCartService.AddToCart(currentCustomer, product, this.CartType, storeId, string.Empty, decimal.Zero, (int)quantity, true);
            }

            return this.GetCart(customerId);
        }

        /// <summary>
        /// Removes a product from the cart.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="externalProductId">The product external ID.</param>
        /// <returns>The updated cart model.</returns>
        protected virtual ShoppingCartModel RemoveProduct(Guid customerId, string externalProductId)
        {
            int productId;

            if (int.TryParse(externalProductId, out productId))
            {
                var product = this.ProductService.GetProductById(productId);
                var currentCustomer = this.CustomerService.GetCustomerByGuid(customerId);

                if (currentCustomer == null)
                {
                    throw new CustomerNotExistException(customerId);
                }

                if (product == null)
                {
                    return this.GetModel(currentCustomer);
                }

                // Gets cart for current customer.
                var cart = currentCustomer
                          .ShoppingCartItems
                          .Where(sci => sci.ShoppingCartType == this.CartType)
                          .ToList();

                // Performs search for cart item that should be changed.
                var shoppingCartItem = this.ShoppingCartService.FindShoppingCartItemInTheCart(cart, this.CartType, product);

                // Deletes specified shopping cart item.
                if (shoppingCartItem != null)
                {
                    this.ShoppingCartService.DeleteShoppingCartItem(shoppingCartItem);
                }
            }

            return this.GetCart(customerId);
        }
    }
}