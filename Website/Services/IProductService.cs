// ----------------------------------------------------------------------------------------------
// <copyright file="IProductService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Provides basic product management operations.
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
namespace Sitecore.Commerce.StarterKit.Services
{
  using System.Collections.Generic;
  using Sitecore;
  using Sitecore.Commerce.StarterKit.Models;
  using Sitecore.ContentSearch.Utilities;
  using Sitecore.Data;
  using Sitecore.Data.Items;

  /// <summary>
  /// Provides basic product management operations.
  /// </summary>
  public interface IProductService
  {
    /// <summary>
    /// The read product.
    /// </summary>
    /// <param name="productId">The product id.</param>
    /// <returns>The <see cref="Item" />.</returns>
    [CanBeNull]
    Item ReadProduct([NotNull] string productId);

    /// <summary>
    /// Gets the products.
    /// </summary>
    /// <param name="searchStringModel">The search string model.</param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>The list of the product items.</returns>
    [NotNull]
    IEnumerable<Item> GetProducts(IEnumerable<SearchStringModel> searchStringModel);

    [NotNull]
    PagedList<Item> GetProducts(IEnumerable<SearchStringModel> searchStringModel, int page, int pageSize);

    /// <summary>
    /// Get category ID by external id
    /// </summary>
    /// <param name="categoryExternalId">The category external id</param>
    /// <returns>Category ID</returns>
    [CanBeNull]
    Item GetCategory([NotNull] string categoryExternalId);

    /// <summary>
    /// Get related categories IDs
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Category IDs</returns>
    [NotNull]
    List<Item> GetRelatedCategories([NotNull] ID categoryId);

    /// <summary>
    /// Get Resources items by product id  
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [CanBeNull]
    List<Item> GetResources([NotNull] string productId);

    /// <summary>
    /// Get Resources items by product item
    /// </summary>
    /// <param name="productItem"></param>
    /// <returns></returns>
    [CanBeNull]
    List<Item> GetResources([NotNull] Item productItem);

    /// <summary>
    /// Get Image by product item
    /// </summary>
    /// <param name="productItem"></param>
    /// <returns></returns>
    [NotNull]
    string GetImage([NotNull] Item productItem);

    /// <summary>
    /// Get Images by product item
    /// </summary>
    /// <param name="productItem"></param>
    /// <returns></returns>
    [NotNull]
    List<string> GetImages([NotNull] Item productItem);
  }
}