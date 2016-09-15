// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCartsTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetWishlistsTest type.
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
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopWishlistsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.GetWishlists;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.WishLists;
  using Sitecore.Common;
  using Xunit;

  /// <summary>
  /// The get цishlists test.
  /// </summary>
  public class GetCartsTest
  {
     /// <summary>
    /// The client.
    /// </summary>
    private readonly IWishlistsServiceChannel client;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly GetWishListsRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly GetWishListsResult result;

    /// <summary>
    /// The processor.
    /// </summary>
    private readonly GetWishlists processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly Guid visitorId;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCartsTest" /> class.
    /// </summary>
    public GetCartsTest()
    {
      this.visitorId = Guid.NewGuid();
      this.request = new GetWishListsRequest(this.visitorId.ToString(), "MyShop");
      this.result = new GetWishListsResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<IWishlistsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IWishlistsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new GetWishlists { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should get wishlists from client.
    /// </summary>
    [Fact]
    public void ShouldGetWishlists()
    {
      // arrange
      var cartLineModel = new ShoppingCartItemModel
      {
        Id = "2",
        ProductId = 41,
        Price = 1300,
        Quantity = 1,
        LineTotal = 1300
      };

      var cartModel = new ShoppingCartModel
      {
        CustomerGuid = this.visitorId,
        CustomerEmail = "cost@mail.com",
        CustomerId = 1,
        IsAnonymous = false,
        TotalItems = 1,
        Total = 1300,
        ShoppingItems = new[] { cartLineModel }
      };

      this.client.GetWishlists().Returns(new[] { cartModel });

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetWishlists();
      this.result.WishLists.Should().NotBeNull();
      this.result.WishLists.First().Should().NotBeNull();
      this.result.WishLists.First().CustomerId.Should().Be(cartModel.CustomerId.ToString());
      this.result.WishLists.First().ExternalId.Should().Be(cartModel.CustomerGuid.ToID().ToString().ToUpper());
    }
  }
}