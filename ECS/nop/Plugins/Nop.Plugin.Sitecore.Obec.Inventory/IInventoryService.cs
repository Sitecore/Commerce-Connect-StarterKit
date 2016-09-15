// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInventoryService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the InventoryServiceIInventoryService interface.
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
namespace Nop.Plugin.Sitecore.Commerce.Inventory
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    /// <summary>
    /// The inventory service interface.
    /// </summary>
    [ServiceContract]
    public interface IInventoryService
    {
        /// <summary>
        /// Gets the stock information.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="externalproductIds">The externalproduct ids.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The stock information.</returns>
        [OperationContract]
        IList<StockInformationModel> GetStocksInformation(string shopName, IList<string> externalproductIds, Guid customerId);

        /// <summary>
        /// Gets the stock information.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="externalProductId">The external product identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The product stock information.</returns>
        [OperationContract]
        StockInformationModel GetStockInformation(string shopName, string externalProductId, Guid customerId);

        /// <summary>
        /// Gets the pre orderable information by product list.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="externalproductIds">The externalproduct ids.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The preorderable product information.</returns>
        [OperationContract]
        IList<OrderableInformationModel> GetPreOrderableInformationList(string shopName, IList<string> externalproductIds, Guid customerId);

        /// <summary>
        /// Gets the pre orderable information by product.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="externalProductId">The external product identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The pre orderable product information.</returns>
        [OperationContract]
        OrderableInformationModel GetPreOrderableInformation(string shopName, string externalProductId, Guid customerId);

        /// <summary>
        /// Gets the back orderable information by product list.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="externalproductIds">The externalproduct ids.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The back orderable product information.</returns>
        [OperationContract]
        IList<OrderableInformationModel> GetBackOrderableInformationList(string shopName, IList<string> externalproductIds, Guid customerId);

        /// <summary>
        /// Gets the back orderable information by product.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="externalProductId">The external product identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The product back orderable information.</returns>
        [OperationContract]
        OrderableInformationModel GetBackOrderableInformation(string shopName, string externalProductId, Guid customerId);

        /// <summary>
        /// Gets the back in stock information by product list.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="externalproductIds">The externalproduct ids.</param>
        /// <returns>The product back in stock information.</returns>
        [OperationContract]
        IList<StockInformationUpdateModel> GetBackInStocksInformation(string shopName, IList<string> externalproductIds);

        /// <summary>
        /// Gets the back in stock information by product.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="externalProductId">The externalproduct identifier.</param>
        /// <returns>The product back in stock information.</returns>
        [OperationContract]
        StockInformationUpdateModel GetBackInStockInformation(string shopName, string externalProductId);

        /// <summary>
        /// Stocks the status for indexing.
        /// </summary>
        /// <param name="externalproductIds">The externalproduct ids.</param>
        /// <returns>The product stock status for indexing.</returns>
        [OperationContract]
        IList<IndexStockInformationModel> StocksStatusForIndexing(IList<string> externalproductIds);

        /// <summary>
        /// Stocks the status for indexing by product.
        /// </summary>
        /// <param name="externalProductId">The externalproduct identifier.</param>
        /// <returns>The product stock status for indexing.</returns>
        [OperationContract]
        IndexStockInformationModel StockStatusForIndexing(string externalProductId);
    }
}