// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductControllerTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ProductControllerTest type.
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
namespace Sitecore.Commerce.StarterKit.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using FluentAssertions;
    using NSubstitute;
    using Sitecore;
    using Sitecore.ContentSearch.Utilities;
    using Sitecore.Data.Items;
    using Sitecore.Commerce.StarterKit.Controllers;
    using Sitecore.Commerce.StarterKit.Helpers;
    using Sitecore.Commerce.StarterKit.Models;
    using Sitecore.Commerce.StarterKit.Services;
    using Sitecore.TestKit.Data.Items;
    using Xunit;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Sites;
    using Sitecore.TestKit.Sites;
    using Sitecore.Commerce.Multishop;

    /// <summary>
    /// The product controller test.
    /// </summary>
    public class ProductControllerTest : IDisposable
    {
        /// <summary>
        /// The IProductService instance.
        /// </summary>
        private readonly IProductService productService;

        /// <summary>
        /// The price service.
        /// </summary>
        private readonly IPricingService pricingService;

        /// <summary>
        /// The controller.
        /// </summary>
        private readonly ProductController controller;

        /// <summary>
        /// The content search helper.
        /// </summary>
        private readonly ContentSearchHelper contentSearchHelper;

        /// <summary>
        /// The _is disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// The _context
        /// </summary>
        private readonly Glass.Sitecore.Mapper.Context _context;

        /// <summary>
        /// The inventory service.
        /// </summary>
        private readonly IInventoryService _inventoryService;

        /// <summary>
        /// The catalog service.
        /// </summary>
        private readonly ICatalogService _catalogService;

        /// <summary>
        /// The Commerce context.
        /// </summary>
        private readonly CommerceContextBase _commerceContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductControllerTest"/> class. 
        /// </summary>
        public ProductControllerTest()
        {
            _context = new Glass.Sitecore.Mapper.Context();
            GlassMapperService.Current = Substitute.For<Glass.Sitecore.Mapper.ISitecoreService>();

            this.productService = Substitute.For<IProductService>();
            this.pricingService = Substitute.For<IPricingService>();
            this.contentSearchHelper = Substitute.For<ContentSearchHelper>();
            this._inventoryService = Substitute.For<IInventoryService>();
            this._catalogService = Substitute.For<ICatalogService>();
            this._commerceContext = Substitute.For<CommerceContextBase>();

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Url.Returns(new Uri("http://host/path"));
            httpContext.Request.QueryString.Returns(new NameValueCollection());

            this.controller = new ProductController(this.productService, this.pricingService, this.contentSearchHelper, this._inventoryService, this._catalogService);
            this.controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), this.controller);
        }

        /// <summary>
        /// Should get product by identifier pre-orderable.
        /// </summary>
        [Fact]
        public void ShouldGetProductByIdPreOrderable()
        {
            // Arrange
            var stockInfo = new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.PreOrderable, AvailabilityDate = new DateTime(2014, 3, 12) };
            var orderableInfo = new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.PreOrderable, InStockDate = new DateTime(2014, 3, 12) };
            
            using (new SiteContextSwitcher(new TSiteContext("shop")))
            {
                using (var tree = new TTree { new TItem("T-800", new NameValueCollection { { "ExternalID", "1001" } }) })
                {
                    Item productItem = tree.Database.GetItem("/sitecore/content/home/T-800");
                    this.productService.ReadProduct("1001").Returns(productItem);
                    this.pricingService.GetProductPrice("1001").Returns(5000);
                    this._inventoryService
                        .GetStockInformation("shop", Arg.Is<IEnumerable<string>>(ids => ids.Contains("1001") && ids.Count() == 1), StockDetailsLevel.All, string.Empty, string.Empty)
                        .Returns(new List<StockInformation> { stockInfo });
                    this._inventoryService
                        .GetPreOrderableInformation("shop", Arg.Is<IEnumerable<string>>(ids => ids.Contains("1001") && ids.Count() == 1), string.Empty, string.Empty)
                        .Returns(new List<OrderableInformation> { orderableInfo });

                    GlassMapperService.Current.CreateClass<ProductModel, decimal, StockInformation, OrderableInformation>(false, false, Arg.Is<Item>(item => item.ID == productItem.ID), 5000, stockInfo, orderableInfo)
                        .Returns(new ProductModel(5000, stockInfo, orderableInfo));

                    // Act
                    var result = this.controller.Index("1001");

                    // Assert
                    var productModel = (ProductModel)((ViewResult)result).Model;
                    productModel.Price.Should().Be(5000);
                    productModel.Status.Should().Be(StockStatus.PreOrderable);
                    productModel.AvailabilityDate.Should().Be(new DateTime(2014, 3, 12));
                    productModel.InStockDate.Should().Be(new DateTime(2014, 3, 12));
                }
            }
        }

        /// <summary>
        /// Should get product by identifier back-orderable.
        /// </summary>
        [Fact]
        public void ShouldGetProductByIdBackOrderable()
        {
            // Arrange
            var stockInfo = new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.BackOrderable, AvailabilityDate = new DateTime(2014, 3, 12) };
            var orderableInfo = new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.BackOrderable, InStockDate = new DateTime(2014, 3, 12) };

            using (new SiteContextSwitcher(new TSiteContext("shopname")))
            {
                using (var tree = new TTree { new TItem("T-800", new NameValueCollection { { "ExternalID", "1001" } }) })
                {
                    Item productItem = tree.Database.GetItem("/sitecore/content/home/T-800");
                    this.productService.ReadProduct("1001").Returns(productItem);
                    this.pricingService.GetProductPrice("1001").Returns(5000);
                    this._inventoryService
                        .GetStockInformation("shopname", Arg.Is<IEnumerable<string>>(ids => ids.Contains("1001") && ids.Count() == 1), StockDetailsLevel.All, string.Empty, string.Empty)
                        .Returns(new List<StockInformation> { stockInfo });
                    this._inventoryService
                        .GetBackOrderableInformation("shopname", Arg.Is<IEnumerable<string>>(ids => ids.Contains("1001") && ids.Count() == 1), string.Empty, string.Empty)
                        .Returns(new List<OrderableInformation> { orderableInfo });

                    GlassMapperService.Current.CreateClass<ProductModel, decimal, StockInformation, OrderableInformation>(false, false, Arg.Is<Item>(item => item.ID == productItem.ID), 5000, stockInfo, orderableInfo)
                        .Returns(new ProductModel(5000, stockInfo, orderableInfo));

                    // Act
                    var result = this.controller.Index("1001");

                    // Assert
                    var productModel = (ProductModel)((ViewResult)result).Model;
                    productModel.Price.Should().Be(5000);
                    productModel.Status.Should().Be(StockStatus.BackOrderable);
                    productModel.AvailabilityDate.Should().Be(new DateTime(2014, 3, 12));
                    productModel.InStockDate.Should().Be(new DateTime(2014, 3, 12));
                }
            }
        }

        /// <summary>
        /// Should get product by identifier.
        /// </summary>
        [Fact]
        public void ShouldGetProductById()
        {
            // Arrange
            var stockInfo = new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock, AvailabilityDate = new DateTime(2014, 3, 12) };

            using (new SiteContextSwitcher(new TSiteContext("shopname")))
            {
                using (var tree = new TTree { new TItem("T-800", new NameValueCollection { { "ExternalID", "1001" } }) })
                {
                    var productItem = tree.Database.GetItem("/sitecore/content/home/T-800");
                    this.productService.ReadProduct("1001").Returns(productItem);
                    this.pricingService.GetProductPrice("1001").Returns(5000);
                    this._inventoryService
                        .GetStockInformation("shopname", Arg.Is<IEnumerable<string>>(ids => ids.Contains("1001") && ids.Count() == 1), StockDetailsLevel.All, string.Empty, string.Empty)
                        .Returns(new List<StockInformation> { stockInfo });

                    GlassMapperService.Current.CreateClass<ProductModel, decimal, StockInformation>(false, false, Arg.Is<Item>(item => item.ID == productItem.ID), 5000, stockInfo)
                        .Returns(new ProductModel(5000, stockInfo));

                    // Act
                    var result = this.controller.Index("1001");

                    // Assert
                    var productModel = (ProductModel)((ViewResult)result).Model;
                    productModel.Price.Should().Be(5000);
                    productModel.Status.Should().Be(StockStatus.InStock);
                    productModel.AvailabilityDate.Should().Be(new DateTime(2014, 3, 12));
                }
            }
        }

        /// <summary>
        /// Should try to fill resources field if resources available.
        /// </summary>
        //[Fact]
        //public void ShouldTryToFillResourcesFieldIfResourcesAvailable()
        //{
        //    // Arrange
        //    var stockInfo = new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock };

        //    using (new SiteContextSwitcher(new TSiteContext("shopname")))
        //    {
        //        using (var tree = new TTree { new TItem("T-800", new NameValueCollection { { "ExternalID", "1001" } }) { new TItem("Resources") { new TItem("T-800-resource") } } })
        //        {
        //            var productItem = tree.Database.GetItem("/sitecore/content/home/T-800");
        //            var productResourceItem = tree.Database.GetItem("/sitecore/content/home/T-800/Resources/T-800-resource");
        //            this.productService.ReadProduct("1001").Returns(productItem);
        //            this.pricingService.GetProductPrice("1001").Returns(5000);
        //            this._inventoryService
        //                .GetStockInformation("shopname", Arg.Is<IEnumerable<string>>(ids => ids.Contains("1001") && ids.Count() == 1), StockDetailsLevel.All, string.Empty, string.Empty)
        //                .Returns(new List<StockInformation> { stockInfo });

        //            GlassMapperService.Current.CreateClass<ProductModel, decimal, StockInformation>(false, false, Arg.Is<Item>(item => item.ID == productItem.ID), 5000, stockInfo)
        //                .Returns(new ProductModel(5000, stockInfo));
        //            // Act
        //            this.controller.Index("1001");

        //            // Assert
        //            GlassMapperService.Current.Received().CreateClass<ProductResourceModel>(false, false, Arg.Is<Item>(item => item.ID == productResourceItem.ID));
        //        }
        //    }
        //}

        /// <summary>
        /// Should return empty view if data source is not defined or empty.
        /// </summary>
        [Fact]
        public void ShouldReturnEmptyViewIfDataSourceIsNotDefinedOrEmpty()
        {
            // Act
            var result = this.controller.List();

            // Assert
            result.Should().BeOfType<EmptyResult>();
        }

        /// <summary>
        /// Should return list of products.
        /// </summary>
        [Fact]
        public void ShouldReturnListOfProductsOrderedByPrice()
        {
            // Arrange
            const decimal Price1 = 5000;
            const decimal Price2 = 10000;
            var stockInfo1 = new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock, AvailabilityDate = new DateTime(2014, 3, 12) };
            var stockInfo2 = new StockInformation { Product = new InventoryProduct { ProductId = "1002" }, Status = StockStatus.InStock, AvailabilityDate = new DateTime(2014, 3, 12) };

            using (new SiteContextSwitcher(new TSiteContext("shopname")))
            {
                this.pricingService
                    .GetProductBulkPrices(Arg.Is<IEnumerable<string>>(ids => ids.Contains("1001") && ids.Contains("1002") && ids.Count() == 2))
                    .Returns(new Dictionary<string, decimal> { { "1001", Price1 }, { "1002", Price2 } });
                this._inventoryService
                    .GetStockInformation("shopname", Arg.Is<IEnumerable<string>>(ids => ids.Contains("1001") && ids.Contains("1002") && ids.Count() == 2), StockDetailsLevel.All, string.Empty, string.Empty)
                    .Returns(new List<StockInformation> { stockInfo1, stockInfo2 });

                using (var tree = new TTree { new TItem("T-800", new NameValueCollection { { "ExternalID", "1001" } }), new TItem("T-1000", new NameValueCollection { { "ExternalID", "1002" } }) })
                {
                    var contextItem = tree.Database.GetItem("/sitecore/content/home");
                    using (new ContextItemSwitcher(contextItem))
                    {
                        var searchModels = new[] { new SearchStringModel("location", contextItem.ID.ToString(), "should") };
                        this.contentSearchHelper.GetDataSourceQuery(contextItem, Context.Device, ProductController.ProductListRenderingId).Returns(searchModels);

                        var products = tree.Database.GetItem("/sitecore/content/home").Children;
                        var productsPage = new PagedList<Item>(products,0,products.Count,products.Count);

                        this.productService.GetProducts(Arg.Any<IEnumerable<SearchStringModel>>(), Arg.Any<int>(), Arg.Any<int>()).Returns(productsPage);

                        GlassMapperService.Current.CreateClass<ProductModel, decimal, StockInformation>(false, false, Arg.Is<Item>(item => item.Name == "T-800"), Price1, stockInfo1).Returns(new ProductModel(Price1, stockInfo1));
                        GlassMapperService.Current.CreateClass<ProductModel, decimal, StockInformation>(false, false, Arg.Is<Item>(item => item.Name == "T-1000"), Price2, stockInfo2).Returns(new ProductModel(Price2, stockInfo2));

                        // Act
                        var result = this.controller.List();

                        // Assert
                        var model = (ProductListModel)((ViewResult)result).Model;
                        model.Products.ElementAt(0).Price.Should().Be(Price2);
                        model.Products.ElementAt(0).Status.Should().Be(StockStatus.InStock);
                        model.Products.ElementAt(0).AvailabilityDate.Should().Be(new DateTime(2014, 3, 12));

                        model.Products.ElementAt(1).Price.Should().Be(Price1);
                        model.Products.ElementAt(1).Status.Should().Be(StockStatus.InStock);
                        model.Products.ElementAt(1).AvailabilityDate.Should().Be(new DateTime(2014, 3, 12));
                    }
                }
            }
        }

        /// <summary>
        /// The should index be marked as http get.
        /// </summary>
        [Fact]
        public void ShouldIndexBeMarkedAsHttpGet()
        {
            // arrange
            var method = this.controller.GetType().GetMethod("List");

            // act
            var attributes = method.GetCustomAttributes(typeof(HttpGetAttribute), false);

            // assert
            attributes.Count().Should().Be(1);
        }

        /// <summary>
        /// Should try to retrieve rendering.
        /// </summary>
        [Fact]
        public void ShouldTryToRetrieveRendering()
        {
            // arrange
            using (var ttree = new TTree { new TItem("Products list") })
            {
                var productsItem = ttree.Database.GetItem("/sitecore/content/home/Products list");

                // act
                using (new ContextItemSwitcher(productsItem))
                {
                    this.controller.List();
                }

                // assert
                this.contentSearchHelper.Received().GetDataSourceQuery(productsItem, Context.Device, ProductController.ProductListRenderingId);
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
                if (null != controller)
                {
                    controller.Dispose();
                }
            }
            _isDisposed = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ProductControllerTest"/> class.
        /// </summary>
        ~ProductControllerTest()
        {
            Dispose(false);
        }
    }
}