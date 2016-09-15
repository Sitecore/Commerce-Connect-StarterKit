// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubmitOrder.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the SubmitOrder class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Orders.SubmitOrder
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Commerce.Connectors.NopCommerce.NopOrdersService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Orders;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Orders;
  using Sitecore.Diagnostics;

  /// <summary>
  ///   Submit an order.
  /// </summary>
  public class SubmitOrder : NopProcessor<IOrdersServiceChannel>
  {
    /// <summary>
    ///   Performs submit order operation.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (SubmitVisitorOrderRequest)args.Request;
      var result = (SubmitVisitorOrderResult)args.Result;

      // Creates instance of WCF service client.
      using (var client = this.GetClient())
      {
        if (request.Cart.ExternalId != null)
        {
          var shipping = request.Cart.Shipping.First(); // TODO: nopCommerce does not support multi-shipment and multi-payment in one order. In future we can submit separate orders for multi-information from Sitecore.
          var payment = request.Cart.Payment.First();
          var shippingDic = shipping.Properties.ToDictionary(item => item.Key, item => item.Value);
          var paymentDic = payment.Properties.ToDictionary(item => item.Key, item => item.Value);

          var dictionary = new Dictionary<string, string>();
          foreach (var pair in shippingDic)
          {
            dictionary.Add(pair.Key, pair.Value.ToString());
          }

          foreach (var pair in paymentDic)
          {
            dictionary.Add(pair.Key, pair.Value.ToString());
          }

          var orderModel = client.SubmitOrder(Guid.Parse(request.Cart.ExternalId), shipping.ShippingMethodID, payment.PaymentMethodID, dictionary);
          if (orderModel == null)
          {
            return;
          }

          var order = new Order
          {
            ExternalId = orderModel.Id.ToString(),
            ShopName = request.Cart.ShopName
          };

          order.MapOrderFromModel(orderModel);

          result.Order = order;
        }
      }
    }
  }
}