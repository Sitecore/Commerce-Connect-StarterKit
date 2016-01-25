//-----------------------------------------------------------------------
// <copyright file="GetPaymentMethods.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The GetPaymentMethods class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Payments.GetPaymentMethods
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using Sitecore.Commerce.Connectors.NopCommerce.NopPaymentService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities.Payments;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Payments;
  using Sitecore.Diagnostics;
  using Sitecore.Xml;

  public class GetPaymentMethods : NopProcessor<IPaymentServiceChannel>
  {
    /// <summary>
    /// Map collection
    /// </summary>
    protected readonly HashSet<Tuple<int, string, string>> PaymentCollectin;

    public GetPaymentMethods()
    {
      this.PaymentCollectin = new HashSet<Tuple<int, string, string>>();
    }

    public override void Process([NotNull]ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Request, "args.Request");
      Assert.ArgumentNotNull(args.Result, "args.Result");

      var request = (GetPaymentMethodsRequest)args.Request;
      var result = (GetPaymentMethodsResult)args.Result;

      Assert.ArgumentNotNull(request.PaymentOption, "request.PaymentOption");

      using (var client = this.GetClient())
      {
        try
        {
          var response = client.GetPaymentMethods(request.PaymentOption.ShopName);
          
          result.Success = response.Success;

          var paymentMethods = new List<PaymentMethod>(0);
          foreach (var paymentMethodModel in response.Result)
          {
            if (this.PaymentCollectin.Contains(Tuple.Create(request.PaymentOption.PaymentOptionType.Value, paymentMethodModel.SystemName, paymentMethodModel.MethodName)))
            {
              paymentMethods.Add(new PaymentMethod()
              {
                Name = paymentMethodModel.MethodName,
                Description = paymentMethodModel.MethodName,
                ExternalId = paymentMethodModel.SystemName,
              });
            }
          }

          result.PaymentMethods = paymentMethods.AsReadOnly();

          if (!string.IsNullOrEmpty(response.Message))
          {
            result.SystemMessages.Add(new SystemMessage() { Message = response.Message });
          }

        }
        catch (Exception e)
        {
          result.Success = false;
          result.SystemMessages.Add(new SystemMessage() { Message = e.Message });
        }
      }
    }

    /// <summary>
    /// Map data from configuration
    /// </summary>
    /// <param name="configNode"></param>
    public virtual void Map(XmlNode configNode)
    {
      Assert.ArgumentNotNull(configNode, "configNode");

      string shippingOptionValue = XmlUtil.GetAttribute("paymentOptionValue", configNode);
      string systemName = XmlUtil.GetAttribute("systemName", configNode);
      string methodName = XmlUtil.GetAttribute("methodName", configNode);

      Assert.IsNotNull(shippingOptionValue, "paymentOptionValue");
      Assert.IsNotNull(systemName, "systemName");
      Assert.IsNotNull(methodName, "methodName");

      this.PaymentCollectin.Add(Tuple.Create(int.Parse(shippingOptionValue), systemName, methodName));
    }
  }
}