//-----------------------------------------------------------------------
// <copyright file="AddPartiesToCart.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The AddPartiesToCart class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.AddParties
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
   * This method was developed only to ASSIGN customer parties to customer as billing (accounting) and shipping (buyer) party.
   * Nop Commerce does not support adding parties to a cart
   * In case, you want to add new address to cart - please use Customer.AddParties pipeline instead.
   * Parties collection in Request is not in use - only Cart.AccountingCustomerParty and Cart.BuyerCustomerParty are took into account.
   */

  public class AddPartiesToCart : NopProcessor<ICartsServiceChannel>
  {
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var addresses = new List<CustomerAddressModel>();
      Guid customerId;

      var request = (AddPartiesRequest)args.Request;
      var result = (AddPartiesResult)args.Result;

      // Add a billing (accounting) party
      var billingParty = request.Cart.AccountingCustomerParty;
      if (billingParty != null)
      {
        int externalId;
        if (int.TryParse(billingParty.ExternalId, out externalId))
        {
          addresses.Add(new CustomerAddressModel { AddressType = AddressTypeModel.Billing, Id = billingParty.ExternalId, Name = billingParty.Name });
        }
      }

      // Add a shipping (buyer) party
      var shippingParty = request.Cart.BuyerCustomerParty;
      if (shippingParty != null)
      {
        int externalId;
        if (int.TryParse(shippingParty.ExternalId, out externalId))
        {
          addresses.Add(new CustomerAddressModel { AddressType = AddressTypeModel.Shipping, Id = shippingParty.ExternalId, Name = shippingParty.Name });
        }
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
          result.SystemMessages.Add(new SystemMessage { Message = "Cannot parse customer Guid " + request.Cart.ExternalId });
          return;
        }

        try
        {
          var nopResponse = channel.AddAddresses(addresses.ToArray(), customerId);

          if (nopResponse.Success)
          {
            result.Success = true;
          }
          else
          {
            result.Success = false;
            result.SystemMessages.Add(new SystemMessage { Message = "Error occuder while assigning shipping and billing parties to Cusomer : " + nopResponse.Message });
          }
        }
        catch (Exception)
        {
          result.Success = false;
          result.SystemMessages.Add(new SystemMessage { Message = "Communication error, while adding parties to Cusomer " + request.Cart.ExternalId });
        }
      }

      result.Cart = request.Cart;
    }
  }
}