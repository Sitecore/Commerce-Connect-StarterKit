// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemovePaymentInfoFromCart.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the RemovePaymentInfoFromCart class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.RemovePaymentInfo
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Commerce.Entities.Carts;
  using Commerce.Services;
  using Commerce.Services.Carts;
  using Diagnostics;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Pipelines.Common;

  /// <summary>
  /// Defines the RemovePaymentInfoFromCart processor.
  /// </summary>
  public class RemovePaymentInfoFromCart : NopProcessor<ICartsServiceChannel>
  {
    /// <summary>
    /// Processes the specified arguments.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Request, "args.Request");
      Assert.ArgumentCondition(args.Request is RemovePaymentInfoRequest, "args.Request", "args.Request is RemovePaymentInfoRequest");
      Assert.ArgumentCondition(args.Result is RemovePaymentInfoResult, "args.Result", "args.Result is RemovePaymentInfoResult");

      var request = (RemovePaymentInfoRequest)args.Request;
      var result = (RemovePaymentInfoResult)args.Result;

      Assert.ArgumentNotNull(request.Cart, "request.Cart");
      Assert.ArgumentNotNullOrEmpty(request.Cart.ExternalId, "request.Cart.ExternalId");
      Assert.ArgumentNotNull(request.Payments, "request.Payments");

      result.Cart = request.Cart;
      result.Payments = request.Payments;
      
      var paymentMethod = request.Payments.FirstOrDefault();

      if (paymentMethod != null)
      {
        var paymentInfoModel = new PaymentInfoModel
        {
          MethodName = paymentMethod.PaymentMethodID,
          SystemName = paymentMethod.PaymentProviderID
        };

        using (var client = this.GetClient())
        {
          var response = client.RemovePaymentInfo(new Guid(request.Cart.ExternalId), paymentInfoModel, request.Cart.ShopName);

          result.Success = response.Success;

          if (response.Success)
          {
            result.Payments = new List<PaymentInfo>(0);
            result.Cart.Payment = new List<PaymentInfo>(0).AsReadOnly();
          }

          if (!string.IsNullOrEmpty(response.Message))
          {
            result.SystemMessages.Add(new SystemMessage { Message = response.Message });
          }
        }
      }
    }
  }
}
