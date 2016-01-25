// ----------------------------------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The RouteConfig class.
// </summary>
// ----------------------------------------------------------------------------------------------
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
using Sitecore.Commerce.StarterKit.App_Start;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(RouteConfig), "RegisterRoutes")]

namespace Sitecore.Commerce.StarterKit.App_Start
{
  using System.Web.Http;
  using System.Web.Mvc;
  using System.Web.Routing;

  /// <summary>
  /// The route config.
  /// </summary>
  public class RouteConfig
  {
    /// <summary>
    /// The register routes.
    /// </summary>
    public static void RegisterRoutes()
    {
      RouteCollection routes = RouteTable.Routes;

      // Getting Sitecore work with bundles
      routes.MapRoute("sc_ignore_Bundles_Css", "content/{*pathInfo}");
      routes.MapRoute("sc_ignore_Bundles_Js", "bundles/{*pathInfo}");

      //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
        name: "CartData",
        url: "data/cart/{action}/{id}/{quantity}",
        defaults: new { controller = "Cart", id = RouteParameter.Optional, quantity = RouteParameter.Optional });

      routes.MapRoute(
          name: "Account",
          url: "account/{action}",
          defaults: new { controller = "Account"});

      routes.MapRoute(name: "Products", url: "products/{id}");

      routes.MapRoute(
          name: "ProductData",
          url: "data/product/{action}/{id}",
          defaults: new { controller = "Product", id = RouteParameter.Optional });

      routes.MapRoute(
        name: "CartSamplesByID",
        url: "samples/{controller}/{action}/by-cartid-{cartId}");

      routes.MapRoute(
        name: "Samples",
        url: "samples/{controller}/{action}/{id}",
        defaults: new { action = "Index", id = RouteParameter.Optional });

      routes.MapRoute(
        name: "Admin",
        url: "data/admin/{controller}/{action}/{id}",
        defaults: new { action = "Index", id = RouteParameter.Optional });
    }
  }
}