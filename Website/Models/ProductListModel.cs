// ---------------------------------------------------------------------
// <copyright file="ProductListModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines ProductListModel class.
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
namespace Sitecore.Commerce.StarterKit.Models
{
  using System.Collections.Generic;
  using Sitecore;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Defines ProductListModel class.
  /// </summary>
  public class ProductListModel
  {
    /// <summary>
    /// Defines set of products
    /// </summary>
    private readonly IEnumerable<ProductModel> products;

    private readonly ProductPagingModel paging;

    private readonly ProductSortingModel sorting;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductListModel"/> class.
    /// </summary>
    /// <param name="products">The products.</param>
    public ProductListModel([NotNull] IEnumerable<ProductModel> products, [NotNull] ProductPagingModel paging, [NotNull] ProductSortingModel sorting)
    {
      Assert.ArgumentNotNull(products, "products");
      Assert.ArgumentNotNull(paging, "paging");
      Assert.ArgumentNotNull(sorting, "sorting");

      this.products = products;
      this.paging = paging;
      this.sorting = sorting;
    }

    /// <summary>
    /// Gets the set of products.
    /// </summary>
    /// <value>
    /// The set of products.
    /// </value>
    [NotNull]
    public IEnumerable<ProductModel> Products
    {
      get { return this.products; }
    }

    [NotNull]
    public ProductPagingModel Paging
    {
      get { return this.paging; }
    }

    [NotNull]
    public ProductSortingModel Sorting
    {
      get { return this.sorting; }
    }
  }
}