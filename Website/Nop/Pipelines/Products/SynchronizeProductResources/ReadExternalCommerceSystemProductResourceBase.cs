// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductResourceBase.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product resource base.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductResources
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Entities.Products;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Products;

    /// <summary>
    /// The read external commerce system product resource base.
    /// </summary>
    public abstract class ReadExternalCommerceSystemProductResourceBase : ReadExternalCommerceSystemProcessor<IProductsServiceChannel>
    {
        /// <summary>
        /// Runs the processor.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process([NotNull] ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var request = (ProductSynchronizationRequest)args.Request;

            using (var nopServiceClient = this.GetClient())
            {
                var productResourceModel = this.GetResource(nopServiceClient, request.ProductId);
                if (productResourceModel == null)
                {
                    return;
                }

                if (productResourceModel.Resources == null)
                {
                    return;
                }

                var productResources = (ICollection<ProductResource>)args.Request.Properties["ProductResources"];
                if (productResources == null)
                {
                    productResources = new Collection<ProductResource>();
                    args.Request.Properties["ProductResources"] = productResources;
                }

                var externalProductResources = productResourceModel.Resources.Select(p =>
                {
                    var productResource = this.InstantiateEntity<ProductResource>();

                    productResource.Created = p.CreatedOnUtc;
                    productResource.Updated = p.UpdatedOnUtc;
                    productResource.ExternalId = p.Id;
                    productResource.Type = p.ResourceType;
                    productResource.Uri = p.Url;
                    productResource.Name = p.Name;

                    return productResource;
                });

                foreach (var externalProductResource in externalProductResources)
                {
                    productResources.Add(externalProductResource);
                }

                nopServiceClient.Close();
            }
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="nopServiceClient">The service client.</param>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>
        /// Product resource model.
        /// </returns>
        protected abstract ProductResourceModel GetResource(IProductsServiceChannel nopServiceClient, string externalProductId);
    }
}