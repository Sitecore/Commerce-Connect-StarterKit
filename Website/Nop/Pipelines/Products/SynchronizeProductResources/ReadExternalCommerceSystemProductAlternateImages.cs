// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductAlternateImages.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product alternate images.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductResources
{
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;

  /// <summary>
  /// The read external commerce system product alternate images.
  /// </summary>
  public class ReadExternalCommerceSystemProductAlternateImages : ReadExternalCommerceSystemProductResourceBase
  {
    /// <summary>
    /// Gets the resource.
    /// </summary>
    /// <param name="nopServiceClient">The service client.</param>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>
    /// Product resource model.
    /// </returns>
    protected override ProductResourceModel GetResource(IProductsServiceChannel nopServiceClient, string externalProductId)
    {
      return nopServiceClient.GetProductAlternateImages(externalProductId);
    }
  }
}