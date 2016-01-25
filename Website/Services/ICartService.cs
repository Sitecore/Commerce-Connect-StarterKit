// ----------------------------------------------------------------------------------------------
// <copyright file="ICartService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Provides basic cart operations for the "Autohaus" web-store visitors.
//   This service is aimed to simplify the Test Driven Development (TDD) and
//   allows MVC controllers to use lite version of the cart management API
//   that satisfies their needs.
// </summary>
// ----------------------------------------------------------------------------------------------
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
  using Sitecore;
  using Sitecore.Commerce.Entities.Carts;
  using System;
  using System.Collections.Generic;
  using Entities;
  using Entities.WishLists;

  /// <summary>
  /// Provides basic cart operations for the Autohaus web-store visitors.
  /// This service is aimed to simplify the Test Driven Development (TDD) and 
  /// allows MVC controllers to use lite version of the cart management API
  /// that satisfies their needs.
  /// </summary>
  public interface ICartService
  {
    /// <summary>
    /// Gets the visitor id.
    /// </summary>
    /// <value>The visitor id.</value>
    [NotNull]
    string VisitorId { get; }

    /// <summary>
    /// Gets the shopping cart for visitor.
    /// </summary>
    /// <returns>The visitor's cart.</returns>
    [NotNull]
    Cart GetCart();

    /// <summary>
    /// Adds product to the visitor's cart.
    /// </summary>
    /// <param name="productId">
    /// The product id.
    /// </param>
    /// <param name="quantity">
    /// The quantity.
    /// </param>
    /// <returns>
    /// The <see cref="Cart"/>.
    /// </returns>
    Cart AddToCart([NotNull] string productId, uint quantity);

    /// <summary>
    /// Removes product from the visitor's cart.
    /// </summary>
    /// <param name="externalCartLineId">
    /// The product id.
    /// </param>
    /// <returns>
    /// The <see cref="Cart"/>.
    /// </returns>
    Cart RemoveFromCart([NotNull] string externalCartLineId);

    /// <summary>
    /// Changes the visitor's cart line quantity.
    /// </summary>
    /// <param name="productId">
    /// The product id.
    /// </param>
    /// <param name="quantity">
    /// The quantity.
    /// </param>
    /// <returns>
    /// The <see cref="Cart"/>.
    /// </returns>
    Cart ChangeLineQuantity([NotNull] string productId, uint quantity);

    /// <summary>
    /// Merges the carts.
    /// </summary>
    /// <param name="cartFromAnonymous">The anonymous visitor cart.</param>
    /// <returns>
    /// The <see cref="Cart"/>.
    /// </returns>
    Cart MergeCarts(Cart cartFromAnonymous);

    /// <summary>
    /// Merges the carts.
    /// </summary>
    /// <param name="userCart">The user cart.</param>
    /// <param name="anonymousCart">The anonymous cart.</param>
    /// <returns>
    /// The <see cref="Cart"/>.
    /// </returns>
    Cart MergeCarts([NotNull] Cart userCart, [NotNull] Cart anonymousCart);

    /// <summary>
    /// Add address
    /// </summary>
    /// <param name="party"></param>
    IEnumerable<Party> AddAddress(Party party);

    /// <summary>
    /// Set billing address to cart
    /// </summary>
    /// <param name="cartParty"></param>
    /// <returns></returns>
    bool SetBillingAddressToCart(CartParty cartParty);

    /// <summary>
    /// Set shipping address to cart
    /// </summary>
    /// <param name="cartParty"></param>
    /// <returns></returns>
    bool SetShippingAddressToCart(CartParty cartParty);

    /// <summary>
    /// Get parties
    /// </summary>
    /// <returns>
    /// The parties
    /// </returns>
    IEnumerable<Party> GetAddresses();

    /// <summary>
    /// Get billing adress
    /// </summary>
    /// <returns>
    /// The billing address
    /// </returns>
    Party GetBillingAddress();

    /// <summary>
    /// Get shipping adress
    /// </summary>
    /// <returns>
    /// The shipping address
    /// </returns>
    Party GetShippingAddress();

    /// <summary>
    /// Set shipping method to cart
    /// </summary>
    /// <param name="shippingInfo"></param>
    /// <returns></returns>
    bool SetShippingMethodToCart(ShippingInfo shippingInfo);

    /// <summary>
    /// Set shipping method to cart
    /// </summary>
    /// <param name="paymentInfo"></param>
    /// <returns></returns>
    bool SetPaymentMethodToCart(PaymentInfo paymentInfo);

    /// <summary>
    /// Update parties
    /// </summary>
    /// <param name="parties"></param>
    /// <returns></returns>
    bool UpdateAddresses(List<Party> parties);

    /// <summary>
    /// Set shipping method from cart
    /// </summary>
    /// <returns>ShippingInfo</returns>
    ShippingInfo GetShippingInfo();

    /// <summary>
    /// Get payment method from cart
    /// </summary>
    /// <returns>PaymentInfo</returns>
    PaymentInfo GetPaymentInfo();

    /// <summary>
    /// Gets wishlist for visitor.
    /// </summary>
    /// <returns>The visitor's wishlist.</returns>
    WishList GetWishList();

    /// <summary>
    /// Create wishlist for visitor.
    /// </summary>
    /// <returns></returns>
    WishList CreateWishList();

    /// <summary>
    /// Add to wishlist.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    WishList AddToWishList([NotNull] string productId, uint quantity);

    /// <summary>
    /// Changes the visitor's wishlist line quantity.
    /// </summary>
    /// <param name="productId">
    /// The product id.
    /// </param>
    /// <param name="quantity">
    /// The quantity.
    /// </param>
    /// <returns>
    /// The <see cref="Cart"/>.
    /// </returns>
    WishList ChangeWishlistLineQuantity([NotNull] string productId, uint quantity);

    /// <summary>
    /// Remove line from wishlist
    /// </summary>
    /// <param name="lineId"></param>
    /// <returns></returns>
    WishList RemoveLineFromWishlist([NotNull] string lineId);


    /// <summary>
    /// Merges the wishlists
    /// </summary>
    /// <param name="anonymousWishlist">The anonymous visitor wishlist.</param>
    /// <returns>
    /// The <see cref="Cart"/>.
    /// </returns>
    WishList MergeWishlist(WishList anonymousWishlist);

    /// <summary>
    /// Merges the wishlists
    /// </summary>
    /// <param name="userWishlist"></param>
    /// <param name="anonymousWishlist"></param>
    /// <returns></returns>
    WishList MergeWishlist([NotNull] WishList userWishlist, [NotNull] WishList anonymousWishlist);
  }
}