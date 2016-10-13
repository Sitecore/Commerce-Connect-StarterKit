// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WishlistsService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The implementation of Wish Lists Service.
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
namespace Sitecore.Commerce.Nop.Wishlists
{
    using System;
    using System.Linq;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web.Services;
    using global::Nop.Core.Domain.Orders;
    using Sitecore.Commerce.Nop.Common;
    using Sitecore.Commerce.Nop.Common.Models;

    /// <summary>
    /// The implementation of Wishlists Service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WishlistsService : CartsServiceBase, IWishlistsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistsService" /> class.
        /// </summary>
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
            return this.GetCarts();
        }

        /// <summary>
        /// Gets the wishlist by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns>List of <see cref="ShoppingCartModel"/></returns>
        [WebMethod(EnableSession = false)]
        public virtual ShoppingCartModel GetWishlist(Guid customerId)
        {
            return this.GetCart(customerId);
        }

        /// <summary>
        /// Deletes the wishlist by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        [WebMethod(EnableSession = false)]
        public void DeleteWishlist(Guid customerId)
        {
            this.DeleteCart(customerId);
        }

        /// <summary>
        /// Creates the wishlist by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns>The customer's wish list.</returns>
        [WebMethod(EnableSession = false)]
        public virtual ShoppingCartModel CreateWishlist(Guid customerId)
        {
            return this.CreateCart(customerId);
        }

        /// <summary>
        /// Adds the product by product variant id to wishlist.
        /// </summary>
        /// <param name="customerId">The id of wishlist customer.</param>
        /// <param name="externalProductId">The external product id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>The customer's wish list.</returns>
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
        /// <returns>The customer's wish list.</returns>
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