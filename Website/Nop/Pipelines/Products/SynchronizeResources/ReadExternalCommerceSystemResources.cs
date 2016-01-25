// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemResources.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system resources.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeResources
{
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;

  /// <summary>
  /// The read external commerce system resources.
  /// </summary>
  public class ReadExternalCommerceSystemResources : ReadExternalCommerceSystemProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// Processes the specified args.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      using (var nopServiceClient = this.GetClient())
      {
        var resources = nopServiceClient.GetAllResources();

        args.Request.Properties["Resources"] = resources.Select(x =>
        {
          var resource = this.InstantiateEntity<Resource>();
          
          resource.ExternalId = x.Id;
          resource.Name = x.Name;
          resource.MimeType = x.MimeType;
          resource.Created = x.CreatedOnUtc;
          resource.Updated = x.UpdatedOnUtc;
          resource.BinaryData = x.BinaryData;
          resource.ResourceType = x.ResourceType;

          return resource;
        }).ToList();
        nopServiceClient.Close();
      }
    }
  }
}