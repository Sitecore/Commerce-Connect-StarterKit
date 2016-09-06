// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductHelperTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ProductHelperTest type.
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
    using System.Web;
    using System.Web.Routing;
    using FluentAssertions;
    using NSubstitute;
    using Sitecore.Commerce.StarterKit.Helpers;
    using Xunit;
    using Xunit.Extensions;

    /// <summary>
    /// Defines the ProductHelperTest type.
    /// </summary>
    public class ProductHelperTest
    {
        /// <summary>
        /// The HTTP context.
        /// </summary>
        private readonly HttpContextBase httpContext;

        /// <summary>
        /// The HTTP request.
        /// </summary>
        private readonly HttpRequestBase httpRequest;

        /// <summary>
        /// The product route.
        /// </summary>
        private readonly RouteBase productRoute;

        /// <summary>
        /// The product link manager.
        /// </summary>
        private readonly ProductHelper productHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductHelperTest" /> class.
        /// </summary>
        public ProductHelperTest()
        {
            RouteCollection routes = new RouteCollection();

            this.productRoute = Substitute.For<RouteBase>();
            routes.Add("Products", this.productRoute);

            this.httpContext = Substitute.For<HttpContextBase>();
            this.httpRequest = Substitute.For<HttpRequestBase>();

            this.httpContext.Request.Returns(this.httpRequest);

            this.productHelper = new ProductHelper { HttpContext = this.httpContext, RouteCollection = routes };
        }

        /// <summary>
        /// Should return null if corresponding product route is not defined.
        /// </summary>
        [Fact]
        public void ShouldReturnNullIfCorrespondingProductRouteIsNotDefined()
        {
            // Arrange
            this.productHelper.RouteName = "__Products";

            // Act
            var result = this.productHelper.GetCatalogItemIdFromIncomingRequest();

            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Should return null if route data is not defined.
        /// </summary>
        [Fact]
        public void ShouldReturnNullIfRouteDataIsNotDefined()
        {
            // Arrange
            this.productRoute.GetRouteData(null).ReturnsForAnyArgs((RouteData)null);

            // Act
            var result = this.productHelper.GetCatalogItemIdFromIncomingRequest();

            // Assert
            result.Should().BeNull();
        }
    }
}