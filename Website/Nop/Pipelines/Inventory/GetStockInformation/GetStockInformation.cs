// ----------------------------------------------------------------------------------------------
// <copyright file="GetStockInformation.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The pipeline processor that gets stock information for products in NOP commerce.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Inventory.GetStockInformation
{
    using Sitecore.Commerce.Connectors.NopCommerce.NopInventoryService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Inventory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using StockInformationModel = Sitecore.Commerce.Connectors.NopCommerce.NopInventoryService.StockInformationModel;
    using StockStatusNop = Sitecore.Commerce.Connectors.NopCommerce.NopInventoryService.StockStatus;
    using StockStatusObec = Sitecore.Commerce.Entities.Inventory.StockStatus;

    /// <summary>
    /// The pipeline processor that gets stock information for products in NOP commerce.
    /// </summary>
    public class GetStockInformation : NopProcessor<IInventoryServiceChannel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopulateStockInformation"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetStockInformation([NotNull] IEntityFactory entityFactory)
        {
            this.EntityFactory = entityFactory;
        }

        /// <summary>
        /// Gets the entity factory.
        /// </summary>
        public IEntityFactory EntityFactory { get; private set; }

        /// <summary>
        /// Processes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process(ServicePipelineArgs args)
        {
            var request = (GetStockInformationRequest)args.Request;
            var result = (GetStockInformationResult)args.Result;
            StockInformationModel[] stockInformationModels;

            using (IInventoryServiceChannel client = this.GetClient())
            {
                Guid visitorId;
                if (!Guid.TryParse(request.VisitorId, out visitorId))
                {
                    visitorId = Guid.Empty;
                }

                stockInformationModels = client.GetStocksInformation(request.ShopName, request.Products.Select(p => p.ProductId).ToArray(), visitorId);
            }

            var stockInformationList = new List<StockInformation>();
            foreach (var product in request.Products)
            {
                var entity = this.EntityFactory.Create<StockInformation>("StockInformation");
                entity.Product = this.EntityFactory.Create<InventoryProduct>("InventoryProduct");
                entity.Product.ProductId = product.ProductId;

                var model = stockInformationModels.FirstOrDefault(m => m != null && m.ProductId == product.ProductId);
                if (model != null)
                {
                    this.PopulateStockInformation(entity, model, request.DetailsLevel);
                }

                stockInformationList.Add(entity);
            }

            result.StockInformation = stockInformationList;
        }

        /// <summary>
        /// Populates an OBEC <see cref="StockInformation"/> entity based on a NOP <see cref="StockInformationModel"/> model.
        /// </summary>
        /// <param name="entity">The OBEC <see cref="StockInformation"/> entity to populate.</param>
        /// <param name="model">The NOP <see cref="StockInformationModel"/> model.</param>
        /// <param name="detailsLevel">The stock information details level to retrieved.</param>
        protected virtual void PopulateStockInformation([NotNull] StockInformation entity, [NotNull] StockInformationModel model, StockDetailsLevel detailsLevel)
        {
            Sitecore.Diagnostics.Assert.IsNotNull(entity, "entity");
            Sitecore.Diagnostics.Assert.IsNotNull(model, "model");

            entity.Product = this.EntityFactory.Create<InventoryProduct>("InventoryProduct");
            entity.Product.ProductId = model.ProductId;

            if ((detailsLevel.Value & StockDetailsLevel.AvailibilityFlag) != 0)
            {
                entity.AvailabilityDate = model.AvailabilityDate;
            }

            if ((detailsLevel.Value & StockDetailsLevel.CountFlag) != 0)
            {
                entity.Count = model.Count;
            }

            if ((detailsLevel.Value & StockDetailsLevel.StatusFlag) != 0)
            {
                entity.Status = this.GetStockStatus(model.Status);
            }
        }

        /// <summary>
        /// Gets a <see cref="StockStatusObec"/> entity that represents a <see cref="StockStatusNop"/> enum value.
        /// </summary>
        /// <param name="modelStatus">The <see cref="StockStatusNop"/> to convert.</param>
        /// <returns>The converted <see cref="StockStatusObec"/> entity.</returns>
        protected virtual StockStatusObec GetStockStatus(StockStatusNop modelStatus)
        {
            return new StockStatusObec((int)modelStatus, modelStatus.ToString());
        }
    }
}