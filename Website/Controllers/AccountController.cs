// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the AccountController type.
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
namespace Sitecore.Commerce.StarterKit.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using Sitecore;
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.StarterKit.Models;
    using Sitecore.Commerce.StarterKit.Services;

    /// <summary>
    /// The account controller.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        /// The customer service.
        /// </summary>
        private readonly IAccountService accountService;

        private readonly IOrderService orderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController" /> class.
        /// </summary>
        /// <param name="accountService">The customer service.</param>
        /// <param name="orderService">The order service.</param>
        public AccountController([NotNull] IAccountService accountService, [NotNull] IOrderService orderService)
        {
            Assert.ArgumentNotNull(accountService, "accountService");

            this.accountService = accountService;
            this.orderService = orderService;
        }

        /// <summary>
        /// visitor account information.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult Info()
        {
            if (!this.Request.IsAuthenticated)
            {
                return this.RedirectToAction("Login");
            }

            var orders = this.orderService.GetOrders();

            var model = new UserInfoModel
            {
                Orders = orders
            };

            return this.View(model);
        }

        // GET: /Account/Login

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        // POST: /Account/Login

        /// <summary>
        /// Logins the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            var userName = @"CommerceUsers\" + model.UserName;
            if (this.ModelState.IsValid && this.accountService.Login(userName, model.Password, model.RememberMe))
            {
                return this.RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            this.ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            return this.View(model);
        }

        // POST: /Account/LogOff

        /// <summary>
        /// The log off.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            this.accountService.Logout();

            return this.RedirectToDefault();
        }

        // GET: /Account/Register

        /// <summary>
        /// The register.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return this.View();
        }

        // POST: /Account/Register

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (this.ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    var userName = @"CommerceUsers\" + model.UserName;
                    var shopName = Context.Site.Name;

                    if (!this.accountService.IsUserExist(userName))
                    {
                        this.accountService.Register(userName, model.Password, model.Email, shopName);
                        this.accountService.Login(userName, model.Password, false);

                        return this.RedirectToDefault();
                    }

                    this.ModelState.AddModelError(string.Empty, "User already exists");
                }
                catch (MembershipCreateUserException e)
                {
                    this.ModelState.AddModelError(string.Empty, e.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        #region Helpers

        /// <summary>
        /// Redirects to default.
        /// </summary>
        /// <returns>The <see cref="ActionResult" />.</returns>
        [NotNull]
        private ActionResult RedirectToDefault()
        {
            return this.Redirect("/");
        }

        /// <summary>
        /// The redirect to local.
        /// </summary>
        /// <param name="returnUrl">The return url.</param>
        /// <returns>The <see cref="ActionResult" />.</returns>
        [NotNull]
        private ActionResult RedirectToLocal([NotNull] string returnUrl)
        {
            if ((this.Url != null) && this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToDefault();
        }

        #endregion
    }
}