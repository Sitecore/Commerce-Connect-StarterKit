// ----------------------------------------------------------------------------------------------
// <copyright file="DeleteCartTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the DeleteCartTest type.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.DeleteCart
{
  using System;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.DeleteCart;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Carts;
  using Xunit;

  /// <summary>
  /// The delete cart test.
  /// </summary>
  public class DeleteCartTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly DeleteCart processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly Guid visitorId;

    /// <summary>
    /// The cart.
    /// </summary>
    private readonly Cart cart;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly DeleteCartRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly ServiceProviderResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly ICartsServiceChannel client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCartTest"/> class.
    /// </summary>
    public DeleteCartTest()
    {
      this.visitorId = Guid.NewGuid();
      this.cart = new Cart { ExternalId = this.visitorId.ToString() };
      this.request = new DeleteCartRequest(this.cart);
      this.result = new CartResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<ICartsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new DeleteCart { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should delete cart.
    /// </summary>
    [Fact]
    public void ShouldDeleteCart()
    {
      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received(1).DeleteCart(this.visitorId);
    }
  }
}
