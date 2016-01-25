// ---------------------------------------------------------------------
// <copyright file="ProductServiceTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines DefaultProductServiceTest class.
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
namespace Sitecore.Commerce.StarterKit.Tests.Services
{
  using System;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore;
  using Sitecore.Configuration;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.SearchTypes;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Globalization;
  using Sitecore.Commerce.StarterKit.Services;
  using Sitecore.TestKit.Data.Items;
  using Xunit;
  using Assert = Sitecore.Diagnostics.Assert;

  /// <summary>
  /// Defines DefaultProductServiceTest class.
  /// </summary>
  public class ProductServiceTest : IDisposable
  {
    /// <summary>
    /// The product template identifier.
    /// </summary>
    private readonly ID productTemplateId = ID.Parse("{47D1A39E-3B4B-4428-A9F8-B446256C9581}");

    /// <summary>
    /// The index name.
    /// </summary>
    private const string IndexName = "commerce_products_web_index";

    /// <summary>
    /// The index.
    /// </summary>
    private readonly ISearchIndex index;

    /// <summary>
    /// The _is disposed
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductServiceTest"/> class.
    /// </summary>
    public ProductServiceTest()
    {
      Factory.CreateObject("contentSearch/configuration", true);
      this.index = Substitute.For<ISearchIndex>();
      ContentSearchManager.SearchConfiguration.Indexes.Add(IndexName, this.index);
    }

    /// <summary>
    /// Should read product in latest version and context language by external identifier.
    /// </summary>
    [Fact]
    public void ShouldReadProductInLatestVersionAndContextLanguageByExternalId()
    {
      // Arrange
      using (var tree = new TTree("web")
                          {
                            new TItem("Product Repository", ID.NewID, ID.NewID, ItemIDs.ContentRoot)
                              {
                                new TItem("T-800", new TemplateID(this.productTemplateId)) { { "ExternalID", "t-800" } }
                              }
                          })
      {
        var productSitecoreItem = tree.Database.GetItem("/sitecore/content/Product Repository/T-800");

        var searchResultItem = Substitute.For<SearchResultItem>();
        searchResultItem.GetItem().Returns(productSitecoreItem);
        searchResultItem["ExternalID"].Returns("t-800");
        searchResultItem.TemplateId.Returns(this.productTemplateId);
        searchResultItem.Language.Returns("en");
        searchResultItem["_latestversion"].Returns("1");

        var searchContext = Substitute.For<IProviderSearchContext>();
        searchContext.GetQueryable<SearchResultItem>().Returns((new[] { searchResultItem }).AsQueryable());

        this.index.CreateSearchContext().Returns(searchContext);

        var service = new ProductService(tree.Database, Language.Parse("en"));

        // Act
        var product = service.ReadProduct("t-800");

        // Assert
        Sitecore.Diagnostics.Assert.IsNotNull(product, typeof(Item));
        product.Paths.FullPath.Should().Be("/sitecore/content/Product Repository/T-800");
      }
    }

    /// <summary>
    /// Should return null if no product found.
    /// </summary>
    [Fact]
    public void ShouldReturnNullIfNoProductFound()
    {
      using (var tree = new TTree("web")
                          {
                            new TItem("Product Repository", ID.NewID, ID.NewID, ItemIDs.ContentRoot)
                          })
      {
        var searchContext = Substitute.For<IProviderSearchContext>();
        searchContext.GetQueryable<SearchResultItem>().Returns((new SearchResultItem[] { }).AsQueryable());

        this.index.CreateSearchContext().Returns(searchContext);

        var service = new ProductService(tree.Database, Language.Parse("en"));

        // Act
        var product = service.ReadProduct("t-800");

        // Assert
        product.Should().BeNull();
      }
    }

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
            ContentSearchManager.SearchConfiguration.Indexes.Remove(IndexName);
        }
        _isDisposed = true;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ProductServiceTest"/> class.
    /// </summary>
    ~ProductServiceTest()
    {
        Dispose(false);
    }
  }
}