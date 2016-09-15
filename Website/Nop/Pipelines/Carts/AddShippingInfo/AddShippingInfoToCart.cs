// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddShippingInfoToCart.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the AddShippingInfoToCart class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.AddShippingInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The add shipping info to cart processor.
    /// </summary>
    public class AddShippingInfoToCart : NopProcessor<ICartsServiceChannel>
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
            Assert.ArgumentCondition(args.Request is AddShippingInfoRequest, "args.Request", "args.Request is AddShippingInfoRequest");
            Assert.ArgumentCondition(args.Result is AddShippingInfoResult, "args.Result", "args.Result is AddShippingInfoResult");

            var request = (AddShippingInfoRequest)args.Request;
            var result = (AddShippingInfoResult)args.Result;

            var resultShipping = new List<ShippingInfo>();

            var shippingInfo = request.ShippingInfo.FirstOrDefault();
            if (shippingInfo != null)
            {
                Guid customerId;
                if (Guid.TryParse(request.Cart.ExternalId, out customerId))
                {
                    var model = new ShippingMethodModel
                    {
                        SystemName = shippingInfo.ShippingProviderID,
                        Name = shippingInfo.ShippingMethodID
                    };

                    using (var client = this.GetClient())
                    {
                        var responseModel = client.AddShippingInfo(customerId, model, request.Cart.ShopName);
                        if (responseModel.Success)
                        {
                            resultShipping.Add(shippingInfo);
                        }
                    }
                }

                request.Cart.Shipping = resultShipping.AsReadOnly();
                result.ShippingInfo = resultShipping;

                result.Cart = request.Cart;
            }
        }
    }
}