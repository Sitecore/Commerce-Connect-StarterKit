// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductHelper.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ProductHelper type.
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
namespace Sitecore.Commerce.StarterKit.Helpers
{
    using System.Web;
    using System.Web.Routing;
    using Sitecore;
    using System.Web.Http;
    using System;    /// <summary>
                     /// Defines the ProductHelper type.
                     /// </summary>
    public class ProductHelper
    {
        /// <summary>
        /// The default product route name.
        /// </summary>
        private const string DefaultProductRouteName = "Products";

        /// <summary>
        /// The category query parameterCategory query parameter name.
        /// </summary>
        private const string CategoryQueryParameter = "category=";

        /// <summary>
        /// The sub category query parameterSub category query parameter name.
        /// </summary>
        private const string SubCategoryQueryParameter = "subcategory=";

        /// <summary>
        /// The HTTP context.
        /// </summary>
        private HttpContextBase httpContext;

        /// <summary>
        /// The route collection.
        /// </summary>
        private RouteCollection routeCollection;

        /// <summary>
        /// The route name.
        /// </summary>
        private string routeName;

        /// <summary>
        /// Gets or sets the HTTP context.
        /// </summary>
        /// <value>
        /// The HTTP context.
        /// </value>
        [CanBeNull]
        public HttpContextBase HttpContext
        {
            get
            {
                if (this.httpContext != null)
                {
                    return this.httpContext;
                }

                if (System.Web.HttpContext.Current != null)
                {
                    return new HttpContextWrapper(System.Web.HttpContext.Current);
                }

                return null;
            }

            set
            {
                this.httpContext = value;
            }
        }

        /// <summary>
        /// Gets or sets the route collection.
        /// </summary>
        /// <value>The route collection.</value>
        [NotNull]
        public RouteCollection RouteCollection
        {
            get { return this.routeCollection ?? (this.routeCollection = RouteTable.Routes); }
            set { this.routeCollection = value; }
        }

        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        /// <value>The name of the route.</value>
        [NotNull]
        public string RouteName
        {
            get { return this.routeName ?? DefaultProductRouteName; }
            set { this.routeName = value; }
        }

        /// <summary>
        /// Gets the catalog item identifier from incoming request.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public virtual CatalogItemUrlData GetCatalogItemIdFromIncomingRequest()
        {
            RouteBase route = this.RouteCollection[this.RouteName];

            if (route == null)
            {
                return null;
            }

            RouteData routeData = route.GetRouteData(this.HttpContext);

            if (routeData == null)
            {
                return this.ExtractCategoryIdFromUrl();
            }

            return new CatalogItemUrlData(CatalogItemType.Product, (string)routeData.Values["id"]);
        }

        /// <summary>
        /// Extracts the category identifier from URL.
        /// </summary>
        /// <returns></returns>
        protected virtual CatalogItemUrlData ExtractCategoryIdFromUrl()
        {
            int categoryPos = this.HttpContext.Request.Url.Query.IndexOf(CategoryQueryParameter, StringComparison.OrdinalIgnoreCase);

            if (this.HttpContext.Request.Url.AbsoluteUri.IndexOf("/" + this.RouteName, StringComparison.OrdinalIgnoreCase) >= 0 && categoryPos >= 0)
            {
                string categoryId = string.Empty;
                int subCategoryPos = this.HttpContext.Request.Url.Query.IndexOf(SubCategoryQueryParameter, StringComparison.OrdinalIgnoreCase);

                if (subCategoryPos >= 0)
                {
                    int ampersandPos = this.HttpContext.Request.Url.Query.IndexOf("&", subCategoryPos);
                    if (ampersandPos >=0)
                    {
                        categoryId = this.HttpContext.Request.Url.Query.Substring(subCategoryPos + SubCategoryQueryParameter.Length, ampersandPos - subCategoryPos - SubCategoryQueryParameter.Length);
                    }
                    else
                    {
                        categoryId = this.HttpContext.Request.Url.Query.Substring(subCategoryPos + SubCategoryQueryParameter.Length);
                    }
                }
                else
                {
                    int ampersandPos = this.HttpContext.Request.Url.Query.IndexOf("&", categoryPos);
                    if (ampersandPos >= 0)
                    {
                        categoryId = this.HttpContext.Request.Url.Query.Substring(categoryPos + CategoryQueryParameter.Length, ampersandPos - categoryPos - CategoryQueryParameter.Length);
                    }
                    else
                    {
                        categoryId = this.HttpContext.Request.Url.Query.Substring(categoryPos + CategoryQueryParameter.Length);
                    }
                }

                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    return new CatalogItemUrlData(CatalogItemType.Category, categoryId);
                }
            }

            return null;
        }
    }
}