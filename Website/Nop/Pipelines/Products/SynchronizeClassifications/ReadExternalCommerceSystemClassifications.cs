// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemClassifications.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the pipeline processor that gets all product categories from external commerce system.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeClassifications
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;

  /// <summary>
  /// Defines the pipeline processor that gets all product categories from external commerce system.
  /// </summary>
  public class ReadExternalCommerceSystemClassifications : ReadExternalCommerceSystemProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// The product classification group name
    /// </summary>
    private readonly string productClassificationGroupName;

    /// <summary>
    /// The product classification group external id
    /// </summary>
    private readonly string productClassificationGroupExternalId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemClassifications" /> class.
    /// </summary>
    /// <param name="productClassificationGroupName">Name of the product classification group.</param>
    /// <param name="productClassificationGroupExternalId">The product classification group external id.</param>
    public ReadExternalCommerceSystemClassifications([NotNull] string productClassificationGroupName, [NotNull] string productClassificationGroupExternalId)
    {
      Assert.IsNotNullOrEmpty(productClassificationGroupName, "productClassificationGroupName");
      Assert.IsNotNullOrEmpty(productClassificationGroupName, "productClassificationGroupExternalId");

      this.productClassificationGroupName = productClassificationGroupName;
      this.productClassificationGroupExternalId = productClassificationGroupExternalId;
    }

    /// <summary>
    /// Processes the specified args.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      using (var nopServiceClient = this.GetClient())
      {
        var categories = nopServiceClient.GetAllCategories();

        var productClassifications =
          categories.Select(
            x => 
              {
                Classification classification = this.InstantiateEntity<Classification>();
                classification.ExternalId = x.Id;
                classification.ExternalParentId = x.ParentCategoryId;
                classification.Description = x.Description;
                classification.Name = x.Name;
                classification.Created = x.CreatedOnUtc;
                classification.Updated = x.UpdatedOnUtc;
                return classification;
              }).ToList();
        
        ClassificationGroup classificationGroup = this.InstantiateEntity<ClassificationGroup>();

        classificationGroup.Classifications = productClassifications.AsReadOnly();
        classificationGroup.Name = this.productClassificationGroupName;
        classificationGroup.ExternalId = this.productClassificationGroupExternalId;
        classificationGroup.Updated = DateTime.UtcNow;

        args.Request.Properties["ClassificationGroups"] = new List<ClassificationGroup>
        {
          classificationGroup
        };
        nopServiceClient.Close();
      }
    }
  }
}