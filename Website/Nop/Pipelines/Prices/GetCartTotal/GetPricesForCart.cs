// ----------------------------------------------------------------------------------------------
// <copyright file="GetPricesForCart.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Gets the prices for cart.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Prices.GetCartTotal
{
  using Sitecore.Commerce.Entities.Prices;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Prices;

  /// <summary>
  /// Gets the prices for cart.
  /// </summary>
  public class GetPricesForCart : PipelineProcessor<ServicePipelineArgs>
  {
    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process(ServicePipelineArgs args)
    {
      var cart = ((GetCartTotalRequest)args.Request).Cart;
      var currencyCode = ((GetCartTotalRequest)args.Request).CurrencyCode;

      decimal cartAmount = 0;
      string currency = null;

      foreach (var line in cart.Lines)
      {
        var amount = line.Product.Price.Amount * line.Quantity;
        currency = line.Product.Price.CurrencyCode;
        line.Total = new Total { Amount = amount, CurrencyCode = currency };

        cartAmount += amount;
      }

      if (string.IsNullOrEmpty(currencyCode))
      {
          currencyCode = currency;
      }

      cart.Total = new Total { Amount = cartAmount, CurrencyCode = currencyCode };

      ((GetCartTotalResult)args.Result).Cart = cart;
    }
  }
}