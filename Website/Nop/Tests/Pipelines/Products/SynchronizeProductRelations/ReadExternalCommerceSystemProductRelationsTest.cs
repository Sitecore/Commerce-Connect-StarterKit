// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductRelationsTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product relations base test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeProductRelations
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductRelations;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system product relations test.
  /// </summary>
  public class ReadExternalCommerceSystemProductRelationsTest
  {
    /// <summary>
    /// The entity factory.
    /// </summary>
    private readonly IEntityFactory entityFactory;

    /// <summary>
    /// The processor.
    /// </summary>
    private readonly ReadExternalCommerceSystemProductRelations processor;

    /// <summary>
    /// The request.
    /// </summary>
    private readonly SynchronizeProductRelationsRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private readonly ServiceProviderResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly IProductsServiceChannel client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemProductRelationsTest"/> class.
    /// </summary>
    public ReadExternalCommerceSystemProductRelationsTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();

      this.entityFactory.Create("RelationType").Returns(callInfo => new RelationType());
      this.entityFactory.Create("Relation").Returns(callInfo => new Relation());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);


      this.processor = new ReadExternalCommerceSystemProductRelations { EntityFactory = this.entityFactory, ClientFactory = clientFactory };
      this.request = new SynchronizeProductRelationsRequest("100500");
      this.result = new ServiceProviderResult();
      this.args = new ServicePipelineArgs(this.request, this.result);
    }

    /// <summary>
    /// Should preform read operation.
    /// </summary>
    [Fact]
    public void ShouldPreformReadOperation()
    {
      // arrange
      this.client.GetRelatedProductsIds("100500").Returns(new[] { "1", "5" });

      // act
      this.processor.Process(this.args);

      // assert
      var relations = (ICollection<RelationType>)this.args.Request.Properties["RelatedProducts"];
      var relationType = relations.Single();
      relationType.Name.Should().Be("Related products");

      relationType.Relations.Should().Contain(r => r.ExternalId == "1" || r.ExternalId == "5");
    }

    /// <summary>
    /// Should not create relations collection if already exist.
    /// </summary>
    [Fact]
    public void ShouldNotCreateRelationsCollectionIfAlreadyExist()
    {
      // arrange
      this.client.GetCrossSellProductsIds("100500").Returns(new[] { "4", "1" });

      var relationsCollection = new Collection<RelationType>();
      this.args.Request.Properties["RelatedProducts"] = relationsCollection;

      // act
      this.processor.Process(this.args);

      // assert
      this.args.Request.Properties["RelatedProducts"].Should().Be(relationsCollection);
    }

    /// <summary>
    /// Should not create relation type object if no related ids received.
    /// </summary>
    [Fact]
    public void ShouldNotCreateRelationTypeObjectIfNoRelatedIdsReceived()
    {
      // arrange
      this.client.GetCrossSellProductsIds("100500").Returns(new string[] { });

      // act
      this.processor.Process(this.args);

      // assert
      var relations = (ICollection<RelationType>)this.args.Request.Properties["RelatedProducts"];
      relations.Should().BeEmpty();
    }
  }
}
