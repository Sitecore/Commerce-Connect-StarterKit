// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProductBulkPrices.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the pipeline processor that gets prices for several products.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Prices.GetProductPrices
{
  using System.Linq;
  using Sitecore.Commerce.Connectors.NopCommerce.NopPricesService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Prices;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Prices;
  using ProductPriceModel = Sitecore.Commerce.Connectors.NopCommerce.NopPricesService.ProductPriceModel;

  /// <summary>
  /// Defines the pipeline processor that gets prices for several products.
  /// </summary>
  public class GetProductBulkPrices : NopProcessor<IPricesServiceChannel>
  {
    /// <summary>
    /// Processes the specified args.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process(ServicePipelineArgs args)
    {
      GetProductBulkPricesRequest request = (GetProductBulkPricesRequest)args.Request;
      GetProductBulkPricesResult result = (GetProductBulkPricesResult)args.Result;
      ProductPriceModel[] productPriceModels;
      var currencyCode = request.CurrencyCode;
      if (string.IsNullOrEmpty(currencyCode))
      {
        currencyCode = "USD";
      }

      using (IPricesServiceChannel client = this.GetClient())
      {
        productPriceModels = client.GetProductPrices(request.ProductIds.ToArray(), request.PriceType);
      }

      foreach (ProductPriceModel model in productPriceModels)
      {
        if (model.Price != null)
        {
          if (result.Prices.ContainsKey(model.ProductId))
          {
            result.Prices[model.ProductId] = new Price(model.Price.Value, currencyCode) { PriceType = request.PriceType };
          }
          else
          {
            result.Prices.Add(model.ProductId, new Price(model.Price.Value, currencyCode) { PriceType = request.PriceType });
          }
        }
      }
    }
  }
}