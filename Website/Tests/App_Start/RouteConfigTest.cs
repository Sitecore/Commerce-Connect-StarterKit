// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteConfigTest.cs" company="Sitecore Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The route config test.
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
namespace Sitecore.Obec.StarterKit.Tests.App_Start
{
  using System;
  using System.Web;
  using System.Web.Routing;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Obec.StarterKit.App_Start;
  using Xunit;

  /// <summary>
  /// The route config test.
  /// </summary>
  public class RouteConfigTest : IDisposable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RouteConfigTest"/> class.
    /// </summary>
    public RouteConfigTest()
    {
      RouteConfig.RegisterRoutes();
    }

    /// <summary>
    /// Should get the "admin" route data.
    /// </summary>
    [Fact]
    public void ShouldGetAdminRouteData()
    {
      // Arrange
      var routes = RouteTable.Routes;

      var httpContext = Substitute.For<HttpContextBase>();
      httpContext.Request.AppRelativeCurrentExecutionFilePath.Returns("/admin/carts/");
      httpContext.Response.ApplyAppPathModifier(null).ReturnsForAnyArgs(ci => ci.Args()[0]);

      // Act
      var routeData = routes.GetRouteData(httpContext);

      // Assert
      routeData.Should().NotBeNull();
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
      RouteTable.Routes.Clear();
    }
  }
}