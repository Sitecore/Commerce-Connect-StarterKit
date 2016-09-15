// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemGlobalSpecificationsTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system global specifications test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeGlobalSpecifications
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeGlobalSpecifications;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system global specifications test.
  /// </summary>
  public class ReadExternalCommerceSystemGlobalSpecificationsTest
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
    private readonly ReadExternalCommerceSystemGlobalSpecifications processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemGlobalSpecificationsTest" /> class.
    /// </summary>
    public ReadExternalCommerceSystemGlobalSpecificationsTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();

      this.entityFactory.Create("GlobalSpecification").Returns(callInfo => new GlobalSpecification());
      this.entityFactory.Create("GlobalSpecificationOption").Returns(callInfo => new GlobalSpecificationOption());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemGlobalSpecifications { EntityFactory = this.entityFactory, ClientFactory = clientFactory };

      this.args = new ServicePipelineArgs(new SynchronizeGlobalSpecificationsRequest(), new ServiceProviderResult());
    }

    /// <summary>
    /// Should get all global specifications.
    /// </summary>
    [Fact]
    public void ShouldGetGlobalSpecifications()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetAllGlobalSpecifications();
    }

    /// <summary>
    /// Should fill request with specifications data.
    /// </summary>
    [Fact]
    public void ShouldFillRequestWithGlobalSpecificationsData()
    {
      // arrange
      var firstDate = DateTime.Now;
      var secondDate = DateTime.Now;
      var specifications = new[]
      {  
        new SpecificationLookupModel
          {
            CreatedOnUtc = firstDate, 
            UpdatedOnUtc = firstDate, 
            Id = "100500", 
            Name = "Porsche",
            SpecificationOptions = new[]
            {
              new LookupValueModel
                {
                  CreatedOnUtc = firstDate, 
                  UpdatedOnUtc = firstDate,
                  Id = "inner1",
                  ParentId = "100500",
                  Name = "firstChild"
                }
            }
          },
        new SpecificationLookupModel
          {
            CreatedOnUtc = secondDate, 
            UpdatedOnUtc = secondDate, 
            Id = "100501", 
            Name = "Citroen",
            SpecificationOptions = new[]
            {
              new LookupValueModel
                {
                  CreatedOnUtc = secondDate, 
                  UpdatedOnUtc = secondDate,
                  Id = "inner2",
                  ParentId = "100501",
                  Name = "secondChild"
                }
            }
          }
      };
      this.client.GetAllGlobalSpecifications().Returns(specifications);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<GlobalSpecification>)this.args.Request.Properties["Specifications"];
      result.Count().Should().Be(2);

      var firstSpecification = result.ElementAt(0);
      firstSpecification.Created.Should().Be(firstDate);
      firstSpecification.Updated.Should().Be(firstDate);
      firstSpecification.Name.Should().Be("Porsche");
      firstSpecification.ExternalId.Should().Be("100500");
      firstSpecification.Options.Should().NotBeNull();
      firstSpecification.Options.Count.Should().Be(1);
      var firstSpecificationOption = firstSpecification.Options.First();
      firstSpecificationOption.Created.Should().Be(firstDate);
      firstSpecificationOption.Updated.Should().Be(firstDate);
      firstSpecificationOption.ExternalId.Should().Be("inner1");
      firstSpecificationOption.ExternalSpecificationId.Should().Be("100500");

      var secondSpecification = result.ElementAt(1);
      secondSpecification.Created.Should().Be(secondDate);
      secondSpecification.Updated.Should().Be(secondDate);
      secondSpecification.Name.Should().Be("Citroen");
      secondSpecification.ExternalId.Should().Be("100501");
      secondSpecification.Options.Should().NotBeNull();
      secondSpecification.Options.Count.Should().Be(1);
      var secondSpecificationOption = secondSpecification.Options.First();
      secondSpecificationOption.Created.Should().Be(secondDate);
      secondSpecificationOption.Updated.Should().Be(secondDate);
      secondSpecificationOption.ExternalId.Should().Be("inner2");
      secondSpecificationOption.ExternalSpecificationId.Should().Be("100501");
    }

    /// <summary>
    /// Should fill request with specifications data.
    /// </summary>
    [Fact]
    public void ShouldFillRequestWithGlobalSpecificationsData2()
    {
      // arrange
      var specifications = new[] { new SpecificationLookupModel() };
      this.client.GetAllGlobalSpecifications().Returns(specifications);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<GlobalSpecification>)this.args.Request.Properties["Specifications"];
      result.Count().Should().Be(1);
      var firstSpecification = result.ElementAt(0);
      firstSpecification.Options.Should().NotBeNull();
      firstSpecification.Options.Should().BeEmpty();
    }
  }
}