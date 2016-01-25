// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductDownloadsTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product downloads test.
// </summary>
// ---------------------------------------------------------------------
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeProductResources
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductResources;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system product downloads test.
  /// </summary>
  public class ReadExternalCommerceSystemProductDownloadsTest
  {
    /// <summary>
    /// The entity factory.
    /// </summary>
    private readonly IEntityFactory entityFactory;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly IProductsServiceChannel client;

    /// <summary>
    /// The processor.
    /// </summary>
    private readonly ReadExternalCommerceSystemProductDownloads processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly SynchronizeProductResourcesRequest request;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemProductDownloadsTest" /> class.
    /// </summary>
    public ReadExternalCommerceSystemProductDownloadsTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();
      this.entityFactory.Create("ProductResource").Returns(callInfo => new ProductResource());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemProductDownloads { EntityFactory = this.entityFactory, ClientFactory = clientFactory };

      this.request = new SynchronizeProductResourcesRequest("100500");
      this.args = new ServicePipelineArgs(this.request, new ServiceProviderResult());
    }

    /// <summary>
    /// Should the get product alternate images.
    /// </summary>
    [Fact]
    public void ShouldGetProductDownloads()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetProductDownloads("100500");
    }

    /// <summary>
    /// Should fill result with manufacturer data.
    /// </summary>
    [Fact]
    public void ShouldFillResultWithProductResourcesDataIfItIsNotNull()
    {
      // arrange
      var firstDate = DateTime.Now;
      var secondDate = DateTime.Now;
      var resourceModels = new[]
                     {
                       new ResourceModel
                         {
                           Id = "157", 
                           CreatedOnUtc = firstDate,
                           UpdatedOnUtc = firstDate,
                           ResourceType = "download 1",
                           Url = "http://download_1",
                           Name = "first"
                         }, 
                         new ResourceModel
                         {
                           Id = "33", 
                           CreatedOnUtc = secondDate,
                           UpdatedOnUtc = secondDate,
                           ResourceType = "download 2",
                           Url = "http://download_2",
                           Name = "second"
                         }, 
                     };
      var productResourceModel = new ProductResourceModel { Resources = resourceModels };
      this.client.GetProductDownloads("100500").Returns(productResourceModel);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<ProductResource>)this.args.Request.Properties["ProductResources"];
      result.Count().Should().Be(2);

      var firstResource = result.ElementAt(0);
      firstResource.Created.Should().Be(firstDate);
      firstResource.Updated.Should().Be(firstDate);
      firstResource.ExternalId.Should().Be("157");
      firstResource.Type.Should().Be("download 1");
      firstResource.Uri.Should().Be("http://download_1");
      firstResource.Name.Should().Be("first");

      var secondResource = result.ElementAt(1);
      secondResource.Created.Should().Be(secondDate);
      secondResource.Updated.Should().Be(secondDate);
      secondResource.ExternalId.Should().Be("33");
      secondResource.Type.Should().Be("download 2");
      secondResource.Uri.Should().Be("http://download_2");
      secondResource.Name.Should().Be("second");
    }

    /// <summary>
    /// Should not fill result with product resources data if it is null.
    /// </summary>
    [Fact]
    public void ShouldNotFillResultWithProductResourcesDataIfItIsNull()
    {
      // arrange
      ProductResourceModel productResourceModel = null;
      this.client.GetProductDownloads("100500").Returns(productResourceModel);

      // act
      this.processor.Process(this.args);

      // assert
      this.args.Request.Properties["ProductResources"].Should().BeNull();
    }

    /// <summary>
    /// Should not fill result with product resources data if it has null resources.
    /// </summary>
    [Fact]
    public void ShouldNotFillResultWithProductResourcesDataIfItHasNullResources()
    {
      // arrange
      ProductResourceModel productResourceModel = new ProductResourceModel { Resources = null };
      this.client.GetProductDownloads("100500").Returns(productResourceModel);

      // act
      this.processor.Process(this.args);

      // assert
      this.args.Request.Properties["ProductResources"].Should().BeNull();
    }
  }
}