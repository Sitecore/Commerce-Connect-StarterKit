// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductDivisions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product divisions.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductDivisions
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Products;

  /// <summary>
  /// The read external commerce system product divisions.
  /// </summary>
  public class ReadExternalCommerceSystemProductDivisions : NopProcessor<IProductsServiceChannel>
  {
    /// <summary>
    /// Runs the processor.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      var request = (ProductSynchronizationRequest)args.Request;

      using (var nopServiceClient = this.GetClient())
      {
        var productDivisionsModel = nopServiceClient.GetRelatedDivisions(request.ProductId);
        if (productDivisionsModel != null && productDivisionsModel.Divisions != null)
        {
          var divisionIds = productDivisionsModel.Divisions.Select(d => d.Id).ToList();
          args.Request.Properties["DivisionIds"] = divisionIds;
        }
        else
        {
          args.Request.Properties["DivisionIds"] = new List<string>();
        }
        nopServiceClient.Close();
      }
    }
  }
}