// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductTypesTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product types test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeProductTypes
{
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductTypes;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;
  using Xunit.Extensions;
    using System.Collections.ObjectModel;

  /// <summary>
  /// The read external commerce system product types test.
  /// </summary>
  public class ReadExternalCommerceSystemProductTypesTest
  {
     /// <summary>
    /// The client.
    /// </summary>
    private readonly IProductsServiceChannel client;

    /// <summary>
    /// The processor.
    /// </summary>
    private readonly ReadExternalCommerceSystemProductTypes processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemProductTypesTest"/> class. 
    /// </summary>
    public ReadExternalCommerceSystemProductTypesTest()
    {
      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemProductTypes { ClientFactory = clientFactory };

      SynchronizeProductTypesRequest request = new SynchronizeProductTypesRequest("100500");
      this.args = new ServicePipelineArgs(request, new ServiceProviderResult());
    }

    /// <summary>
    /// Should get specific manufacturer.
    /// </summary>
    [Fact]
    public void ShouldGetSpecificProductTypes()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetProductTypes("100500");
    }

    /// <summary>
    /// Should fill result with manufacturer data.
    /// </summary>
    [Fact]
    public void ShouldFillResultWithProductTypeData()
    {
      // arrange
      var models = new[] { new ProductTypeModel { Id = "157" }, new ProductTypeModel { Id = "33" } };
      this.client.GetProductTypes("100500").Returns(models);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<string>)this.args.Request.Properties["ProductTypeIds"];
      result.Count().Should().Be(2);
      result.ElementAt(0).Should().Be("157");
      result.ElementAt(1).Should().Be("33");
    }

    /// <summary>
    /// Should fill result with data from saved product.
    /// </summary>
    [Fact]
    public void ShouldFillResultWithDataFromSavedProduct()
    {
      // arrange
      this.args.Request.Properties["Product"] = new Product
      {
          ProductTypes = new ReadOnlyCollection<ProductType>(new List<ProductType> { new ProductType { ExternalId = "Cars" } })
      };

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<string>)this.args.Request.Properties["ProductTypeIds"];
      result.Count().Should().Be(1);
      result.ElementAt(0).Should().Be("Cars");
    }

    /// <summary>
    /// Should not call network if data from saved product exists.
    /// </summary>
    [Fact]
    public void ShouldNotCallNetworkIfDataFromSavedProductExists()
    {
      // arrange
      this.args.Request.Properties["Product"] = new Product { ProductTypes = new ReadOnlyCollectionAdapter<ProductType> { new ProductType { ExternalId = "Carts" } } };

      // act
      this.processor.Process(this.args);

      // assert
      this.client.DidNotReceive().GetManufacturer(Arg.Any<string>());
    }
  }
}