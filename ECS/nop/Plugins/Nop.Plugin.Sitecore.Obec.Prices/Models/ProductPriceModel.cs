// -----------------------------------------------------------------
// <copyright file="ProductPriceModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ProductPriceModel type.
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
namespace Nop.Plugin.Sitecore.Commerce.Prices.Models
{
  using System;

  /// <summary>
  /// The product price model.
  /// </summary>
  public class ProductPriceModel
  {
    /// <summary>
    /// Gets or sets the stock identifier.
    /// </summary>
    /// <value>
    /// The stock identifier.
    /// </value>
    [Obsolete("Should not be used - only there for compatibility with AutoHaus.WebUI product model")]
    public string Sku { get; set; }

    /// <summary>
    /// Gets or sets the product unique identifier.
    /// </summary>
    /// <value>
    /// The product unique identifier.
    /// </value>
    public string ProductId { get; set; }

    /// <summary>
    /// Gets or sets the price.
    /// </summary>
    /// <value>
    /// The price.
    /// </value>
    public decimal? Price { get; set; }
  }
}