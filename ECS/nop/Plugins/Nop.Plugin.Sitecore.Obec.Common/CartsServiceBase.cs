// -----------------------------------------------------------------
// <copyright file="CartsServiceBase.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The CartsServiceBase class.
// </summary>
// -----------------------------------------------------------------
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
namespace Nop.Plugin.Sitecore.Commerce.Common
{
  using System;
  using System.Linq;
  using Nop.Core.Domain.Customers;
  using Nop.Core.Domain.Orders;
  using Nop.Core.Infrastructure;
  using Nop.Plugin.Sitecore.Commerce.Common.Exceptions;
  using Nop.Plugin.Sitecore.Commerce.Common.Helpers;
  using Nop.Plugin.Sitecore.Commerce.Common.Models;
  using Nop.Services.Catalog;
  using Nop.Services.Customers;
  using Nop.Services.Orders;

  public abstract class CartsServiceBase
  {
    /// <summary>
    /// The customer service.
    /// </summary>
    protected readonly ICustomerService customerService;

    /// <summary>
    /// The order total calculation service.
    /// </summary>
    protected readonly IOrderTotalCalculationService orderTotalCalculationService;

    /// <summary>
    /// The shopping cart service.
    /// </summary>
    protected readonly IShoppingCartService shoppingCartService;

    /// <summary>
    /// The product service.
    /// </summary>
    protected readonly IProductService productService;


    protected ShoppingCartType CartType { get; set; }

    
    public CartsServiceBase()
    {
      this.customerService = EngineContext.Current.Resolve<ICustomerService>();
      this.shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
      this.orderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
      this.productService = EngineContext.Current.Resolve<IProductService>();
    }


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


    protected virtual IQueryable<ShoppingCartModel> GetCarts()
    {
      return this.customerService.GetAllCustomers(loadOnlyWithShoppingCart: true, sct: this.CartType)
        .Select(x=> this.GetModel(x))
        .AsQueryable();
    }

    protected virtual ShoppingCartModel GetCart(Guid customerId, string storeName = null)
    {
      // Gets existing or creates new customer with specified customer id.
      var customer = this.customerService.GetCustomerByGuid(customerId) ?? CustomerHelper.CreateCustomer(customerId, this.customerService);

      // Returns shopping cart models for specified customer.
      return this.GetModel(customer, storeName);
    }

    protected virtual ShoppingCartModel CreateCart(Guid customerId, string storeName = null)
    {
      // Gets customer by specified customer id.
      Customer customer = this.customerService.GetCustomerByGuid(customerId);

      // Creates customer with specified customer id if customer was not found.
      if (customer == null)
      {
        customer = CustomerHelper.CreateCustomer(customerId, this.customerService);
      }

      return this.GetModel(customer, storeName);
    }

    /// <summary>
    /// Deletes the cart entity by customer id.
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    protected void DeleteCart(Guid customerId)
    {
      var customer = this.customerService.GetCustomerByGuid(customerId);

      if (customer == null)
      {
        return;
      }

      var cartEntityItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == this.CartType).ToList();

      foreach (var cartItem in cartEntityItems)
      {
        this.shoppingCartService.DeleteShoppingCartItem(cartItem);
      }
    }

    protected ShoppingCartModel UpdateQuantity(Guid customerId, string externalProductId, int newQuantity)
    {
      int productId;
      if (int.TryParse(externalProductId, out productId))
      {
        var product = this.productService.GetProductById(productId);
        if (product == null)
        {
          return this.GetCart(customerId);
        }

        var currentCustomer = this.customerService.GetCustomerByGuid(customerId);
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
        var shoppingCartItem = this.shoppingCartService.FindShoppingCartItemInTheCart(cart, this.CartType, product);

        // Updates quantity of specified cart item.
        if (shoppingCartItem != null)
        {
          this.shoppingCartService.UpdateShoppingCartItem(currentCustomer, shoppingCartItem.Id, newQuantity, true);
        }
      }

      return this.GetCart(customerId);
    }

    protected virtual ShoppingCartModel AddProduct(Guid customerId, string externalProductId, uint quantity)
    {
      int productId;

      if (int.TryParse(externalProductId, out productId))
      {
        var product = this.productService.GetProductById(productId);
        var currentCustomer = this.customerService.GetCustomerByGuid(customerId);

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
        var cartItem = this.shoppingCartService.FindShoppingCartItemInTheCart(cart, this.CartType, product);

        // Gets overall quantity of wishlist item.
        var quantityToValidate = cartItem != null ? cartItem.Quantity + quantity : quantity;

        var storeId = cartItem != null ? cartItem.StoreId : 0;

        // Checks that it is possible to set specified overall quantity.
        var addToCartWarnings = this.shoppingCartService.GetShoppingCartItemWarnings(currentCustomer, this.CartType, product, storeId, string.Empty, decimal.Zero, (int)quantityToValidate, false, true, false, false, false);

        // If it is not possible to set specified quantity skip processing.
        if (addToCartWarnings.Count != 0)
        {
          return this.GetModel(currentCustomer);
        }

        // Adds specified product in specified quantity to wishlist of specified customer.
        this.shoppingCartService.AddToCart(currentCustomer, product, this.CartType, storeId, string.Empty, decimal.Zero, (int)quantity, true);
      }

      return this.GetCart(customerId);
    }

    protected virtual ShoppingCartModel RemoveProduct(Guid customerId, string externalProductId)
    {
      int productId;

      if (int.TryParse(externalProductId, out productId))
      {
        var product = this.productService.GetProductById(productId);
        var currentCustomer = this.customerService.GetCustomerByGuid(customerId);

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
        var shoppingCartItem = this.shoppingCartService.FindShoppingCartItemInTheCart(cart, this.CartType, product);

        // Deletes specified shopping cart item.
        if (shoppingCartItem != null)
        {
          this.shoppingCartService.DeleteShoppingCartItem(shoppingCartItem);
        }
      }

      return this.GetCart(customerId);
    }
  }
}