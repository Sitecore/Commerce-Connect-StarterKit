// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShippingService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the ShippingService class.</summary>
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
  using System.Linq;
  using System.ServiceModel.Activation;
  using System.Web.Services;
  using Nop.Core.Infrastructure;
  using Nop.Plugin.Sitecore.Commerce.Common.Models;
  using Nop.Services.Customers;
  using Nop.Services.Stores;

  /// <summary>
  ///   Shipping Service
  /// </summary>
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  public class ShippingService : IShippingService
  {
    /// <summary>
    ///   The customer service.
    /// </summary>
    private readonly ICustomerService customerService;

    /// <summary>
    ///   The shipping service
    /// </summary>
    private readonly Services.Shipping.IShippingService shippingService;

    /// <summary>
    ///   The store service
    /// </summary>
    private readonly IStoreService storeService;

    /// <summary>
    ///   Initializes a new instance of the <see cref="ShippingService" /> class.
    /// </summary>
    public ShippingService()
    {
      this.shippingService = EngineContext.Current.Resolve<Services.Shipping.IShippingService>();
      this.customerService = EngineContext.Current.Resolve<ICustomerService>();
      this.storeService = EngineContext.Current.Resolve<IStoreService>();
    }

    /// <summary>
    ///   Get shippin methods
    /// </summary>
    /// <param name="shoppingCartModel">The shopping cart.</param>
    /// <param name="storeName">The store name.</param>
    /// <param name="addressModel">The address.</param>
    /// <returns>A service response.</returns>
    [WebMethod(EnableSession = false)]
    public virtual Response<IList<ShippingMethodModel>> GetShippingMethods(ShoppingCartModel shoppingCartModel, string storeName = "", AddressModel addressModel = null)
    {
      var customer = this.customerService.GetCustomerByGuid(shoppingCartModel.CustomerGuid);
      if (customer == null)
      {
        return new Response<IList<ShippingMethodModel>>
        {
          Success = false,
          Message = "Cannot find customer by guid"
        };
      }

      if (customer.ShoppingCartItems == null)
      {
        return new Response<IList<ShippingMethodModel>>
        {
          Success = false,
          Message = "Shopping curt items is null"
        };
      }

      var store = this.storeService.GetAllStores()
        .FirstOrDefault(s => s.Name.Equals(storeName, StringComparison.CurrentCultureIgnoreCase));

      var address = addressModel != null
        ? addressModel.MapToAddress()
        : customer.ShippingAddress;

      var optionResponse = this.shippingService.GetShippingOptions(customer.ShoppingCartItems.ToList(), address, string.Empty, store == null ? 0 : store.Id);

      var result = new List<ShippingMethodModel>(0);

      if (optionResponse.Success)
      {
        foreach (var shippingOption in optionResponse.ShippingOptions)
        {
          result.Add(new ShippingMethodModel
          {
            Name = shippingOption.Name,
            Description = shippingOption.Description,
            SystemName = shippingOption.ShippingRateComputationMethodSystemName
          });
        }
      }

      return new Response<IList<ShippingMethodModel>>
      {
        Success = true,
        Result = result
      };
    }
  }
}