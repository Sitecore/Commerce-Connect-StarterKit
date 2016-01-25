// ----------------------------------------------------------------------------------------------
// <copyright file="GetCartsTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetCartsTest type.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.GetCarts
{
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.GetCarts;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;
  using Xunit;

  /// <summary>
  /// The get carts test.
  /// </summary>
  public class GetCartsTest
  {
     /// <summary>
    /// The client.
    /// </summary>
    private readonly ICartsServiceChannel client;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly GetCartsRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly GetCartsResult result;

    /// <summary>
    /// The processor.
    /// </summary>
    private readonly GetCarts processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCartsTest" /> class.
    /// </summary>
    public GetCartsTest()
    {
      this.request = new GetCartsRequest("MyShop");
      this.result = new GetCartsResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<ICartsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new GetCarts { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should get cart all carts from client.
    /// </summary>
    [Fact]
    public void ShouldGetCartAllCartsFromClient()
    {
      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetCarts();
    }

    /// <summary>
    /// Should map nop carts to obec carts.
    /// </summary>
    [Fact]
    public void ShouldMapNopCartsToObecCarts()
    {
      // arrange
      var cartModels = new[] { new ShoppingCartModel { ShoppingItems = new ShoppingCartItemModel[0] }, new ShoppingCartModel { ShoppingItems = new ShoppingCartItemModel[0] } };
      this.client.GetCarts().Returns(cartModels);

      // act
      this.processor.Process(this.args);

      // assert
      this.result.Carts.Should().NotBeEmpty();
      this.result.Carts.Count().Should().Be(2);
    }
  }
}