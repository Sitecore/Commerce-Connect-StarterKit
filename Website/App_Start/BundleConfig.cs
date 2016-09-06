// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the BundleConfig class.</summary>
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

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Sitecore.Commerce.StarterKit.App_Start.BundleConfig), "RegisterBundles")]

namespace Sitecore.Commerce.StarterKit.App_Start
{
    using System.Web.Optimization;

    /// <summary>
    /// Bundle configuration.
    /// </summary>
    public static class BundleConfig
    {
        /// <summary>
        /// registers the site script bundles.
        /// </summary>
        public static void RegisterBundles()
        {
            // Add @Scripts.Render("~/bundles/starterkit")
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/starterkit").Include("~/Scripts/StarterKit/ui-interactions.js"));
        }
    }
}