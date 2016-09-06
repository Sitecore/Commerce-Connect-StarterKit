// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProductClassificationsTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The read external commerce system product classifications test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Products.SynchronizeProductClassifications
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using NSubstitute;
    using Sitecore.Commerce.Connectors.NopCommerce.NopProductsService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Products.SynchronizeProductClassifications;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Products;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Products;
    using Xunit;

    /// <summary>
    /// The read external commerce system product classifications test.
    /// </summary>
    public class ReadExternalCommerceSystemProductClassificationsTest
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
        private readonly ReadExternalCommerceSystemProductClassifications processor;

        /// <summary>
        /// The args.
        /// </summary>
        private readonly ServicePipelineArgs args;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadExternalCommerceSystemProductClassificationsTest"/> class. 
        /// </summary>
        public ReadExternalCommerceSystemProductClassificationsTest()
        {
            this.entityFactory = Substitute.For<IEntityFactory>();

            this.entityFactory.Create("ClassificationGroup").Returns(callInfo => new ClassificationGroup());
            this.entityFactory.Create("Classification").Returns(callInfo => new Classification());

            this.client = Substitute.For<IProductsServiceChannel>();

            var clientFactory = Substitute.For<ServiceClientFactory>();
            clientFactory.CreateClient<IProductsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

            this.processor = new ReadExternalCommerceSystemProductClassifications("Categories", "111") { EntityFactory = this.entityFactory, ClientFactory = clientFactory };

            SynchronizeProductClassificationsRequest request = new SynchronizeProductClassificationsRequest("100500");
            this.args = new ServicePipelineArgs(request, new ServiceProviderResult());
        }

        /// <summary>
        /// Should get specific manufacturer.
        /// </summary>
        [Fact]
        public void ShouldGetSpecificClassifications()
        {
            // arrange

            // act
            this.processor.Process(this.args);

            // assert
            this.client.Received().GetCategories("100500");
        }

        /// <summary>
        /// Should fill result with manufacturer data.
        /// </summary>
        [Fact]
        public void ShouldFillResultWithClassificationsData()
        {
            // arrange
            var models = new[] { new CategoryModel { Id = "157" }, new CategoryModel { Id = "33" } };
            this.client.GetCategories("100500").Returns(models);

            // act
            this.processor.Process(this.args);

            // assert
            var result = (IEnumerable<ClassificationGroup>)this.args.Request.Properties["ClassificationGroups"];
            result.Count().Should().Be(1);
            var classificationGroup = result.ElementAt(0);
            classificationGroup.Name.Should().Be("Categories");
            classificationGroup.ExternalId.Should().Be("111");
            classificationGroup.Classifications.Count.Should().Be(2);
            classificationGroup.Classifications.ElementAt(0).ExternalId.Should().Be("157");
            classificationGroup.Classifications.ElementAt(1).ExternalId.Should().Be("33");
        }

        /// <summary>
        /// Should fill result with data from saved product.
        /// </summary>
        [Fact]
        public void ShouldFillResultWithDataFromSavedProduct()
        {
            // arrange
            this.args.Request.Properties["Product"] = new Product
            {
                ClassificationGroups = new ReadOnlyCollectionAdapter<ClassificationGroup>
                {
                    new ClassificationGroup
                    {
                        Name = "My categories",
                        ExternalId = "222",
                        Classifications = new ReadOnlyCollection<Classification>(new List<Classification> { new Classification { ExternalId = "300" } })
                    } 
                }
            };

            // act
            this.processor.Process(this.args);

            // assert
            // assert
            var result = (IEnumerable<ClassificationGroup>)this.args.Request.Properties["ClassificationGroups"];
            result.Count().Should().Be(1);
            var classificationGroup = result.ElementAt(0);
            classificationGroup.Name.Should().Be("My categories");
            classificationGroup.ExternalId.Should().Be("222");
            classificationGroup.Classifications.Count.Should().Be(1);
            classificationGroup.Classifications.ElementAt(0).ExternalId.Should().Be("300");
        }

        /// <summary>
        /// Should not call network if data from saved product exists.
        /// </summary>
        [Fact]
        public void ShouldNotCallNetworkIfDataFromSavedProductExists()
        {
            // arrange
            this.args.Request.Properties["Product"] = new Product();

            // act
            this.processor.Process(this.args);

            // assert
            this.client.DidNotReceive().GetCategories(Arg.Any<string>());
        }
    }
}