// ----------------------------------------------------------------------------------------------
// <copyright file="InventoryExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The inventory extensions.
// </summary>
// ----------------------------------------------------------------------------------------------
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
using Sitecore.Commerce.Connectors.NopCommerce.NopInventoryService;
using Sitecore.Commerce.Entities.Inventory;
using Sitecore.Diagnostics;
using StockStatus = Sitecore.Commerce.Entities.Inventory.StockStatus;

namespace Sitecore.Commerce.Connectors.NopCommerce
{
    /// <summary>
    /// The inventory extensions.
    /// </summary>
    public static class InventoryExtensions
    {
        /// <summary>
        /// Maps orderable information from model.
        /// </summary>
        /// <param name="orderableInfo">The orderable information.</param>
        /// <param name="orderableInfoModel">The orderable information model.</param>
        public static void MapOrderableInformationFromModel([NotNull] this OrderableInformation orderableInfo, [NotNull] OrderableInformationModel orderableInfoModel) 
        {
            Assert.ArgumentNotNull(orderableInfo, "orderableInfo");
            Assert.ArgumentNotNull(orderableInfoModel, "orderableInfoModel");

            orderableInfo.CartQuantityLimit = orderableInfoModel.CartQuantityLimit;
            orderableInfo.InStockDate = orderableInfoModel.InStockDate;
            orderableInfo.OrderableEndDate = orderableInfoModel.OrderableEndDate;
            orderableInfo.OrderableStartDate = orderableInfoModel.OrderableStartDate;
            orderableInfo.RemainingQuantity = orderableInfoModel.RemainingQuantity;
            orderableInfo.ShippingDate = orderableInfoModel.ShippingDate;
            orderableInfo.Status = new StockStatus((int)orderableInfoModel.Status, orderableInfoModel.Status.ToString());            
        }
    }
}