﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequireCartLineAttribute.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the RequireCartLineAttribute class.</summary>
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
namespace Sitecore.Commerce.StarterKit.Filters
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using App_Start;
    using Services;

    /// <summary>
    /// Specifies that a cart line is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class RequireCartLineAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The cart service.
        /// </summary>
        private readonly ICartService _cartService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireCartLineAttribute"/> class.
        /// </summary>
        public RequireCartLineAttribute()
        {
            this._cartService = WindsorConfig.Container.Resolve<ICartService>();
            var db = Context.Database ?? Context.ContentDatabase;
        }

        /// <summary>
        /// On Action Executing
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cart = this._cartService.GetCart();
            if (cart != null && !cart.Lines.Any())
            {
                //filterContext.Result = new RedirectResult(_cartUrl);
                filterContext.Result = new RedirectResult("/Cart");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}