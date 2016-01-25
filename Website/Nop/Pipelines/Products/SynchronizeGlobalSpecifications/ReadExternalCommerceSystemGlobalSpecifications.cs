// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemGlobalSpecifications.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system global specifications.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeGlobalSpecifications
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;

  /// <summary>
  /// The read external commerce system global specifications.
  /// </summary>
  public class ReadExternalCommerceSystemGlobalSpecifications : ReadExternalCommerceSystemProcessor<IProductsServiceChannel>
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
        var specifications = nopServiceClient.GetAllGlobalSpecifications();

        args.Request.Properties["Specifications"] = specifications.Select(x =>
        {
          GlobalSpecification specification = this.InstantiateEntity<GlobalSpecification>();
          specification.ExternalId = x.Id;
          specification.Name = x.Name;
          specification.Created = x.CreatedOnUtc;
          specification.Updated = x.UpdatedOnUtc;
          specification.Options = x.SpecificationOptions != null ? x.SpecificationOptions.Select(o =>
          {
            GlobalSpecificationOption globalSpecificationOption = this.InstantiateEntity<GlobalSpecificationOption>();
            
            globalSpecificationOption.ExternalId = o.Id;
            globalSpecificationOption.ExternalSpecificationId = o.ParentId;
            globalSpecificationOption.Name = o.Name;
            globalSpecificationOption.Created = o.CreatedOnUtc;
            globalSpecificationOption.Updated = o.UpdatedOnUtc;

            return globalSpecificationOption;
          }).ToList().AsReadOnly() : (new List<GlobalSpecificationOption>()).AsReadOnly();
          return specification;
        }).ToList();
        nopServiceClient.Close();
      }
    }
  }
}