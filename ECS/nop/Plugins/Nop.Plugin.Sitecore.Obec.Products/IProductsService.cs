// -----------------------------------------------------------------
// <copyright file="IProductsService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The ProductsService interface.
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
namespace Nop.Plugin.Sitecore.Commerce.Products
{
  using System.Collections.Generic;
  using System.Linq;
  using System.ServiceModel;
  using Models;

  /// <summary>
  /// The ProductsService interface.
  /// </summary>
  [ServiceContract]
  public interface IProductsService
  {
    /// <summary>
    /// Gets the product by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <param name="languageCode">The language code.</param>
    /// <returns>
    /// Instance of <see cref="ProductModel" />
    /// </returns>
    [OperationContract]
    ProductModel GetProduct(string externalProductId, string languageCode);

    /// <summary>
    /// Gets all product ids.
    /// </summary>
    /// <returns>The list of product identifiers.</returns>
    [OperationContract]
    IList<string> GetAllProductsIds();

    /// <summary>
    /// Gets the manufacturer by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>List of <see cref="ManufacturerModel"/></returns>
    [OperationContract]
    IQueryable<ManufacturerModel> GetManufacturer(string externalProductId);

    /// <summary>
    /// Gets all manufacturers.
    /// </summary>
    /// <returns>List of <see cref="ManufacturerModel"/></returns>
    [OperationContract]
    IQueryable<ManufacturerModel> GetAllManufacturers();

    /// <summary>
    /// Gets the categories by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>List of <see cref="CategoryModel"/></returns>
    [OperationContract]
    IQueryable<CategoryModel> GetCategories(string externalProductId);

    /// <summary>
    /// Gets all categories.
    /// </summary>
    /// <returns>List of <see cref="CategoryModel"/></returns>
    [OperationContract]
    IQueryable<CategoryModel> GetAllCategories();

    /// <summary>
    /// Gets all global specifications.
    /// </summary>
    /// <returns>The list of global specifications.</returns>
    [OperationContract]
    IQueryable<SpecificationLookupModel> GetAllGlobalSpecifications();

    /// <summary>
    /// Gets the product global specifications by external product unique identifier.
    /// </summary>
    /// <param name="externalProductId">The external product unique identifier.</param>
    /// <returns>The list of product global specifications by external product id.</returns>
    [OperationContract]
    IList<ProductGlobalSpecificationModel> GetProductGlobalSpecifications(string externalProductId);

    /// <summary>
    /// Gets the related product ids by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>List of productIds</returns>
    [OperationContract]
    IList<string> GetRelatedProductsIds(string externalProductId);

    /// <summary>
    /// Gets the cross sell product ids by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>list of cross sell products</returns>
    [OperationContract]
    IList<string> GetCrossSellProductsIds(string externalProductId);

    /// <summary>
    /// Gets the variant product ids by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns></returns>
    [OperationContract]
    IList<string> GetVariantProductsIds(string externalProductId);

    /// <summary>
    /// Gets all divisions.
    /// </summary>
    /// <returns>The list of all divisions.</returns>
    [OperationContract]
    IList<DivisionModel> GetAllDivisions();

    /// <summary>
    /// Gets the related divisions by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>Instance of <see cref="ProductDivisionsModel" /></returns>
    [OperationContract]
    ProductDivisionsModel GetRelatedDivisions(string externalProductId);

    /// <summary>
    /// Gets the product manufacturers by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>The list of product manufacturers.</returns>
    [OperationContract]
    ProductManufacturersModel GetProductManufacturers(string externalProductId);

    /// <summary>
    /// Gets all product types.
    /// </summary>
    /// <returns>The list of product types.</returns>
    [OperationContract]
    IQueryable<ProductTypeModel> GetAllProductTypes();

    /// <summary>
    /// Gets the product types by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>The list of product types.</returns>
    [OperationContract]
    IQueryable<ProductTypeModel> GetProductTypes(string externalProductId);

    /// <summary>
    /// Gets the product main image by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>The product main image.</returns>
    [OperationContract]
    ProductResourceModel GetProductMainImage(string externalProductId);

    /// <summary>
    /// Gets the product alternate images by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>The list of alternate images.</returns>
    [OperationContract]
    ProductResourceModel GetProductAlternateImages(string externalProductId);

    /// <summary>
    /// Gets the product downloads by external product id.
    /// </summary>
    /// <param name="externalProductId">The external product id.</param>
    /// <returns>The list of product downloads.</returns>
    [OperationContract]
    ProductResourceModel GetProductDownloads(string externalProductId);

    /// <summary>
    /// Gets all resources.
    /// </summary>
    /// <returns>The collection of resources.</returns>
    [OperationContract]
    IQueryable<ResourceModel> GetAllResources();

    /// <summary>
    /// Adds the product.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <param name="languageCode">The language code.</param>
    [OperationContract]
    void UpdateProduct(ProductModel product, string languageCode);
  }
}