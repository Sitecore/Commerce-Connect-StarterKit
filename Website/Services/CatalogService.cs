// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Provides basic catalog operations for the "Autohaus" web-store visitors.
//   This service is aimed to simplify the Test Driven Development (TDD) and
//   allows MVC controllers to use lite version of the cart management API
//   that satisfies their needs.
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

namespace Sitecore.Commerce.StarterKit.Services
{
    using Commerce.Services.Catalog;
    using Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the CatalogService class.
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.StarterKit.Services.ICatalogService" />
    public class CatalogService : ICatalogService
    {
        private readonly CatalogServiceProvider _catalogServiceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogService"/> class.
        /// </summary>
        /// <param name="catalogServiceProvider">The catalog service provider.</param>
        public CatalogService([NotNull] CatalogServiceProvider catalogServiceProvider)
        {
            Assert.IsNotNull(catalogServiceProvider, "catalogServiceProvider");

            this._catalogServiceProvider = catalogServiceProvider;
        }

        /// <summary>
        /// Visiteds the category page.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="categoryName">Name of the category.</param>
        public void VisitedCategoryPage(string categoryId, string categoryName)
        {
            this._catalogServiceProvider.VisitedCategoryPage(new VisitedCategoryPageRequest(Context.Site.Name, categoryId, categoryName));
        }

        /// <summary>
        /// Visiteds the product details page.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="productName">Name of the product.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="categoryName">Name of the category.</param>
        public void VisitedProductDetailsPage(string productId, string productName, string categoryId, string categoryName)
        {
            this._catalogServiceProvider.VisitedProductDetailsPage(new VisitedProductDetailsPageRequest(Context.Site.Name, productId, productName, categoryId, categoryName));
        }
    }
}