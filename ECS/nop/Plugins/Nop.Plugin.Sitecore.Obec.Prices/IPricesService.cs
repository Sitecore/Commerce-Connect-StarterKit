// -----------------------------------------------------------------
// <copyright file="IPricesService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The PricesService interface.
// </summary>
// -----------------------------------------------------------------
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
namespace Nop.Plugin.Sitecore.Commerce.Prices
{
  using System.Collections.Generic;
  using System.ServiceModel;
  using Models;

  /// <summary>
  /// The PricesService interface.
  /// </summary>
  [ServiceContract]
  public interface IPricesService
  {
    /// <summary>
    /// Gets the product prices by list of external product ids.
    /// </summary>
    /// <param name="externalproductIds">The external product ids.</param>
    /// <param name="priceType">Type of the price.</param>
    /// <returns>List of prices.</returns>
    [OperationContract]
    IList<ProductPriceModel> GetProductPrices(IList<string> externalproductIds, string priceType);

    /// <summary>
    /// Gets the product price by external product unique identifier.
    /// </summary>
    /// <param name="externalProductId">The external product unique identifier.</param>
    /// <param name="priceType">Type of the price.</param>
    /// <returns>Price value.</returns>
    [OperationContract]
    decimal? GetProductPrice(string externalProductId, string priceType);
  }
}