// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemTypes.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system types.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeTypes
{
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Pipelines;
  using ProductType = Sitecore.Commerce.Entities.Products.ProductType;

  /// <summary>
  /// The read external commerce system types.
  /// </summary>
  public class ReadExternalCommerceSystemTypes : ReadExternalCommerceSystemProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// Runs the processor.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      using (var nopServiceClient = this.GetClient())
      {
        var nopTypes = nopServiceClient.GetAllProductTypes();
        var types = nopTypes.Select(x =>
        {
          var type = this.InstantiateEntity<ProductType>();

          type.ExternalId = x.Id;
          type.Description = x.Description;
          type.Name = x.Name;
          type.ProductTypeId = x.Id;
          type.ParentProductTypeId = x.ParentProductTypeId;
          type.Created = x.CreatedOnUtc;
          type.Updated = x.UpdatedOnUtc;

          return type;
        }).ToList();

        args.Request.Properties["ProductTypes"] = types;
        nopServiceClient.Close();
      }
    }
  }
}