// ---------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemDivisionsTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system divisions test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeDivisions
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeDivisions;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Products;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services;
  using Sitecore.Commerce.Services.Products;
  using Xunit;

  /// <summary>
  /// The read external commerce system divisions test.
  /// </summary>
  public class ReadExternalCommerceSystemDivisionsTest
  {
    /// <summary>
    /// The entity factory.
    /// </summary>
    private readonly IEntityFactory entityFactory;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly IProductsServiceChannel client;

    /// <summary>
    /// The processor.
    /// </summary>
    private readonly ReadExternalCommerceSystemDivisions processor;

    /// <summary>
    /// The args.
    /// </summary>
    private readonly ServicePipelineArgs args;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemDivisionsTest" /> class.
    /// </summary>
    public ReadExternalCommerceSystemDivisionsTest()
    {
      this.entityFactory = Substitute.For<IEntityFactory>();
      this.entityFactory.Create("Division").Returns(callInfo => new Division());

      this.client = Substitute.For<IProductsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

      this.processor = new ReadExternalCommerceSystemDivisions { EntityFactory = this.entityFactory, ClientFactory = clientFactory };

      this.args = new ServicePipelineArgs(new SynchronizeDivisionsRequest(), new ServiceProviderResult());
    }

    /// <summary>
    /// Should get all manufacturers.
    /// </summary>
    [Fact]
    public void ShouldGetAllDivisions()
    {
      // arrange

      // act
      this.processor.Process(this.args);

      // assert
      this.client.Received().GetAllDivisions();
    }

    /// <summary>
    /// Should fill request with manufacturers data.
    /// </summary>
    [Fact]
    public void ShouldFillRequestWithDivisionsData()
    {
      // arrange
      var firstDate = DateTime.Now;
      var secondDate = DateTime.Now;
      var divisions = new[]
      {  
        new DivisionModel { CreatedOnUtc = firstDate, UpdatedOnUtc = firstDate, Description = "Porsche the best", Id = "100500", Name = "Porsche" },
        new DivisionModel { CreatedOnUtc = secondDate, UpdatedOnUtc = secondDate, Description = "Citroen cool", Id = "100501", Name = "Citroen" },
      };
      this.client.GetAllDivisions().Returns(divisions);

      // act
      this.processor.Process(this.args);

      // assert
      var result = (IEnumerable<Division>)this.args.Request.Properties["Divisions"];
      result.Count().Should().Be(2);

      var firstDivision = result.ElementAt(0);
      firstDivision.Created.Should().Be(firstDate);
      firstDivision.Updated.Should().Be(firstDate);
      firstDivision.Description.Should().Be("Porsche the best");
      firstDivision.Name.Should().Be("Porsche");
      firstDivision.ExternalId.Should().Be("100500");

      var secondDivision = result.ElementAt(1);
      secondDivision.Created.Should().Be(secondDate);
      secondDivision.Updated.Should().Be(secondDate);
      secondDivision.Description.Should().Be("Citroen cool");
      secondDivision.Name.Should().Be("Citroen");
      secondDivision.ExternalId.Should().Be("100501");
    }
  }
}