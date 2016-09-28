// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderableInformationModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the OrderableInformationModel class.
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
    using System;

    /// <summary>
    /// represents product orderable information.
    /// </summary>
    public class OrderableInformationModel
    {
        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public StockStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the in stock date.
        /// </summary>
        /// <value>
        /// The in stock date.
        /// </value>
        public DateTime? InStockDate { get; set; }

        /// <summary>
        /// Gets or sets the shipping date.
        /// </summary>
        /// <value>
        /// The shipping date.
        /// </value>
        public DateTime? ShippingDate { get; set; }

        /// <summary>
        /// Gets or sets the cart quantity limit.
        /// </summary>
        /// <value>
        /// The cart quantity limit.
        /// </value>
        public double CartQuantityLimit { get; set; }

        /// <summary>
        /// Gets or sets the orderable start date.
        /// </summary>
        /// <value>
        /// The orderable start date.
        /// </value>
        public DateTime? OrderableStartDate { get; set; }

        /// <summary>
        /// Gets or sets the orderable end date.
        /// </summary>
        /// <value>
        /// The orderable end date.
        /// </value>
        public DateTime? OrderableEndDate { get; set; }

        /// <summary>
        /// Gets or sets the remaining quantity.
        /// </summary>
        /// <value>
        /// The remaining quantity.
        /// </value>
        public double RemainingQuantity { get; set; }
    }
}
