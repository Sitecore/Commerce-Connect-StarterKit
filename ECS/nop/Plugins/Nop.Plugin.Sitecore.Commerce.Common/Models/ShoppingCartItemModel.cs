// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShoppingCartItemModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The class for Nop cart line data.
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
namespace Nop.Plugin.Sitecore.Commerce.Common.Models
{
  using System.Collections;
  using System.Collections.Generic;
  using Nop.Core.Domain.Orders;

  /// <summary>
  /// The class for Nop cart line data.
  /// </summary>
  public class ShoppingCartItemModel
  {
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>
    /// The id.
    /// </value>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the product id.
    /// </summary>
    /// <value>
    /// The product id.
    /// </value>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the Stock Keeping Unit.
    /// </summary>
    /// <value>
    /// The Stock Keeping Unit.
    /// </value>
    public string Sku { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    /// <value>
    /// The quantity.
    /// </value>
    public uint Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price.
    /// </summary>
    /// <value>
    /// The price.
    /// </value>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the line total.
    /// </summary>
    /// <value>
    /// The line total.
    /// </value>
    public decimal LineTotal { get; set; }
  }
}