// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWishlist.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the GetWishlist class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.GetWishlist
{
  using System;
  using Sitecore.Commerce.Connectors.NopCommerce.NopWishlistsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.WishLists;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.WishLists;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Data.Products;

  /// <summary>
  /// The get wishlist.
  /// </summary>
  public class GetWishlist : NopProcessor<IWishlistsServiceChannel>
  {
    /// <summary>
    /// Performs get wishlist operation.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      // Gets id of castomer to load from NopCommerce instance.
      var request = (GetWishListRequest)args.Request;
      var result = (GetWishListResult)args.Result;

      // Creates instance of WCF service client.
      using (var client = this.GetClient())
      {
        Guid userGuid;
        string userId = request.UserId;
        if (!Guid.TryParse(userId, out userGuid))
        {
          var idGenerator = new Md5IdGenerator();
          userGuid = Guid.Parse(idGenerator.StringToID(userId, string.Empty).ToString());
        }

        ShoppingCartModel wishlistModel = client.GetWishlist(userGuid);
        if (wishlistModel == null)
        {
          return;
        }

        var wishlist = new WishList
        {
          ExternalId = request.WishListId
        };

        wishlist.MapWishlistFromModel(wishlistModel);

        result.WishList = wishlist;
      }
    }
  }
}