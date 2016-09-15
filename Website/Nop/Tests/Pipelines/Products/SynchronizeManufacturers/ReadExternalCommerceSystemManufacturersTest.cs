// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemManufacturersTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system manufacturers test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeManufacturers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeManufacturers;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system manufacturers.
  /// </summary>
  public class ReadExternalCommerceSystemManufacturersTest
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
    private readonly ReadExternalCommerceSystemManufacturers processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly SynchronizeManufacturersRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly ServiceProviderResult result;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemManufacturersTest" /> class.
    /// </summary>
    public ReadExternalCommerceSystemManufacturersTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();

      this.entityFactory.Create("Manufacturer").Returns(callInfo => new Manufacturer());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemManufacturers { EntityFactory = entityFactory, ClientFactory = clientFactory };

      this.request = new SynchronizeManufacturersRequest();
      this.result = new ServiceProviderResult();
      this.args = new ServicePipelineArgs(this.request, this.result);
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
      this.client.Received().GetAllManufacturers();
    }

    /// <summary>
    /// Should fill request with manufacturers data.
    /// </summary>
    [Fact]
    public void ShouldFillRequestWithManufacturersData()
    {
      // arrange
      var firstDate = DateTime.Now;
      var secondDate = DateTime.Now;
      var manufacturers = new[]
      {
        new ManufacturerModel { CreatedOnUtc = firstDate, UpdatedOnUtc = firstDate, Description = "Porsche the best", Id = "100500", Name = "Porsche" },
        new ManufacturerModel { CreatedOnUtc = secondDate, UpdatedOnUtc = secondDate, Description = "Citroen cool", Id = "100501", Name = "Citroen" },
      };
      this.client.GetAllManufacturers().Returns(manufacturers);

      // act
      this.processor.Process(this.args);

      // assert
      var manufacturersResult = (IEnumerable<Manufacturer>)this.args.Request.Properties["Manufacturers"];
      manufacturersResult.Count().Should().Be(2);

      var firstManufacturer = manufacturersResult.ElementAt(0);
      firstManufacturer.Created.Should().Be(firstDate);
      firstManufacturer.Updated.Should().Be(firstDate);
      firstManufacturer.Description.Should().Be("Porsche the best");
      firstManufacturer.Name.Should().Be("Porsche");
      firstManufacturer.ExternalId.Should().Be("100500");

      var secondManufacturer = manufacturersResult.ElementAt(1);
      secondManufacturer.Created.Should().Be(secondDate);
      secondManufacturer.Updated.Should().Be(secondDate);
      secondManufacturer.Description.Should().Be("Citroen cool");
      secondManufacturer.Name.Should().Be("Citroen");
      secondManufacturer.ExternalId.Should().Be("100501");
    }
  }
}