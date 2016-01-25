//-----------------------------------------------------------------------
// <copyright file="GetOrdersTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The GetOrdersTest class.</summary>
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
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Connectors.NopCommerce.NopOrdersService;
  using Sitecore.Commerce.Pipelines;
  using Xunit;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Orders.GetOrder;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Orders.GetOrders;
  using Sitecore.Commerce.Services.Orders;

  public class GetOrdersTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly GetOrders processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly int customerId;

    private readonly Guid customerGuid;

    private readonly int orderId;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly GetVisitorOrdersRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly GetVisitorOrdersResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly IOrdersServiceChannel client;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrdersTest"/> class.
    /// </summary>
    public GetOrdersTest()
    {
      this.orderId = 1;
      this.customerId = 1;
      this.customerGuid = Guid.NewGuid();
      this.request = new GetVisitorOrdersRequest(this.customerGuid.ToString(), "NopShop");
      this.result = new GetVisitorOrdersResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.client = Substitute.For<IOrdersServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IOrdersServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new GetOrders { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should get order.
    /// </summary>
    [Fact]
    public void ShouldGetOrders()
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
                          ShoppingItems = new[] { cartLineModel },
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
                        };

      this.client.GetOrders(this.customerGuid, "NopShop").Returns(new[] { orderModel });

      // act
      this.processor.Process(this.args);

      // assert
      this.result.OrderHeaders.Count.Should().Be(1);
      this.result.OrderHeaders.First().CustomerId.Should().Be(this.customerId.ToString());

      var header = this.result.OrderHeaders.First();
      header.CustomerId.Should().Be("1");
      header.OrderID.Should().Be("1");
      header.IsLocked.Should().BeFalse();
    }
  }
}