// ---------------------------------------------------------------------
// <copyright file="PricingService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The pricing service.
// </summary>
// ---------------------------------------------------------------------
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
namespace Sitecore.Commerce.StarterKit.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Commerce.Services.Prices;

  /// <summary>
  /// The pricing service.
  /// </summary>
  public class PricingService : IPricingService
  {
    /// <summary>
    /// The list price key.
    /// </summary>
    private const string ListPriceKey = "List";

    /// <summary>
    /// The service provider.
    /// </summary>
    private readonly PricingServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="PricingService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public PricingService(PricingServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the product price.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <returns>The product price.</returns>
    public decimal GetProductPrice(string productId)
    {
      var request = new GetProductPricesRequest(productId);
      var result = this.serviceProvider.GetProductPrices(request);
      var price = result.Prices.ContainsKey(ListPriceKey) ? result.Prices[ListPriceKey].Amount : decimal.Zero;

      return price;
    }

    /// <summary>
    /// Gets the product bulk prices.
    /// </summary>
    /// <param name="productIds">The product ids.</param>
    /// <returns>The product bulk prices.</returns>
    public IDictionary<string, decimal> GetProductBulkPrices(IEnumerable<string> productIds)
    {
      var ids = productIds as IList<string> ?? productIds.ToList();

      GetProductBulkPricesRequest request = new GetProductBulkPricesRequest(ids);
      GetProductBulkPricesResult result = this.serviceProvider.GetProductBulkPrices(request);

      var prices = new Dictionary<string, decimal>(ids.Count);
      foreach (var id in ids)
      {
        var price = result.Prices.ContainsKey(id) ? result.Prices[id].Amount : decimal.Zero;
        if (prices.ContainsKey(id))
        {
          continue;
        }

        prices.Add(id, price);
      }

      return prices;
    }
  }
}