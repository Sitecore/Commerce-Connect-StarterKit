// ----------------------------------------------------------------------------------------------
// <copyright file="GetCarts.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetWishlists type.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.GetWishlists
{
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
  /// Get wishlists.
  /// </summary>
  public class GetWishlists : NopProcessor<IWishlistsServiceChannel>
  {
    /// <summary>
    /// Runs the processor.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var result = (GetWishListsResult)args.Result;

      // Creates instance of WCF service client.
      using (var client = this.GetClient())
      {
        var wishlistsHeaders = new List<WishListHeader>();
        var wishlists = new ReadOnlyCollection<WishListHeader>(wishlistsHeaders);
        result.WishLists = wishlists;

        var wishlistModels = client.GetWishlists();
        if (!wishlistModels.Any())
        {
          return;
        }

        // Maps wishlist lines from NopCommerce wishlist model to OBEC wishlist.
        foreach (var wishlistModel in wishlistModels)
        {
          var wishlist = new WishList();

          wishlist.MapWishlistFromModel(wishlistModel);
          var header = new WishListHeader();
          header.MapWishlistHeaderFromModel(wishlistModel);
          wishlistsHeaders.Add(header);
        }
      }
    }
  }
}