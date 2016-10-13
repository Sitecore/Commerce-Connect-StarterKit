// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The implementation of CustomerModel.
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
// -----------------------------------------------------------------
namespace Sitecore.Commerce.Nop.Common.Models
{
    using System;

    /// <summary>
    /// custommer model.
    /// </summary>
    public class CustomerModel
    {
        /// <summary>
        /// Gets or sets the customer unique identifier.
        /// </summary>
        /// <value>
        /// The customer unique identifier.
        /// </value>
        public Guid CustomerGuid { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>
        /// The company.
        /// </value>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [active]; otherwise, <c>false</c>.
        /// </value>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [deleted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [deleted]; otherwise, <c>false</c>.
        /// </value>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the created configuration UTC.
        /// </summary>
        /// <value>
        /// The created configuration UTC.
        /// </value>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        /// <value>
        /// The billing address.
        /// </value>
        public virtual AddressModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        /// <value>
        /// The shipping address.
        /// </value>
        public virtual AddressModel ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is registered].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is registered]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsRegistered { get; set; }

        /// <summary>
        /// Gets or sets the admin content.
        /// </summary>
        public string AdminContent { get; set; }
    }
}