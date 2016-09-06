﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INopTestService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the service interface that allows to setup the environment on Nop side.
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
namespace Nop.Plugin.Sitecore.Commerce.Tests
{
  using System.ServiceModel;
  using Core.Domain.Catalog;

  /// <summary>
  /// Defines the service interface that allows to setup the environment on Nop side.
  /// </summary>
  [ServiceContract]
  public interface INopTestService
  {
    /// <summary>
    /// Deletes the product.
    /// </summary>
    /// <param name="product">The product.</param>
    [OperationContract]
    void DeleteProduct(Product product);

    /// <summary>
    /// Deletes the product by identifier.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    [OperationContract]
    void DeleteProductById(int productId);

    /// <summary>
    /// Gets the product by id.
    /// </summary>
    /// <param name="productId">The product id.</param>
    /// <returns>Product with the specified id.</returns>
    [OperationContract]
    Product GetProductById(int productId);

    /// <summary>
    /// Inserts the product.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <returns>Id of the inserted product.</returns>
    [OperationContract]
    int InsertProduct(Product product);

    /// <summary>
    /// Updates the product.
    /// </summary>
    /// <param name="product">The product.</param>
    [OperationContract]
    void UpdateProduct(Product product);
  }
}
