// ----------------------------------------------------------------------------------------------
// <copyright file="GetStockInformationTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GetStockInformationTest class.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Inventory.GetStockInformation
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using Sitecore.Commerce.Connectors.NopCommerce.NopInventoryService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Inventory.GetStockInformation;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Inventory;
    using Sitecore.Commerce.Entities.Inventory;
    using Xunit;

    /// <summary>
    /// Defines the GetStockInformationTest class.
    /// </summary>
    public class GetStockInformationTest
    {
        /// <summary>
        /// The client.
        /// </summary>
        private readonly IInventoryServiceChannel _client;

        /// <summary>
        /// The get product stock information.
        /// </summary>
        private readonly GetStockInformation _processor;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Sitecore.Obec.Connectors.NopCommerce.Tests.Pipelines.Inventory.GetStockInformation.GetStockInformationTest" /> class.
        /// </summary>
        public GetStockInformationTest()
        {
            var entityFactory = Substitute.For<IEntityFactory>();
            entityFactory.Create("StockInformation").Returns(new StockInformation());
            entityFactory.Create("InventoryProduct").Returns(new InventoryProduct());

            this._client = Substitute.For<IInventoryServiceChannel>();
            
            var clientFactory = Substitute.For<ServiceClientFactory>();
            clientFactory.CreateClient<IInventoryServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this._client);

            this._processor = new GetStockInformation(entityFactory) { ClientFactory = clientFactory };
        }

        /// <summary>
        /// Should get product stock information for the specified products.
        /// </summary>
        [Fact]
        public void ShouldGetStockInformationOnlyStatusForTheSpecifiedProducts()
        {
            // Arrange
            var request = new GetStockInformationRequest("shopname", new List<InventoryProduct> { new InventoryProduct { ProductId = "pid1" } });
            var result = new GetStockInformationResult();
            var args = new ServicePipelineArgs(request, result);

            this._client.GetStocksInformation("shopname", Arg.Is<string[]>(ids => (ids.Length == 1) && (ids[0] == "pid1")), new System.Guid())
                .Returns(new[] { new StockInformationModel { ProductId = "pid1", Status = NopCommerce.NopInventoryService.StockStatus.InStock } });

            // Act
            this._processor.Process(args);

            // Assert
            result.StockInformation.Should().HaveCount(1);
            result.StockInformation.ElementAt(0).Product.ProductId.Should().Be("pid1");
            result.StockInformation.ElementAt(0).Status.Should().Be(1);
            result.StockInformation.ElementAt(0).Count.Should().Be(0);
            result.StockInformation.ElementAt(0).AvailabilityDate.Should().Be(null);
        }

        /// <summary>
        /// Should get product stock information only count for the specified products.
        /// </summary>
        [Fact]
        public void ShouldGetStockInformationOnlyCountForTheSpecifiedProducts()
        {
            // Arrange
            var request = new GetStockInformationRequest("shopname", new List<InventoryProduct> { new InventoryProduct { ProductId = "pid1" }}, StockDetailsLevel.CountFlag);
            var result = new GetStockInformationResult();
            var args = new ServicePipelineArgs(request, result);

            this._client.GetStocksInformation("shopname", Arg.Is<string[]>(ids => (ids.Length == 1) && (ids[0] == "pid1")), new System.Guid())
                .Returns(new[] { new StockInformationModel { ProductId = "pid1", Count = 10 }});

            // Act
            this._processor.Process(args);

            // Assert
            result.StockInformation.Should().HaveCount(1);
            result.StockInformation.ElementAt(0).Product.ProductId.Should().Be("pid1");
            result.StockInformation.ElementAt(0).Count.Should().Be(10);
            result.StockInformation.ElementAt(0).Status.Should().Be(null);
            result.StockInformation.ElementAt(0).AvailabilityDate.Should().Be(null);
        }

        /// <summary>
        /// Should get product stock information only availability date for the specified products.
        /// </summary>
        [Fact]
        public void ShouldGetStockInformationOnlyAvailabilityDateForTheSpecifiedProducts()
        {
            // Arrange
            var request = new GetStockInformationRequest("shopname", new List<InventoryProduct> { new InventoryProduct { ProductId = "pid1" }}, StockDetailsLevel.AvailibilityFlag);
            var result = new GetStockInformationResult();
            var args = new ServicePipelineArgs(request, result);

            this._client.GetStocksInformation("shopname", Arg.Is<string[]>(ids => (ids.Length == 1) && (ids[0] == "pid1")), new System.Guid())
                .Returns(new[] { new StockInformationModel { ProductId = "pid1", AvailabilityDate = new DateTime(2014, 04, 21) }});

            // Act
            this._processor.Process(args);

            // Assert
            result.StockInformation.Should().HaveCount(1);
            result.StockInformation.ElementAt(0).Product.ProductId.Should().Be("pid1");
            result.StockInformation.ElementAt(0).Count.Should().Be(0);
            result.StockInformation.ElementAt(0).Status.Should().Be(null);
            result.StockInformation.ElementAt(0).AvailabilityDate.Should().Be(new DateTime(2014, 04, 21));
        }

        /// <summary>
        /// Should get product stock information all for the specified products.
        /// </summary>
        [Fact]
        public void ShouldGetStockInformationAllForTheSpecifiedProducts()
        {
            // Arrange
            var request = new GetStockInformationRequest("shopname", new List<InventoryProduct> { new InventoryProduct { ProductId = "pid1" }}, StockDetailsLevel.All);
            var result = new GetStockInformationResult();
            var args = new ServicePipelineArgs(request, result);

            this._client.GetStocksInformation("shopname", Arg.Is<string[]>(ids => (ids.Length == 1) && (ids[0] == "pid1")), new System.Guid())
                .Returns(new[] { new StockInformationModel { ProductId = "pid1", AvailabilityDate = new DateTime(2014, 04, 21), Count = 10, Status = NopCommerce.NopInventoryService.StockStatus.InStock } });

            // Act
            this._processor.Process(args);

            // Assert
            result.StockInformation.Should().HaveCount(1);
            result.StockInformation.ElementAt(0).Product.ProductId.Should().Be("pid1");
            result.StockInformation.ElementAt(0).Count.Should().Be(10);
            result.StockInformation.ElementAt(0).Status.Should().Be(1);
            result.StockInformation.ElementAt(0).AvailabilityDate.Should().Be(new DateTime(2014, 04, 21));
        }

        /// <summary>
        /// Should not add new records if product stock information is not found.
        /// </summary>
        [Fact]
        public void ShouldNotAddNewRecordsIfProductStockInformationIsNotFound()
        {
            // Arrange
            var request = new GetStockInformationRequest("shopname", new List<InventoryProduct> { new InventoryProduct { ProductId = "noExist" } });
            var result = new GetStockInformationResult();
            var args = new ServicePipelineArgs(request, result);

            var results = new StockInformationModel[1];
            results[0] = null;
            this._client.GetStocksInformation("shopname", Arg.Is<string[]>(ids => (ids.Length == 1) && (ids[1] == "noExist")), new System.Guid()).Returns(results);

            // Act
            this._processor.Process(args);

            // Assert
            result.StockInformation.Should().HaveCount(1);
            result.StockInformation.ElementAt(0).Product.ProductId.Should().Be("noExist");
            result.StockInformation.ElementAt(0).Status.Should().Be(null);
            result.StockInformation.ElementAt(0).Count.Should().Be(0);
            result.StockInformation.ElementAt(0).AvailabilityDate.Should().Be(null);
        }
    }
}
