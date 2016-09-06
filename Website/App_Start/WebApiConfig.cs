// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the WebApiConfig type.
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
namespace Sitecore.Commerce.StarterKit.App_Start
{
  using System.Web.Http;

  /// <summary>
  /// The web programming interface config.
  /// </summary>
  public static class WebApiConfig
  {
    /// <summary>
    /// The register.
    /// </summary>
    /// <param name="config">
    /// The config.
    /// </param>
    public static void Register(HttpConfiguration config)
    {
      config.Routes.MapHttpRoute(
        name: "Price",
        routeTemplate: "api/price/{product}",
        defaults: new { controller = "Price" });

      config.Routes.MapHttpRoute(
        name: "ApiSamples",
        routeTemplate: "api/{controller}/{action}/{id}",
        defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional });

      config.Routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional });
    }
  }
}