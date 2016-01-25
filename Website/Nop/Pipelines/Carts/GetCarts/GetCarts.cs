// ----------------------------------------------------------------------------------------------
// <copyright file="GetCarts.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetCarts type.
// </summary>
// ----------------------------------------------------------------------------------------------
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.GetCarts
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;

  /// <summary>
  /// The get carts.
  /// </summary>
  public class GetCarts : NopProcessor<ICartsServiceChannel>
  {
    /// <summary>
    /// Runs the processor.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      // Prepare request/result objects
      var request = (GetCartsRequest)args.Request;
      var result = (GetCartsResult)args.Result;

      // Creates instance of WCF service client.
      using (var client = this.GetClient())
      {
        var cartModels = client.GetCarts();
        if (!cartModels.Any())
        {
          // Returns empty collection if there are no carts.
          result.Carts = Enumerable.Empty<CartBase>();
          return;
        }

        var carts = new List<CartBase>();

        // Maps cart lines from NopCommerce cart model to OBEC cart.
        foreach (var cartModel in cartModels)
        {
          var cart = new CartBase();

          cart.MapCartBaseFromModel(cartModel);

          carts.Add(cart);
        }

        if (request.UserIds.Any())
        {
          // ToDo:
          // filter by user id here in some way
        }

        result.Carts = carts;
      }
    }
  }
}