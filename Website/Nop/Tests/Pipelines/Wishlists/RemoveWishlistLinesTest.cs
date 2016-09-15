// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddLinesToCartTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the RemoveWishlistLinesTest type.
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
// ---------------------------------------------------------------------
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Wishlists
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopWishlistsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.RemoveWishlistLines;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Entities.Prices;
  using Sitecore.Commerce.Entities.WishLists;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.WishLists;
  using Xunit;

  /// <summary>
  /// The add lines to wishlist test.
  /// </summary>
  public class RemoveWishlistLinesTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly RemoveWishlistLines processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly Guid visitorId;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly RemoveWishListLinesRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly RemoveWishListLinesResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly IWishlistsServiceChannel client;

    /// <summary>
    /// The wishlist.
    /// </summary>
    private readonly WishList wishlist;

    /// <summary>
    /// The line to add
    /// </summary>
    private readonly WishListLine lineToRemove;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveWishlistLinesTest"/> class.
    /// </summary>
    public RemoveWishlistLinesTest()
    {
      this.visitorId = Guid.NewGuid();

      this.lineToRemove = new WishListLine()
      {
        ExternalId = "10",
        Product = new CartProduct
        {
          ProductId = "100500",
          Price = new Price { Amount = 100 }
        },
        Quantity = 12
      };

      this.wishlist = new WishList
      {
        ExternalId = this.visitorId.ToString(),
        Lines = new ReadOnlyCollection<WishListLine>(new List<WishListLine> { this.lineToRemove })
      };
      
      this.request = new RemoveWishListLinesRequest(this.wishlist, new List<string> { "10" });
      this.result = new RemoveWishListLinesResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<IWishlistsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IWishlistsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new RemoveWishlistLines { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should add lines to wishlist.
    /// </summary>
    [Fact]
    public void ShouldAddLinesToWishlist()
    {
      var resCartModel = new ShoppingCartModel
      {
        ShoppingItems = new ShoppingCartItemModel[0]
      };

      this.client.GetWishlist(this.visitorId).Returns(resCartModel);

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received(1).RemoveProduct(this.visitorId, "100500");
      this.result.WishList.Lines.Count.Should().Be(0);
    }
  }
}