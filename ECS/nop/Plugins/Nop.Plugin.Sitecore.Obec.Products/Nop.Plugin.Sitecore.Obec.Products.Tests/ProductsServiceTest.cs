// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductInformationServiceTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The ProductInformationServiceTest class.
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
// -----------------------------------------------------------------
namespace Nop.Plugin.Sitecore.Commerce.Carts.Products.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using FluentAssertions;
  using Nop.Core;
  using Nop.Core.Data;
  using Nop.Core.Domain.Catalog;
  using Nop.Core.Domain.Localization;
  using Nop.Core.Domain.Media;
  using Nop.Core.Domain.Stores;
  using Nop.Core.Infrastructure;
  using Nop.Plugin.Sitecore.Commerce.Products;
  using Nop.Plugin.Sitecore.Commerce.Products.Models;
  using Nop.Services.Catalog;
  using Nop.Services.Localization;
  using Nop.Services.Media;
  using Nop.Services.Stores;
  using NSubstitute;
  using Xunit;

  /// <summary>
  /// The ProductInformationServiceTest class.
  /// </summary>
  public class ProductsServiceTest
  {
    /// <summary>
    /// The product service.
    /// </summary>
    private readonly IProductService productService;

    /// <summary>
    /// The store service.
    /// </summary>
    private readonly IStoreService storeService;

    /// <summary>
    /// The product information service.
    /// </summary>
    private readonly ProductsService productInformationService;

    /// <summary>
    /// The picture service.
    /// </summary>
    private readonly IPictureService pictureService;

    /// <summary>
    /// The download repository.
    /// </summary>
    private readonly IRepository<Download> downloadRepository;

    /// <summary>
    /// The language service.
    /// </summary>
    private readonly ILanguageService languageService;

    /// <summary>
    /// The localized entity service.
    /// </summary>
    private readonly ILocalizedEntityService localizedEntityService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsServiceTest" /> class.
    /// </summary>
    public ProductsServiceTest()
    {
      var container = Substitute.For<IEngine>();
      this.productService = Substitute.For<IProductService>();
      this.storeService = Substitute.For<IStoreService>();
      this.pictureService = Substitute.For<IPictureService>();
      this.downloadRepository = Substitute.For<IRepository<Download>>();
      this.languageService = Substitute.For<ILanguageService>();
      this.localizedEntityService = Substitute.For<ILocalizedEntityService>();

      container.Resolve<IProductService>().Returns(this.productService);
      container.Resolve<IStoreService>().Returns(this.storeService);
      container.Resolve<IPictureService>().Returns(this.pictureService);
      container.Resolve<IRepository<Download>>().Returns(this.downloadRepository);
      container.Resolve<ILanguageService>().Returns(this.languageService);
      container.Resolve<ILocalizedEntityService>().Returns(this.localizedEntityService);
      EngineContext.Replace(container);

      this.productInformationService = new ProductsService();
    }

    /// <summary>
    /// Should return all divisions.
    /// </summary>
    [Fact]
    public void ShouldReturnAllDivisions()
    {
      // Arrange
      IList<Store> stores = new List<Store> { new Store { Id = 1, Name = "first" }, new Store { Id = 2, Name = "second" } };
      this.storeService.GetAllStores().Returns(stores);

      // Act
      var result = this.productInformationService.GetAllDivisions();

      // Assert
      result.Count.Should().Be(2);
      var firstDivision = result.ElementAt(0);
      firstDivision.Id.Should().Be("1");
      firstDivision.Name.Should().Be("first");
      var secondDivision = result.ElementAt(1);
      secondDivision.Id.Should().Be("2");
      secondDivision.Name.Should().Be("second");
    }

    /// <summary>
    /// Should return all resources.
    /// </summary>
    [Fact]
    public void ShouldReturnAllResources()
    {
      // arrange
      var firstPictureBinary = new byte[10];
      var secondPictureBinary = new byte[20];
      var pictures = new List<Picture>
                                  {
                                    new Picture { Id = 1, SeoFilename = "first.jpg", MimeType = "image/jpeg", PictureBinary = firstPictureBinary }, 
                                    new Picture { Id = 2, SeoFilename = "second.png", MimeType = "image/png", PictureBinary = secondPictureBinary }
                                  };
      var picturesList = new PagedList<Picture>(pictures, 0, int.MaxValue);
      this.pictureService.GetPictures(0, int.MaxValue).Returns(picturesList);

      var downloadBinary = new byte[30];
      var downloads = new List<Download> { new Download { Id = 3, Filename = "third.doc", ContentType = "application/msword", DownloadBinary = downloadBinary } };
      this.downloadRepository.Table.Returns(downloads.AsQueryable());

      // act
      var result = this.productInformationService.GetAllResources();

      // assert
      result.Count().Should().Be(3);
      var firstResource = result.ElementAt(0);
      firstResource.Id.Should().Be("1");
      firstResource.Name.Should().Be("first.jpg");
      firstResource.MimeType.Should().Be("image/jpeg");
      firstResource.BinaryData.Should().BeEquivalentTo(firstPictureBinary);
      firstResource.ResourceType.Should().Be("Image");
      var secondResource = result.ElementAt(1);
      secondResource.Id.Should().Be("2");
      secondResource.Name.Should().Be("second.png");
      secondResource.MimeType.Should().Be("image/png");
      secondResource.BinaryData.Should().BeEquivalentTo(secondPictureBinary);
      secondResource.ResourceType.Should().Be("Image");
      var thirdResource = result.ElementAt(2);
      thirdResource.Id.Should().Be("3");
      thirdResource.Name.Should().Be("third.doc");
      thirdResource.MimeType.Should().Be("application/msword");
      thirdResource.BinaryData.Should().BeEquivalentTo(downloadBinary);
      thirdResource.ResourceType.Should().Be("Download");
    }

    /// <summary>
    /// Should not create or update product if identifier is not provided.
    /// </summary>
    [Fact]
    public void ShouldNotUpdateProductIfIdIsNotProvided()
    {
      // arrange

      // act
      this.productInformationService.UpdateProduct(new ProductModel(), "en");

      // assert;
      this.productService.DidNotReceive().UpdateProduct(Arg.Any<Product>());
    }

    /// <summary>
    /// Should not create or update product if language is not provided.
    /// </summary>
    [Fact]
    public void ShouldNotUpdateProductIfLanguageIsNotProvided()
    {
      // arrange

      // act
      this.productInformationService.UpdateProduct(new ProductModel { ProductId = "100500" }, string.Empty);

      // assert
      this.productService.DidNotReceive().UpdateProduct(Arg.Any<Product>());
    }

    /// <summary>
    /// Should not create or update product if language is null.
    /// </summary>
    [Fact]
    public void ShouldNotUpdateProductIfLanguageIsNull()
    {
      // arrange
      this.languageService.GetAllLanguages().Returns(new[] { new Language { UniqueSeoCode = "da" } });

      // act
      this.productInformationService.UpdateProduct(new ProductModel { ProductId = "100500" }, "en");

      // assert
      this.productService.DidNotReceive().UpdateProduct(Arg.Any<Product>());
    }

    /// <summary>
    /// Should update existing product.
    /// </summary>
    [Fact]
    public void ShouldUpdateExistingProduct()
    {
      // arrange
      var productModel = new ProductModel();
      productModel.ProductId = "100500";
      var product = new Product { Id = 100500 };
      productModel.Name = "name";
      productModel.ShortDescription = "short description";
      productModel.FullDescription = "full description";
      productModel.MetaDescription = "meta description";
      productModel.Sku = "sku";
      this.productService.GetProductById(100500).Returns(product);
      this.languageService.GetAllLanguages().Returns(new[] { new Language { UniqueSeoCode = "en", Id = 1 } });

      // act
      this.productInformationService.UpdateProduct(productModel, "en");

      // assert
      this.productService.DidNotReceive().InsertProduct(Arg.Any<Product>());
      product.Id.Should().Be(100500);
      this.localizedEntityService.Received().SaveLocalizedValue(product, Arg.Any<Expression<Func<Product, string>>>(), "name", 1);
      this.localizedEntityService.Received().SaveLocalizedValue(product, Arg.Any<Expression<Func<Product, string>>>(), "short description", 1);
      this.localizedEntityService.Received().SaveLocalizedValue(product, Arg.Any<Expression<Func<Product, string>>>(), "full description", 1);
      this.productService.Received().UpdateProduct(product);
      this.productService.Received().UpdateProduct(Arg.Is<Product>(p => p.MetaDescription == "meta description" && p.Sku == "sku"));
    }

    /// <summary>
    /// Should get all languages on create.
    /// </summary>
    [Fact]
    public void ShouldGetAllLanguagesOnUpdate()
    {
      // arrange
      // act
      this.productInformationService.UpdateProduct(new ProductModel { ProductId = "100500" }, "en");

      // assert
      this.languageService.Received().GetAllLanguages();
    }
  }
}
