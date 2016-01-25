//-----------------------------------------------------------------------
// <copyright file="Default.aspx.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The AddParties class.</summary>
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
using System;
using System.Collections.Generic;
using Sitecore.Commerce.Connectors.NopCommerce.NopCustomersService;
using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Diagnostics;

namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Customers
{
  using System.Linq;

  public class AddParties : NopProcessor<ICustomersServiceChannel>
    {
        public IEntityFactory EntityFactory { get; private set; }

        public AddParties(IEntityFactory entityFactory)
        {
            this.EntityFactory = entityFactory;
        }

        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            
            var addresses = new List<AddressModel>();
            Guid customerId;

            var request = (AddPartiesRequest)args.Request;
            var result = (AddPartiesResult)args.Result;

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
                        CountryTwoLetterIsoCode = party.Country, //TODO: make sure it is twoo letter code
                        CountryThreeLetterIsoCode = "",
                        FaxNumber = "",
                        PhoneNumber = party.PhoneNumber,
                        StateProvinceAbbreviation = party.State,
                        ZipPostalCode = party.ZipPostalCode
                    });
                }

                try
                {
                  var nopResponse = channel.AddAddresses(customerId, addresses.ToArray());

                  if (nopResponse.Success)
                  {
                      request.Properties.Add("UserId", customerId);
                      result.Success = true;
                      result.Parties = nopResponse.Result.Select(address => new Party
                      {
                        Address1 = address.Address1,
                        Address2 = address.Address2,
                        City = address.City,
                        Company = address.Company,
                        Email = address.Email,
                        Country = address.CountryThreeLetterIsoCode,
                        ExternalId = address.Id,
                        FirstName = address.FirstName,
                        LastName = address.LastName,
                        PhoneNumber = address.PhoneNumber,
                        State = address.StateProvinceAbbreviation,
                        ZipPostalCode = address.ZipPostalCode,
                        PartyId = address.Id
                      }).ToArray();
                  }
                  else
                  {
                      result.Success = false;
                      result.SystemMessages.Add(new SystemMessage { Message = "Error occuder while adding parties to Cusomer : " + nopResponse.Message });
                  }
                }
                catch (Exception)
                {
                    result.Success = false;
                    result.SystemMessages.Add(new SystemMessage { Message = "Communication error, while adding parties to Cusomer " + request.CommerceCustomer.ExternalId });
                }
                
            }

        }
    }
}
