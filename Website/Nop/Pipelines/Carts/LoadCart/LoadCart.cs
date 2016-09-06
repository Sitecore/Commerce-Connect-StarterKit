// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadCart.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The load cart.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.LoadCart
{
    using System;
    using System.Linq;
    using Sitecore;
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Commerce.Data.Products;

    /// <summary>
    /// The processor that loads cart from NopCommerce e-commerce system via the WCF service.
    /// </summary>
    public class LoadCart : NopProcessor<ICartsServiceChannel>
    {
        /// <summary>
        /// Performs load cart operation.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process([NotNull] ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var request = (LoadCartRequest)args.Request;

            // Gets id of cart to load from NopCommerce instance.
            var cartId = request.CartId;

            // Creates instance of WCF service client.
            using (var client = this.GetClient())
            {
                Guid nopCartId;
                if (!Guid.TryParse(cartId, out nopCartId))
                {
                    var idGenerator = new Md5IdGenerator();
                    nopCartId = Guid.Parse(idGenerator.StringToID(cartId, string.Empty).ToString());
                }

                // Check that NopCommerce instance contains cart with specified id and if not stops loading operation.
                var cartModel = client.GetCart(nopCartId, request.Shop.Name);
                if (cartModel != null)
                {
                    // Creates OBEC cart with specified id.
                    var cart = new Cart
                    {
                        ExternalId = cartId
                    };

                    // Maps cart lines from NopCommerce cart model to OBEC cart.
                    cart.MapCartFromModel(cartModel);

                    // Sets loaded cart to result property.
                    ((CartResult)args.Result).Cart = cart;
                }
            }
        }
    }
}