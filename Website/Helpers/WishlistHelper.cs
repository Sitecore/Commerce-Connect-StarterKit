// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WishlistHelper.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the WishlistHelper class.</summary>
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

namespace Sitecore.Commerce.StarterKit.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Entities.WishLists;
    using Microsoft.Ajax.Utilities;
    using Models;
    using Services;

    /// <summary>
    /// Wish list helper class.
    /// </summary>
    public static class WishlistHelper
    {
        /// <summary>
        /// Converts a wish list to a wish list model.
        /// </summary>
        /// <param name="wishList">The wish list.</param>
        /// <param name="productService">The product service.</param>
        /// <returns>The wish list model.</returns>
        [NotNull]
        public static WishlistModel ToWishlistModel([NotNull]this WishList wishList, IProductService productService = null)
        {
            var cartLineModel = new List<WishlistLineModel>(0);
            if (wishList.Lines != null && wishList.Lines.Any())
            {
                foreach (var line in wishList.Lines)
                {
                    cartLineModel.Add(line.ToWishlistLineModel(productService));
                }
            }

            var wishlitstModel = new WishlistModel()
            {
                Lines = cartLineModel
            };

            return wishlitstModel;
        }

        /// <summary>
        /// Converts a wish list line to a wish list line model.
        /// </summary>
        /// <param name="wishListLine">The wish list line.</param>
        /// <param name="productService">The product service.</param>
        /// <returns>The wish list line model.</returns>
        [NotNull]
        public static WishlistLineModel ToWishlistLineModel([NotNull]this WishListLine wishListLine, IProductService productService = null)
        {
            var wishlistLineModel = new WishlistLineModel(wishListLine)
            {
                Id = wishListLine.ExternalId,
                Quantity = wishListLine.Quantity,
            };

            if (productService != null)
            {
                var product = productService.ReadProduct(wishListLine.Product.ProductId);
                wishlistLineModel.ProductName = product.Name;

                string image = productService.GetImage(product);
                if (string.IsNullOrEmpty(image))
                {
                    image = productService.GetImages(product).FirstOrDefault();
                    wishlistLineModel.Image = image;
                }

                wishlistLineModel.Image = image;
            }

            return wishlistLineModel;
        }
    }
}