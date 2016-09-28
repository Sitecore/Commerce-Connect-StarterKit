// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartExtensions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The cart extensions.
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
// ---------------------------------------------------------------------

namespace Sitecore.Commerce.Connectors.NopCommerce
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Diagnostics;

  /// <summary>
  /// The cart extensions.
  /// </summary>
  public static class CartExtensions
  {
    /// <summary>
    /// Converts NopCommerce cart line model to the Commerce cart line.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The CartLine.</returns>
    [NotNull]
    public static CartLine ToCommerceCartLine([NotNull] this ShoppingCartItemModel model)
    {
      Assert.ArgumentNotNull(model, "model");
      return new CartLine
               {
                 ExternalCartLineId = model.Id,
                 Product = new CartProduct { ProductId = model.ProductId.ToString(CultureInfo.InvariantCulture), Price = new Price { Amount = model.Price } },
                 Quantity = model.Quantity,
                 Total = new Total { Amount = model.LineTotal }
               };
    }

    /// <summary>
    /// Maps cart from model.
    /// </summary>
    /// <param name="cart">The cart.</param>
    /// <param name="cartModel">The cart model.</param>
    public static void MapCartFromModel([NotNull] this Cart cart, [NotNull] ShoppingCartModel cartModel)
    {
      Assert.ArgumentNotNull(cart, "cart");
      Assert.ArgumentNotNull(cartModel, "cartModel");

      cart.MapCartBaseFromModel(cartModel);
      
      var cartlines = new List<CartLine>();
      foreach (var cartItemModel in cartModel.ShoppingItems)
      {
        cartlines.Add(cartItemModel.ToCommerceCartLine());
      }

      cart.Lines = cartlines.AsReadOnly();

      if (cartModel.Total != null && cartModel.Total.Value != 0)
      {
        cart.Total = new Total { Amount = cartModel.Total.Value };
      }

      if (cartModel.ShippingInfo != null)
      {
        cart.Shipping = new ReadOnlyCollection<ShippingInfo>(new[]
        {
          new ShippingInfo
          {
            ShippingMethodID = cartModel.ShippingInfo.Name,
            ShippingProviderID = cartModel.ShippingInfo.SystemName
          }
        });
      }

      if (cartModel.PaymentInfo != null)
      {
        cart.Payment = new ReadOnlyCollection<PaymentInfo>(new[]
        {
          new PaymentInfo
          {
            PaymentMethodID = cartModel.PaymentInfo.MethodName,
            PaymentProviderID = cartModel.PaymentInfo.SystemName
          }
        });
      }
    }

    /// <summary>
    /// Maps the cart base from model.
    /// </summary>
    /// <param name="cart">The cart.</param>
    /// <param name="cartModel">The cart model.</param>
    public static void MapCartBaseFromModel([NotNull] this CartBase cart, [NotNull] ShoppingCartModel cartModel)
    {
      //cart.CustomerId = cartModel.CustomerId.ToString();
      cart.ExternalId = cartModel.CustomerGuid.ToString("B").ToUpper();
      cart.AccountingCustomerParty = new CartParty
      {
        ExternalId = cartModel.BillingAddressId.HasValue ? cartModel.BillingAddressId.Value.ToString() : null,
        PartyID = cartModel.BillingAddressId.HasValue ? cartModel.BillingAddressId.Value.ToString() : null
      };

      cart.BuyerCustomerParty = new CartParty
      {
        ExternalId = cartModel.ShippingAddressId.HasValue ? cartModel.ShippingAddressId.Value.ToString() : null,
        PartyID = cartModel.ShippingAddressId.HasValue ? cartModel.ShippingAddressId.ToString() : null
      };
    }
  }
}