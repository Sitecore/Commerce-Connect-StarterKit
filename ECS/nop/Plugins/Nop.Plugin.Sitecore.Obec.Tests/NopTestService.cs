// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NopTestService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the default implementation of INopTestService.
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
using Nop.Services.Events;

namespace Nop.Plugin.Sitecore.Commerce.Tests
{
  using Core.Caching;
  using Core.Data;
  using Core.Domain.Catalog;
  using Core.Infrastructure;
  using Services.Catalog;

  /// <summary>
  /// Defines the default implementation of <see cref="INopTestService"/>.
  /// </summary>
  public class NopTestService : INopTestService
  {
    /// <summary>
    /// The product service.
    /// </summary>
    private readonly IProductService _productService;

    /// <summary>
    /// The _product repository
    /// </summary>
    private readonly IRepository<Product> _productRepository;

    /// <summary>
    /// The i event publisher
    /// </summary>
    private readonly IEventPublisher _eventPublisher;

    private readonly ICacheManager _cacheManager;

    private const string ProductsPatternKey = "Nop.product.";

    /// <summary>
    /// Initializes a new instance of the <see cref="NopTestService" /> class.
    /// </summary>
    public NopTestService()
    {
      _productService = EngineContext.Current.Resolve<IProductService>();


      _cacheManager = EngineContext.Current.Resolve<ICacheManager>();
      _productRepository = EngineContext.Current.Resolve<IRepository<Product>>();
      _eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

    }

    /// <summary>
    /// Deletes the product.
    /// </summary>
    /// <param name="product">The product.</param>
    public void DeleteProduct(Product product)
    {
      _productService.DeleteProduct(product);
    }

    /// <summary>
    /// Deletes the product by identifier.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    public void DeleteProductById(int productId)
    {
      var product = _productService.GetProductById(productId);
      if (product == null)
      {
        return;
      }
      _productService.DeleteProduct(product);      
      //update
      _productRepository.Delete(product);
      
      //cache
      _cacheManager.RemoveByPattern(ProductsPatternKey);

      //event notification
      _eventPublisher.EntityDeleted(product);
    }

    /// <summary>
    /// Gets the product by id.
    /// </summary>
    /// <param name="productId">The product id.</param>
    /// <returns>Product with the specified id.</returns>
    public Product GetProductById(int productId)
    {
      return PrototypeFactory.Clone(_productService.GetProductById(productId));
      //return _productService.GetProductById(productId).GetClone();
    }

    

    /// <summary>
    /// Inserts the product.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <returns>Id of the inserted product.</returns>
    /// <exception cref="System.NotImplementedException">The method is not implemented.</exception>
    public int InsertProduct(Product product)
    {
      _productService.InsertProduct(product);
      return product.Id;
    }

    /// <summary>
    /// Updates the product.
    /// </summary>
    /// <param name="product">The product.</param>
    public void UpdateProduct(Product product)
    {
      _productService.UpdateProduct(product);
    }
  }
}