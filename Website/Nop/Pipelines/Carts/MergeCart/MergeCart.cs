// --------------------------------------------------------------------
// <copyright file="MergeCart.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   MergeCart class
// </summary>
// --------------------------------------------------------------------
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.MergeCart
{
  using System;

  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;
  using Sitecore.Diagnostics;

  public class MergeCart : NopProcessor<ICartsServiceChannel>
  {
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (MergeCartRequest)args.Request;

      if (request.FromCart != null && request.ToCart != null)
      {
        Guid fromCartId;
        Guid toCartId;

        if (Guid.TryParse(request.FromCart.ExternalId, out fromCartId)
            && Guid.TryParse(request.ToCart.ExternalId, out toCartId))
        {
          using (var client = this.GetClient())
          {
            client.MigrateShoppingCart(fromCartId, toCartId, false);
          }
        }
      }
    }
  }
}
