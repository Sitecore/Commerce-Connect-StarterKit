// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommerceBundleConfig.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CommerceBundleConfig type.
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
using Sitecore.Commerce.StarterKit.App_Start;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(CommerceBundleConfig), "RegisterBundles")]

namespace Sitecore.Commerce.StarterKit.App_Start
{
    using System.Web.Optimization;

    /// <summary>
    /// The Commerce bundle config.
    /// </summary>
    public class CommerceBundleConfig
    {
        /// <summary>
        /// Registers the bundles.
        /// </summary>
        public static void RegisterBundles()
        {
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/Commerce/cart")
                .Include("~/Scripts/cart.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/Commerce/carts")
                .Include("~/Scripts/carts.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/Commerce/cartwidget")
                .Include("~/Scripts/cartwidget.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/Commerce/products")
                .Include("~/Scripts/products.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/knockout")
                .Include("~/Scripts/knockout*"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/Commerce/product")
                .Include("~/Scripts/product.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/Commerce/paymentservice")
                .Include("~/Scripts/paymentservice.js"));
        }
    }
}