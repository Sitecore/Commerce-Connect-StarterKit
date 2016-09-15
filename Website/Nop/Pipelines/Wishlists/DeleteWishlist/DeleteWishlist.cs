// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteWishlist.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the DeleteWishlist class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.DeleteWishlist
{
  using System;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopWishlistsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.WishLists;

  /// <summary>
  /// The processor that deletes specified wishlist in NopCommerce e-commerce system via the WCF service.
  /// </summary>
  public class DeleteWishlist : NopProcessor<IWishlistsServiceChannel>
  {
    /// <summary>
    /// Performs delete wishlist operation.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (DeleteWishListRequest)args.Request;
      var result = (DeleteWishListResult)args.Result;

      using (var client = this.GetClient())
      {
        client.DeleteWishlist(Guid.Parse(request.WishList.ExternalId));
        result.WishList = request.WishList;
      }
    }
  }
}