//-----------------------------------------------------------------------
// <copyright file="AddPaymentInfoToCartTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The AddPaymentInfoToCartTest class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.AddPaymentInfo
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Commerce.Entities.Carts;
  using Commerce.Pipelines;
  using Commerce.Services.Carts;
  using FluentAssertions;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using NSubstitute;
  using Xunit;

  /// <summary>
  /// Defines the AddPaymentInfoToCartTest class.
  /// </summary>
  public class AddPaymentInfoToCartTest
  {
    /// <summary>
    /// The client
    /// </summary>
    private readonly ICartsServiceChannel _client;

    /// <summary>
    /// The AddPaymentInfoToCart processor
    /// </summary>
    private readonly NopCommerce.Pipelines.Carts.AddPaymentInfo.AddPaymentInfoToCart _processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddPaymentInfoToCartTest"/> class.
    /// </summary>
    public AddPaymentInfoToCartTest()
    {
      this._client = Substitute.For<ICartsServiceChannel>();
      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this._client);
      _processor = new NopCommerce.Pipelines.Carts.AddPaymentInfo.AddPaymentInfoToCart() { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should add payment info to cart
    /// </summary>
    [Fact]
    [Trait("Catygory", "Cart")] 
    public void ShouldAddPaymentInfoToCart()
    {
      var guid = Guid.NewGuid();
      var request = new AddPaymentInfoRequest(
        new Cart()
        {
          ExternalId = guid.ToString(),
          ShopName = "my store"
        },
        new List<PaymentInfo>()
        {
          new PaymentInfo()
          {
            PaymentProviderID = "Payments.PurchaseOrder",
            PaymentMethodID = "Purchase Order"
          }
        });

      var result = new AddPaymentInfoResult();
      var args = new ServicePipelineArgs(request, result);

      _client.AddPaymentInfo(guid, Arg.Any<PaymentInfoModel>(), request.Cart.ShopName).Returns(new PaymentInfoModelResponse()
      {
        Success = true,
        Result = new PaymentInfoModel()
        {
          SystemName = "Payments.PurchaseOrder",
          MethodName = "Purchase Order"
        }
      });

      _processor.Process(args);

      result.Success.Should().BeTrue();
      result.Payments.Should().HaveCount(1);
      result.Cart.Payment.Should().HaveCount(1);
      result.Payments.ElementAt(0).PaymentProviderID.Should().Be("Payments.PurchaseOrder");
      result.Payments.ElementAt(0).PaymentMethodID.Should().Be("Purchase Order");
    }
  }
}
