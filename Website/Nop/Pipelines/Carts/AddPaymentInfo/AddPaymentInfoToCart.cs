// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddPaymentInfoToCart.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the AddPaymentInfoToCart class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.AddPaymentInfo
{
    using Commerce.Entities.Carts;
    using Commerce.Pipelines;
    using Commerce.Services;
    using Commerce.Services.Carts;
    using Diagnostics;
    using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
    using Pipelines.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the AddPaymentInfoToCart processor.
    /// </summary>
    public class AddPaymentInfoToCart : NopProcessor<ICartsServiceChannel>
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentCondition(args.Request is AddPaymentInfoRequest, "args.Request", "args.Request is AddPaymentInfoRequest");
            Assert.ArgumentCondition(args.Result is AddPaymentInfoResult, "args.Result", "args.Result is AddPaymentInfoResult");

            var request = (AddPaymentInfoRequest)args.Request;
            var result = (AddPaymentInfoResult)args.Result;

            var resultPaymentInfo = new List<PaymentInfo>(0);
            var paymentMethod = request.Payments.FirstOrDefault();

            if (paymentMethod != null)
            {
                Guid customerId;
                if (Guid.TryParse(request.Cart.ExternalId, out customerId))
                {
                    var paymentInfoModel = new PaymentInfoModel()
                    {
                        MethodName = paymentMethod.PaymentMethodID,
                        SystemName = paymentMethod.PaymentProviderID
                    };

                    using (var client = this.GetClient())
                    {
                        var response = client.AddPaymentInfo(new Guid(request.Cart.ExternalId), paymentInfoModel, request.Cart.ShopName);

                        result.Success = response.Success;
                        if (response.Result != null)
                        {
                            resultPaymentInfo.Add(new PaymentInfo { PaymentMethodID = response.Result.MethodName, PaymentProviderID = response.Result.SystemName });
                        }

                        if (!string.IsNullOrEmpty(response.Message))
                        {
                            result.SystemMessages.Add(new SystemMessage { Message = response.Message });
                        }
                    }
                }
            }

            result.Payments = resultPaymentInfo.AsReadOnly();
            result.Cart = request.Cart;
            result.Cart.Payment = resultPaymentInfo.AsReadOnly();
        }
    }
}