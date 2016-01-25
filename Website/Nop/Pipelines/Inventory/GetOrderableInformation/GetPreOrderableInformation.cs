// ----------------------------------------------------------------------------------------------
// <copyright file="GetPreOrderableInformation.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the pipeline processor that gets pre-orderable stock information.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Inventory.GetOrderableInformation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sitecore.Commerce.Connectors.NopCommerce.NopInventoryService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Inventory;

    /// <summary>
    /// Defines the pipeline processor that gets product pre-orderable stock information.
    /// </summary>
    public class GetPreOrderableInformation : NopProcessor<IInventoryServiceChannel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPreOrderableInformation"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetPreOrderableInformation([NotNull] IEntityFactory entityFactory)
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
            var request = (GetPreOrderableInformationRequest)args.Request;
            var result = (GetPreOrderableInformationResult)args.Result;
            OrderableInformationModel[] orderableInformationModels;

            using (IInventoryServiceChannel client = this.GetClient())
            {
                Guid visitorId;
                if (!Guid.TryParse(request.VisitorId, out visitorId))
                {
                    visitorId = Guid.Empty;
                }

                orderableInformationModels = client.GetPreOrderableInformationList(request.ShopName, request.Products.Select(p => p.ProductId).ToArray(), visitorId);
            }
            
            var orderableInfos = new List<OrderableInformation>();
            foreach (var product in request.Products)
            {
                var orderableInfo = this.EntityFactory.Create<OrderableInformation>("OrderableInformation");
                orderableInfo.Product = this.EntityFactory.Create<InventoryProduct>("InventoryProduct");
                orderableInfo.Product.ProductId = product.ProductId;

                var model = orderableInformationModels.FirstOrDefault(m => m != null && m.ProductId == product.ProductId);
                if (model != null)
                {
                    orderableInfo.MapOrderableInformationFromModel(model);
                }

                orderableInfos.Add(orderableInfo);
            }
            
            result.OrderableInformation = orderableInfos;
        }
    }
}
