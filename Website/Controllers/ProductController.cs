// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The product controller.
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
namespace Sitecore.Commerce.StarterKit.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Glass.Sitecore.Mapper;

    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Multishop;
    using Sitecore.Commerce.StarterKit.Helpers;
    using Sitecore.Commerce.StarterKit.Models;
    using Sitecore.Commerce.StarterKit.Services;
    using Sitecore.Configuration;
    using Sitecore.ContentSearch.Utilities;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Mvc.Presentation;

    using Constants = Sitecore.Commerce.Constants;
    using Context = Sitecore.Context;

    /// <summary>
    ///   The product controller.
    /// </summary>
    public class ProductController : Controller
    {
        /// <summary>
        /// The product list rendering id.
        /// </summary>
        public const string ProductListRenderingId = "{AD2501FA-46FB-4B2B-A668-5F4976BF9DD5}";

        /// <summary>
        /// Defines IProductService instance.
        /// </summary>
        private readonly IProductService productService;

        /// <summary>
        /// Defines ICatalogService instance.
        /// </summary>
        private readonly ICatalogService _catalogService;

        /// <summary>
        /// The price service.
        /// </summary>
        private readonly IPricingService pricingService;

        /// <summary>
        /// The content search helper.
        /// </summary>
        private readonly ContentSearchHelper contentSearchHelper;

        /// <summary>
        /// The inventory service.
        /// </summary>
        private readonly IInventoryService _inventoryService;

        /// <summary>
        /// The obec context.
        /// </summary>
        private readonly CommerceContextBase _obecContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController" /> class.
        /// </summary>
        /// <param name="productService">The product service.</param>
        /// <param name="pricingService">The price service.</param>
        /// <param name="contentSearchHelper">The content search helper.</param>
        /// <param name="inventoryService">The inventory service.</param>
        public ProductController([NotNull] IProductService productService, [NotNull] IPricingService pricingService, ContentSearchHelper contentSearchHelper, [NotNull] IInventoryService inventoryService, [NotNull] ICatalogService catalogService)
        {
            this.productService = productService;
            this.pricingService = pricingService;
            this.contentSearchHelper = contentSearchHelper;
            this._inventoryService = inventoryService;
            this._catalogService = catalogService;
            this._obecContext = (CommerceContextBase)Factory.CreateObject(Constants.CommerceContext, true);
        }

        /// <summary>
        /// Gets product view by its Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The action result.</returns>
        [NotNull]
        public ActionResult Index([NotNull] string id)
        {
            Item productItem;
            if (string.IsNullOrEmpty(id))
            {
                productItem = RenderingContext.Current.Rendering.Item;
                if (productItem != null)
                {
                    var field = productItem.Fields[Constants.KnownFieldNames.ExternalId];
                    if (field != null && field.HasValue)
                    {
                        id = field.Value;
                    }
                }
            }
            else
            {
                productItem = this.productService.ReadProduct(id);
            }

            var price = this.pricingService.GetProductPrice(id);
            var ids = new List<string> { id };
            var stockInfos = this._inventoryService.GetStockInformation(Context.Site.Name, ids, StockDetailsLevel.All, this._obecContext.InventoryLocation, string.Empty);
            var stockInfo = stockInfos.FirstOrDefault();
            OrderableInformation orderableInfo = null;

            if (stockInfo != null && stockInfo.Status != null)
            {
                if (Equals(stockInfo.Status, StockStatus.PreOrderable))
                {
                    orderableInfo = this._inventoryService.GetPreOrderableInformation(Context.Site.Name, ids, string.Empty, this._obecContext.InventoryLocation).FirstOrDefault();
                }
                else if (Equals(stockInfo.Status, StockStatus.BackOrderable))
                {
                    orderableInfo = this._inventoryService.GetBackOrderableInformation(Context.Site.Name, ids, string.Empty, this._obecContext.InventoryLocation).FirstOrDefault();
                }

                this._inventoryService.VisitedProductStockStatus(Context.Site.Name, stockInfo, string.Empty);
            }

            ProductModel productModel = orderableInfo == null ? this.GetProductModel(productItem, price, stockInfo) : this.GetProductModel(productItem, price, stockInfo, orderableInfo);

            this._catalogService.VisitedProductDetailsPage(id, productModel.Name, string.Empty, string.Empty);

            return this.View(productModel);
        }

        /// <summary>
        /// Gets the product list.
        /// </summary>
        /// <param name="category">The category name.</param>
        /// <param name="subCategory">The sub category name.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sort">The sort direction.</param>
        /// <returns>The action result.</returns>
        [NotNull]
        [HttpGet]
        public ActionResult List(
          [CanBeNull] [Bind(Prefix = "category")] string category = null,
          [CanBeNull] [Bind(Prefix = "subCategory")] string subCategory = null,
          [CanBeNull] [Bind(Prefix = "page")] int page = 0,
          [CanBeNull] [Bind(Prefix = "pageSize")] int pageSize = 6,
          [CanBeNull] [Bind(Prefix = "sort")] string sort = "name:asc")
        {
            if (page < 0)
            {
                page = 0;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            var searchStringModel = this.contentSearchHelper.GetDataSourceQuery(Context.Item, Context.Device, ProductListRenderingId).ToList();
            if (!searchStringModel.Any())
            {
                return new EmptyResult();
            }

            string currectCategory = !string.IsNullOrEmpty(subCategory) ? subCategory : category;

            if (!string.IsNullOrEmpty(currectCategory))
            {
                Item categoryItem = this.productService.GetCategory(currectCategory);
                if (ReferenceEquals(categoryItem, null))
                {
                    return new EmptyResult();
                }

                var categories = this.productService.GetRelatedCategories(categoryItem.ID);
                categories.Insert(0, categoryItem);

                foreach (Item item in categories)
                {
                    searchStringModel.Add(new SearchStringModel(Constants.KnownFieldNames.ProductClassesFieldName, IdHelper.NormalizeGuid(item.ID), "should"));
                }
            }

            var sorting = ProductSortingModel.Parse(sort);

            searchStringModel.Add(new SearchStringModel("sort", sorting.Field, sorting.Direction));

            var pagedProducts = this.productService.GetProducts(searchStringModel, page, pageSize);
            var productIds = pagedProducts.List.Select(p => p["ExternalID"]).ToList();

            var prices = this.pricingService.GetProductBulkPrices(productIds);
            var stockInfos = this._inventoryService.GetStockInformation(Context.Site.Name, productIds, StockDetailsLevel.All, this._obecContext.InventoryLocation, string.Empty);

            var productModels = pagedProducts.List.Select(p => this.GetProductModel(p, prices[p["ExternalID"]], stockInfos.FirstOrDefault(i => i.Product.ProductId.Equals(p["ExternalID"], StringComparison.OrdinalIgnoreCase))));

            var pagingModel = new ProductPagingModel(pagedProducts.Page, pagedProducts.PageSize, pagedProducts.TotalCount);

            var model = new ProductListModel(productModels, pagingModel, sorting);

            return this.View(model);
        }

        /// <summary>
        /// Views a category facet.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="subCategory">The sub category.</param>
        /// <returns>The action result.</returns>
        [NotNull]
        [HttpGet]
        public ActionResult CategoryFacet(
            [CanBeNull] [Bind(Prefix = "category")] string category,
            [CanBeNull] [Bind(Prefix = "subCategory")] string subCategory)
        {
            if (string.IsNullOrEmpty(category))
            {
                return new EmptyResult();
            }

            Item categoryItem = this.productService.GetCategory(category);
            if (ReferenceEquals(categoryItem, null))
            {
                return new EmptyResult();
            }

            var categories = this.productService.GetRelatedCategories(categoryItem.ID);

            ISitecoreService glassMapper = GlassMapperService.Current;

            var categoryModels = categories.Select(item => glassMapper.CreateClass<CategoryModel>(false, false, item)).ToList();

            var model = new CategoryFacetModel(categoryModels, category, subCategory);

            if (!string.IsNullOrWhiteSpace(subCategory))
            {
                var foundModel = model.Categories.Where(c => c.ExternalID.Equals(subCategory, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (foundModel != null)
                {
                    this._catalogService.VisitedCategoryPage(subCategory, foundModel.Name);
                }
            }
            else
            {
                this._catalogService.VisitedCategoryPage(category, categoryItem.Name);
            }

            return this.View(model);
        }

        /// <summary>
        /// Sign up visitor for product back in stock notification.
        /// </summary>
        /// <param name="id">The product id.</param>
        [HttpPost]
        public void SignUpForBackInStockNotification(string id)
        {
            this._inventoryService.VisitorSignUpForStockNotification(Context.Site.Name, id, string.Empty, null);
        }

        /// <summary>
        /// Search autocomplete
        /// </summary>
        /// <param name="term">The term to complete.</param>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult Autocomplete(string term)
        {
            var model = productService.GetProducts(new List<SearchStringModel> { new SearchStringModel("name", "*" + term + "*") }).Select(i => new { label = i.Name, value = i.Name, id = i["ExternalID"] });

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the product model.
        /// </summary>
        /// <param name="productItem">The product item.</param>
        /// <param name="price">The product price.</param>
        /// <param name="stockInformation">The stock information.</param>
        /// <returns>The product model.</returns>
        private ProductModel GetProductModel(Item productItem, decimal price, StockInformation stockInformation)
        {
            ISitecoreService glassMapper = GlassMapperService.Current;

            var productModel = glassMapper.CreateClass<ProductModel, decimal, StockInformation>(false, false, productItem, price, stockInformation);

            return productModel.SetProductResource(this.productService, productItem);
        }

        /// <summary>
        /// Gets the product model.
        /// </summary>
        /// <param name="productItem">The product item.</param>
        /// <param name="price">The product price.</param>
        /// <param name="stockInformation">The stock information.</param>
        /// <param name="orderableInformation">The pre-orderable information.</param>
        /// <returns>The product model.</returns>
        private ProductModel GetProductModel(Item productItem, decimal price, StockInformation stockInformation, OrderableInformation orderableInformation)
        {
            ISitecoreService glassMapper = GlassMapperService.Current;

            var productModel = glassMapper.CreateClass<ProductModel, decimal, StockInformation, OrderableInformation>(false, false, productItem, price, stockInformation, orderableInformation);

            return productModel.SetProductResource(this.productService, productItem);
        }
    }
}