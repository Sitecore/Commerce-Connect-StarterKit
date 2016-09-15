// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateContactInXDb.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The update customer in external system.
// </summary>
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

namespace Sitecore.Commerce.StarterKit.Pipelines
{
    using Sitecore.Analytics.Model.Entities;
    using Sitecore.Commerce.Data.Customers;
    using Sitecore.Commerce.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Creates the Contact for a Sitecore user
    /// </summary>
    public class CreateContactInXDb : Sitecore.Commerce.Pipelines.Customers.CreateContact.CreateContactInXDb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateContactInXDb" /> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="entityFactory">The entity factory.</param>
        public CreateContactInXDb(IUserRepository userRepository, IEntityFactory entityFactory)
            : base(userRepository, entityFactory)
        {
        }

        /// <summary>
        /// Adds some basic facets to the Contact
        /// </summary>
        /// <param name="contact">The Contact to add facets to</param>
        /// <param name="createUserResult">The result containing details about the created user</param>
        protected override void AddFacets(Analytics.Tracking.Contact contact, Commerce.Services.Customers.CreateUserResult createUserResult)
        {
            base.AddFacets(contact, createUserResult);

            var personalInfo = contact.GetFacet<IContactPersonalInfo>("Personal");
            personalInfo.FirstName = createUserResult.CommerceUser.UserName;
        }
    }
}