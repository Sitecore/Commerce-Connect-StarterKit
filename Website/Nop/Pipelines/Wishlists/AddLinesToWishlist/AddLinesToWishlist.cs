// ----------------------------------------------------------------------------------------------
// <copyright file="AddLinesToCart.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The add lines to wishlist.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.AddLinesToWishlist
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
  public class AddLinesToWishlist : NopProcessor<IWishlistsServiceChannel>
  {
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (AddLinesToWishListRequest)args.Request;
      var result = (AddLinesToWishListResult)args.Result;

      var addedLines = new List<WishListLine>();

      using (var client = this.GetClient())
      {
        var wishlistId = new Guid(request.WishList.ExternalId);

        foreach (WishListLine line in request.Lines)
        {
          if (client.AddProduct(wishlistId, line.Product.ProductId, line.Quantity) != null)
          {
            addedLines.Add(line);
          }
        }

        ShoppingCartModel wishlistModel = client.GetWishlist(wishlistId);

        var wishlist = new WishList
        {
          ExternalId = request.WishList.ExternalId
        };

        wishlist.MapWishlistFromModel(wishlistModel);

        result.WishList = wishlist;
        result.AddedLines = new ReadOnlyCollection<WishListLine>(addedLines);
      }
    }
  }
}