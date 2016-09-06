// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NopProductProcessorTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The nop product processor test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Common
{
  using FluentAssertions;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Xunit;
  using Xunit.Extensions;

  /// <summary>
  /// The product processor test.
  /// </summary>
  public class NopProductProcessorTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly FakeProductProcessor processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="NopProductProcessorTest"/> class.
    /// </summary>
    public NopProductProcessorTest()
    {
      this.processor = new FakeProductProcessor();
    }

    /// <summary>
    /// Should get product data from previous processors.
    /// </summary>
    [Fact]
    public void ShouldGetProductDataFromPreviousProcessors()
    {
      // arrange
      var args = new ServicePipelineArgs(new ServiceProviderRequest(), new ServiceProviderResult());
      args.Request.Properties["Product"] = new Product();

      // act
      this.processor.Process(args);

      // assert
      args.Request.Properties["Result"].Should().Be("From saved product");
    }

    /// <summary>
    /// Should get product data from network if custom data is empty.
    /// </summary>
    /// <param name="value">The value.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldGetProductDataFromNetworkIfCustomDataIsEmptyOrNotProduct(object value)
    {
      // arrange
      var args = new ServicePipelineArgs(new ServiceProviderRequest(), new ServiceProviderResult());
      args.Request.Properties["Product"] = value;

      // act
      this.processor.Process(args);

      // assert
      args.Request.Properties["Result"].Should().Be("From network");
    }

    /// <summary>
    /// The fake product processor.
    /// </summary>
    private class FakeProductProcessor : NopProductProcessor<IProductsServiceChannel>
    {
      /// <summary>
      /// Gets the product data from service.
      /// </summary>
      /// <param name="args">The arguments.</param>
      protected override void GetProductDataFromService(ServicePipelineArgs args)
      {
        args.Request.Properties["Result"] = "From network";
      }

      /// <summary>
      /// Sets the data from saved product.
      /// </summary>
      /// <param name="args">The arguments.</param>
      /// <param name="product">The product.</param>
      protected override void SetDataFromSavedProduct(ServicePipelineArgs args, Product product)
      {
        args.Request.Properties["Result"] = "From saved product";
      }
    }
  }
}