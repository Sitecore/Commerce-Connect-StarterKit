// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetExternalCommerceSystemProductListTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The get external commerce system product list test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.GetExternalCommerceSystemProductList
{
  using System.Collections.Generic;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The get external commerce system product list test.
  /// </summary>
  public class GetExternalCommerceSystemProductListTest
  {
         /// <summary>
    /// The client.
    /// </summary>
    private readonly IProductsServiceChannel client;

    /// <summary>
    /// The processor.
    /// </summary>
    private readonly NopCommerce.Pipelines.Products.GetExternalCommerceSystemProductList.GetExternalCommerceSystemProductList processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly GetExternalCommerceSystemProductListRequest request;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetExternalCommerceSystemProductListTest" /> class.
    /// </summary>
    public GetExternalCommerceSystemProductListTest()
    {
      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new NopCommerce.Pipelines.Products.GetExternalCommerceSystemProductList.GetExternalCommerceSystemProductList { ClientFactory = clientFactory };

      this.request = new GetExternalCommerceSystemProductListRequest();
      this.args = new ServicePipelineArgs(this.request, new ServiceProviderResult());
    }

    /// <summary>
    /// Should get all product ids.
    /// </summary>
    [Fact]
    public void ShouldGetAllProductIds()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetAllProductsIds();
    }

    /// <summary>
    /// Should set all product ids to custom args.
    /// </summary>
    [Fact]
    public void ShouldSetAllProductIdsToCustomArgs()
    {
      // arrange
      var productIds = new[] { "1", "2" };
      this.client.GetAllProductsIds().Returns(productIds);

      // act
      this.processor.Process(this.args);

      // assert
      this.args.Request.Properties["ExternalCommerceSystemProductIds"].Should().Be(productIds);
    }

    /// <summary>
    /// Should return empty collection in custom args if product ids are null.
    /// </summary>
    [Fact]
    public void ShouldReturnEmptyCollectionInCustomArgsIfProductIdsAreNull()
    {
      // arrange
      string[] productIds = null;
      this.client.GetAllProductsIds().Returns(productIds);

      // act
      this.processor.Process(this.args);

      // assert
      this.args.Request.Properties["ExternalCommerceSystemProductIds"].Should().NotBeNull();
      ((IEnumerable<string>)this.args.Request.Properties["ExternalCommerceSystemProductIds"]).Should().BeEmpty();
    }
  }
}