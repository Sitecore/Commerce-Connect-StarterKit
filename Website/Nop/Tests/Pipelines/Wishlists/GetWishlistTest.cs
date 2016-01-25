// --------------------------------------------------------------------
// <copyright file="GetWishlistTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   GetWishlistTest test class
// </summary>
// --------------------------------------------------------------------
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
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Pipelines;
  using Xunit;
  using Sitecore.Commerce.Services.WishLists;
  using Sitecore.Commerce.Connectors.NopCommerce.NopWishlistsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Wishlists.GetWishlist;

  public class GetWishlistTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly GetWishlist processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly Guid visitorId;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly GetWishListRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly GetWishListResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly IWishlistsServiceChannel client;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWishlistTest"/> class.
    /// </summary>
    public GetWishlistTest()
    {
      this.visitorId = Guid.NewGuid();
      this.request = new GetWishListRequest(this.visitorId.ToString(), this.visitorId.ToString(), "NopShop");
      this.result = new GetWishListResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<IWishlistsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IWishlistsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new GetWishlist { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should get wishlist.
    /// </summary>
    [Fact]
    public void ShouldGetWishlist()
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

      this.client.GetWishlist(this.visitorId).Returns(cartModel);

      // act
      this.processor.Process(this.args);

      // assert
      this.result.WishList.ExternalId.Should().Be(this.visitorId.ToString("B").ToUpper());

      var cartLine = this.result.WishList.Lines.Single();
      cartLine.Product.ProductId.Should().Be("41");
      cartLine.Product.Price.Amount.Should().Be(1300);
      cartLine.Quantity.Should().Be(1);
    }
  }
}