// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveProductToExternalCommerceSystemTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The save product to external commerce system test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeProductEntity
{
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductEntity;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  ///   The save product to external commerce system test.
  /// </summary>
  public class SaveProductToExternalCommerceSystemTest
  {
    /// <summary>
    ///   The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    ///   The client
    /// </summary>
    private readonly IProductsServiceChannel client;

    /// <summary>
    ///   The processor
    /// </summary>
    private readonly SaveProductToExternalCommerceSystem processor;

    /// <summary>
    ///   The product.
    /// </summary>
    private readonly Product product;

    /// <summary>
    ///   The request.
    /// </summary>
    private readonly SynchronizeProductRequest request;

    /// <summary>
    ///   The result.
    /// </summary>
    private readonly ServiceProviderResult result;

    /// <summary>
    ///   The language.
    /// </summary>
    private readonly string language;

    /// <summary>
    ///   Initializes a new instance of the <see cref="SaveProductToExternalCommerceSystemTest" /> class.
    /// </summary>
    public SaveProductToExternalCommerceSystemTest()
    {
      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new SaveProductToExternalCommerceSystem { ClientFactory = clientFactory };

      this.product = new Product { ExternalId = "100500", Name = "Cool car", ShortDescription = "Yea, dude. You wants this one", FullDescription = "Don't think, just pay for this" };
      this.language = "en";
      this.request = new SynchronizeProductRequest("100500") { Language = this.language };
      this.result = new ServiceProviderResult();
      this.args = new ServicePipelineArgs(this.request, this.result);
      this.args.Request.Properties["Product"] = this.product;
    }

    /// <summary>
    ///   Should try to get product by external product id.
    /// </summary>
    [Fact]
    public void ShouldTryCreateOrUpdateProduct()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().UpdateProduct(Arg.Is<ProductModel>(p => p.ProductId == "100500" && p.Name == "Cool car" && p.ShortDescription == "Yea, dude. You wants this one" && p.Sku == string.Empty), this.language);
    }

    /// <summary>
    /// Should set sku if it is present in identification dictionary.
    /// </summary>
    [Fact]
    public void ShouldSetSkuIfItIsPresentInIdentificationDictionary()
    {
      // arrange
      this.product.Identification["Sku"] = "sku";

      // act
      this.processor.Process(this.args);
      this.client.Received().UpdateProduct(Arg.Is<ProductModel>(p => p.Sku == "sku"), this.language);
    }
  }
}