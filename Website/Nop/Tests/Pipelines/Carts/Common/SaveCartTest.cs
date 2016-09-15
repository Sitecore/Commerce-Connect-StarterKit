// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveCartTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the SaveCartTest type.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.Common
{
  using System;
  using System.Collections.Generic;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Data;
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.Common;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;
  using Xunit;
  using Xunit.Extensions;
    using System.Collections.ObjectModel;

  /// <summary>
  /// The save cart test.
  /// </summary>
  public class SaveCartTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly SaveCart processor;

    /// <summary>
    /// The cart.
    /// </summary>
    private readonly Cart cart;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly SaveCartRequest request;

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
    /// Initializes a new instance of the <see cref="SaveCartTest"/> class.
    /// </summary>
    public SaveCartTest()
    {
      this.cart = new Cart { Lines = new ReadOnlyCollection<CartLine>(new List<CartLine> { new CartLine { Product = new CartProduct { ProductId = "Audi S4" }, Quantity = 5 }, }) };
      this.request = new SaveCartRequest(this.cart);
      this.result = new CartResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<ICartsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new SaveCart { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should the set cart id to new id if it is null or empty.
    /// </summary>
    /// <param name="id">The id.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldSetCartIdToNewIdIfItIsNullOrEmpty(string id)
    {
      // arrange
      this.cart.ExternalId = id;

      // act
      this.processor.Process(this.args);

      // assert
      ID.IsID(this.cart.ExternalId).Should().BeTrue();
      this.cart.ExternalId.StartsWith("{").Should().BeTrue();
      this.cart.ExternalId.EndsWith("}").Should().BeTrue();
    }

    /// <summary>
    /// Should create cart on nop side.
    /// </summary>
    [Fact]
    public void ShouldCreateCartOnNopSide()
    {
      // arrange
      var cartIdGuid = Guid.NewGuid();
      this.cart.ExternalId = cartIdGuid.ToString();

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().CreateCart(cartIdGuid);
    }

    /// <summary>
    /// Should delete previously saved cart.
    /// </summary>
    [Fact(Skip = "Do not recreate cart during save")]
    public void ShouldDeletePreviouslySavedCart()
    {
      // arrange
      var cartIdGuid = Guid.NewGuid();
      this.cart.ExternalId = cartIdGuid.ToString();

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().DeleteCart(cartIdGuid);
    }

    /// <summary>
    /// Should add cart lines to new cart.
    /// </summary>
    [Fact(Skip = "Do not recreate cart during save")]
    public void ShouldAddCartLinesToNewCart()
    {
      // arrange
      var id = Guid.NewGuid();
      this.cart.ExternalId = id.ToString();
      var model = new ShoppingCartModel();
      this.client.GetCart(id,null).Returns(model);
      this.client.CreateCart(id).Returns(model);

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().AddProduct(id, "Audi S4", 5);
    }
  }
}