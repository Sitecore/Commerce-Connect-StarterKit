// -----------------------------------------------------------------
// <copyright file="ICustomersService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The Customers interface.
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
namespace Nop.Plugin.Sitecore.Commerce.Customers
{
  using System;
  using System.Collections.Generic;
  using System.ServiceModel;

  using Common.Models;

  [ServiceContract]
  public interface ICustomersService
  {
    /// <summary>
    /// Creates the customer by customer unique identifier.
    /// </summary>
    /// <param name="customerId">The customer unique identifier.</param>
    /// <param name="customerModel">The customer model.</param>
    /// <returns></returns>
    [OperationContract]
    CustomerModel CreateCustomer(Guid customerId, CustomerModel customerModel = null);

    /// <summary>
    /// Gets the customer by customer unique identifier.
    /// </summary>
    /// <param name="customerId">The customer unique identifier.</param>
    /// <returns></returns>
    [OperationContract]
    CustomerModel GetCustomer(Guid customerId);

    /// <summary>
    /// Add addresses by customer unique identifier.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="addressModels"></param>
    /// <returns></returns>
    [OperationContract]
    Response<IEnumerable<AddressModel>> AddAddresses(Guid customerId, IList<AddressModel> addressModels);

    /// <summary>
    /// Remove addresses by customer unique identifier.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="addressIds"></param>
    /// <returns></returns>
    [OperationContract]
    Response RemoveAddresses(Guid customerId, IList<string> addressIds);

    /// <summary>
    /// Update addresses by customer unique identifier.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="addressModels"></param>
    /// <returns></returns>
    [OperationContract]
    Response UpdateAddresses(Guid customerId, IList<AddressModel> addressModels);

    /// <summary>
    /// Get all addresses by customer unique identifier.
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [OperationContract]
    Response<IEnumerable<AddressModel>> GetAllAddresses(Guid customerId);
  }
}