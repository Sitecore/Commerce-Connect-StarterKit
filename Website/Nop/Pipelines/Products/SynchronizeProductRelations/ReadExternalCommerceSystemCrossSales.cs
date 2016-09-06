// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemCrossSales.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Reads external commerce system cross sales.
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
// ---------------------------------------------------------------------
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductRelations
{
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;

  /// <summary>
  /// Reads external commerce system cross sales.
  /// </summary>
  public class ReadExternalCommerceSystemCrossSales : ReadExternalCommerceSystemProductRelationsBase
  {
    /// <summary>
    /// Gets the name of the relation type.
    /// </summary>
    /// <value>
    /// The name of the relation type.
    /// </value>
    protected override string RelationTypeName
    {
      get { return "Cross-sells"; }
    }

    /// <summary>
    /// Gets the product ids.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>Related ids.</returns>
    protected override string[] GetRelatedIds(IProductsServiceChannel client, string externalProductId)
    {
      return client.GetCrossSellProductsIds(externalProductId);
    }
  }
}