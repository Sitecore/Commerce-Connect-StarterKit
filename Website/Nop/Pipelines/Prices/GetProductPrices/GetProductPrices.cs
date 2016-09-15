// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProductPrices.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the pipeline processor that gets product prices.
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

  /// <summary>
  /// Defines the pipeline processor that gets product prices.
  /// </summary>
  public class GetProductPrices : NopProcessor<IPricesServiceChannel>
  {
    /// <summary>
    /// The list price type.
    /// </summary>
    private const string ListPriceType = "List";

    /// <summary>
    /// Processes the specified args.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process(ServicePipelineArgs args)
    {
      var request = (GetProductPricesRequest)args.Request;
      var result = (GetProductPricesResult)args.Result;
      var currencyCode = request.CurrencyCode;
      if (string.IsNullOrEmpty(currencyCode))
      {
        currencyCode = "USD";
      }

      using (var client = this.GetClient())
      {
        if (request.PriceTypeIds.Any())
        {
          foreach (var priceType in request.PriceTypeIds)
          {
            result.Prices.Add(priceType, this.GetProductPrice(client, request.ProductId, priceType, currencyCode));
          }
        }
        else
        {
          result.Prices.Add(ListPriceType, this.GetProductPrice(client, request.ProductId, ListPriceType, currencyCode));
        }
      }
    }

    /// <summary>
    /// Gets the product price.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="productId">The product identifier.</param>
    /// <param name="priceType">Type of the price.</param>
    /// <param name="currency">The currency.</param>
    /// <returns>
    /// Price of the product. <value>0</value> is returned when corresponding product was not found.
    /// </returns>
    private Price GetProductPrice(IPricesServiceChannel client, string productId, string priceType, string currency)
    {
      var price = client.GetProductPrice(productId, priceType);

      if (price != null)
      {
        return new Price(price.Value, currency) { PriceType = priceType };
      }
      
      return new Price(0, currency) { PriceType = priceType };
    }
  }
}
