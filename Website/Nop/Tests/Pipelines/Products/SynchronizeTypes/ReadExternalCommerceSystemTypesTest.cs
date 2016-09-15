// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemTypesTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system types test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeTypes
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeTypes;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system types test.
  /// </summary>
  public class ReadExternalCommerceSystemTypesTest
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
    private readonly ReadExternalCommerceSystemTypes processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly SynchronizeTypesRequest request;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemTypesTest" /> class.
    /// </summary>
    public ReadExternalCommerceSystemTypesTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();
      this.entityFactory.Create("ProductType").Returns(callInfo => new ProductType());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemTypes { EntityFactory = this.entityFactory, ClientFactory = clientFactory };

      this.request = new SynchronizeTypesRequest();
      this.args = new ServicePipelineArgs(this.request, new ServiceProviderResult());
    }

    /// <summary>
    /// Should get specific manufacturer.
    /// </summary>
    [Fact]
    public void ShouldGetTypes()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetAllProductTypes();
    }

    /// <summary>
    /// Should fill request with manufacturers data.
    /// </summary>
    [Fact]
    public void ShouldFillRequestWithDivisionsData()
    {
      // arrange
      var firstDate = DateTime.Now;
      var secondDate = DateTime.Now;
      var types = new[]
      {  
        new ProductTypeModel { CreatedOnUtc = firstDate, UpdatedOnUtc = firstDate, Description = "Porsche the best", Id = "100500", Name = "Porsche", ParentProductTypeId = "Volkswagen" },
        new ProductTypeModel { CreatedOnUtc = secondDate, UpdatedOnUtc = secondDate, Description = "Citroen cool", Id = "100501", Name = "Citroen", ParentProductTypeId = "none" }
      };
      this.client.GetAllProductTypes().Returns(types);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<ProductType>)this.args.Request.Properties["ProductTypes"];
      result.Count().Should().Be(2);

      var firstType = result.ElementAt(0);
      firstType.Created.Should().Be(firstDate);
      firstType.Updated.Should().Be(firstDate);
      firstType.Description.Should().Be("Porsche the best");
      firstType.Name.Should().Be("Porsche");
      firstType.ExternalId.Should().Be("100500");
      firstType.ProductTypeId.Should().Be("100500");
      firstType.ParentProductTypeId.Should().Be("Volkswagen");

      var secondType = result.ElementAt(1);
      secondType.Created.Should().Be(secondDate);
      secondType.Updated.Should().Be(secondDate);
      secondType.Description.Should().Be("Citroen cool");
      secondType.Name.Should().Be("Citroen");
      secondType.ExternalId.Should().Be("100501");
      secondType.ProductTypeId.Should().Be("100501");
      secondType.ParentProductTypeId.Should().Be("none");
    }
  }
}