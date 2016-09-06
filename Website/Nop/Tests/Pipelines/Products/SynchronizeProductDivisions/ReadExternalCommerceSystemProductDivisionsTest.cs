// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductDivisionsTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product divisions test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeProductDivisions
{
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductDivisions;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system product divisions test.
  /// </summary>
  public class ReadExternalCommerceSystemProductDivisionsTest
  {
     /// <summary>
    /// The client.
    /// </summary>
    private readonly IProductsServiceChannel client;

    /// <summary>
    /// The processor.
    /// </summary>
    private readonly ReadExternalCommerceSystemProductDivisions processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly SynchronizeProductDivisionsRequest request;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemProductDivisionsTest"/> class. 
    /// </summary>
    public ReadExternalCommerceSystemProductDivisionsTest()
    {
      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemProductDivisions { ClientFactory = clientFactory };

      this.request = new SynchronizeProductDivisionsRequest("100500");
      this.args = new ServicePipelineArgs(this.request, new ServiceProviderResult());
    }

    /// <summary>
    /// Should get specific manufacturer.
    /// </summary>
    [Fact]
    public void ShouldGetRelatedDivisions()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetRelatedDivisions("100500");
    }

    /// <summary>
    /// Should get specific manufacturer.
    /// </summary>
    [Fact]
    public void ShouldReturnEmptyCollectionIfProductDataModelIsNull()
    {
      // arrange
      ProductDivisionsModel productDivisionsModel = null;
      this.client.GetRelatedDivisions("100500").Returns(productDivisionsModel);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<string>)this.args.Request.Properties["DivisionIds"];
      result.Should().NotBeNull();
      result.Should().BeEmpty();
    }

    /// <summary>
    /// Should get specific manufacturer.
    /// </summary>
    [Fact]
    public void ShouldReturnEmptyCollectionIfProductDataModelDivisionsIsNull()
    {
      // arrange
      ProductDivisionsModel productDivisionsModel = new ProductDivisionsModel { Divisions = null };
      this.client.GetRelatedDivisions("100500").Returns(productDivisionsModel);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<string>)this.args.Request.Properties["DivisionIds"];
      result.Should().NotBeNull();
      result.Should().BeEmpty();
    }

    /// <summary>
    /// Should fill result with manufacturer data.
    /// </summary>
    [Fact]
    public void ShouldFillResultWithDivisionsData()
    {
      // arrange
      var models = new[] { new DivisionModel { Id = "157" }, new DivisionModel { Id = "33" } };
      var productDivisionModel = new ProductDivisionsModel { Divisions = models };
      this.client.GetRelatedDivisions("100500").Returns(productDivisionModel);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<string>)this.args.Request.Properties["DivisionIds"];
      result.Count().Should().Be(2);
      result.ElementAt(0).Should().Be("157");
      result.ElementAt(1).Should().Be("33");
    }
  }
}