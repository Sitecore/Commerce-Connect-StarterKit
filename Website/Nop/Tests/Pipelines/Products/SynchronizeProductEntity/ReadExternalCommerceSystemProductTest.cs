// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read product test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeProductEntity
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductEntity;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;
  using Xunit.Extensions;

  /// <summary>
  /// The read product test.
  /// </summary>
  public class ReadExternalCommerceSystemProductTest
  {
    /// <summary>
    /// The product classification group name.
    /// </summary>
    private readonly string productClassificationGroupName;

    /// <summary>
    /// The product classification group external id.
    /// </summary>
    private readonly string productClassificationGroupExternalId;

    /// <summary>
    /// The entity factory.
    /// </summary>
    private readonly IEntityFactory entityFactory;

    /// <summary>
    /// The client
    /// </summary>
    private readonly IProductsServiceChannel client;

    /// <summary>
    /// The processor
    /// </summary>
    private readonly ReadExternalCommerceSystemProduct processor;

    /// <summary>
    /// The product.
    /// </summary>
    private readonly Product product;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly SynchronizeProductRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly ServiceProviderResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The product model.
    /// </summary>
    private readonly ProductModel productModel;

    /// <summary>
    /// The created.
    /// </summary>
    private readonly DateTime created;

    /// <summary>
    /// The updated.
    /// </summary>
    private readonly DateTime updated;

    /// <summary>
    /// The language.
    /// </summary>
    private string language;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemProductTest" /> class.
    /// </summary>
    // TODO:[Minor] Test initialization is too complex and it is not required for all the tests in the class. This lead to unclear test behaviour.
    public ReadExternalCommerceSystemProductTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();

      this.entityFactory.Create("Product").Returns(callInfo => new Product());
      this.entityFactory.Create("ClassificationGroup").Returns(callInfo => new ClassificationGroup());
      this.entityFactory.Create("Classification").Returns(callInfo => new Classification());
      this.entityFactory.Create("Specification").Returns(callInfo => new Specification());
      this.entityFactory.Create("ProductType").Returns(callInfo => new ProductType());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.productClassificationGroupName = "Categories";
      this.productClassificationGroupExternalId = "0";
      this.processor = new ReadExternalCommerceSystemProduct(this.productClassificationGroupName, this.productClassificationGroupExternalId) { EntityFactory = this.entityFactory, ClientFactory = clientFactory };

      this.product = new Product { ExternalId = "100500" };
      this.language = "en";
      this.request = new SynchronizeProductRequest("100500") { Language = this.language };
      this.result = new ServiceProviderResult();
      this.args = new ServicePipelineArgs(this.request, this.result);

      this.created = DateTime.Now;
      this.updated = DateTime.Now;
      this.productModel = new ProductModel
                            {
                              ProductId = "100500",
                              ProductVariantId = "100500",
                              Name = "Cool car",
                              ShortDescription = "Yea, dude. You wants this one",
                              FullDescription = "Don't think, just pay for this",
                              ManufacturerIds = new[] { "The best company in the world" },
                              CreatedOnUtc = this.created,
                              UpdatedOnUtc = this.updated,
                              CategoryIds = new[] { "Cars", "Luxury" },
                              Sku = "sku",
                              ProductGlobalSpecifications = new[]
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
        }
                            };
    }

    /// <summary>
    /// Should try to get product by external product id.
    /// </summary>
    [Fact]
    public void ShouldTryToGetProductByExternalProductId()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetProduct("100500", this.language);
    }

    /// <summary>
    /// Should return error message if product model is not found.
    /// </summary>
    [Fact]
    public void ShouldReturnErrorMessageIfProductModelIsNotFound()
    {
      // arrange
      ProductModel productModelTemp = null;
      this.client.GetProduct("100500", this.language).Returns(productModelTemp);

      // act
      this.processor.Process(this.args);

      // assert
      this.result.SystemMessages.Count.Should().Be(1);
      this.result.SystemMessages.First().Message.Should().Be(string.Format(Commerce.Texts.FailedToSynchronizeProduct0, "100500"));
    }

    /// <summary>
    /// Should return error message if product model contains null or empty id.
    /// </summary>
    /// <param name="productVariantId">The product variant id.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldReturnErrorMessageIfProductModelContainsNullOrEmptyId(string productVariantId)
    {
      // arrange
      ProductModel productModelTemp = new ProductModel { ProductId = "100500", ProductVariantId = productVariantId };
      this.client.GetProduct("100500", this.language).Returns(productModelTemp);

      // act
      this.processor.Process(this.args);

      // assert
      this.result.SystemMessages.Count.Should().Be(1);
      this.result.SystemMessages.First().Message.Should().Be(string.Format(Commerce.Texts.FailedToSynchronizeProduct0, "100500"));
    }

    /// <summary>
    /// Should return product filled by data from external system.
    /// </summary>
    [Fact]
    public void ShouldReturnProductFilledByDataFromExternalSystem()
    {
      // arrange
      this.client.GetProduct("100500", this.language).Returns(this.productModel);

      // act
      this.processor.Process(this.args);

      // assert
      var actual = this.args.Request.Properties["Product"] as Product;
      Diagnostics.Assert.IsNotNull(actual, typeof(Product));
      actual.ExternalId.Should().Be("100500");
      actual.Name.Should().Be("Cool car");
      actual.ShortDescription.Should().Be("Yea, dude. You wants this one");
      actual.FullDescription.Should().Be("Don't think, just pay for this");
      Diagnostics.Assert.IsNotNull(actual.Manufacturers, typeof(Manufacturer));
      actual.Manufacturers.First().ExternalId.Should().Be("The best company in the world");
      actual.Created.Should().Be(this.created);
      actual.Updated.Should().Be(this.updated);
      Diagnostics.Assert.IsNotNull(actual.ClassificationGroups, typeof(IReadOnlyCollection<ClassificationGroup>));
      actual.ClassificationGroups.Count.Should().Be(1);
      var classificationGroup = actual.ClassificationGroups.First();
      classificationGroup.Name.Should().Be(this.productClassificationGroupName);
      classificationGroup.ExternalId.Should().Be(this.productClassificationGroupExternalId);
      Diagnostics.Assert.IsNotNull(classificationGroup.Classifications, typeof(IReadOnlyCollection<ClassificationGroup>));
      classificationGroup.Classifications.Count.Should().Be(2);
      Diagnostics.Assert.IsNotNull(classificationGroup.Classifications.ElementAt(0), typeof(Classification));
      classificationGroup.Classifications.ElementAt(0).ExternalId.Should().Be("Cars");
      Diagnostics.Assert.IsNotNull(classificationGroup.Classifications.ElementAt(1), typeof(Classification));
      classificationGroup.Classifications.ElementAt(1).ExternalId.Should().Be("Luxury");
      actual.ProductTypes.Count.Should().Be(2);
      actual.ProductTypes.ElementAt(0).ExternalId.Should().Be("Cars");
      actual.ProductTypes.ElementAt(1).ExternalId.Should().Be("Luxury");
      actual.Identification.Should().HaveCount(1);
      actual.Identification["Sku"].Should().Be("sku");

      actual.ProductSpecifications.Count().Should().Be(2);

      var firstSpecification = actual.ProductSpecifications.ElementAt(0);
      firstSpecification.ExternalId.Should().Be("157");
      firstSpecification.Key.Should().Be("hard drive");
      firstSpecification.Value.Should().Be("xxx");

      var secondSpecification = actual.ProductSpecifications.ElementAt(1);
      secondSpecification.ExternalId.Should().Be("200");
      secondSpecification.Key.Should().Be("memory");
      secondSpecification.Value.Should().Be("yyy");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldNotCreateSkuIdentificationRecordIfSkuValueIsNullOrEmpty(string skuValue)
    {
      // arrange
      this.client.GetProduct("100500", this.language).Returns(new ProductModel { ProductId = "100500", ProductVariantId = "100500", Sku = skuValue, CategoryIds = new string[0], ManufacturerIds = new string[0], ProductGlobalSpecifications = new ProductGlobalSpecificationModel[0] });

      // act
      this.processor.Process(this.args);

      // assert
      var actual = this.args.Request.Properties["Product"] as Product;
      Diagnostics.Assert.IsNotNull(actual, typeof(Product));
      actual.Identification.ContainsKey("Sku").Should().BeFalse("Sku identification key should not be created");
    }

    /// <summary>
    /// Should not return any message if product is read correctly.
    /// </summary>
    [Fact]
    public void ShouldNotReturnAnyMessageIfProductIsReadCorrectly()
    {
      // arrange
      this.client.GetProduct("100500", this.language).Returns(this.productModel);

      // act
      this.processor.Process(this.args);

      // assert
      this.result.SystemMessages.Should().BeEmpty();
    }

    /// <summary>
    /// Should call NOP service using two letter ISO language name.
    /// </summary>
    [Fact]
    public void ShouldCallNopServiceUsingTwoLetterIsoLanguageName()
    {
      // arrange
      var request = new SynchronizeProductRequest("1001") { Language = "uk-UA" };

      // act
      this.processor.Process(new ServicePipelineArgs(request, new ServiceProviderResult()));

      // assert
      this.client.Received().GetProduct("1001", "uk");
    }
  }
}