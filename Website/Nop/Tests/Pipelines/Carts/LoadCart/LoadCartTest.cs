// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadCartTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The load cart test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.LoadCart
{
  using System;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.LoadCart;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;
  using Xunit;

  /// <summary>
  /// The load cart test.
  /// </summary>
  public class LoadCartTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly LoadCart processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly Guid visitorId;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly LoadCartRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly CartResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly ICartsServiceChannel client;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadCartTest"/> class.
    /// </summary>
    public LoadCartTest()
    {
      this.visitorId = Guid.NewGuid();
      this.request = new LoadCartRequest("NopShop", this.visitorId.ToString());
      this.result = new CartResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<ICartsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new LoadCart { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should load cart.
    /// </summary>
    [Fact]
    public void ShouldLoadCart()
    {
      // arrange
      var cartLineModel = new ShoppingCartItemModel
                            {
                              Id = "CartLineId",
                              Price = 100500,
                              Quantity = 5,
                              ProductId = 100500,
                              LineTotal = 5555
                            };

      var cartModel = new ShoppingCartModel
                        {
                          ShoppingItems = new[] { cartLineModel },
                          Total = 2345,
                          CustomerGuid = this.visitorId
                        };

      this.client.GetCart(this.visitorId, Arg.Any<string>()).Returns(cartModel);

      // act
      this.processor.Process(this.args);

      // assert
      this.result.Cart.ExternalId.Should().Be(this.visitorId.ToString("B").ToUpper());
      this.result.Cart.Total.Amount.Should().Be(2345);

      var cartLine = this.result.Cart.Lines.Single();
      cartLine.ExternalCartLineId.Should().Be("CartLineId");
      cartLine.Product.ProductId.Should().Be("100500");
      cartLine.Product.Price.Amount.Should().Be(100500);
      cartLine.Quantity.Should().Be(5);
    }
  }
}
