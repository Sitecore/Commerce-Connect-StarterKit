// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProduct.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the pipeline processor that reads product from external commerce system.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductEntity
{
  using System.Collections.Generic;
  using Sitecore.Commerce.Products;
  using System.Linq;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;

  /// <summary>
  /// Defines the pipeline processor that reads product from external commerce system.
  /// </summary>
  public class ReadExternalCommerceSystemProduct : ReadExternalCommerceSystemProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// The sku identification key.
    /// </summary>
    private const string SkuIdentificationKey = "Sku";

    /// <summary>
    /// The product classification group name
    /// </summary>
    private readonly string productClassificationGroupName;

    /// <summary>
    /// The product classification group external id
    /// </summary>
    private readonly string productClassificationGroupExternalId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemProduct" /> class.
    /// </summary>
    /// <param name="productClassificationGroupName">Name of the product classification group.</param>
    /// <param name="productClassificationGroupExternalId">The product classification group external id.</param>
    public ReadExternalCommerceSystemProduct([NotNull] string productClassificationGroupName, [NotNull] string productClassificationGroupExternalId)
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

      var request = (ProductSynchronizationRequest)args.Request;

      var language = Language.Parse(request.Language).CultureInfo.TwoLetterISOLanguageName;

      using (var nopServiceClient = this.GetClient())
      {
        var productModel = nopServiceClient.GetProduct(request.ProductId, language);

        if (productModel != null && productModel.ProductId != null && !productModel.Deleted && !string.IsNullOrEmpty(productModel.ProductVariantId))
        {
          var product = this.InstantiateEntity<Product>();

          this.FillProduct(product, productModel);

          //Todo : Revert to original strongly typed objects
          args.Request.Properties["Product"] = product;
        }
        else if (productModel != null && (productModel.ProductId == null || productModel.Deleted))
        {
          if (!request.Direction.Equals(Direction.Outbound))
          {
            args.Request.Properties["ProductToDelete"] = request.ProductId;
          }
        }
        else
        {
          var message = new SystemMessage
          {
            Message = string.Format(Commerce.Texts.FailedToSynchronizeProduct0, request.ProductId)
          };
          args.Result.SystemMessages.Add(message);
          args.AbortPipeline();
        }
        nopServiceClient.Close();
      }
    }

    /// <summary>
    /// Fills the product.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <param name="productModel">The product model.</param>
    protected virtual void FillProduct(Product product, ProductModel productModel)
    {
      var classificationGroup = this.InstantiateEntity<ClassificationGroup>();

      classificationGroup.Name = this.productClassificationGroupName;
      classificationGroup.ExternalId = this.productClassificationGroupExternalId;
      classificationGroup.Classifications = productModel.CategoryIds.Select(x =>
      {
        var classification = this.InstantiateEntity<Classification>();
        classification.ExternalId = x;

        return classification;
      }).ToList().AsReadOnly();


      product.ExternalId = productModel.ProductVariantId;
      product.Name = productModel.Name;
      product.ShortDescription = productModel.ShortDescription;
      product.FullDescription = productModel.FullDescription;
      product.Manufacturers = productModel.ManufacturerIds.Select(m => new Manufacturer
      {
        ExternalId = m
      }).ToList().AsReadOnly();
      product.Created = productModel.CreatedOnUtc;
      product.Updated = productModel.UpdatedOnUtc;
      product.ClassificationGroups = (new List<ClassificationGroup>
      {
        classificationGroup
      }).AsReadOnly();
      if (!string.IsNullOrEmpty(productModel.Sku))
      {
        product.Identification[SkuIdentificationKey] = productModel.Sku;
      }
      product.ProductTypes = productModel.CategoryIds.Select(x =>
      {
        var type = this.InstantiateEntity<ProductType>();
        type.ExternalId = x;
        return type;
      }).ToList().AsReadOnly();

      if (productModel.ProductGlobalSpecifications != null && productModel.ProductGlobalSpecifications.Any())
      {
        product.ProductSpecifications = productModel.ProductGlobalSpecifications.Select(sm =>
        {
          var specification = this.InstantiateEntity<Specification>();

          specification.ExternalId = sm.SpecificationLookupId;
          specification.Key = sm.SpecificationLookupName;
          specification.Value = sm.LookupValueName;

          return specification;
        }).ToList().AsReadOnly();
      }
    }
  }
}