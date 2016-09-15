// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveProductToExternalCommerceSystem.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The save product to external commerce system.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductEntity
{
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Products;

  /// <summary>
  /// The save product to external commerce system.
  /// </summary>
  public class SaveProductToExternalCommerceSystem : NopProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// The sku identification key.
    /// </summary>
    private const string SkuIdentificationKey = "Sku";

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    public override void Process(ServicePipelineArgs args)
    {
       Assert.ArgumentNotNull(args, "args");

      var request = (ProductSynchronizationRequest)args.Request;

      var language = Language.Parse(request.Language).CultureInfo.TwoLetterISOLanguageName;

      var product = (Product)args.Request.Properties["Product"];

      using (var nopServiceClient = this.GetClient())
      {
        var productModel = new ProductModel();
        this.FillProductModel(productModel, product);

        nopServiceClient.UpdateProduct(productModel, language);
      }
    }

    /// <summary>
    /// Fills the product model.
    /// </summary>
    /// <param name="productModel">The product model.</param>
    /// <param name="product">The product.</param>
    protected virtual void FillProductModel(ProductModel productModel, Product product)
    {
      productModel.ProductId = product.ExternalId;
      productModel.Name = product.Name;
      productModel.ShortDescription = product.ShortDescription;
      productModel.FullDescription = product.FullDescription;
      productModel.Sku = product.Identification.ContainsKey(SkuIdentificationKey) ? product.Identification[SkuIdentificationKey] : string.Empty;
    }
  }
}