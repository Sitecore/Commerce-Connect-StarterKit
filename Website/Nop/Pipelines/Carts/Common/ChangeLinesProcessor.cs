// ----------------------------------------------------------------------------------------------
// <copyright file="ChangeLinesProcessor.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The change lines processor.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.Common
{
  using System;
  using System.Linq;
  using Newtonsoft.Json;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;

  /// <summary>
  /// The change lines processor.
  /// </summary>
  public abstract class ChangeLinesProcessor : NopProcessor<ICartsServiceChannel>
  {
    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      // Getting current cart from request field.
      var request = (CartLinesRequest)args.Request;

      // make deep copy of the request cart
      var type = request.Cart.GetType();
      var cart = (Cart)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(request.Cart), type);

      // Do not add cart lines if request field doesn't contain any.
      if (request.Lines.Any())
      {
        // Creates instance of WCF service client.
        using (var client = this.GetClient())
        {
          // Check that NopCommerce instance contains cart with specified id and if not stop adding cart lines.
          var cartModel = client.GetCart(Guid.Parse(cart.ExternalId),cart.ShopName);
          if (cartModel != null)
          {
            // Adds all specified cart lines to cart on NopCommerce instance side.
            cartModel = request.Lines.Aggregate(cartModel, (current, cartLine) => this.ChangeLines(current, client, cartLine));

            // Maps fields from updated cart model to OBEC cart.
            cart.MapCartFromModel(cartModel);

            // Update pricing information in the request so it can be used in other pipelines
            foreach (var requestLine in request.Lines)
            {
              if (requestLine.Product != null && requestLine.Product.Price == null)
              {
                var cartLine = cart.Lines.FirstOrDefault(l => l.Product.ProductId == requestLine.Product.ProductId);
                if (cartLine != null)
                {
                  requestLine.Product.Price = cartLine.Product.Price;
                }
              }
            }
          }
        }
      }

      // Set updated cart to result field.
      ((CartResult)args.Result).Cart = cart;
    }

    /// <summary>
    /// Changes the lines.
    /// </summary>
    /// <param name="cartModel">The cart model.</param>
    /// <param name="client">The client.</param>
    /// <param name="cartLine">The cart line.</param>
    /// <returns>The </returns>
    protected abstract ShoppingCartModel ChangeLines(ShoppingCartModel cartModel, ICartsServiceChannel client, CartLine cartLine);
  }
}