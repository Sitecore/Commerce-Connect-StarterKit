// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductTypes.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product types.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductTypes
{
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Products;

  /// <summary>
  /// The read external commerce system product types.
  /// </summary>
  public class ReadExternalCommerceSystemProductTypes : NopProductProcessor<IProductsServiceChannel>
  {
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
        var productTypeModels = nopServiceClient.GetProductTypes(request.ProductId);
        args.Request.Properties["ProductTypeIds"] = productTypeModels.Select(m => m.Id).ToList();
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

      args.Request.Properties["ProductTypeIds"] = product.ProductTypes.Select(t => t.ExternalId);
    }
  }
}