// ----------------------------------------------------------------------------------------------
// <copyright file="UpdateParties.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The UpdateParties class.
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
using System;
using System.Collections.Generic;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Commerce.Connectors.NopCommerce.NopCustomersService;
using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Diagnostics;

namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Customers
{
    public class UpdateParties : NopProcessor<ICustomersServiceChannel>
    {
        public IEntityFactory EntityFactory { get; private set; }

        public UpdateParties (IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
        }

        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var addresses = new List<AddressModel>();
            Guid customerId;
            
            var request = (UpdatePartiesRequest)args.Request;
            var result = (CustomerResult)args.Result;

            using (ICustomersServiceChannel channel = base.GetClient())
            {

                try
                {
                    customerId = new Guid(request.CommerceCustomer.ExternalId);
                }
                catch
                {
                    result.Success = false;
                    result.SystemMessages.Add(new SystemMessage { Message = "Cannot parse customer Guid " + request.CommerceCustomer.ExternalId });
                    return;
                }

                foreach (var party in request.Parties)
                {
                    addresses.Add(new AddressModel
                    {
                        Id = party.ExternalId,
                        FirstName = party.FirstName,
                        LastName = party.LastName,
                        Email = party.Email,
                        Address1 = party.Address1,
                        Address2 = party.Address2,
                        City = party.City,
                        Company = party.Company,
                        CountryThreeLetterIsoCode = (party.Country != null && party.Country.Length == 3) ? party.Country : string.Empty,
                        CountryTwoLetterIsoCode = (party.Country != null && party.Country.Length == 2) ? party.Country : string.Empty,
                        FaxNumber = "",
                        PhoneNumber = party.PhoneNumber,
                        StateProvinceAbbreviation = party.State,
                        ZipPostalCode = party.ZipPostalCode
                    });
                }

                var  nopResponse = channel.UpdateAddresses(customerId, addresses.ToArray());
                
                if (nopResponse.Success)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.SystemMessages.Add(new SystemMessage { Message = "Error occuder while updating parties in Cusomer : " + nopResponse.Message });
                }
            }

        }
    }
}
