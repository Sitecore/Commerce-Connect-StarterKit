﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemDivisions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system divisions.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeDivisions
{
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;

  /// <summary>
  /// The read external commerce system divisions.
  /// </summary>
  public class ReadExternalCommerceSystemDivisions : ReadExternalCommerceSystemProcessor<IProductsServiceChannel>
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
        var nopDivisions = nopServiceClient.GetAllDivisions();
        var divisions = nopDivisions.Select(x =>
        {
          var division = this.InstantiateEntity<Division>();
          
          division.ExternalId = x.Id;
          division.Description = x.Description;
          division.Name = x.Name;
          division.Created = x.CreatedOnUtc;
          division.Updated = x.UpdatedOnUtc;
          
          return division;
        }).ToList();

        args.Request.Properties["Divisions"] = divisions;
        nopServiceClient.Close();
      }
    }
  }
}