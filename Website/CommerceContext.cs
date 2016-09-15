// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommerceContext.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CommerceContext class.
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
namespace Sitecore.Commerce.StarterKit
{
    using Castle.Windsor;
    using Sitecore.Commerce.StarterKit.App_Start;
    using Sitecore.Commerce.StarterKit.Helpers;

    /// <summary>
    /// Defines the class which allows to determine obec context.
    /// </summary>
    public class CommerceContext : Sitecore.Commerce.Multishop.CommerceContextBase
    {
        /// <summary>
        /// The product helper.
        /// </summary>
        private ProductHelper _productHelper;

        /// <summary>
        /// The windsor container.
        /// </summary>
        private IWindsorContainer _windsorContainer;

        /// <summary>
        /// Retrieves the ID  of the currently selected product.
        /// </summary>
        /// <returns>The ID of the currently selected product.</returns>
        public override string ProductId
        {
            get
            {
                var catalogItemUrlData = this.ProductHelper.GetCatalogItemIdFromIncomingRequest();
                if (catalogItemUrlData != null && catalogItemUrlData.ItemType == CatalogItemType.Product)
                {
                    return catalogItemUrlData.CatalogItemId;
                }

                return null;
            }
        }

        /// <summary>
        /// Retrieves the name  of the currently selected inventory location.
        /// </summary>
        /// <returns>The name of the currently selected inventory location.</returns>
        public override string InventoryLocation
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the windsor container.
        /// </summary>
        /// <value>The windsor container.</value>
        private IWindsorContainer WindsorContainer
        {
            get { return this._windsorContainer ?? (this._windsorContainer = WindsorConfig.Container); }
        }

        /// <summary>
        /// Gets the product helper.
        /// </summary>
        /// <value>The product helper.</value>
        private ProductHelper ProductHelper
        {
            get { return this._productHelper ?? (this._productHelper = this.WindsorContainer.Resolve<ProductHelper>()); }
        }
    }
}