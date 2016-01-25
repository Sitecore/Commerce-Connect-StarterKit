// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemResourcesTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system resources test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeResources
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeResources;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system resources test.
  /// </summary>
  public class ReadExternalCommerceSystemResourcesTest
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
    private readonly ReadExternalCommerceSystemResources processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemResourcesTest" /> class.
    /// </summary>
    public ReadExternalCommerceSystemResourcesTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();
      this.entityFactory.Create("Resource").Returns(callInfo => new Resource());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemResources { EntityFactory = this.entityFactory, ClientFactory = clientFactory };

      var request = new SynchronizeResourcesRequest();
      var result = new ServiceProviderResult();
      this.args = new ServicePipelineArgs(request, result);
    }

    /// <summary>
    /// Should get all manufacturers.
    /// </summary>
    [Fact]
    public void ShouldGetAllManufacturers()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetAllResources();
    }

    /// <summary>
    /// Should fill request with manufacturers data.
    /// </summary>
    [Fact]
    public void ShouldFillRequestWithResourcesData()
    {
      // arrange
      var firstDate = DateTime.Now;
      var secondDate = DateTime.Now;
      var firstPictureBinary = new byte[10];
      var secondPictureBinary = new byte[20];
      var resources = new[]
      {
        new ResourceModel
        {
          CreatedOnUtc = firstDate, 
          UpdatedOnUtc = firstDate, 
          Id = "100500", 
          Name = "first.jpg", 
          MimeType = "image/jpg", 
          BinaryData = firstPictureBinary,
          ResourceType = "Image"
        },
        new ResourceModel
        {
          CreatedOnUtc = secondDate, 
          UpdatedOnUtc = secondDate, 
          Id = "100501", 
          Name = "second.png", 
          MimeType = "image/png", 
          BinaryData = secondPictureBinary,
          ResourceType = "Image"
        }
      };
      this.client.GetAllResources().Returns(resources);

      // act
      this.processor.Process(this.args);

      // assert
      var resourcesResult = (IEnumerable<Resource>)this.args.Request.Properties["Resources"];
      resourcesResult.Count().Should().Be(2);

      var firstResource = resourcesResult.ElementAt(0);
      firstResource.Created.Should().Be(firstDate);
      firstResource.Updated.Should().Be(firstDate);
      firstResource.Name.Should().Be("first.jpg");
      firstResource.ExternalId.Should().Be("100500");
      firstResource.MimeType.Should().Be("image/jpg");
      firstResource.BinaryData.Should().BeEquivalentTo(firstPictureBinary);
      firstResource.ResourceType.Should().Be("Image");

      var secondResource = resourcesResult.ElementAt(1);
      secondResource.Created.Should().Be(secondDate);
      secondResource.Updated.Should().Be(secondDate);
      secondResource.Name.Should().Be("second.png");
      secondResource.ExternalId.Should().Be("100501");
      secondResource.MimeType.Should().Be("image/png");
      secondResource.BinaryData.Should().BeEquivalentTo(secondPictureBinary);
      secondResource.ResourceType.Should().Be("Image");
    }
  }
}