// ----------------------------------------------------------------------------------------------
// <copyright file="FakeAuthenticationProvider.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The fake authentication provider.
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
namespace Sitecore.Commerce.StarterKit.Tests.Services
{
  using Sitecore.Security.Accounts;
  using Sitecore.Security.Authentication;

  /// <summary>
  /// The fake authentication provider.
  /// </summary>
  public class FakeAuthenticationProvider : AuthenticationProvider
  {
    /// <summary>
    /// Logs in the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>True value.</returns>
    public override bool Login(User user)
    {
      return true;
    }

    /// <summary>
    /// Logs the specified user into the system.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="persistent">If set to <c>true</c> (and the provider supports it), the login will be persisted.</param>
    /// <returns>
    ///   <c>true</c> if user was logged in. Otherwise <c>false</c>.
    /// </returns>
    public override bool Login(string userName, string password, bool persistent)
    {
      return true;
    }

    /// <summary>
    /// Logs the specified user into the system without checking password.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="persistent">If set to <c>true</c> (and the provider supports it), the login will be persisted.</param>
    /// <returns>
    ///   <c>true</c> if user was logged in. Otherwise <c>false</c>.
    /// </returns>
    public override bool Login(string userName, bool persistent)
    {
      return true;
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    public override void Logout()
    {
    }

    /// <summary>
    /// Sets the active user.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    public override void SetActiveUser(string userName)
    {
    }

    /// <summary>
    /// Sets the active user.
    /// </summary>
    /// <param name="user">The user object.</param>
    public override void SetActiveUser(User user)
    {
    }
  }
}