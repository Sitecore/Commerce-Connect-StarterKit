//-----------------------------------------------------------------------
// <copyright file="CheckoutService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The CheckoutService class.</summary>
//-----------------------------------------------------------------------
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Commerce.StarterKit.Services
{
  using Commerce.Services.Customers;
  using Commerce.Services.Payments;
  using Commerce.Services.Shipping;
  using Diagnostics;
  using Entities;
  using Entities.Customers;
  using Entities.Payments;
  using Entities.Shipping;

  /// <summary>
  /// The checkout service.
  /// </summary>
  public class CheckoutService : ICheckoutService
  {

    /// <summary>
    /// The shipping service provider.
    /// </summary>
    private readonly ShippingServiceProvider _shippingServiceProvider;

    /// <summary>
    /// The payment service provider
    /// </summary>
    private readonly PaymentServiceProvider _paymentServiceProvider;

    /// <summary>
    /// The customer service provider
    /// </summary>
    private readonly CustomerServiceProvider _customerServiceProvider;

    /// <summary>
    /// The cart service
    /// </summary>
    private readonly ICartService _cartService;

    /// <summary>
    /// The shop name.
    /// </summary>
    private string shopName;

    /// <summary>
    /// Gets or sets the name of the shop.
    /// </summary>
    /// <value>The name of the shop.</value>
    [NotNull]
    public string ShopName
    {
      get { return this.shopName; }
      set { this.shopName = value; }
    }

    /// <summary>
    /// Initializes a new instance
    /// </summary>
    /// <param name="shippingServiceProvider"></param>
    /// <param name="paymentServiceProvider"></param>
    /// <param name="cartService"></param>
    /// <param name="shopName"></param>
    /// <param name="customerServiceProvider"></param>
    public CheckoutService([NotNull]ShippingServiceProvider shippingServiceProvider, 
      [NotNull] PaymentServiceProvider paymentServiceProvider, 
      [NotNull] CustomerServiceProvider customerServiceProvider,
      [NotNull] ICartService cartService,
      [NotNull] string shopName)
    {
      Assert.ArgumentNotNull(shippingServiceProvider, "shippingServiceProvider");
      Assert.ArgumentNotNull(paymentServiceProvider, "paymentServiceProvider");
      Assert.ArgumentNotNull(customerServiceProvider, "customerServiceProvider");
      Assert.ArgumentNotNull(cartService, "cartService");
      Assert.ArgumentNotNullOrEmpty(shopName, "shopName");
      
      _shippingServiceProvider = shippingServiceProvider;
      _paymentServiceProvider = paymentServiceProvider;
      _customerServiceProvider = customerServiceProvider;
      _cartService = cartService;
      this.shopName = shopName;
    }

    /// <summary>
    /// Get shipping options
    /// </summary>
    /// <returns></returns>
    [NotNull]
    public IEnumerable<ShippingOption> GetShippingOptions()
    {
      var result = _shippingServiceProvider.GetShippingOptions(new GetShippingOptionsRequest());
      if (result == null)
      {
        return Enumerable.Empty<ShippingOption>();
      }
      return result.ShippingOptions ?? Enumerable.Empty<Entities.Shipping.ShippingOption>();
    }

    /// <summary>
    /// Get payment options
    /// </summary>
    /// <returns></returns>
    [NotNull]
    public IEnumerable<Entities.Payments.PaymentOption> GetPaymentOptions()
    {
      var result = _paymentServiceProvider.GetPaymentOptions(new GetPaymentOptionsRequest(this.ShopName));
      if (result == null)
      {
        return Enumerable.Empty<Entities.Payments.PaymentOption>();
      }
      return result.PaymentOptions ?? Enumerable.Empty<Entities.Payments.PaymentOption>();
    }

    /// <summary>
    /// Get shipping methods
    /// </summary>
    /// <param name="shippingOption"></param>
    /// <returns></returns>
    [NotNull]
    public IEnumerable<Entities.Shipping.ShippingMethod> GetShippingMethods([NotNull]ShippingOption shippingOption)
    {
      var result = _shippingServiceProvider.GetShippingMethods(new GetShippingMethodsRequest(shippingOption)
      {
        Properties = new PropertyCollection()
        {
          new PropertyItem()
          {
            Key = "cart",
            Value = _cartService.GetCart()
          }
        }
      });

      if (!result.Success)
      {
        return Enumerable.Empty<Entities.Shipping.ShippingMethod>();
      }

      return result.ShippingMethods ?? Enumerable.Empty<Entities.Shipping.ShippingMethod>();
    }

    /// <summary>
    /// Get shipping methods
    /// </summary>
    /// <param name="paymentOption"></param>
    /// <returns></returns>
    [NotNull]
    public IEnumerable<PaymentMethod> GetPaymentMethods([NotNull]PaymentOption paymentOption)
    {
      paymentOption.ShopName = this.ShopName;

      var result = _paymentServiceProvider.GetPaymentMethods(new GetPaymentMethodsRequest(paymentOption));
      if (result == null || !result.Success)
      {
        return Enumerable.Empty<PaymentMethod>();
      }

      return result.PaymentMethods ?? Enumerable.Empty<PaymentMethod>();
    }
  }
}