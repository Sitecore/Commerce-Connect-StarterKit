// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Provides basic customer management operations.
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
namespace Sitecore.Commerce.StarterKit.Services
{
    using Sitecore;

    /// <summary>
    /// Provides basic customer management operations.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Checks if a user exists.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>True if the user exists, otherwise false.</returns>
        bool IsUserExist([NotNull] string userName);

        /// <summary>
        /// Registers the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="shopName">Name of the shop.</param>
        void Register([NotNull]string userName, [NotNull]string password, [NotNull] string email, [NotNull]string shopName);

        /// <summary>
        /// Logins the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="persistent">if set to <c>true</c> the session is persistent.</param>
        /// <returns>
        ///   <c>true</c> if login attempt succeeded, otherwise <c>false</c>.
        /// </returns>
        bool Login([NotNull]string userName, [NotNull]string password, bool persistent);

        /// <summary>
        /// Logouts the current user.
        /// </summary>
        void Logout();
    }
}