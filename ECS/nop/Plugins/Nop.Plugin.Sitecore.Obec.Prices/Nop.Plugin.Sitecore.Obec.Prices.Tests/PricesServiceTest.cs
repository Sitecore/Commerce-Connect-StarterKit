// -----------------------------------------------------------------
// <copyright file="PricesServiceTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The price service test.
// </summary>
// -----------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -----------------------------------------------------------------
namespace Nop.Plugin.Sitecore.Commerce.Prices.Tests
{
  using FluentAssertions;
  using Nop.Core.Domain.Catalog;
  using Nop.Core.Infrastructure;
  using Nop.Services.Catalog;
  using NSubstitute;
  using Xunit;

  /// <summary>
  /// The price service test.
  /// </summary>
  public class PricesServiceTest
  {
    /// <summary>
    /// The product service.
    /// </summary>
    private readonly IProductService productService;

    /// <summary>
    /// The product information service.
    /// </summary>
    private readonly PricesService pricesService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PricesServiceTest" /> class.
    /// </summary>
    public PricesServiceTest()
    {
      var container = Substitute.For<IEngine>();
      this.productService = Substitute.For<IProductService>();

      container.Resolve<IProductService>().Returns(this.productService);
      EngineContext.Replace(container);

      this.pricesService = new PricesService();
    }

    /// <summary>
    /// Should return product price if list price type was specified.
    /// </summary>
    [Fact]
    public void ShouldReturnProductPriceIfListPriceTypeWasSpecified()
    {
      // Arrange
      this.productService.GetProductById(100500).Returns(new Product { Price = 1 });

      // Act
      var price = this.pricesService.GetProductPrice("100500", "List");

      // Assert
      price.Value.Should().Be(1);
    }

    /// <summary>
    /// Should throw argument exception if unknown price type was specified.
    /// </summary>
    [Fact]
    public void ShouldReturnNullIfUnknownPriceTypeWasSpecified()
    {
      // Act
      this.productService.GetProductById(100500).Returns(new Product { Id = 100500 });

      var price = this.pricesService.GetProductPrice("100500", "Custom price type");

      // Assert
      price.Should().Be(null);
    }

    /// <summary>
    /// Should throw invalid operation exception if product cannot be found.
    /// </summary>
    [Fact]
    public void ShouldReturnNullIfProductCannotBeFound()
    {
      // Arrange
      this.productService.GetProductById(100500).Returns((Product)null);

      // Act
      var price = this.pricesService.GetProductPrice("100500", "List");

      // Assert
      price.Should().Be(null);
    }
  }
}