// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInventoryService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The InventoryService interface.
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
namespace Sitecore.Commerce.StarterKit.Services
{
    using System.Collections.Generic;
    using Sitecore.Commerce.Entities.Inventory;

    /// <summary>
    /// The InventoryService interface.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Gets the product stock information.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="productIds">The product ids.</param>
        /// <param name="detailsLevel">The details level.</param>
        /// <param name="location">The location.</param>
        /// <param name="visitorId">The visitor id.</param>
        /// <returns>The products stock information.</returns>
        List<StockInformation> GetStockInformation(string shopName, IEnumerable<string> productIds, StockDetailsLevel detailsLevel, string location, string visitorId);

        /// <summary>
        /// Gets the product pre-orderable information.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="productIds">The product ids.</param>
        /// <param name="visitorId">The visitor id.</param>
        /// <param name="location">The warehouse location.</param>
        /// <returns>The products pre-orderable information.</returns>
        List<OrderableInformation> GetPreOrderableInformation(string shopName, IEnumerable<string> productIds, string visitorId, string location);

        /// <summary>
        /// Gets the product back-orderable information.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="productIds">The product ids.</param>
        /// <param name="visitorId">The visitor id.</param>
        /// <param name="location">The warehouse location.</param>
        /// <returns>The products back-orderable information.</returns>
        List<OrderableInformation> GetBackOrderableInformation(string shopName, IEnumerable<string> productIds, string visitorId, string location);

        /// <summary>
        /// Gets the product back-orderable information.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="stockInformation">The product stock information.</param>
        /// <param name="location">The location.</param>
        void VisitedProductStockStatus(string shopName, StockInformation stockInformation, string location);

        /// <summary>
        /// Sign up the visitor for product back in stock notification.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="productId">The product id.</param>
        /// <param name="location">The location.</param>
        /// <param name="interestDate">The interestDate.</param>
        void VisitorSignUpForStockNotification(string shopName, string productId, string location, System.DateTime? interestDate);
    }
}