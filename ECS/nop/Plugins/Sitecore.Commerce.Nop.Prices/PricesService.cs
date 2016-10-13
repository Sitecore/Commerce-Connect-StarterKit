// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PricesService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The implementation of the price service.
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
namespace Sitecore.Commerce.Nop.Prices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Activation;
    using System.Web.Services;
    using global::Nop.Core.Infrastructure;
    using Sitecore.Commerce.Nop.Prices.Models;
    using global::Nop.Services.Catalog;

    /// <summary>
    ///   The implementation of the price service..
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PricesService : IPricesService
    {
        /// <summary>
        ///   The product service.
        /// </summary>
        private readonly IProductService productService;

        /// <summary>
        ///   Initializes a new instance of the <see cref="PricesService" /> class.
        /// </summary>
        public PricesService()
        {
            this.productService = EngineContext.Current.Resolve<IProductService>();
        }

        /// <summary>
        ///   Gets the product prices by list of external product ids.
        /// </summary>
        /// <param name="externalproductIds">The external product ids.</param>
        /// <param name="priceType">Type of the price.</param>
        /// <returns>
        ///   List of prices.
        /// </returns>
        [WebMethod(EnableSession = false)]
        public virtual IList<ProductPriceModel> GetProductPrices(IList<string> externalproductIds, string priceType)
        {
            return externalproductIds.Select(externalproductId => new ProductPriceModel
            {
                Price = this.GetProductPrice(externalproductId, priceType),
                ProductId = externalproductId
            }).ToList();
        }

        /// <summary>
        ///   Gets the product price by external product unique identifier.
        /// </summary>
        /// <param name="externalProductId">The external product unique identifier.</param>
        /// <param name="priceType">Type of the price.</param>
        /// <returns>
        ///   Price value.
        /// </returns>
        [WebMethod(EnableSession = false)]
        public virtual decimal? GetProductPrice(string externalProductId, string priceType)
        {
            int productId;

            if (!int.TryParse(externalProductId, out productId))
            {
                return null;
            }

            var product = this.productService.GetProductById(productId);

            if (product == null)
            {
                return null;
            }

            switch (priceType)
            {
                case "List":
                    {
                        return product.Price;
                    }

                default:
                    {
                        return null;
                    }
            }
        }
    }
}