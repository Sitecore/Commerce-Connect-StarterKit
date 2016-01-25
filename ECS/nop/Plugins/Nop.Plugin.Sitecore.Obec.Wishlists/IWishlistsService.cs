// -----------------------------------------------------------------
// <copyright file="ICartService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The CartService interface.
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
namespace Nop.Plugin.Sitecore.Commerce.Wishlists
{
  using System;
  using System.Linq;
  using System.ServiceModel;
  using Common.Models;

  /// <summary>
  /// The CartService interface.
  /// </summary>
  [ServiceContract]
  public interface IWishlistsService
  {
    /// <summary>
    /// Gets the carts.
    /// </summary>
    /// <returns>List of <see cref="ShoppingCartModel"/></returns>
    [OperationContract]
    IQueryable<ShoppingCartModel> GetWishlists();

    /// <summary>
    /// Gets the wishlist by customer id.
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    /// <returns>List of <see cref="ShoppingCartModel"/></returns>
    [OperationContract]
    ShoppingCartModel GetWishlist(Guid customerId);

    /// <summary>
    /// Deletes the wishlist by customer id.
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    [OperationContract]
    void DeleteWishlist(Guid customerId);

    /// <summary>
    /// Creates the wishlist by customer id.
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    [OperationContract]
    ShoppingCartModel CreateWishlist(Guid customerId);

    /// <summary>
    /// Adds the product by product variant id to wishlist.
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    /// <param name="externalProductId">The external product id.</param>
    /// <param name="quantity">The quantity.</param>
    [OperationContract]
    ShoppingCartModel AddProduct(Guid customerId, string externalProductId, uint quantity);

    /// <summary>
    /// Removes the product by product variant id from wishlist.
    /// </summary>
    /// <param name="customerId">The customer Id.</param>
    /// <param name="externalProductId">The external product id.</param>
    [OperationContract]
    ShoppingCartModel RemoveProduct(Guid customerId, string externalProductId);

    /// <summary>
    /// Updates the quantity by external product id on cart.
    /// </summary>
    /// <param name="customerId">The customer Id.</param>
    /// <param name="externalProductId">The external product id.</param>
    /// <param name="newQuantity">The new quantity.</param>
    /// <returns>Instance of <see cref="ShoppingCartModel"/></returns>
    [OperationContract]
    ShoppingCartModel UpdateQuantity(Guid customerId, string externalProductId, int newQuantity);
  }
}