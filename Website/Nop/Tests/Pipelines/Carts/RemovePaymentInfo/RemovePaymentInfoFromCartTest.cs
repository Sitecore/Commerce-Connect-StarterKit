// ----------------------------------------------------------------------------------------------
// <copyright file="RemovePaymentInfoFromCartTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The RemovePaymentInfoFromCartTest class.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.RemovePaymentInfo
{
  using System;
  using System.Collections.Generic;
  using Commerce.Entities.Carts;
  using Commerce.Pipelines;
  using Commerce.Services.Carts;
  using FluentAssertions;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using NSubstitute;
  using Xunit;

  /// <summary>
  /// Defines the RemovePaymentInfoFromCartTest class.
  /// </summary>
  public class RemovePaymentInfoFromCartTest
  {
    /// <summary>
    /// The client
    /// </summary>
    private readonly ICartsServiceChannel _client;

    /// <summary>
    /// The RemovePaymentInfoFromCart processor
    /// </summary>
    private readonly NopCommerce.Pipelines.Carts.RemovePaymentInfo.RemovePaymentInfoFromCart _processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemovePaymentInfoFromCartTest"/> class.
    /// </summary>
    public RemovePaymentInfoFromCartTest()
    {
      this._client = Substitute.For<ICartsServiceChannel>();
      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this._client);
      _processor = new NopCommerce.Pipelines.Carts.RemovePaymentInfo.RemovePaymentInfoFromCart() { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Remove payment info from cart test
    /// </summary>
    [Fact]
    [Trait("Catygory", "Cart")] 
    public void ShouldRemovePaymentInfoFromCart()
    {
      var guid = Guid.NewGuid();
      var request = new RemovePaymentInfoRequest(
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

      var result = new RemovePaymentInfoResult();
      var args = new ServicePipelineArgs(request, result);

      _client.RemovePaymentInfo(guid, Arg.Any<PaymentInfoModel>(), request.Cart.ShopName)
        .Returns(new PaymentInfoModelResponse
        {
          Success = true
        });

      _processor.Process(args);

      result.Success.Should().BeTrue();
      result.Payments.Should().HaveCount(0);
      result.Cart.Payment.Should().HaveCount(0);
    }
  }
}
