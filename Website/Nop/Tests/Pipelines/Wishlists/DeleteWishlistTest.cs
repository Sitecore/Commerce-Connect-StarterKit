// ----------------------------------------------------------------------------------------------
// <copyright file="DeleteCartTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the DeleteWishlistTest type.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Wishlists
{
  using System;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopWishlistsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.DeleteWishlist;
  using Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.DeleteCart;
  using Sitecore.Commerce.Entities.WishLists;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.WishLists;
  using Xunit;

  /// <summary>
  /// The delete wishlist test.
  /// </summary>
  public class DeleteWishlistTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly DeleteWishlist processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly Guid visitorId;

    /// <summary>
    /// The cart.
    /// </summary>
    private readonly WishList wishlist;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly DeleteWishListRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly DeleteWishListResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly IWishlistsServiceChannel client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCartTest"/> class.
    /// </summary>
    public DeleteWishlistTest()
    {
      this.visitorId = Guid.NewGuid();
      this.wishlist = new WishList { ExternalId = this.visitorId.ToString() };
      this.request = new DeleteWishListRequest(this.wishlist);
      this.result = new DeleteWishListResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<IWishlistsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IWishlistsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new DeleteWishlist { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should delete wishlist test.
    /// </summary>
    [Fact]
    public void ShouldDeleteWishlist()
    {
      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received(1).DeleteWishlist(this.visitorId);
    }
  }
}
