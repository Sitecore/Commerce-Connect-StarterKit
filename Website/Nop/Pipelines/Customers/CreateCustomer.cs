// ----------------------------------------------------------------------------------------------
// <copyright file="CreateCustomer.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The CreateCustomer class inheriting NopProcessor<ICustomersServiceChannel>.
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
using Sitecore.Commerce.Services;

namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Customers
{
  using Diagnostics;
  using NopCustomersService;
  using Common;
  using Commerce.Pipelines;
  using System;
  using Commerce.Entities;
  using Commerce.Services.Customers;


  /// <summary>
  /// Processor that creates a customer in NOPCommerce and inserts the customer Id into the CreateCustomerRequest property bag.
  /// </summary>
  public class CreateCustomer : NopProcessor<ICustomersServiceChannel>
  {
    /// <summary>
    /// The entity factory.
    /// </summary>
    public IEntityFactory EntityFactory { get; private set; }

    /// <summary>
    /// Initializes an instance of the <see cref="CreateCustomer" /> class.
    /// </summary>
    /// <param name="entityFactory">The entity factory.</param>
    public CreateCustomer(IEntityFactory entityFactory)
    {
      this.EntityFactory = entityFactory;
    }

    /// <summary>
    /// Executes the business logic of the CreateCustomer processor.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var request = (CreateCustomerRequest)args.Request;
      var result = (CreateCustomerResult)args.Result;

      using (ICustomersServiceChannel channel = this.GetClient())
      {
        Guid customerId;
        if (!request.Properties.Contains("UserId") || !Guid.TryParse(request.Properties["UserId"].ToString(), out customerId))
        {
          customerId = Guid.NewGuid();
        }
        
        CustomerModel customerModel = new CustomerModel
        {
          CustomerGuid = customerId,
          FirstName = request.CommerceCustomer.Name,
          LastName = request.CommerceCustomer.Name,
          //BillingAddress = new AddressModel(),
          //ShippingAddress = new AddressModel(),
          Active = true
        };


        var nopResponse = channel.CreateCustomer(customerId, customerModel);

        if (nopResponse.IsRegistered)
        {
          // Add customer ID to request to create same customer in Sitecore
          request.Properties.Add("UserId", customerId);

          result.Success = true;
        }
        else
        {
          request.Properties.Remove("UserId");

          result.Success = false;
          result.SystemMessages.Add(new SystemMessage { Message = "Error Creating Customer in NopCommerce " });
        }
      }
    }
  }
}
