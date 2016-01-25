// ----------------------------------------------------------------------------------------------
// <copyright file="DeleteCart.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The delete cart.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.DeleteCart
{
  using System;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;

  /// <summary>
  /// The processor that deletes specified cart in NopCommerce e-commerce system via the WCF service.
  /// </summary>
  public class DeleteCart : NopProcessor<ICartsServiceChannel>
  {
    /// <summary>
    /// Performs delete cart operation.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (CartRequestWithCart)args.Request;

      // Creates instance of WCF service client.
      using (var client = this.GetClient())
      {
        // Deletes cart by id on NopCommerce side via WCF service.
        client.DeleteCart(Guid.Parse(request.Cart.ExternalId));
      }
    }
  }
}
