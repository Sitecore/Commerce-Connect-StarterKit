// ----------------------------------------------------------------------------------------------
// <copyright file="GetBackOrderableInformationTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetBackOrderableInformationTest class.
// </summary>
// ----------------------------------------------------------------------------------------------
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Inventory.GetOrderableInformation
{
    using System.Linq;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using Sitecore.Commerce.Connectors.NopCommerce.NopInventoryService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Inventory.GetOrderableInformation;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Inventory;
    using Sitecore.Commerce.Entities.Inventory;
    using Xunit;

    /// <summary>
    /// Defines the GetBackOrderableInformationTest class.
    /// </summary>
    public class GetBackOrderableInformationTest
    {
        /// <summary>
        /// The client.
        /// </summary>
        private readonly IInventoryServiceChannel _client;

        /// <summary>
        /// The get product back-orderable information.
        /// </summary>
        private readonly GetBackOrderableInformation _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetBackOrderableInformationTest" /> class.
        /// </summary>
        public GetBackOrderableInformationTest()
        {
            var entityFactory = Substitute.For<IEntityFactory>();
            entityFactory.Create("OrderableInformation").Returns(new OrderableInformation());
            entityFactory.Create("InventoryProduct").Returns(new InventoryProduct());
            
            this._client = Substitute.For<IInventoryServiceChannel>();
            
            var clientFactory = Substitute.For<ServiceClientFactory>();
            clientFactory.CreateClient<IInventoryServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this._client);

            this._processor = new GetBackOrderableInformation(entityFactory) { ClientFactory = clientFactory };
        }

        /// <summary>
        /// Should get product back-orderable information for the specified products.
        /// </summary>
        [Fact]
        public void ShouldGetBackOrderableInformationForTheSpecifiedProducts()
        {
            // Arrange
            var request = new GetBackOrderableInformationRequest("shopname", new List<InventoryProduct> { new InventoryProduct { ProductId = "pid1" }});
            var result = new GetBackOrderableInformationResult();
            var args = new ServicePipelineArgs(request, result);

            this._client.GetBackOrderableInformationList("shopname", Arg.Is<string[]>(ids => (ids.Length == 1) && (ids[0] == "pid1")), new System.Guid())
                .Returns(new[] { new OrderableInformationModel { ProductId = "pid1", Status = NopCommerce.NopInventoryService.StockStatus.InStock } });

            // Act
            this._processor.Process(args);

            // Assert
            result.OrderableInformation.Should().HaveCount(1);
            result.OrderableInformation.ElementAt(0).Product.ProductId.Should().Be("pid1");
            result.OrderableInformation.ElementAt(0).Status.Should().Be(1);
        }

        /// <summary>
        /// Should not add new records if product back-orderable information is not found.
        /// </summary>
        [Fact]
        public void ShouldNotAddNewRecordsIfProductBackOrderableInformationIsNotFound()
        {
            // Arrange
            var request = new GetBackOrderableInformationRequest("shopname", new List<InventoryProduct> { new InventoryProduct { ProductId = "noExist" } });
            var result = new GetBackOrderableInformationResult();
            var args = new ServicePipelineArgs(request, result);

            var results = new OrderableInformationModel[1];
            results[0] = null;
            this._client.GetBackOrderableInformationList("shopname", Arg.Is<string[]>(ids => (ids.Length == 1) && (ids[1] == "noExist")), new System.Guid()).Returns(results);

            // Act
            this._processor.Process(args);

            // Assert
            result.OrderableInformation.Should().HaveCount(1);
            result.OrderableInformation.ElementAt(0).Product.ProductId.Should().Be("noExist");
            result.OrderableInformation.ElementAt(0).Status.Should().Be(null);
            result.OrderableInformation.ElementAt(0).InStockDate.Should().Be(null);
            result.OrderableInformation.ElementAt(0).OrderableEndDate.Should().Be(null);
            result.OrderableInformation.ElementAt(0).OrderableStartDate.Should().Be(null);
            result.OrderableInformation.ElementAt(0).RemainingQuantity.Should().Be(0);
            result.OrderableInformation.ElementAt(0).ShippingDate.Should().Be(null);
            result.OrderableInformation.ElementAt(0).CartQuantityLimit.Should().Be(0);
        }
    }
}
