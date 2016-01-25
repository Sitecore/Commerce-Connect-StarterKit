// -----------------------------------------------------------------
// <copyright file="CartService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The implementation of Cart Service.
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
  using System.ServiceModel.Activation;
  using System.ServiceModel.Web;
  using System.Web.Services;
  using Nop.Core.Domain.Orders;
  using Nop.Plugin.Sitecore.Commerce.Common;
  using Nop.Plugin.Sitecore.Commerce.Common.Models;

  /// <summary>
  /// The implementation of Wishlists Service.
  /// </summary>
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  public class WishlistsService : CartsServiceBase, IWishlistsService
  {
    public WishlistsService()
    {
      this.CartType = ShoppingCartType.Wishlist;
    }

    /// <summary>
    /// Gets all carts.
    /// </summary>
    /// <returns>List of <see cref="ShoppingCartModel"/></returns>
    [WebGet]
    public virtual IQueryable<ShoppingCartModel> GetWishlists()
    {
      return base.GetCarts();
    }

    /// <summary>
    /// Gets the wishlist by customer id.
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    /// <returns>List of <see cref="ShoppingCartModel"/></returns>
    [WebMethod(EnableSession = false)]
    public virtual ShoppingCartModel GetWishlist(Guid customerId)
    {
      return base.GetCart(customerId);
    }

    /// <summary>
    /// Deletes the wishlist by customer id.
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    [WebMethod(EnableSession = false)]
    public void DeleteWishlist(Guid customerId)
    {
      base.DeleteCart(customerId);
    }

    /// <summary>
    /// Creates the wishlist by customer id.
    /// </summary>
    /// <param name="customerId">The customer id.</param>
    [WebMethod(EnableSession = false)]
    public virtual ShoppingCartModel CreateWishlist(Guid customerId)
    {
      return base.CreateCart(customerId);
    }

    /// <summary>
    /// Adds the product by product variant id to wishlist.
    /// </summary>
    /// <param name="customerId">The id of wishlist customer.</param>
    /// <param name="externalProductId">The external product id.</param>
    /// <param name="quantity">The quantity.</param>
    [WebMethod(EnableSession = false)]
    public new virtual ShoppingCartModel AddProduct(Guid customerId, string externalProductId, uint quantity)
    {
      return base.AddProduct(customerId, externalProductId, quantity);
    }

    /// <summary>
    /// Removes the product by product variant id from cart.
    /// </summary>
    /// <param name="customerId">The customer Id.</param>
    /// <param name="externalProductId">The external product id.</param>
    [WebMethod(EnableSession = false)]
    public new virtual ShoppingCartModel RemoveProduct(Guid customerId, string externalProductId)
    {
      return base.RemoveProduct(customerId, externalProductId);
    }

    /// <summary>
    /// Updates the quantity by external product id on cart.
    /// </summary>
    /// <param name="customerId">The customer Id.</param>
    /// <param name="externalProductId">The external product id.</param>
    /// <param name="newQuantity">The new quantity.</param>
    /// <returns>
    /// Instance of <see cref="ShoppingCartModel" />
    /// </returns>
    [WebMethod(EnableSession = false)]
    public new virtual ShoppingCartModel UpdateQuantity(Guid customerId, string externalProductId, int newQuantity)
    {
      return base.UpdateQuantity(customerId, externalProductId, newQuantity);
    }
  }
}