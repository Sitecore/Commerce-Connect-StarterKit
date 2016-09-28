// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommerceProductsPermissionProvider.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the CommerceProductsPermissionProvider class.</summary>
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
namespace Nop.Plugin.Sitecore.Commerce.Products.Security
{
    using System.Collections.Generic;
    using System.Linq;
    using Nop.Core.Domain.Security;
    using Nop.Services.Security;

    /// <summary>
    /// The products permission provider.
    /// </summary>
    public class CommerceProductsPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord AccessWebService = new PermissionRecord { Name = "Plugins. Access Synchronize Service", SystemName = "AccessSitecoreCommerceProductsService", Category = "Plugin" };

        /// <summary>
        /// Gets the permissions.
        /// </summary>
        /// <returns>The permission records.</returns>
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                AccessWebService,
            };
        }

        /// <summary>
        /// Gets the default permissions.
        /// </summary>
        /// <returns>The default permission records.</returns>
        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return Enumerable.Empty<DefaultPermissionRecord>();

            //uncomment code below in order to give appropriate permissions to admin by default
            //return new[] 
            //{
            //    new DefaultPermissionRecord 
            //    {
            //        CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
            //        PermissionRecords = new[] 
            //        {
            //            AccessWebService,
            //        }
            //    },
            //};
        }
    }
}