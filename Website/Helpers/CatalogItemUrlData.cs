// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogItemUrlData.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CatalogItemUrlData class.
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

namespace Sitecore.Commerce.StarterKit.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public enum CatalogItemType
    {
        Category,
        Product
    };

    /// <summary>
    /// Represents the catalog item information extracted from the url.
    /// </summary>
    public class CatalogItemUrlData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogItemUrlData"/> class.
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="catalogItemId">The catalog item identifier.</param>
        public CatalogItemUrlData(CatalogItemType itemType, string catalogItemId)
        {
            this.ItemType = itemType;
            this.CatalogItemId = catalogItemId;
        }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        /// <value>
        /// The type of the item.
        /// </value>
        public CatalogItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the catalog item identifier.
        /// </summary>
        /// <value>
        /// The catalog item identifier.
        /// </value>
        public string CatalogItemId { get; set; }
    }
}