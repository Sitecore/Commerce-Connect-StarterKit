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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.RemoveWishlistLines
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using Sitecore.Commerce.Connectors.NopCommerce.NopWishlistsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.WishLists;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.WishLists;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Processor that adds specified wishlist lines to wishlist in instance of NopCommerce e-commerce system via the WCF service.
  /// </summary>
  public class RemoveWishlistLines : NopProcessor<IWishlistsServiceChannel>
  {
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (RemoveWishListLinesRequest)args.Request;
      var result = (RemoveWishListLinesResult)args.Result;

      var removedLines = new List<WishListLine>();

      using (var client = this.GetClient())
      {
        var wishlistId = new Guid(request.WishList.ExternalId);
        foreach (string lineId in request.LineIds)
        {
          WishListLine line = request.WishList.Lines.FirstOrDefault(p => p.ExternalId == lineId);
          if (line != null)
          {
            client.RemoveProduct(wishlistId, line.Product.ProductId);
            removedLines.Add(line);
          }
        }

        ShoppingCartModel wishlistModel = client.GetWishlist(wishlistId);
        
        var wishlist = new WishList
        {
          ExternalId = request.WishList.ExternalId
        };

        wishlist.MapWishlistFromModel(wishlistModel);

        result.WishList = wishlist;
        result.RemovedLines = new ReadOnlyCollection<WishListLine>(removedLines);
      }
    }
  }
}