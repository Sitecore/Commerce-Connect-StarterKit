// ----------------------------------------------------------------------------------------------
// <copyright file="RemoveLinesFromCart.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The remove lines from cart.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.RemoveCartLines
{
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.Common;
  using Sitecore.Commerce.Entities.Carts;

  /// <summary>
  /// Processor that removes specified cart lines from cart in instance of NopCommerce e-commerce system via the WCF service.
  /// </summary>
  public class RemoveLinesFromCart : ChangeLinesProcessor
  {
    /// <summary>
    /// Changes the lines.
    /// </summary>
    /// <param name="cartModel">The cart model.</param>
    /// <param name="client">The client.</param>
    /// <param name="cartLine">The cart line.</param>
    /// <returns>The changed. cart model.</returns>
    protected override ShoppingCartModel ChangeLines(ShoppingCartModel cartModel, ICartsServiceChannel client, CartLine cartLine)
    {
      if (cartLine.Product != null)
      {
        cartModel = client.RemoveProduct(cartModel.CustomerGuid, cartLine.Product.ProductId);
      }

      return cartModel;
    }
  }
}