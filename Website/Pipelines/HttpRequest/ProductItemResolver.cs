// ---------------------------------------------------------------------
// <copyright file="ProductItemResolver.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The product item resolver.
// </summary>
// ---------------------------------------------------------------------
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
namespace Sitecore.Commerce.StarterKit.Pipelines.HttpRequest
{
  using Castle.Windsor;
  using Sitecore;
  using Sitecore.Commerce.StarterKit.App_Start;
  using Sitecore.Commerce.StarterKit.Helpers;
  using Sitecore.Commerce.StarterKit.Services;
  using Sitecore.Pipelines;

  /// <summary>
  /// The product item resolver.
  /// </summary>
  public class ProductItemResolver
  {
    /// <summary>
    /// The product service.
    /// </summary>
    private IProductService productService;

    /// <summary>
    /// The product link manager.
    /// </summary>
    private ProductHelper productHelper;

    /// <summary>
    /// The windsor container.
    /// </summary>
    private IWindsorContainer windsorContainer;

    /// <summary>
    /// Gets or sets the windsor container.
    /// </summary>
    /// <value>The windsor container.</value>
    public IWindsorContainer WindsorContainer
    {
      get { return this.windsorContainer ?? (this.windsorContainer = WindsorConfig.Container); }
      set { this.windsorContainer = value; }
    }

    /// <summary>
    /// Gets or sets the product service.
    /// </summary>
    /// <value>The product service.</value>
    public IProductService ProductService
    {
      get { return this.productService ?? (this.productService = this.WindsorContainer.Resolve<IProductService>()); }
      set { this.productService = value; }
    }

    /// <summary>
    /// Gets or sets the product helper.
    /// </summary>
    /// <value>The product helper.</value>
    public ProductHelper ProductHelper
    {
      get { return this.productHelper ?? (this.productHelper = this.WindsorContainer.Resolve<ProductHelper>()); }
      set { this.productHelper = value; }
    }

    /// <summary>
    /// Runs the processor.
    /// </summary>
    /// <param name="args">The args.</param>
    public virtual void Process(PipelineArgs args)
    {
      var productId = this.ProductHelper.GetProductIdFromIncomingRequest();

      if (string.IsNullOrEmpty(productId))
      {
        return;
      }

      var product = this.ProductService.ReadProduct(productId);

      Context.Item = product;
    }
  }
}