// ----------------------------------------------------------------------------------------------
// <copyright file="GetProductPricesTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetProductPricesTest class.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Prices.GetProductPrices
{
  using System;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce;
  using Sitecore.Commerce.Connectors.NopCommerce.NopPricesService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Prices.GetProductPrices;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Prices;
  using Xunit;
  using Xunit.Extensions;

  /// <summary>
  /// Defines the GetProductPricesTest class
  /// </summary>
  public class GetProductPricesTest
  {
    /// <summary>
    /// The client.
    /// </summary>
    private readonly IPricesServiceChannel client;    

    /// <summary>
    /// The get product prices.
    /// </summary>
    private readonly GetProductPrices processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductPricesTest" /> class.
    /// </summary>
    public GetProductPricesTest()
    {
      this.client = Substitute.For<IPricesServiceChannel>();
      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IPricesServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new GetProductPrices { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should get product price from service.
    /// </summary>
    /// <param name="priceType">Type of the price.</param>
    [Theory]
    [InlineData("List")]
    [InlineData("Custom")]
    public void ShouldGetProductPriceFromService(string priceType)
    {
      // Arrange
      this.client.GetProductPrice("Product", priceType).Returns(1);
      var args = new ServicePipelineArgs(new GetProductPricesRequest("Product", new[] { priceType }), new GetProductPricesResult());

      // Act
      this.processor.Process(args);
      var result = ((GetProductPricesResult)args.Result).Prices;

      // Assert
      result[priceType].Amount.Should().Be(1);
      result[priceType].PriceType.Should().Be(priceType);
    }

    /// <summary>
    /// Should get product price in specified cureency from service.
    /// </summary>
    /// <param name="priceType">Type of the price.</param>
    [Theory]
    [InlineData("List")]
    [InlineData("Custom")]
    public void ShouldGetProductPriceInCurrencyFromService(string priceType)
    {
        // Arrange
        this.client.GetProductPrice("Product", priceType).Returns(1);
        var request = new GetProductPricesRequest("Product", new[] { priceType });
        request.CurrencyCode = "CAD";
        var args = new ServicePipelineArgs(request, new GetProductPricesResult());

        // Act
        this.processor.Process(args);
        var result = ((GetProductPricesResult)args.Result).Prices;

        // Assert
        result[priceType].Amount.Should().Be(1);
        result[priceType].PriceType.Should().Be(priceType);
        result[priceType].CurrencyCode = "CAD";
    }

    /// <summary>
    /// Should get list prices if no price types are available.
    /// </summary>
    [Fact]
    public void ShouldGetListPricesIfNoPriceTypesAreAvailable()
    {
      // Arrange
      this.client.GetProductPrice("Product", "List").Returns(2);
      var args = new ServicePipelineArgs(new GetProductPricesRequest("Product", new string[0]), new GetProductPricesResult());

      // Act
      this.processor.Process(args);
      var result = ((GetProductPricesResult)args.Result).Prices;
      
      // Assert
      result["List"].Amount.Should().Be(2);
      result["List"].PriceType.Should().Be("List");
    }

    /// <summary>
    /// Should return zero if fault exception was thrown.
    /// </summary>
    [Theory]
    [InlineData("List,Custom")]
    public void ShouldReturnZeroIfNullReturned(string stringList)
    {
      // Arrange
      this.client.GetProductPrice("Product", "List").Returns((decimal?)null);
      var args = new ServicePipelineArgs(new GetProductPricesRequest("Product", stringList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)), new GetProductPricesResult());

      // Act
      this.processor.Process(args);
      var result = ((GetProductPricesResult)args.Result).Prices;

      // Assert
      result["List"].Amount.Should().Be(0);
      result["List"].PriceType.Should().Be("List");
      result["Custom"].PriceType.Should().Be("Custom");
    }
  }
}
