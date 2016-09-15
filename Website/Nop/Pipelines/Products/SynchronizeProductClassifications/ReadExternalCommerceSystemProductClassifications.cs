// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductClassifications.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product classifications.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductClassifications
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Products;

  /// <summary>
  /// Defines the read external commerce system product classifications.
  /// NopCommerce stores stores the classification groups in the custom data.
  /// In the SynchronizeClassifications pipeline, the ResolveClassificationsChanges processor uses the classification groups stored in custom data.
  /// </summary>
  public class ReadExternalCommerceSystemProductClassifications : NopProductProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// The product classification group name
    /// </summary>
    private readonly string productClassificationGroupName;

    /// <summary>
    /// The product classification group external ID
    /// </summary>
    private readonly string productClassificationGroupExternalId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemProductClassifications"/> class.
    /// </summary>
    /// <param name="productClassificationGroupName">
    /// The product classification group name.
    /// </param>
    /// <param name="productClassificationGroupExternalId">
    /// The product classification group external ID
    /// </param>
    public ReadExternalCommerceSystemProductClassifications([NotNull] string productClassificationGroupName, [NotNull] string productClassificationGroupExternalId)
    {
      Assert.IsNotNullOrEmpty(productClassificationGroupName, "productClassificationGroupName");
      Assert.IsNotNullOrEmpty(productClassificationGroupName, "productClassificationGroupExternalId");

      this.productClassificationGroupName = productClassificationGroupName;
      this.productClassificationGroupExternalId = productClassificationGroupExternalId;
    }

    /// <summary>
    /// Gets the product data from service.
    /// </summary>
    /// <param name="args">The arguments.</param>
    protected override void GetProductDataFromService(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (ProductSynchronizationRequest)args.Request;

      using (var nopServiceClient = this.GetClient())
      {
        var categoryModels = nopServiceClient.GetCategories(request.ProductId);
        var classificationGroup = this.InstantiateEntity<ClassificationGroup>();

        classificationGroup.Name = this.productClassificationGroupName;
        classificationGroup.ExternalId = this.productClassificationGroupExternalId;
        classificationGroup.Classifications = categoryModels.Select(category =>
        {
          var classification = this.InstantiateEntity<Classification>();
          classification.ExternalId = category.Id;

          return classification;
        }).ToList().AsReadOnly();

        args.Request.Properties["ClassificationGroups"] = new List<ClassificationGroup>
        {
          classificationGroup
        };
        nopServiceClient.Close();
      }
    }

    /// <summary>
    /// Sets the data from saved product.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="product">The product.</param>
    protected override void SetDataFromSavedProduct(ServicePipelineArgs args, Product product)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(product, "product");

      args.Request.Properties["ClassificationGroups"] = product.ClassificationGroups;
    }
  }
}