// ---------------------------------------------------------------------
// <copyright file="ProductExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The product extensions.
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
namespace Sitecore.Commerce.StarterKit.Helpers
{
  using App_Start;
  using Services;
  using Sitecore.Data.Items;
  using Sitecore.Commerce.StarterKit.Models;
  using Sitecore.Diagnostics;

  /// <summary>
  /// The product extensions.
  /// </summary>
  public static class ProductExtensions
  {
    /// <summary>
    /// The product resources name.
    /// </summary>
    private const string ProductResourcesName = "Resources";

    /// <summary>
    /// The product type field name.
    /// </summary>
    private const string ProductTypeFieldName = "Type";

    /// <summary>
    /// The set product resource.
    /// </summary>
    /// <param name="productModel">The product model.</param>
    /// <param name="productService">The product service.</param>
    /// <param name="productItem">The product item.</param>
    /// <returns>
    /// The <see cref="ProductModel" />.
    /// </returns>
    public static ProductModel SetProductResource(this ProductModel productModel, IProductService productService, Item productItem)
    {
      Assert.ArgumentNotNull(productModel, "productModel");
      Assert.ArgumentNotNull(productService, "productService");

      productModel.Images = productService.GetImages(productItem);
      productModel.Image = productService.GetImage(productItem);

      return productModel;
    }
  }
}
