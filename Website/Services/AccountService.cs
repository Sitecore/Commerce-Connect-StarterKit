// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Provides default implementation of basic customer management operations.
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
  using System;
  using System.Web;
  using System.Web.Security;

  using Sitecore.Analytics;
  using Sitecore.Commerce.Services.Customers;
  using Sitecore.Diagnostics;
  using Sitecore.Security.Authentication;
    using Sitecore.Commerce.Automation.MarketingAutomation;

  /// <summary>
    /// Provides default implementation of basic customer management operations.
    /// </summary>
    public class AccountService : IAccountService
    {
        /// <summary>
        /// The cart service.
        /// </summary>
        private readonly ICartService cartService;

        /// <summary>
        /// The customer provider.
        /// </summary>
        private readonly CustomerServiceProvider customerServiceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService" /> class.
        /// </summary>
        /// <param name="cartService">The cart service.</param>
        /// <param name="customerServiceProvider">The customer provider.</param>
        public AccountService(ICartService cartService, CustomerServiceProvider customerServiceProvider)
        {
            this.cartService = cartService;
            this.customerServiceProvider = customerServiceProvider;
        }

        /// <summary>
        /// Checks if a user exists.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>True if the user exists, otherwise false.</returns>
        public bool IsUserExist([NotNull] string userName)
        {
          Assert.ArgumentNotNullOrEmpty(userName, "userName");

          return Membership.GetUser(userName) != null;
        }

        /// <summary>
        /// Registers the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="shopName">Name of the shop.</param>
        public virtual void Register(string userName, string password, string email, string shopName)
        {
            Assert.ArgumentNotNullOrEmpty(userName, "userName");
            Assert.ArgumentNotNullOrEmpty(password, "password");
            Assert.ArgumentNotNullOrEmpty(email, "email");
            Assert.ArgumentNotNullOrEmpty(shopName, "shopName");

          var request = new CreateUserRequest(userName, password, email, shopName);
          request.Properties["UserId"] = this.cartService.VisitorId;

          this.customerServiceProvider.CreateUser(request);
        }

        /// <summary>
        /// Logins the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="persistent">if set to <c>true</c> the session is persistent.</param>
        /// <returns>
        /// <c>true</c> if login attempt succeeded, otherwise <c>false</c>.
        /// </returns>
        public virtual bool Login(string userName, string password, bool persistent)
        {
            Assert.ArgumentNotNullOrEmpty(userName, "userName");
            Assert.ArgumentNotNullOrEmpty(password, "password");

            var isLoggedIn = AuthenticationManager.Login(userName, password, persistent);
            if (isLoggedIn)
            {
                try
                {
                    var cartFromAnonymous = this.cartService.GetCart();
                    var wishlistFromAnonymous = this.cartService.GetWishList();

                    if (Tracker.Current.Session != null &&
                        (Tracker.Current.Contact.Identifiers.IdentificationLevel == Analytics.Model.ContactIdentificationLevel.None ||
                        !string.Equals(userName, Tracker.Current.Contact.Identifiers.Identifier, StringComparison.OrdinalIgnoreCase)))
                    {
                        Tracker.Current.Session.Identify(userName);
                    }

                    Tracker.Current.Contact.Identifiers.AuthenticationLevel = Analytics.Model.AuthenticationLevel.PasswordValidated;

                    this.cartService.MergeCarts(cartFromAnonymous);
                    this.cartService.MergeWishlist(wishlistFromAnonymous);
                }
                catch (Exception ex)
                {
                    Log.Error("login failed", ex, this);
                    throw;
                }
            }

            return isLoggedIn;
        }

        /// <summary>
        /// Logouts the current user.
        /// </summary>
        public virtual void Logout()
        {
            if (Tracker.Current != null)
            {
                Tracker.Current.EndVisit(true);
            }

            // release the HTTP session to trigger DMS to commit interaction data.
            HttpContext.Current.Session.Abandon();

            AuthenticationManager.Logout();
        }
    }
}