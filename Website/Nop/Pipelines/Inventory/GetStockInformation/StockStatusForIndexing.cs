// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StockStatusForIndexing.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The pipeline processor that gets stock information for products in NOP commerce.
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
    using StockStatusCommerce = Sitecore.Commerce.Entities.Inventory.StockStatus;

    /// <summary>
    /// The pipeline processor that gets stock information for products in NOP commerce.
    /// </summary>
    public class StockStatusForIndexing : NopProcessor<IInventoryServiceChannel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StockStatusForIndexing"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        /// <param name="shopName">The shop name.</param>
        public StockStatusForIndexing([NotNull] IEntityFactory entityFactory, [NotNull] string shopName)
        {
            this.EntityFactory = entityFactory;
            this.ShopName = shopName;
        }

        /// <summary>
        /// Gets the entity factory.
        /// </summary>
        public IEntityFactory EntityFactory { get; private set; }

        /// <summary>
        /// Gets the shop name.
        /// </summary>
        public string ShopName { get; private set; }

        /// <summary>
        /// Processes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process(ServicePipelineArgs args)
        {
            StockStatusForIndexingRequest request = (StockStatusForIndexingRequest)args.Request;
            StockStatusForIndexingResult result = (StockStatusForIndexingResult)args.Result;
            IndexStockInformationModel[] indexStockInformationModels;

            using (var client = this.GetClient())
            {
                var productIds = request.Products.Select(p => p.ProductId).ToArray();
                indexStockInformationModels = client.StocksStatusForIndexing(productIds);
            }

            List<IndexStockInformation> indexStockInformationList = new List<IndexStockInformation>();
            foreach (var model in indexStockInformationModels)
            {
                var entity = this.EntityFactory.Create<IndexStockInformation>("IndexStockInformation");
                this.PopulateIndexStockInformation(entity, model);
                indexStockInformationList.Add(entity);
            }

            result.IndexStockInformation = indexStockInformationList;
        }

        /// <summary>
        /// Populates an Commerce <see cref="StockInformation"/> entity based on a NOP <see cref="StockInformationModel"/> model.
        /// </summary>
        /// <param name="entity">The Commerce <see cref="StockInformation"/> entity to populate.</param>
        /// <param name="model">The NOP <see cref="StockInformationModel"/> model.</param>
        protected virtual void PopulateIndexStockInformation([NotNull] IndexStockInformation entity, [NotNull] IndexStockInformationModel model)
        {
            Sitecore.Diagnostics.Assert.IsNotNull(entity, "entity");
            Sitecore.Diagnostics.Assert.IsNotNull(model, "model");

            entity.InStockLocations = (new List<string>(model.InStockLocations)).AsReadOnly();
            entity.OrderableLocations = (new List<string>(model.OrderableLocations)).AsReadOnly();
            entity.OutOfStockLocations = (new List<string>(model.OutOfStockLocations)).AsReadOnly();
            entity.PreOrderable = model.PreOrderable;

            var product = this.EntityFactory.Create<InventoryProduct>("InventoryProduct");
            product.ProductId = model.ProductId;
            entity.Product = product;
        }

        private IReadOnlyCollection<string> NewReadOnlyList(IEnumerable<string> items)
        {
            return (new List<string>(items)).AsReadOnly();
        }
    }
}
