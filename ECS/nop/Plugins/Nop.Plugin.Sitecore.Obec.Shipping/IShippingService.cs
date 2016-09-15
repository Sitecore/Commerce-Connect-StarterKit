// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IShippingService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The Customers interface.
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
// -----------------------------------------------------------------
namespace Nop.Plugin.Sitecore.Commerce.Shipping
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using Common.Models;

    /// <summary>
    /// The shipping service interface.
    /// </summary>
    [ServiceContract]
    public interface IShippingService
    {
        /// <summary>
        /// Gets the shipping methods.
        /// </summary>
        /// <param name="shoppingCartModel">The shopping cart.</param>
        /// <param name="storeName">The store name.</param>
        /// <param name="addressModel">The address.</param>
        /// <returns>A service response.</returns>
        [OperationContract]
        Response<IList<ShippingMethodModel>> GetShippingMethods(ShoppingCartModel shoppingCartModel, string storeName = "", AddressModel addressModel = null);
    }
}