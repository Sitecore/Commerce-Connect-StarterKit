// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProductBulkPricesTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetProductBulkPricesTest class.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Prices.GetProductPrices
{
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopPricesService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Prices.GetProductPrices;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Prices;
  using Xunit;

  /// <summary>
  /// Defines the GetProductBulkPricesTest class.
  /// </summary>
  public class GetProductBulkPricesTest
  {
    /// <summary>
    /// The client.
    /// </summary>
    private readonly IPricesServiceChannel client;    

    /// <summary>
    /// The get product prices.
    /// </summary>
    private readonly GetProductBulkPrices processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductBulkPricesTest" /> class.
    /// </summary>
    public GetProductBulkPricesTest()
    {
      this.client = Substitute.For<IPricesServiceChannel>();
      ServiceClientFactory clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IPricesServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new GetProductBulkPrices { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should get product prices for the specified products.
    /// </summary>
    [Fact]
    public void ShouldGetProductPricesForTheSpecifiedProducts()
    {
      // Arrange
      GetProductBulkPricesRequest request = new GetProductBulkPricesRequest(new[] { "ProductId1", "ProductId2" }, "PriceType");
      GetProductBulkPricesResult result = new GetProductBulkPricesResult();
      ServicePipelineArgs args = new ServicePipelineArgs(request, result);

      result.Prices.Add("ProductId1", null);
      this.client.GetProductPrices(Arg.Is<string[]>(argument => (argument.Length == 2) && (argument[0] == "ProductId1") && (argument[1] == "ProductId2")), "PriceType").Returns(new[] { new ProductPriceModel { ProductId = "ProductId1", Price = 2 }, new ProductPriceModel { ProductId = "ProductId2", Price = 1 } });

      // Act
      this.processor.Process(args);

      // Assert
      result.Prices.Should().HaveCount(2);
      result.Prices["ProductId1"].Amount.Should().Be(2);
      result.Prices["ProductId2"].Amount.Should().Be(1);
    }

    /// <summary>
    /// Should get product prices in specified currency for the specified products.
    /// </summary>
    [Fact]
    public void ShouldGetProductPricesInCuurencyForTheSpecifiedProducts()
    {
        // Arrange
        GetProductBulkPricesRequest request = new GetProductBulkPricesRequest(new[] { "ProductId1", "ProductId2" }, "PriceType");
        request.CurrencyCode = "CAD";
        GetProductBulkPricesResult result = new GetProductBulkPricesResult();
        ServicePipelineArgs args = new ServicePipelineArgs(request, result);

        result.Prices.Add("ProductId1", null);
        this.client.GetProductPrices(Arg.Is<string[]>(argument => (argument.Length == 2) && (argument[0] == "ProductId1") && (argument[1] == "ProductId2")), "PriceType").Returns(new[] { new ProductPriceModel { ProductId = "ProductId1", Price = 2 }, new ProductPriceModel { ProductId = "ProductId2", Price = 1 } });

        // Act
        this.processor.Process(args);

        // Assert
        result.Prices.Should().HaveCount(2);
        result.Prices["ProductId1"].Amount.Should().Be(2);
        result.Prices["ProductId1"].CurrencyCode.Should().Be("CAD");
        result.Prices["ProductId2"].Amount.Should().Be(1);
        result.Prices["ProductId2"].CurrencyCode.Should().Be("CAD");
    }

    /// <summary>
    /// Should not add new records if product price is not found.
    /// </summary>
    [Fact]
    public void ShouldNotAddNewRecordsIfProductPriceIsNotFound()
    {
      // Arrange
      GetProductBulkPricesRequest request = new GetProductBulkPricesRequest(new[] { "ProductId1" }, "PriceType");
      GetProductBulkPricesResult result = new GetProductBulkPricesResult();
      ServicePipelineArgs args = new ServicePipelineArgs(request, result);

      this.client.GetProductPrices(Arg.Is<string[]>(argument => (argument.Length == 1) && (argument[0] == "ProductId1")), "PriceType").Returns(new[] { new ProductPriceModel { ProductId = "ProductId1", Price = null } });

      // Act
      this.processor.Process(args);

      // Assert
      result.Prices.Should().BeEmpty();
    }
  }
}
