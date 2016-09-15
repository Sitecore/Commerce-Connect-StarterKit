// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NopProductProcessor.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The nop product processor.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common
{
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;

  /// <summary>
  /// The product processor.
  /// </summary>
  /// <typeparam name="TChannel">The type of the channel.</typeparam>
  public abstract class NopProductProcessor<TChannel> : ReadExternalCommerceSystemProcessor<TChannel> where TChannel : System.ServiceModel.IClientChannel
  {
    /// <summary>
    /// Runs the processor.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (!this.GetSavedProductData(args))
      {
        this.GetProductDataFromService(args);
      }
    }

    /// <summary>
    /// Gets the product data from service.
    /// </summary>
    /// <param name="args">The arguments.</param>
    protected abstract void GetProductDataFromService(ServicePipelineArgs args);

    /// <summary>
    /// Sets the output data.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="product">The product.</param>
    protected abstract void SetDataFromSavedProduct(ServicePipelineArgs args, Product product);

    /// <summary>
    /// Gets the product data.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// The result, that notifies 
    /// if the product exists and the data
    /// could be read.
    /// </returns>
    private bool GetSavedProductData(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var result = false;
      if (args.Request.Properties["Product"] != null)
      {
        var product = args.Request.Properties["Product"] as Product;
        if (product != null)
        {
          this.SetDataFromSavedProduct(args, product);
          result = true;
        }
      }

      return result;
    }
  }
}