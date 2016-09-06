// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductRelationsBase.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Base class for reading external commerce system product relations.
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
  using System.Collections.ObjectModel;
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Products;

  /// <summary>
  /// Base class for reading external commerce system product relations.
  /// </summary>
  public abstract class ReadExternalCommerceSystemProductRelationsBase : ReadExternalCommerceSystemProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// Gets the name of the relation type.
    /// </summary>
    /// <value>
    /// The name of the relation type.
    /// </value>
    [NotNull]
    protected abstract string RelationTypeName { get; }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      var request = (ProductSynchronizationRequest)args.Request;

      var relatedProducts = args.Request.Properties["RelatedProducts"] as Collection<RelationType>;
      if (relatedProducts == null)
      {
        relatedProducts = new Collection<RelationType>();
        args.Request.Properties["RelatedProducts"] = relatedProducts;
      }

      using (var client = this.GetClient())
      {
        var relationTypeName = this.RelationTypeName;

        var relatedProductIds = this.GetRelatedIds(client, request.ProductId);

        if (!relatedProductIds.Any())
        {
          return;
        }

        var relationType = this.InstantiateEntity<RelationType>();

        relationType.Name = relationTypeName;
        relationType.Relations = relatedProductIds.Select(id =>
        {
          var relation = this.InstantiateEntity<Relation>();
          relation.ExternalId = id;
          return relation;
        }).ToList().AsReadOnly();

        relatedProducts.Add(relationType);
        client.Close();
      }
    }

    /// <summary>
    /// Gets the related ids.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>Related ids.</returns>
    [NotNull]
    protected abstract string[] GetRelatedIds(IProductsServiceChannel client, string externalProductId);
  }
}