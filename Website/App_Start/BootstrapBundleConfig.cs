// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapBundleConfig.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the BootstrapBundleConfig class.</summary>
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

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Sitecore.Commerce.StarterKit.App_Start.BootstrapBundleConfig), "RegisterBundles")]

namespace Sitecore.Commerce.StarterKit.App_Start
{
    using System.Web.Optimization;

    /// <summary>
    /// Bootstrap bundle configuration.
    /// </summary>
	public class BootstrapBundleConfig
	{
        /// <summary>
        /// Registers the bootstrap bundles.
        /// </summary>
        public static void RegisterBundles()
        {
            // Add @Styles.Render("~/Content/bootstrap") in the <head/> of your _Layout.cshtml view
            // For Bootstrap theme add @Styles.Render("~/Content/bootstrap-theme") in the <head/> of your _Layout.cshtml view
            // Add @Scripts.Render("~/bundles/bootstrap") after jQuery in your _Layout.cshtml view
            // When <compilation debug="true" />, MVC4 will render the full readable version. When set to <compilation debug="false" />, the minified version will be rendered automatically
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));
            BundleTable.Bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/bootstrap.css").Include("~/Content/bootstrap-addins.css"));
            BundleTable.Bundles.Add(new StyleBundle("~/Content/bootstrap-theme").Include("~/Content/bootstrap-theme.css"));
        }
	}
}
