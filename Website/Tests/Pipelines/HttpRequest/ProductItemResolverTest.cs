// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductItemResolverTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The product item resolver test.
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
namespace Sitecore.Commerce.StarterKit.Tests.Pipelines.HttpRequest
{
  using System;
  using System.Web.Routing;
  using Castle.Windsor;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore;
  using Sitecore.Commerce.StarterKit.App_Start;
  using Sitecore.Commerce.StarterKit.Helpers;
  using Sitecore.Commerce.StarterKit.Pipelines.HttpRequest;
  using Sitecore.Commerce.StarterKit.Services;
  using Sitecore.Pipelines;
  using Sitecore.Sites;
  using Sitecore.TestKit.Data.Items;
  using Sitecore.TestKit.Sites;
  using Xunit;

  /// <summary>
  /// The product item resolver test.
  /// </summary>
  public class ProductItemResolverTest : IDisposable
  {
    /// <summary>
    /// The product service.
    /// </summary>
    private readonly IProductService productService;

    /// <summary>
    /// The product link manager.
    /// </summary>                
    private readonly ProductHelper productHelper;

    /// <summary>
    /// The processor.
    /// </summary>
    private ProductItemResolver processor;

    /// <summary>
    /// The _is disposed
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductItemResolverTest"/> class.
    /// </summary>
    public ProductItemResolverTest()
    {
      this.productService = Substitute.For<IProductService>();
      this.productHelper = Substitute.For<ProductHelper>();

      RouteConfig.RegisterRoutes();

      this.processor = new ProductItemResolver { ProductService = this.productService, ProductHelper = this.productHelper };
    }

    /// <summary>
    /// Should read product service and product helper from windsor container.
    /// </summary>
    [Fact]
    public void ShouldReadProductServiceAndProductHelperFromWindsorContainer()
    {
      // Arrange
      var container = Substitute.For<IWindsorContainer>();
      container.Resolve<IProductService>().Returns(this.productService);
      container.Resolve<ProductHelper>().Returns(this.productHelper);

      // Act
      this.processor = new ProductItemResolver { WindsorContainer = container };

      // Assert
      this.processor.ProductService.Should().Be(this.productService);
      this.processor.ProductHelper.Should().Be(this.productHelper);
    }

    /// <summary>
    /// Should return default windsor container if not set.
    /// </summary>
    [Fact]
    public void ShouldReturnDefaultWindsorContainerIfNotSet()
    {
      // Act & Assert
      new ProductItemResolver().WindsorContainer.Should().Be(WindsorConfig.Container);
    }

    /// <summary>
    /// Should resolve product item if URL matches product route and set to context.
    /// </summary>
    [Fact]
    public void ShouldResolveProductItemIfUrlMatchesProductRouteAndSetToContext()
    {
      // Arrange
      var mystore = new TSiteContext("Autohaus");
      using (new SiteContextSwitcher(mystore))
      {
        using (TTree tree = new TTree("web") { new TItem("products") { new TItem("Beverages") { new TItem("Cola") } } })
        {
          var cola = tree.Database.GetItem("/sitecore/content/home/products/beverages/cola");
          this.productService.ReadProduct("cola").Returns(cola);

          // Act
          this.processor.Process(new PipelineArgs());

          // Assert
          Context.Item.Paths.FullPath.ToLowerInvariant().Should().Be("/sitecore/content/home/products/beverages/cola");
        }
      }
    }

    /// <summary>
    /// Should not resolve context item if product route does not match request.
    /// </summary>
    [Fact]
    public void ShouldNotResolveContextItemIfProductRouteDoesNotMatchRequest()
    {
      // Arrange
      this.productHelper.GetCatalogItemIdFromIncomingRequest().Returns((CatalogItemUrlData)null);

      // Act
      this.processor.Process(new PipelineArgs());

      // Assert
      Context.Item.Should().BeNull();
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
            Context.Item = null;
            RouteTable.Routes.Clear();
        }
        _isDisposed = true;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ProductItemResolverTest"/> class.
    /// </summary>
    ~ProductItemResolverTest()
    {
        Dispose(false);
    }
  }
}