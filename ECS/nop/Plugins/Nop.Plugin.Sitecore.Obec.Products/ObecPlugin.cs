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
namespace Nop.Plugin.Sitecore.Commerce.Products
{
    using Core.Domain.Catalog;
    using Core.Events;
    using Core.Plugins;
    using Properties;
    using Security;
    using Services.Common;
    using Services.Events;
    using Services.Localization;
    using Services.Security;
    using System.Globalization;
    using System.ServiceModel;
    using System.Web.Routing;
    using TestWcfService.ObecProductService;

    /// <summary>
    /// Entry point for Commerce plugin.
    /// </summary>
    public class ObecPlugin : BasePlugin, IMiscPlugin, IConsumer<EntityUpdated<Product>>,
                                                                IConsumer<EntityInserted<Product>>,
                                                                IConsumer<EntityInserted<ProductVariantAttribute>>,
                                                                IConsumer<EntityUpdated<ProductVariantAttribute>>
    {
        /// <summary>
        /// The endpoint address.
        /// </summary>
        private readonly string _endpointAddress = Settings.Default.Nop_Plugin_Sitecore_Commerce_Products_Endpoint;

        /// <summary>
        /// The permission service
        /// </summary>
        private readonly IPermissionService _permissionService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ObecPlugin" /> class.
        /// </summary>
        /// <param name="permissionService">The permission service.</param>
        public ObecPlugin(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(EntityUpdated<Product> eventMessage)
        {
            // using eventMessage.Entity
            // Need to call external service to handle updated product.

            if (eventMessage.Entity.ProductType == ProductType.GroupedProduct)
            {
                foreach (var productVariant in eventMessage.Entity.ProductVariantAttributes)
                {
                    UpdateProductInSitecore(productVariant.Id.ToString(CultureInfo.InvariantCulture));
                }
            }
            else
            {
                UpdateProductInSitecore(eventMessage.Entity.Id.ToString(CultureInfo.InvariantCulture));
            }

        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(EntityUpdated<ProductVariantAttribute> eventMessage)
        {
            UpdateProductInSitecore(eventMessage.Entity.Id.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(EntityInserted<Product> eventMessage)
        {
            // using eventMessage.Entity
            // Need to call external service to handle inserted product.
            if (eventMessage.Entity.ProductType == ProductType.GroupedProduct)
            {
                foreach (var productVariant in eventMessage.Entity.ProductVariantAttributes)
                {
                    UpdateProductInSitecore(productVariant.Id.ToString(CultureInfo.InvariantCulture));
                }
            }
            else
            {
                UpdateProductInSitecore(eventMessage.Entity.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(EntityInserted<ProductVariantAttribute> eventMessage)
        {
            // using eventMessage.Entity
            // Need to call external service to handle inserted product.
            UpdateProductInSitecore(eventMessage.Entity.Id.ToString(CultureInfo.InvariantCulture));
        }




        /// <summary>
        /// Updates the product in sitecore.
        /// </summary>
        /// <param name="productId">The product id.</param>
        private void UpdateProductInSitecore(string productId)
        {
            // TODO : Change following so it reads from correct config file.
            var myBinding = new BasicHttpBinding();

            var myEndpoint = new EndpointAddress(_endpointAddress);


            var channelFactory = new ChannelFactory<IProductServiceChannel>(myBinding, myEndpoint);

            using (var productsChannel = channelFactory.CreateChannel())
            {
                productsChannel.SynchronizeProduct(productId);
            }
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "SitecoreObecProducts";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Sitecore.Commerce.Products.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //install new permissions
            _permissionService.InstallPermissions(new ObecProductsPermissionProvider());

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Sitecore.Obec.Products.Description1", "Actually configuration is not required. Just some notes:");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sitecore.Obec.Products.Description2", "Ensure that permissions are properly configured on Access Control List page (disabled by default)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sitecore.Obec.Products.Description3", "To access service use {0}");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sitecore.Obec.Products.Description4", "For mex endpoint use {0}");
            base.Install();
        }

        /// <summary>
        /// Uninstalls this instance.
        /// </summary>
        public override void Uninstall()
        {
            //uninstall permissions
            _permissionService.UninstallPermissions(new ObecProductsPermissionProvider());

            //locales
            this.DeletePluginLocaleResource("Plugins.Sitecore.Obec.Products.Description1");
            this.DeletePluginLocaleResource("Plugins.Sitecore.Obec.Products.Description2");
            this.DeletePluginLocaleResource("Plugins.Sitecore.Obec.Products.Description3");
            this.DeletePluginLocaleResource("Plugins.Sitecore.Obec.Products.Description4");

            base.Uninstall();
        }
    }
}