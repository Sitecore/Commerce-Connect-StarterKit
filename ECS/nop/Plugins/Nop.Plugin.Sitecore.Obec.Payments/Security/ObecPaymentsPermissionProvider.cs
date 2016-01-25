// -----------------------------------------------------------------
// <copyright file="ObecCustomersPermissionProvider.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The ObecCustomersPermissionProvider class.
// </summary>
// -----------------------------------------------------------------
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
namespace Nop.Plugin.Sitecore.Commerce.Payments.Security
{
  using System.Collections.Generic;
  using System.Linq;
  using Core.Domain.Security;
  using Services.Security;

  public class ObecCustomersPermissionProvider : IPermissionProvider
  {
    public static readonly PermissionRecord AccessWebService = new PermissionRecord { Name = "Plugins. Access Payments Service", SystemName = "AccessSitecoreObecPaymentsService", Category = "Plugin" };

    /// <summary>
    /// Gets the permissions.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<PermissionRecord> GetPermissions()
    {
      return new[] 
            {
                AccessWebService
            };
    }

    /// <summary>
    /// Gets the default permissions.
    /// </summary>
    /// <returns></returns>
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