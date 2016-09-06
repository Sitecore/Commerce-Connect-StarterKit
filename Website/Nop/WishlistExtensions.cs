// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WishlistExtensions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the WishlistExtensions class.</summary>
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
  using Sitecore.Commerce.Entities.WishLists;
  using System.Collections.Generic;
  using System.Globalization;
  using Sitecore.Commerce.Connectors.NopCommerce.NopWishlistsService;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Entities.Prices;
  using Sitecore.Common;
  using Sitecore.Diagnostics;

  /// <summary>
  /// The wishlist extensions.
  /// </summary>
  public static class WishlistExtensions
  {
    /// <summary>
    /// Converts NopCommerce wishlist line model to the OBEC cart line.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The WishlistLine.</returns>
    [NotNull]
    public static CartLine ToObecCartLine([NotNull] this ShoppingCartItemModel model)
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
    /// Maps wishlist from model.
    /// </summary>
    /// <param name="wishlist">The wish list.</param>
    /// <param name="wishlistModel">The wishlist model.</param>
    public static void MapWishlistFromModel([NotNull] this WishList wishlist, [NotNull] ShoppingCartModel wishlistModel)
    {
      Assert.ArgumentNotNull(wishlist, "wishlist");
      Assert.ArgumentNotNull(wishlistModel, "wishlistModel");

      var wishlistlines = new List<WishListLine>();
      foreach (var wishlistItemModel in wishlistModel.ShoppingItems)
      {
        CartLine line = wishlistItemModel.ToObecCartLine();
        wishlistlines.Add(new WishListLine()
        {
          ExternalId = line.ExternalCartLineId,
          Product = line.Product,
          Properties = line.Properties,
          Quantity = line.Quantity,
          Total = line.Total
        });
      }

      wishlist.Lines = wishlistlines.AsReadOnly();

      wishlist.CustomerId = wishlistModel.CustomerId.ToString();
      wishlist.UserId = 
      wishlist.ExternalId = wishlistModel.CustomerGuid.ToID().ToString().ToUpper();
    }

    /// <summary>
    /// Maps wishlist header from model.
    /// </summary>
    /// <param name="wishlistHeader">The wishlist header.</param>
    /// <param name="wishlistModel">The wishlist model.</param>
    public static void MapWishlistHeaderFromModel([NotNull] this WishListHeader wishlistHeader, [NotNull] ShoppingCartModel wishlistModel)
    {
      Assert.ArgumentNotNull(wishlistHeader, "wishlistHeader");
      Assert.ArgumentNotNull(wishlistModel, "wishlistModel");

      wishlistHeader.CustomerId = wishlistModel.CustomerId.ToString();
      wishlistHeader.ExternalId = wishlistModel.CustomerGuid.ToID().ToString().ToUpper();
    }
  }
}