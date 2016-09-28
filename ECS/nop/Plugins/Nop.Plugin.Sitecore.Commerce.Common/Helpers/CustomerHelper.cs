// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerHelper.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the CustomerHelper class.</summary>
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
// -----------------------------------------------------------------
namespace Nop.Plugin.Sitecore.Commerce.Common.Helpers
{
    using System;
    using Nop.Core;
    using Nop.Core.Domain.Customers;
    using Nop.Services.Customers;

    /// <summary>
    /// Class that contains customer helper methods.
    /// </summary>
    public static class CustomerHelper
    {
        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <param name="customerService">The customer service.</param>
        /// <returns>Instance of <see cref="Customer"/></returns>
        /// <exception cref="Nop.Core.NopException">'Guests' role could not be loaded</exception>
        public static Customer CreateCustomer(Guid customerId, ICustomerService customerService)
        {
            // Creates new instance of customer.
            var customer = new Customer
            {
                CustomerGuid = customerId,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            // Gets guest role.
            var guestRole = customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
            if (guestRole == null)
            {
                throw new NopException("'Guests' role could not be loaded");
            }

            // Adds guest role to customer.
            customer.CustomerRoles.Add(guestRole);

            // Saves new customer.
            customerService.InsertCustomer(customer);

            return customer;
        }
    }
}