// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateUser.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the CreateUser class.</summary>
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
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Customers;
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Connectors.NopCommerce.NopCustomersService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Entities;
    using Commerce.Entities.Customers;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;/// <summary>
                                     /// Processor that creates a user in NOPCommerce and inserts the user Id into the CreateUserRequest property bag.
                                     /// </summary>
    public class CreateUser : NopProcessor<ICustomersServiceChannel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUser" /> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public CreateUser(IEntityFactory entityFactory)
        {
            this.EntityFactory = entityFactory;
        }

        /// <summary>
        /// Gets the entity factory.
        /// </summary>
        public IEntityFactory EntityFactory { get; private set; }

        /// <summary>
        /// Executes the business logic of the CreateUser processor.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            CreateUserRequest request = (CreateUserRequest)args.Request;
            CreateUserResult result = (CreateUserResult)args.Result;

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
                    FirstName = request.UserName,
                    LastName = request.UserName,
                    Company = request.Shop.Name,
                    Active = true
                };

                channel.CreateCustomer(customerId, customerModel);
                request.Properties["UserId"] = customerId;

                var commerceUser = this.EntityFactory.Create<CommerceUser>("CommerceUser");
                commerceUser.ExternalId = customerId.ToString();
                commerceUser.UserName = request.UserName;
                commerceUser.Email = request.Email;
                commerceUser.Shops = new ReadOnlyCollection<string>(new List<string> { request.Shop.Name });
                commerceUser.CreationDate = DateTime.UtcNow;

                result.CommerceUser = commerceUser;
            }
        }
    }
}
