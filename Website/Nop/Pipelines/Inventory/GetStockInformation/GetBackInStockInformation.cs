// ----------------------------------------------------------------------------------------------
// <copyright file="GetBackInStockInformation.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The pipeline processor that gets back-in-stock information for products in NOP commerce.
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

    /// <summary>
    /// The pipeline processor that gets back-in-stock information for products in NOP commerce.
    /// </summary>
    public class GetBackInStockInformation : NopProcessor<IInventoryServiceChannel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopulateStockInformation"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetBackInStockInformation([NotNull] IEntityFactory entityFactory)
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
            var request = (GetBackInStockInformationRequest)args.Request;
            var result = (GetBackInStockInformationResult)args.Result;
            StockInformationUpdateModel[] stockInformationUpdateModels;

            using (IInventoryServiceChannel client = this.GetClient())
            {
                stockInformationUpdateModels = client.GetBackInStocksInformation(
                  request.ShopName,
                  request.Products.Select(p => p.ProductId).ToArray());
            }

            var stockInformationUpdateList = new List<StockInformationUpdate>();
            foreach (var model in stockInformationUpdateModels)
            {
                var entity = this.EntityFactory.Create<StockInformationUpdate>("StockInformationUpdate");
                this.PopulateStockInformationUpdate(entity, model);
                stockInformationUpdateList.Add(entity);
            }

            result.StockInformationUpdates = stockInformationUpdateList;
        }

        /// <summary>
        /// Populates an OBEC <see cref="StockInformationUpdate"/> entity based on a NOP <see cref="StockInformationUpdateModel"/> model.
        /// </summary>
        /// <param name="entity">The OBEC <see cref="StockInformationUpdate"/> entity to populate.</param>
        /// <param name="model">The NOP <see cref="StockInformationUpdateModel"/> model.</param>
        protected virtual void PopulateStockInformationUpdate(StockInformationUpdate entity, StockInformationUpdateModel model)
        {
            Sitecore.Diagnostics.Assert.IsNotNull(entity, "entity");
            Sitecore.Diagnostics.Assert.IsNotNull(model, "model");

            entity.Product = this.EntityFactory.Create<InventoryProduct>("InventoryProduct");
            entity.Product.ProductId = model.ProductId;

            if (model.StockInformationUpdateLocation != null)
            {
                var locationList = new List<StockInformationUpdateLocation>();
                foreach (var locationModel in model.StockInformationUpdateLocation)
                {
                    var locationEntity = this.EntityFactory.Create<StockInformationUpdateLocation>("StockInformationUpdateLocation");
                    this.PopulateStockInformationUpdate(locationEntity, locationModel);
                    locationList.Add(locationEntity);
                }

                entity.StockInformationUpdateLocations = locationList.AsReadOnly();
            }
        }

        /// <summary>
        /// Populates an OBEC <see cref="StockInformationUpdateLocation"/> entity based on a NOP <see cref="StockInformationUpdateLocationModel"/> model.
        /// </summary>
        /// <param name="entity">The OBEC <see cref="StockInformationUpdateLocation"/> entity to populate.</param>
        /// <param name="model">The NOP <see cref="StockInformationUpdateLocationModel"/> model.</param>
        protected virtual void PopulateStockInformationUpdate(StockInformationUpdateLocation entity, StockInformationUpdateLocationModel model)
        {
            Sitecore.Diagnostics.Assert.IsNotNull(entity, "entity");
            Sitecore.Diagnostics.Assert.IsNotNull(model, "model");

            entity.AvailabilityDate = model.AvailabilityDate;
            entity.Count = model.Count;
            entity.Location = model.Location;
        }
    }
}
