// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexStockInformationModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the IndexStockInformationModel class.
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
// -----------------------------------------------------------------
namespace Nop.Plugin.Sitecore.Commerce.Inventory.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents index stock information.
    /// </summary>
    public class IndexStockInformationModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexStockInformationModel"/> class.
        /// </summary>
        public IndexStockInformationModel()
        {
            this.InStockLocations = new List<string>();
            this.OutOfStockLocations = new List<string>();
            this.OrderableLocations = new List<string>();
        }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the in stock locations.
        /// </summary>
        /// <value>
        /// The in stock locations.
        /// </value>
        public IList<string> InStockLocations { get; set; }

        /// <summary>
        /// Gets or sets the out of stock locations.
        /// </summary>
        /// <value>
        /// The out of stock locations.
        /// </value>
        public IList<string> OutOfStockLocations { get; set; }

        /// <summary>
        /// Gets or sets the orderable locations.
        /// </summary>
        /// <value>
        /// The orderable locations.
        /// </value>
        public IList<string> OrderableLocations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pre orderable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [pre orderable]; otherwise, <c>false</c>.
        /// </value>
        public bool PreOrderable { get; set; }
    }
}