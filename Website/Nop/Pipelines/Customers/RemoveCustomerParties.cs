// ----------------------------------------------------------------------------------------------
// <copyright file="RemoveCustomerParties.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The RemoveCustomerParties class.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Customers
{
  using System;
  using System.Linq;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCustomersService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Customers;
  using Sitecore.Diagnostics;

  public class RemoveCustomerParties : NopProcessor<ICustomersServiceChannel>
    {
        public IEntityFactory EntityFactory { get; private set; }

        public RemoveCustomerParties(IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
        }

        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            RemoveCustomerPartiesRequest request = (RemoveCustomerPartiesRequest)args.Request;
            CustomerPartiesResult result = (CustomerPartiesResult) args.Result; //- there is no RemovePartiesResult in Services.Customers

            using (ICustomersServiceChannel channel = base.GetClient())
            {
                Guid customerId;
                try
                {
                    customerId = new Guid(request.CommerceCustomer.ExternalId);
                }
                catch
                {
                    customerId = Guid.Empty;
                }

                var addressIds = request.Parties.Select(party => party.ExternalId).ToArray();

                //var nopResult = channel.RemoveCustomerAddressesByCusomerId(customerId, addressIds.ToArray());

                //if (nopResult.Success)
                //{
                //    request.Properties.Add("UserId", customerId);
                //    result.Success = true;
                //    result.SystemMessages.Add(new SystemMessage { Message = "Successfully added parties to Nop Commerce System" });
                //}
                //else
                //{
                //    result.Success = false;
                //    result.SystemMessages.Add(new SystemMessage { Message = "Error occuder while adding parties to Cusomer " + request.CommerceCustomer.ExternalId });
                //}
                 
            }
        }
    }
}
