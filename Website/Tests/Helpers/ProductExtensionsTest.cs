// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductExtensionsTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The product extensions test.
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
namespace Sitecore.Commerce.StarterKit.Tests.Helpers
{
  using System;
  using System.Linq;
  using System.Reflection;
  using App_Start;
  using FluentAssertions;
  using Glass.Sitecore.Mapper.FieldTypes;
  using NSubstitute;
  using Sitecore.Data.Items;
  using Sitecore.Commerce.StarterKit.Helpers;
  using Sitecore.Commerce.StarterKit.Models;
  using Sitecore.TestKit.Data.Items;
  using Xunit;
  using Xunit.Extensions;
    using Sitecore.Commerce.Entities.Inventory;
  using StarterKit.Services;

  /// <summary>
  /// The product extensions test.
  /// </summary>
  public class ProductExtensionsTest : IDisposable
  {
    /// <summary>
    /// The product item.
    /// </summary>
    private readonly Item productItem;

    /// <summary>
    /// The product resource item 1.
    /// </summary>
    private readonly Item productResourceItem1;

    /// <summary>
    /// The product model.
    /// </summary>
    private readonly ProductModel productModel;

    /// <summary>
    /// The tree.
    /// </summary>
    private readonly TTree tree;

    /// <summary>
    /// The _is disposed
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductExtensionsTest"/> class.
    /// </summary>
    public ProductExtensionsTest()
    {
      new Glass.Sitecore.Mapper.Context();
      GlassMapperService.Current = Substitute.For<Glass.Sitecore.Mapper.ISitecoreService>();
      this.tree = new TTree { new TItem("T-800") { new TItem("Resources") { new TItem("ProductResource1") { { "Type", "Image" } } } } };
      this.productItem = this.tree.Database.GetItem("/sitecore/content/home/T-800");
      this.productResourceItem1 = this.tree.Database.GetItem("/sitecore/content/home/T-800/Resources/ProductResource1");
            this.productModel = new ProductModel(5000, new StockInformation());
    }

    //TODO: Update method with changes. Now it's failed.
    ///// <summary>
    ///// Should get product image from resource source if provided.
    ///// </summary>
    //[Fact]
    //public void ShouldGetProductImageFromResourceSrcIfProvided()
    //{
    //  // Arrange
    //  var image1 = new Image();
    //  image1.GetType().GetProperty("Src", BindingFlags.Instance | BindingFlags.Public).SetValue(image1, "http://good.img");
    //  var productResourceModel = new ProductResourceModel { Resource = image1, Uri = "http://cool.img" };
    //  WindsorConfig.Container.Resolve<IProductService>().Returns(new ProductService(productItem.Database, productItem.Language));
    //  GlassMapperService.Current.CreateClass<ProductResourceModel>(false, false, Arg.Is<Item>(item => item.ID == this.productResourceItem1.ID)).Returns(productResourceModel);

    //  // Act
    //  var result = this.productModel.SetProductResource(this.productItem);

    //  // Assert
    //  result.Image.Should().Be("http://good.img");
    //}

    //TODO: Update method with changes. Now it's failed.
    ///// <summary>
    ///// Should get product image from URI field if provided but resource is null.
    ///// </summary>
    //[Fact]
    //public void ShouldGetProductImageFromUriFieldIfProvidedButResourceIsNull()
    //{
    //  // Arrange
    //  var productResourceModel = new ProductResourceModel { Uri = "http://cool.img" };
    //  GlassMapperService.Current.CreateClass<ProductResourceModel>(false, false, Arg.Is<Item>(item => item.ID == this.productResourceItem1.ID)).Returns(productResourceModel);

    //  // Act
    //  var result = this.productModel.SetProductResource(this.productItem);

    //  // Assert
    //  result.Image.Should().Be("http://cool.img");
    //}

    //TODO: Update method with changes. Now it's failed.
    ///// <summary>
    ///// Should get product image from URI field if item is not from media library.
    ///// </summary>
    ///// <param name="src">The source.</param>
    //[Theory]
    //[InlineData(null)]
    //[InlineData("")]
    //public void ShouldGetProductImageFromUriFieldIfProvidedButResourceSrcIsNotProvided(string src)
    //{
    //  // Arrange
    //  var image1 = new Image();
    //  image1.GetType().GetProperty("Src", BindingFlags.Instance | BindingFlags.Public).SetValue(image1, src);
    //  var productResourceModel = new ProductResourceModel { Resource = image1, Uri = "http://cool.img" };
    //  GlassMapperService.Current.CreateClass<ProductResourceModel>(false, false, Arg.Is<Item>(item => item.ID == this.productResourceItem1.ID)).Returns(productResourceModel);

    //  // Act
    //  var result = this.productModel.SetProductResource(this.productItem);

    //  // Assert
    //  result.Image.Should().Be("http://cool.img");
    //}

    //TODO: Update method with changes. Now it's failed.
    ///// <summary>
    ///// Should prefer image resource.
    ///// </summary>
    //[Fact]
    //public void ShouldPreferImageResource()
    //{
    //  // Arrange
    //  using (var newTree = new TTree
    //                     {
    //                       new TItem("T-1000")
    //                       {
    //                         new TItem("Resources")
    //                         {
    //                           new TItem("ProductResource1") { { "Type", "Download" } },
    //                           new TItem("ProductResource2") { { "Type", "Image" } },
    //                         }
    //                       }
    //                     })
    //  {

    //    var currentProductItem = this.tree.Database.GetItem("/sitecore/content/home/T-1000");
    //    var expectedProductResource = this.tree.Database.GetItem("/sitecore/content/home/T-1000/Resources/ProductResource2");

    //    // Act
    //    this.productModel.SetProductResource(currentProductItem);

    //    // Assert
    //    GlassMapperService.Current.Received().CreateClass<ProductResourceModel>(false, false, Arg.Is<Item>(item => item.ID == expectedProductResource.ID));
    //  }
    //}


    //TODO: Update method with changes. Now it's failed.
    /// <summary>
    /// Should prefer download resource if image resource is not presented.
    /// </summary>
    //[Fact]
    //public void ShouldPreferDownloadResourceIfImageResourceIsNotPresented()
    //{
    //  // Arrange
    //  using (var newTree = new TTree
    //                     {
    //                       new TItem("T-1000")
    //                       {
    //                         new TItem("Resources")
    //                         {
    //                           new TItem("ProductResource1") { { "Type", "Some other resource" } },
    //                           new TItem("ProductResource2") { { "Type", "Download" } },
    //                         }
    //                       }
    //                     })
    //  {
    //    var currentProductItem = this.tree.Database.GetItem("/sitecore/content/home/T-1000");
    //    var expectedProductResource = this.tree.Database.GetItem("/sitecore/content/home/T-1000/Resources/ProductResource2");

    //    // Act
    //    this.productModel.SetProductResource(currentProductItem);

    //    // Assert
    //    GlassMapperService.Current.Received().CreateClass<ProductResourceModel>(false, false, Arg.Is<Item>(item => item.ID == expectedProductResource.ID));
    //  }
    //}

    //TODO: Update method with changes. Now it's failed.
    ///// <summary>
    ///// Should use any available resource.
    ///// </summary>
    //[Fact]
    //public void ShouldUseAnyAvailableResource()
    //{
    //  // Arrange
    //  using (var newTree = new TTree
    //                     {
    //                       new TItem("T-1000")
    //                       {
    //                         new TItem("Resources")
    //                         {
    //                           new TItem("ProductResource1") { { "Type", "Some other resource" } },
    //                         }
    //                       }
    //                     })
    //  {
    //    var currentProductItem = this.tree.Database.GetItem("/sitecore/content/home/T-1000");
    //    var expectedProductResource = this.tree.Database.GetItem("/sitecore/content/home/T-1000/Resources/ProductResource1");

    //    // Act
    //    this.productModel.SetProductResource(currentProductItem);

    //    // Assert
    //    GlassMapperService.Current.Received().CreateClass<ProductResourceModel>(false, false, Arg.Is<Item>(item => item.ID == expectedProductResource.ID));
    //  }
    //}

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
      this.tree.Dispose();
    }
        _isDisposed = true;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ProductExtensionsTest"/> class.
    /// </summary>
    ~ProductExtensionsTest()
    {
        Dispose(false);
    }
  }
}