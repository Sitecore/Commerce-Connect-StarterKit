// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PricingServiceTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The pricing service test.
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
namespace Sitecore.Commerce.StarterKit.Tests.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Entities.Prices;
  using Sitecore.Commerce.Services.Prices;
  using Sitecore.Commerce.StarterKit.Services;
  using Xunit;

  /// <summary>
  /// The pricing service test.
  /// </summary>
  public class PricesServiceTest
  {
    /// <summary>
    /// The service provider.
    /// </summary>
    private readonly PricingServiceProvider serviceProvider;

    /// <summary>
    /// The pricing service.
    /// </summary>
    private readonly PricingService pricingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PricesServiceTest"/> class.
    /// </summary>
    public PricesServiceTest()
    {
      this.serviceProvider = Substitute.For<PricingServiceProvider>();
      this.pricingService = new PricingService(this.serviceProvider);
    }

    /// <summary>
    /// Should get the product price.
    /// </summary>
    [Fact]
    public void ShouldGetProductPrice()
    {
      // Arrange
      this.serviceProvider
        .GetProductPrices(Arg.Is<GetProductPricesRequest>(r => r.ProductId == "1001"))
        .Returns(new GetProductPricesResult { Prices = { { "Sale", new Price(80, "USD") }, { "List", new Price(100, "USD") } } });

      // Act & Assert
      this.pricingService.GetProductPrice("1001").Should().Be(100);
    }

    /// <summary>
    /// Should return zero if no list price found.
    /// </summary>
    [Fact]
    public void ShouldReturnZeroIfNoListPriceFound()
    {
      this.serviceProvider
        .GetProductPrices(Arg.Is<GetProductPricesRequest>(r => r.ProductId == "1001"))
        .Returns(new GetProductPricesResult());

      // Act & Assert
      this.pricingService.GetProductPrice("1001").Should().Be(0);
    }

    /// <summary>
    /// Should get the product bulk prices by product Ids.
    /// </summary>
    [Fact]
    public void ShouldGetProductBulkPricesByProductIds()
    {
      // Arrange
      this.serviceProvider
        .GetProductBulkPrices(Arg.Is<GetProductBulkPricesRequest>(r => r.PriceType == "List" && r.ProductIds.Contains("1001") && r.ProductIds.Contains("1002") && r.ProductIds.Count() == 2))
        .Returns(new GetProductBulkPricesResult { Prices = { { "1001", new Price(80, "USD") }, { "1002", new Price(100, "USD") } } });

      // Act & Assert
      this.pricingService.GetProductBulkPrices(new[] { "1001", "1002" })
        .ShouldBeEquivalentTo(new Dictionary<string, decimal> { { "1001", 80 }, { "1002", 100 } });
    }

    /// <summary>
    /// Should return zero if no bulk prices found.
    /// </summary>
    [Fact]
    public void ShouldReturnZeroIfNoBulkPricesFound()
    {
      // Arange
      this.serviceProvider
        .GetProductBulkPrices(null)
        .ReturnsForAnyArgs(new GetProductBulkPricesResult());

      // Act
      var prices = this.pricingService.GetProductBulkPrices(new[] { "1001" });

      // Assert
      prices.Count.Should().Be(1);
      prices["1001"].Should().Be(0);
    }

    /// <summary>
    /// Should return price if duplicated product ids passed.
    /// </summary>
    [Fact]
    public void ShouldReturnPriceIfDuplicatedProductIdsPassed()
    {
      // Arrange
      this.serviceProvider
        .GetProductBulkPrices(null)
        .ReturnsForAnyArgs(new GetProductBulkPricesResult { Prices = { { "1001", new Price(80, "USD") } } });

      // Act
      var prices = this.pricingService.GetProductBulkPrices(new[] { "1001", "1001" });

      // Assert
      prices.Count.Should().Be(1);
      prices["1001"].Should().Be(80);
    }
  }
}