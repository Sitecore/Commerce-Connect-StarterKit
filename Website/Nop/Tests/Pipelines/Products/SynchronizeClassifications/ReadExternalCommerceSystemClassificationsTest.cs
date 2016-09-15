// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemClassificationsTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system classifications test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeClassifications
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeClassifications;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system classifications test.
  /// </summary>
  public class ReadExternalCommerceSystemClassificationsTest
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
    private readonly ReadExternalCommerceSystemClassifications processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly SynchronizeClassificationsRequest request;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemClassificationsTest" /> class.
    /// </summary>
    public ReadExternalCommerceSystemClassificationsTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();

      this.entityFactory.Create("Classification").Returns(callInfo => new Classification());
      this.entityFactory.Create("ClassificationGroup").Returns(callInfo => new ClassificationGroup());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemClassifications("categories", "100500") { EntityFactory = this.entityFactory, ClientFactory = clientFactory };

      this.request = new SynchronizeClassificationsRequest();
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
      this.client.Received().GetAllCategories();
    }

    /// <summary>
    /// Should fill request with classifications data.
    /// </summary>
    [Fact]
    public void ShouldFillRequestWithClassificationsData()
    {
      // arrange
      var firstDate = DateTime.Now;
      var secondDate = DateTime.Now;
      var categoryModels = new[]
      {  
        new CategoryModel { Description = "Porsche the best", Id = "100500", Name = "Porsche", ParentCategoryId = "Volkswagen", CreatedOnUtc = firstDate, UpdatedOnUtc = secondDate },
        new CategoryModel { Description = "Citroen cool", Id = "100501", Name = "Citroen", ParentCategoryId = "none", CreatedOnUtc = secondDate, UpdatedOnUtc = secondDate }
      };

      this.client.GetAllCategories().Returns(categoryModels);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<ClassificationGroup>)this.args.Request.Properties["ClassificationGroups"];
      result.Count().Should().Be(1);
      var classificationGroup = result.First();
      classificationGroup.Name.Should().Be("categories");
      classificationGroup.ExternalId.Should().Be("100500");
      Diagnostics.Assert.IsNotNull(classificationGroup.Classifications, "Classifications should not be null.");
      classificationGroup.Classifications.Count.Should().Be(2);
      var firstClassification = classificationGroup.Classifications.ElementAt(0);
      firstClassification.Created.Should().Be(firstDate);
      firstClassification.Updated.Should().Be(firstDate);
      firstClassification.Description.Should().Be("Porsche the best");
      firstClassification.Name.Should().Be("Porsche");
      firstClassification.ExternalId.Should().Be("100500");
      firstClassification.ExternalParentId.Should().Be("Volkswagen");
      var secondClassification = classificationGroup.Classifications.ElementAt(1);
      secondClassification.Created.Should().Be(secondDate);
      secondClassification.Updated.Should().Be(secondDate);
      secondClassification.Description.Should().Be("Citroen cool");
      secondClassification.Name.Should().Be("Citroen");
      secondClassification.ExternalId.Should().Be("100501");
      secondClassification.ExternalParentId.Should().Be("none");
    }
  }
}