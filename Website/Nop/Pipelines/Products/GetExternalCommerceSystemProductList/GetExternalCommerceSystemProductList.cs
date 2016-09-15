// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetExternalCommerceSystemProductList.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the pipeline processor that gets product list from external commerce system.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.GetExternalCommerceSystemProductList
{
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Pipelines;

  /// <summary>
  /// Defines the pipeline processor that gets product list from external commerce system.
  /// </summary>
  public class GetExternalCommerceSystemProductList : NopProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// Executes the business logic of the GetExternalCommerceSystemProductList processor.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      using (var nopServiceClient = this.GetClient())
      {
        var productIds = nopServiceClient.GetAllProductsIds();
        args.Request.Properties["ExternalCommerceSystemProductIds"] = productIds ?? Enumerable.Empty<string>();
        nopServiceClient.Close();
      }      
    }
  }
}
