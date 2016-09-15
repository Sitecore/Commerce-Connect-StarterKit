// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateLinesOnWishlist.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the UpdateLinesOnWishlist class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.UpdateLinesOnWishlist
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Sitecore.Commerce.Connectors.NopCommerce.NopWishlistsService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Entities.WishLists;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.WishLists;
    using Sitecore.Diagnostics;

    /// <summary>
    /// Processor that adds specified wishlist lines to wishlist in instance of NopCommerce e-commerce system via the WCF service.
    /// </summary>
    public class UpdateLinesOnWishlist : NopProcessor<IWishlistsServiceChannel>
    {
        /// <summary>
        /// Processes the arguments.
        /// </summary>
        /// <param name="args">The pipeline arguments.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var request = (UpdateWishListLinesRequest)args.Request;
            var result = (UpdateWishListLinesResult)args.Result;

            var updatedLines = new List<WishListLine>();

            using (var client = this.GetClient())
            {
                var wishlistId = new Guid(request.WishList.ExternalId);

                foreach (WishListLine line in request.Lines)
                {
                    if (client.UpdateQuantity(wishlistId, line.Product.ProductId, int.Parse(line.Quantity.ToString())) != null)
                    {
                        updatedLines.Add(line);
                    }
                }

                ShoppingCartModel wishlistModel = client.GetWishlist(wishlistId);

                var wishlist = new WishList
                {
                    ExternalId = request.WishList.ExternalId
                };

                wishlist.MapWishlistFromModel(wishlistModel);

                result.WishList = wishlist;
                result.UpdatedLines = new ReadOnlyCollection<WishListLine>(updatedLines);
            }
        }
    }
}