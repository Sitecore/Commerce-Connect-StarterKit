//-----------------------------------------------------------------------
// <copyright file="GetOrderTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The GetOrderTest class.</summary>
//-----------------------------------------------------------------------
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Orders
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Connectors.NopCommerce.NopOrdersService;
  using Sitecore.Commerce.Pipelines;
  using Xunit;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Orders.GetOrder;
  using Sitecore.Commerce.Services.Orders;

  public class GetOrderTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly GetOrder processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly int customerId;

    private readonly int orderId;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly GetVisitorOrderRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly GetVisitorOrderResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly IOrdersServiceChannel client;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrderTest"/> class.
    /// </summary>
    public GetOrderTest()
    {
      this.orderId = 1;
      this.customerId = 1;
      this.request = new GetVisitorOrderRequest(this.orderId.ToString(), this.customerId.ToString(), "NopShop");
      this.result = new GetVisitorOrderResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<IOrdersServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IOrdersServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new GetOrder { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should get order.
    /// </summary>
    [Fact]
    public void ShouldGetOrder()
    {
      // arrange
      var cartLineModel = new NopOrdersService.ShoppingCartItemModel
                            {
                              Id = "1",
                              ProductId = 41,
                              Price = 1300,
                              Quantity = 1,
                              LineTotal = 1300
                            };

      var orderModel = new OrderModel
                        {
                          CustomerId = 1,
                          IsAnonymous = false,
                          ShoppingItems = new ShoppingCartItemModel[0],
                          CreatedDateTime = DateTime.Now,
                          CardType = string.Empty,
                          BillingName = string.Empty,
                          BillingAddressId = 1,
                          Addresses = new AddressModel[0],
                          CustomerEmail = string.Empty,
                          CustomerGuid = new Guid(),
                          Id = 1,
                          IsDeleted = false,
                          Name = "order",
                          OrderGuid = new Guid(),
                          Shipments = new ShipmentModel[0],
                          OrderItems = new[] { cartLineModel },
                        };

      this.client.GetOrder(this.orderId, "NopShop").Returns(orderModel);

      // act
      this.processor.Process(this.args);

      // assert
      this.result.Order.CustomerId.Should().Be(this.customerId.ToString());

      var cartLine = this.result.Order.Lines.Single();
      cartLine.Product.ProductId.Should().Be("41");
      cartLine.Product.Price.Amount.Should().Be(1300);
      cartLine.Quantity.Should().Be(1);
    }
  }
}