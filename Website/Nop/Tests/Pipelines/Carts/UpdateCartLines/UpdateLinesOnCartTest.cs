// ----------------------------------------------------------------------------------------------
// <copyright file="UpdateLinesOnCartTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the UpdateLinesOnCartTest type.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.UpdateCartLines
{
  using System;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.UpdateCartLines;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Entities.Prices;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;
  using Xunit;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

  /// <summary>
  /// The update lines on cart test.
  /// </summary>
  public class UpdateLinesOnCartTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly UpdateLinesOnCart processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly Guid visitorId;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly UpdateCartLinesRequest request;

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
    /// The line to add.
    /// </summary>
    private readonly CartLine lineToUpdate;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateLinesOnCartTest"/> class.
    /// </summary>
    public UpdateLinesOnCartTest()
    {
      this.visitorId = Guid.NewGuid();

      this.lineToUpdate = new CartLine
                         {
                           ExternalCartLineId = "LineId2",
                           Product = new CartProduct
                                       {
                                         ProductId = "100500",
                                         Price = new Price { Amount = 100 }
                                       },
                           Quantity = 12
                         };
      this.cart = new Cart
      {
          ExternalId = this.visitorId.ToString(),
          Lines = new ReadOnlyCollection<CartLine>(new List<CartLine> { new CartLine(), this.lineToUpdate })
      };

      this.request = new UpdateCartLinesRequest(this.cart, new[] { this.lineToUpdate });
      this.result = new CartResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<ICartsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new UpdateLinesOnCart { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should update line on cart.
    /// </summary>
    [Fact]
    public void ShouldUpdateLineOnCart()
    {
      // arrange
      var inintialLineModel = new ShoppingCartItemModel();
      var initialCartModel = new ShoppingCartModel { ShoppingItems = new[] { inintialLineModel } };
      this.client.GetCart(this.visitorId,null).Returns(initialCartModel);

      var resultLineModel = new ShoppingCartItemModel { Id = "LineId2", ProductId = 100500, Price = 100, Quantity = 12 };
      var resultCartModel = new ShoppingCartModel { ShoppingItems = new[] { resultLineModel } };
      this.client.UpdateQuantity(initialCartModel.CustomerGuid, "100500", 12).Returns(resultCartModel);

      // act
      this.processor.Process(this.args);

      // assert
      var line = this.result.Cart.Lines.Single();
      line.ExternalCartLineId.Should().Be("LineId2");
      line.Product.ProductId.Should().Be("100500");
      line.Product.Price.Amount.Should().Be(100);
      line.Quantity.Should().Be(12);
    }
  }
}