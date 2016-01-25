// ----------------------------------------------------------------------------------------------
// <copyright file="RequireAdminAttribute.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The require https attribute.
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
namespace Sitecore.Commerce.StarterKit.Filters
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Web.Http.Controllers;
  using System.Web.Http.Filters;
  using Sitecore.Diagnostics;

  /// <summary>
  /// The require https attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  public class RequireAdminAttribute : AuthorizationFilterAttribute
  {
    /// <summary>
    /// The is administrator.
    /// </summary>
    private bool? isAdministrator;

    /// <summary>
    /// Gets or sets a value indicating whether [is administrator].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [is administrator]; otherwise, <c>false</c>.
    /// </value>
    public bool IsAdministrator
    {
      get
      {
        return this.isAdministrator == null ? Context.User.IsAdministrator : this.isAdministrator.Value;
      }

      set
      {
        this.isAdministrator = value;
      }
    }

    /// <summary>
    /// Calls when an action is being authorized.
    /// </summary>
    /// <param name="actionContext">The context.</param>
    public override void OnAuthorization(HttpActionContext actionContext)
    {
      Assert.ArgumentNotNull(actionContext, "actionContext");
      if (!this.IsAdministrator)
      {
        actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, Texts.PermissionIsDenied);
      }
    }
  }
}