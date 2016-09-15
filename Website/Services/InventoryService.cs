// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The inventory service.
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
    using System.Linq;
    using System.Collections.Generic;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Services.Inventory;
    using Sitecore.Commerce.Contacts;

    /// <summary>
    /// The inventory service.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly InventoryServiceProvider _serviceProvider;

        /// <summary>
        /// The visitor factory.
        /// </summary>
        private readonly ContactFactory _contactFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="contactFactory">The visitor factory.</param>
        public InventoryService(InventoryServiceProvider serviceProvider, ContactFactory contactFactory)
        {
            this._serviceProvider = serviceProvider;
            this._contactFactory = contactFactory;
        }

        /// <summary>
        /// Gets the product stock information.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="productIds">The product ids.</param>
        /// <param name="detailsLevel">The details level.</param>
        /// <param name="location">The location.</param>
        /// <param name="visitorId">The visitor id.</param>
        /// <returns>The products stock information.</returns>
        public List<StockInformation> GetStockInformation(string shopName, IEnumerable<string> productIds, StockDetailsLevel detailsLevel, string location, string visitorId)
        {
            var request = new GetStockInformationRequest(shopName, productIds.Select(pid => new InventoryProduct { ProductId = pid }).ToList(), detailsLevel) { Location = location, VisitorId = visitorId };
            GetStockInformationResult result = this._serviceProvider.GetStockInformation(request);
            var stockInfos = new List<StockInformation>();
            if (result == null)
            {
                return stockInfos;
            }

            foreach (var stockInfo in productIds.Select(id => (result.StockInformation.FirstOrDefault(i => i.Product.ProductId.Equals(id, System.StringComparison.OrdinalIgnoreCase)) ?? new StockInformation { Product = new InventoryProduct { ProductId = id } })).Where(stockInfo => !stockInfos.Contains(stockInfo)))
            {
                stockInfos.Add(stockInfo);
            }
            
            return stockInfos;
        }

        /// <summary>
        /// Gets the product pre-orderable information.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="productIds">The product ids.</param>
        /// <param name="visitorId">The visitor id.</param>
        /// <param name="location">The warehouse location.</param>
        /// <returns>The products pre-orderable information.</returns>
        public List<OrderableInformation> GetPreOrderableInformation(string shopName, IEnumerable<string> productIds, string visitorId, string location)
        {
            var request = new GetPreOrderableInformationRequest(shopName, productIds.Select(pid => new InventoryProduct { ProductId = pid }).ToList()) { VisitorId = visitorId, Location = location };
            GetPreOrderableInformationResult result = this._serviceProvider.GetPreOrderableInformation(request);
            var orderableInfos = new List<OrderableInformation>();
            if (result == null)
            {
                return orderableInfos;
            }

            foreach (var orderableInfo in productIds.Select(id => (result.OrderableInformation.FirstOrDefault(i => i.Product.ProductId.Equals(id, System.StringComparison.OrdinalIgnoreCase)) ?? new OrderableInformation { Product = new InventoryProduct { ProductId = id } })).Where(orderableInfo => !orderableInfos.Contains(orderableInfo)))
            {
                orderableInfos.Add(orderableInfo);
            }

            return orderableInfos;
        }

        /// <summary>
        /// Gets the product back-orderable information.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="productIds">The product ids.</param>
        /// <param name="visitorId">The visitor id.</param>
        /// <param name="location">The wharehouse location.</param>
        /// <returns>The products back-orderable information.</returns>
        public List<OrderableInformation> GetBackOrderableInformation(string shopName, IEnumerable<string> productIds, string visitorId, string location)
        {
            var request = new GetBackOrderableInformationRequest(shopName, productIds.Select(pid => new InventoryProduct { ProductId = pid }).ToList()) { VisitorId = visitorId, Location = location };
            GetBackOrderableInformationResult result = this._serviceProvider.GetBackOrderableInformation(request);
            var orderableInfos = new List<OrderableInformation>();
            if (result == null)
            {
                return orderableInfos;
            }

            foreach (var orderableInfo in productIds.Select(id => (result.OrderableInformation.FirstOrDefault(i => i.Product.ProductId.Equals(id, System.StringComparison.OrdinalIgnoreCase)) ?? new OrderableInformation { Product = new InventoryProduct { ProductId = id } })).Where(orderableInfo => !orderableInfos.Contains(orderableInfo)))
            {
                orderableInfos.Add(orderableInfo);
            }

            return orderableInfos;
        }

        /// <summary>
        /// Gets the product back-orderable information.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="stockInformation">The product stock information.</param>
        /// <param name="location">The location.</param>
        public void VisitedProductStockStatus(string shopName, StockInformation stockInformation, string location)
        {
            var request = new VisitedProductStockStatusRequest(shopName, stockInformation) { Location = location };
            this._serviceProvider.VisitedProductStockStatus(request);
        }

        /// <summary>
        /// Sign up the visitor for product back in stock notification.
        /// </summary>
        /// <param name="shopName">The shop name.</param>
        /// <param name="productId">The product id.</param>
        /// <param name="location">The location.</param>
        /// <param name="interestDate">The interestDate.</param>
        public void VisitorSignUpForStockNotification(string shopName, string productId, string location, System.DateTime? interestDate)
        {
            var visitorId = this._contactFactory.GetContact();
            var request = new VisitorSignUpForStockNotificationRequest(shopName, visitorId, Context.User.Profile.Email, new InventoryProduct { ProductId = productId }) { Location = location, InterestDate = interestDate };
            this._serviceProvider.VisitorSignUpForStockNotification(request);
        }
    }
}