//-----------------------------------------------------------------------
// <copyright file="GetParties.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The GetParties class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Customers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCustomersService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Customers;
  using Sitecore.Diagnostics;

  public class GetParties : NopProcessor<ICustomersServiceChannel>
  {
      public IEntityFactory EntityFactory { get; private set; }

      public GetParties(IEntityFactory entityFactory)
      {
          this.EntityFactory = entityFactory;
      }

      public override void Process(ServicePipelineArgs args)
      {
          Assert.ArgumentNotNull(args, "args");
            
          Guid customerId;
            
          var request = (GetPartiesRequest) args.Request;
          var result = (GetPartiesResult) args.Result;

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
              try
              {
                  var nopResponse = channel.GetAllAddresses(customerId);

                  if (nopResponse.Success)
                  {
                      result.Parties = nopResponse.Result.Select(address => new Party
                      {
                          Address1 = address.Address1,
                          Address2 = address.Address2,
                          City = address.City,
                          Company = address.Company,
                          Email = address.Email,
                          Country = address.CountryTwoLetterIsoCode,
                          ExternalId = address.Id,
                          FirstName = address.FirstName,
                          LastName = address.LastName,
                          PhoneNumber = address.PhoneNumber,
                          State = address.StateProvinceAbbreviation,
                          ZipPostalCode = address.ZipPostalCode
                      }).ToArray();

                      result.Success = true;
                  }
                  else
                  {
                      result.Success = false;
                      result.SystemMessages.Add(new SystemMessage { Message = "Error getting Parties: " + nopResponse.Message });
                  }
              }
              catch (Exception)
              {
                  result.Success = false;
                  result.SystemMessages.Add(new SystemMessage { Message = "Communication error, while connecting to NOP [Get Parties]" });
              }
          }
      }
  }
}
