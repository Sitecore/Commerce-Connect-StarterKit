// ----------------------------------------------------------------------------------------------
// <copyright file="RemovePartiesFromCart.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The RemovePartiesFromCart class.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.RemoveParties
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Carts;
  using Sitecore.Diagnostics;

  /*
 * This method was developed only to DEassign billing (accounting) and shipping (buyer) customer parties FROM customer.
 * Nop Commerce does not support adding/removing parties to a cart
 * In case, you want to physically remove address t - please use Customer.RemoveParties pipeline instead.
 * Parties collection in Request is not in use - only Cart.AccountingCustomerParty and Cart.BuyerCustomerParty are took into account.
 */

  public class RemovePartiesFromCart : NopProcessor<ICartsServiceChannel>
  {
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var addresses = new List<CustomerAddressModel>();
      Guid customerId;

      var request = (RemovePartiesRequest)args.Request;
      var result = (RemovePartiesResult)args.Result;
      
      // Add a billing (accounting) party
      if (request.Cart.AccountingCustomerParty == null)
      {
        addresses.Add(new CustomerAddressModel() { AddressType = AddressTypeModel.Billing, Id = "0" });
      }

      // Add a shipping (buyer) party
      if (request.Cart.BuyerCustomerParty == null)
      {
        addresses.Add(new CustomerAddressModel() { AddressType = AddressTypeModel.Shipping, Id = "0" });
      }

      if (addresses.Count > 0)
      {
          result.Success = true;
          result.SystemMessages.Add(new SystemMessage { Message = string.Format("No parties to remove from customer Guid:{0}", request.Cart.ExternalId) });
          return;
      }

      using (ICartsServiceChannel channel = base.GetClient())
      {
        try
        {
          customerId = new Guid(request.Cart.ExternalId);
        }
        catch
        {
          result.Success = false;
          result.SystemMessages.Add(new SystemMessage { Message = string.Format("Cannot parse customer Guid:{0}", request.Cart.ExternalId) });
          return;
        }

        try
        {
          var nopResponse = channel.RemoveAddresses(addresses.ToArray(), customerId);

          if (nopResponse.Success)
          {
            result.Success = true;
          }
          else
          {
            result.Success = false;
            result.SystemMessages.Add(new SystemMessage { Message = string.Format("Error occuder while removing shipping or billing party from Cusomer : {0}", nopResponse.Message) });
          }
        }
        catch (Exception)
        {
          result.Success = false;
          result.SystemMessages.Add(new SystemMessage { Message = string.Format("Communication error, while removing parties from Cusomer Guid:{0}", request.Cart.ExternalId) });
        }
      }
    }
  }
}
