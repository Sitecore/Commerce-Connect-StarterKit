// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveShippingInfoFromCart.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the RemoveShippingInfoFromCart class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.RemoveShippingInfo
{
    using System;
    using System.Collections.Generic;

    using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The remove shipping info from cart processor.
    /// </summary>
    public class RemoveShippingInfoFromCart : NopProcessor<ICartsServiceChannel>
    {
        /// <summary>
        /// Processes the arguments.
        /// </summary>
        /// <param name="args">The pipeline arguments.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentNotNull(args.Result, "args.Result");
            Assert.ArgumentCondition(args.Request is RemoveShippingInfoRequest, "args.Request", "args.Request is RemoveShippingInfoRequest");
            Assert.ArgumentCondition(args.Result is RemoveShippingInfoResult, "args.Result", "args.Result is RemoveShippingInfoResult");

            var request = (RemoveShippingInfoRequest)args.Request;
            var result = (RemoveShippingInfoResult)args.Result;

            Guid customerId;
            if (Guid.TryParse(request.Cart.ExternalId, out customerId))
            {
                using (var client = this.GetClient())
                {
                    var responseModel = client.RemoveShippingInfo(customerId, request.Cart.ShopName);
                    if (responseModel.Success)
                    {
                        var empty = new List<ShippingInfo>(0);

                        request.Cart.Shipping = empty.AsReadOnly();
                        result.ShippingInfo = empty;

                        result.Cart = request.Cart;
                    }
                }
            }
        }
    }
}