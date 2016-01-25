// ----------------------------------------------------------------------------------------------
// <copyright file="WishlistLineModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the WishlistLineModel class.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Commerce.StarterKit.Models
{
  using Diagnostics;
  using Entities.WishLists;

  public class WishlistLineModel
  {
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>
    /// The id.
    /// </value>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the product.
    /// </summary>
    /// <value>
    /// The product.
    /// </value>
    public string ProductId { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    /// <value>
    /// The name of the product.
    /// </value>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    /// <value>
    /// The quantity.
    /// </value>
    public uint Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    /// <value>
    /// The unit price.
    /// </value>
    public decimal UnitPrice { get; set; }

    public string UnitPriceLocal
    {
      get
      {
        return this.UnitPrice.ToString("C", Context.Language.CultureInfo);
      }
    }

    /// <summary>
    /// Gets or sets the total price.
    /// </summary>
    /// <value>
    /// The total price.
    /// </value>
    public string TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets Image.
    /// </summary>
    /// <value>
    /// The Image.
    /// </value>
    public string Image { get; set; }

    public WishlistLineModel(WishListLine wishListLine)
    {
      Assert.ArgumentNotNull(wishListLine, "wishListLine");

      this.Id = wishListLine.ExternalId;
      this.Quantity = wishListLine.Quantity;

      if (wishListLine.Product != null)
      {
        this.ProductId = wishListLine.Product.ProductId;

        if (wishListLine.Product.Price != null)
        {
          this.UnitPrice = wishListLine.Product.Price.Amount;
        }

        //this.ProductName = productName ?? line.Product.ProductId;
        this.ProductName = wishListLine.Product.ProductId;
      }

      if (wishListLine.Total != null)
      {
        this.TotalPrice = wishListLine.Total.Amount.ToString("C", Context.Language.CultureInfo);
      }
    }
  
  }
}