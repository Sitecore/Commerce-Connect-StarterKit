﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserInfoModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the UserInfoModel class.</summary>
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
namespace Sitecore.Commerce.StarterKit.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Sitecore.Commerce.Entities.Orders;

    /// <summary>
    /// Represents user information.
    /// </summary>
    public class UserInfoModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoModel"/> class.
        /// </summary>
        public UserInfoModel()
        {
            this.Orders = new Collection<OrderHeader>();
        }

        /// <summary>
        /// Gets or sets the orders.
        /// </summary>
        public IReadOnlyCollection<OrderHeader> Orders { get; set; }
    }
}