// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveCart.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The save cart.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.Common
{
  using System;
  using System.Linq;
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;

  /// <summary>
  /// The processor that saves cart to NopCommerce e-commerce system via the WCF service.
  /// </summary>
  public class SaveCart : NopProcessor<ICartsServiceChannel>
  {
    /// <summary>
    /// The GUID format.
    /// </summary>
    private const string GuidFormat = "B";

    /// <summary>
    /// Performs save cart operation.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      // Creates instance of WCF service client.
      using (var client = this.GetClient())
      {
        // Gets current cart from request.
        var cart = ((CartRequestWithCart)args.Request).Cart;

        Guid cartId;
        if (!Guid.TryParse(cart.ExternalId, out cartId) && !Guid.TryParse(cart.UserId, out cartId))
        {
          cartId = Guid.NewGuid();
        }

        cart.ExternalId = cartId.ToString(GuidFormat).ToUpper();

        var cartModel = client.CreateCart(cartId);
        if (cartModel != null)
        {
          args.Request.Properties["CartId"] = cartModel.CustomerGuid;
        }

        //Guid id = Guid.Parse(cart.ExternalId);

        //// Delete cart if the one exists.
        //client.DeleteCart(id);

        //// Saves cart to the instance of NopCommerce system via the WCF service.
        //var cartModel = client.CreateCart(id);
        
        //if (cartModel != null)
        //{
        //  // Adds all specified cart lines to cart on NopCommerce instance side.
        //  cart.Lines.Aggregate(cartModel, (current, cartLine) => cartLine.Product != null ? client.AddProduct(new Guid(cart.ExternalId), cartLine.Product.ProductId, cartLine.Quantity) : null);
        //}
      }
    }
  }
}