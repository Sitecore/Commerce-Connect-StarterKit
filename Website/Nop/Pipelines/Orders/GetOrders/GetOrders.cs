//-----------------------------------------------------------------------
// <copyright file="GetOrders.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The GetOrders class.</summary>
//-----------------------------------------------------------------------
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Orders.GetOrders
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using Sitecore.Commerce.Connectors.NopCommerce.NopOrdersService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Orders;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Orders;
  using Sitecore.Diagnostics;

  /// <summary>
  ///   The get orders.
  /// </summary>
  public class GetOrders : NopProcessor<IOrdersServiceChannel>
  {
    /// <summary>
    ///   Performs get order operation.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process([NotNull] ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (GetVisitorOrdersRequest)args.Request;
      var result = (GetVisitorOrdersResult)args.Result;

      // Creates instance of WCF service client.
      using (var client = this.GetClient())
      {
        var ordersHeaders = new List<OrderHeader>();
        var orders = new ReadOnlyCollection<OrderHeader>(ordersHeaders);
        result.OrderHeaders = orders;

        var ordersModels = client.GetOrders(Guid.Parse(request.CustomerID), request.ShopName);
        if (ordersModels == null)
        {
          return;
        }

        foreach (var model in ordersModels)
        {
          var header = new OrderHeader();
          header.MapOrderHeaderFromModel(model);
          ordersHeaders.Add(header);
        }
      }
    }
  }
}