// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadECSProductGlobalSpecificationsTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product global specifications test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeProductGlobalSpecifications
{
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductGlobalSpecifications;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system product global specifications test.
  /// </summary>
  public class ReadECSProductGlobalSpecificationsTest
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
    private readonly ReadExternalCommerceSystemProductGlobalSpecifications processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly SynchronizeProductGlobalSpecificationsRequest request;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadECSProductGlobalSpecificationsTest" /> class.
    /// </summary>
    public ReadECSProductGlobalSpecificationsTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();

      this.entityFactory.Create("Specification").Returns(callInfo => new Specification());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemProductGlobalSpecifications { EntityFactory = this.entityFactory, ClientFactory = clientFactory };

      this.request = new SynchronizeProductGlobalSpecificationsRequest("100500");
      this.args = new ServicePipelineArgs(this.request, new ServiceProviderResult());
    }

    /// <summary>
    /// Should get product global specifications.
    /// </summary>
    [Fact]
    public void ShouldGetProductGlobalSpecifications()
    {
      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetProductGlobalSpecifications("100500");
    }

    /// <summary>
    /// Should not fill result with product resources data if it is null.
    /// </summary>
    [Fact]
    public void ShouldNotFillResultWithProductSpecificationsDataIfItIsNull()
    {
      // arrange
      this.client.GetProductGlobalSpecifications("100500").Returns((ProductGlobalSpecificationModel[])null);

      // act
      this.processor.Process(this.args);

      // assert
      this.args.Request.Properties["ProductSpecifications"].Should().BeNull();
    }

    /// <summary>
    /// Should fill result with product specifications data if it is not null.
    /// </summary>
    [Fact]
    public void ShouldFillResultWithProductSpecificationsDataIfItIsNotNull()
    {
      // arrange
      var productSpecificationModels = new[]
        {
          new ProductGlobalSpecificationModel
            {
              SpecificationLookupId = "157", 
              LookupValueId = "157_300",
              SpecificationLookupName = "hard drive",
              LookupValueName = "xxx"
            }, 
          new ProductGlobalSpecificationModel
            {
              SpecificationLookupId = "200", 
              LookupValueId = "200_400",
              SpecificationLookupName = "memory",
              LookupValueName = "yyy"
            }
        };
      this.client.GetProductGlobalSpecifications("100500").Returns(productSpecificationModels);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<Specification>)this.args.Request.Properties["ProductSpecifications"];
      result.Count().Should().Be(2);

      var firstSpecification = result.ElementAt(0);
      firstSpecification.ExternalId.Should().Be("157");
      firstSpecification.Key.Should().Be("hard drive");
      firstSpecification.Value.Should().Be("xxx");

      var secondSpecification = result.ElementAt(1);
      secondSpecification.ExternalId.Should().Be("200");
      secondSpecification.Key.Should().Be("memory");
      secondSpecification.Value.Should().Be("yyy");
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
                                          ProductSpecifications = new ReadOnlyCollectionAdapter<Specification> { new Specification { ExternalId = "1", Key = "engine", Value = "petrol" } },
                                        };

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<Specification>)this.args.Request.Properties["ProductSpecifications"];
      result.Count().Should().Be(1);
      var firstSpecification = result.ElementAt(0);
      firstSpecification.ExternalId.Should().Be("1");
      firstSpecification.Key.Should().Be("engine");
      firstSpecification.Value.Should().Be("petrol");
    }

    /// <summary>
    /// Should not call network if data from saved product exists.
    /// </summary>
    [Fact]
    public void ShouldNotCallNetworkIfDataFromSavedProductExists()
    {
      // arrange
      this.args.Request.Properties["Product"] = new Product { Manufacturers = new ReadOnlyCollectionAdapter<Manufacturer> { new Manufacturer { ExternalId = "Audi" } } };

      // act
      this.processor.Process(this.args);

      // assert
      this.client.DidNotReceive().GetManufacturer(Arg.Any<string>());
    }
  }
}