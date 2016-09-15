// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The carts controller.
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
namespace Sitecore.Commerce.StarterKit.Controllers.Admin
{
    using System.Linq;
    using System.Web.Http;
    using Sitecore.Commerce.StarterKit.Filters;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Services.Carts;

    /// <summary>
    /// The carts controller.
    /// </summary>
    public class CartsController : ApiController
    {
        /// <summary>
        /// The cart service.
        /// </summary>
        private readonly CartServiceProvider cartService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartsController"/> class.
        /// </summary>
        /// <param name="cartService">The cart service.</param>
        public CartsController(CartServiceProvider cartService)
        {
            this.cartService = cartService;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="customerId">The customer id.</param>
        /// <param name="cartName">Name of the cart.</param>
        /// <param name="cartStatus">The cart status.</param>
        /// <returns>
        /// The <see cref="IQueryable" />.
        /// </returns>
        [Queryable]
        [HttpGet]
        [RequireAdmin]
        public IQueryable<CartBase> Get(string userId = null, string customerId = null, string cartName = null, string cartStatus = null)
        {
            var request = new GetCartsRequest(Context.Site.Name);
            if (userId != null)
            {
                request.UserIds = new[] { userId };
            }

            if (customerId != null)
            {
                request.CustomerIds = new[] { customerId };
            }

            if (cartName != null)
            {
                request.Names = new[] { cartName };
            }

            if (cartStatus != null)
            {
                request.Statuses = new[] { cartStatus };
            }

            var result = this.cartService.GetCarts(request);
            return result.Carts
              .OrderBy(c => c.ShopName)
              .ThenBy(c => c.CustomerId)
              .ThenBy(c => c.UserId)
              .ThenBy(c => c.Name)
              .AsQueryable();
        }
    }
}