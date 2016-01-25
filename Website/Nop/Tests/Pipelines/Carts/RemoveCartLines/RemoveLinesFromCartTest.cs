// ----------------------------------------------------------------------------------------------
// <copyright file="RemoveLinesFromCartTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The remove lines from cart test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.RemoveCartLines
{
  using System;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.RemoveCartLines;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Entities.Prices;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;
  using Xunit;

  /// <summary>
  /// The remove lines from cart test.
  /// </summary>
  public class RemoveLinesFromCartTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly RemoveLinesFromCart processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly Guid visitorId;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly RemoveCartLinesRequest request;

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
    /// The cart.
    /// </summary>
    private readonly Cart cart;

    /// <summary>
    /// The line to add
    /// </summary>
    private readonly CartLine lineToRemove;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveLinesFromCartTest"/> class.
    /// </summary>
    public RemoveLinesFromCartTest()
    {
      this.visitorId = Guid.NewGuid();

      this.cart = new Cart { ExternalId = this.visitorId.ToString(), Lines = new ReadOnlyCollectionAdapter<CartLine> { new CartLine() } };
      this.lineToRemove = new CartLine
      {
        Product = new CartProduct
        {
          ProductId = "100500",
          Price = new Price { Amount = 100 }
        },
        Quantity = 12
      };

      this.request = new RemoveCartLinesRequest(this.cart, new[] { this.lineToRemove });
      this.result = new CartResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<ICartsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new RemoveLinesFromCart { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should remove cart line.
    /// </summary>
    [Fact]
    public void ShouldRemoveCartLine()
    {
      // arrange
      var inintialLineModel = new ShoppingCartItemModel();
      var initialCartModel = new ShoppingCartModel { CustomerGuid = this.visitorId, ShoppingItems = new[] { inintialLineModel } };
      this.client.GetCart(this.visitorId,null).Returns(initialCartModel);

      var resultLineModel = new ShoppingCartItemModel { Id = "LineId2", LineTotal = 1010, Price = 50, ProductId = 100500, Quantity = 12 };
      var resultingCartModel = new ShoppingCartModel { Total = 2233, ShoppingItems = new[] { resultLineModel } };
      this.client.RemoveProduct(this.visitorId, "100500").Returns(resultingCartModel);

      // act
      this.processor.Process(this.args);

      // assert
      var line = this.result.Cart.Lines.Single();
      line.ExternalCartLineId.Should().Be("LineId2");
      line.Product.ProductId.Should().Be("100500");
      line.Product.Price.Amount.Should().Be(50);
      line.Quantity.Should().Be(12);
    }
  }
}