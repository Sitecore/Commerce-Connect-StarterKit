// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddCustomerParties.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the AddCustomerParties class.</summary>
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

namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Customers
{
    using System;
    using System.Collections.Generic;
    using Sitecore.Commerce.Connectors.NopCommerce.NopCustomersService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Customers;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The add customer parties processor.
    /// </summary>
    public class AddCustomerParties : NopProcessor<ICustomersServiceChannel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCustomerParties"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public AddCustomerParties(IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
        }

        /// <summary>
        /// Gets the entity factory.
        /// </summary>
        public IEntityFactory EntityFactory { get; private set; }

        /// <summary>
        /// Processes the arguments.
        /// </summary>
        /// <param name="args">The pipeline arguments.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var addresses = new List<AddressModel>();
            Guid customerId;

            var request = (AddCustomerPartiesRequest)args.Request;
            var result = (CustomerPartiesResult)args.Result;

            using (ICustomersServiceChannel channel = this.GetClient())
            {
                try
                {
                    customerId = new Guid(request.CommerceCustomer.ExternalId);
                }
                catch
                {
                    result.Success = false;
                    result.SystemMessages.Add(new SystemMessage
                    {
                        Message = "Cannot parse customer Guid " + request.CommerceCustomer.ExternalId
                    });
                    return;
                }

                /*  need to develop Plugin for this type of request
                        foreach (var party in request.Parties)
                        {
                            addresses.Add(new AddressModel
                            {
                                Id = party.ExternalId,
                                Name = party.Name,
                                Type = party.Type
                            });
                        }
                               
                           nopResponse = channel.AddAddressesByCustomerId(customerId, addresses.ToArray());
                
                        if (nopResponse.Success)
                        {
                            request.Properties.Add("UserId", customerId);
                            result.Success = true;
                            result.SystemMessages.Add(new SystemMessage { Message = "Successfully added parties to Nop Commerce System" });
                        }
                        else
                        {
                            result.Success = false;
                            result.SystemMessages.Add(new SystemMessage { Message = "Error occuder while adding parties to Cusomer : " + nopResponse.Message });
                        }
                        */
            }
        }
    }
}