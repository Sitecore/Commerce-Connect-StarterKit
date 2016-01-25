// ----------------------------------------------------------------------------------------------
// <copyright file="CartLineModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The cart line model.
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
namespace Sitecore.Commerce.StarterKit.Models
{
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Entities.Carts;

  /// <summary>
  /// The cart line model.
  /// </summary>
  public class CartLineModel
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CartLineModel" /> class.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="productName">Name of the product.</param>
    public CartLineModel([NotNull] CartLine line, string productName = null)
    {
      Assert.ArgumentNotNull(line, "line");

      this.Id = line.ExternalCartLineId;
      this.Quantity = line.Quantity;

      if (line.Product != null)
      {
        this.ProductId = line.Product.ProductId;

        if (line.Product.Price != null)
        {
          this.UnitPrice = line.Product.Price.Amount;

        }

        this.ProductName = productName ?? line.Product.ProductId;
      }

      if (line.Total != null)
      {
        this.TotalPrice = line.Total.Amount.ToString("C", Context.Language.CultureInfo);
      }
    }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>
    /// The id.
    /// </value>
    public string Id { get; protected set; }

    /// <summary>
    /// Gets or sets the product.
    /// </summary>
    /// <value>
    /// The product.
    /// </value>
    public string ProductId { get; protected set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    /// <value>
    /// The name of the product.
    /// </value>
    public string ProductName { get; protected set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    /// <value>
    /// The quantity.
    /// </value>
    public uint Quantity { get; protected set; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    /// <value>
    /// The unit price.
    /// </value>
    public decimal UnitPrice { get; protected set; }

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
    public string TotalPrice { get; protected set; }

    /// <summary>
    /// Gets or sets Image.
    /// </summary>
    /// <value>
    /// The Image.
    /// </value>
    public string Image { get; set; }
  }
}