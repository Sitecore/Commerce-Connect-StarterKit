// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductManufacturers.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product manufacturers.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductManufacturers
{
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Products;

  /// <summary>
  /// Defines the processor that reads the product manufacturers from the external commerce system.
  /// This processor stores the manufacturer IDs in the custom data.
  /// </summary>
  public class ReadExternalCommerceSystemProductManufacturers : NopProductProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// Runs the processor.
    /// </summary>
    /// <param name="args">The arguments.</param>
    protected override void GetProductDataFromService(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (ProductSynchronizationRequest)args.Request;

      using (var nopServiceClient = this.GetClient())
      {
        var manufacturerModels = nopServiceClient.GetManufacturer(request.ProductId);
        args.Request.Properties["ManufacturerIds"] = manufacturerModels.Select(m => m.Id);
        nopServiceClient.Close();
      }
    }

    /// <summary>
    /// Sets the output data.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="product">The product.</param>
    protected override void SetDataFromSavedProduct(ServicePipelineArgs args, Product product)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(product, "product");

      args.Request.Properties["ManufacturerIds"] = product.Manufacturers.Select(m => m.ExternalId);
    }
  }
}