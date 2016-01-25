// -----------------------------------------------------------------
// <copyright file="ObecPlugin.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ObecPlugin type.
// </summary>
// -----------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -----------------------------------------------------------------
namespace Nop.Plugin.Sitecore.Commerce.Inventory
{
    using System.Web.Routing;
    using Core.Plugins;
    using Services.Common;


    public class ObecPlugin : BasePlugin, IMiscPlugin
    {
        /// <summary>
        /// Gets a route for plugin configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName,
            out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "SitecoreObecInventory";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Sitecore.Obec.Inventory.Controllers" }, { "area", null } };
        }
    }
}
