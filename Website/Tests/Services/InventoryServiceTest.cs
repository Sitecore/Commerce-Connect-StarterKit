// ----------------------------------------------------------------------------------------------
// <copyright file="InventoryServiceTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The inventory service test.
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
namespace Sitecore.Commerce.StarterKit.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using NSubstitute;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Services.Inventory;
    using Sitecore.Commerce.StarterKit.Services;
    using Xunit;
    using Sitecore.Commerce.Contacts;

    /// <summary>
    /// The inventory service test.
    /// </summary>
    public class InventoryServiceTest
    {
        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly InventoryServiceProvider _serviceProvider;

        /// <summary>
        /// The inventory service.
        /// </summary>
        private readonly InventoryService _inventoryService;

        /// <summary>
        /// The visitor factory.
        /// </summary>
        private ContactFactory _contactFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryServiceTest"/> class.
        /// </summary>
        public InventoryServiceTest()
        {
            this._serviceProvider = Substitute.For<InventoryServiceProvider>();
            this._contactFactory = Substitute.For<ContactFactory>();
            this._contactFactory.GetContact().Returns("John Doe");
            this._inventoryService = new InventoryService(this._serviceProvider, this._contactFactory);
        }

        /// <summary>
        /// Should get the product stock information by product Ids.
        /// </summary>
        [Fact]
        public void ShouldGetStockInformation()
        {
            // Arrange
            var stockInfo1 = new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock, AvailabilityDate = new DateTime(2014, 3, 12) };
            var stockInfo2 = new StockInformation { Product = new InventoryProduct { ProductId = "1002" }, Status = StockStatus.InStock, AvailabilityDate = new DateTime(2014, 3, 12) };
            var stockInfos = new List<StockInformation> { stockInfo1, stockInfo2 };
            var products = new List<string> { "1001", "1002" };

            this._serviceProvider
                .GetStockInformation(Arg.Is<GetStockInformationRequest>(r => r.ShopName == "shopname" && r.Products.First().ProductId == "1001" && r.Products.Last().ProductId == "1002" && r.Products.Count() == 2 && r.DetailsLevel == StockDetailsLevel.All))
                .Returns(new GetStockInformationResult { StockInformation = stockInfos });

            // Act & Assert
            this._inventoryService.GetStockInformation("shopname", products, StockDetailsLevel.All, string.Empty, string.Empty).ShouldBeEquivalentTo(stockInfos);
        }

        /// <summary>
        /// Should return only product id stock information if no stock information found.
        /// </summary>
        [Fact]
        public void ShouldReturnOnlyProductIdIfNoStockInformationFound()
        {
            // Arrange
            var stockInfos = new List<StockInformation> { new StockInformation { Product = new InventoryProduct { ProductId = "1001" } } };

            this._serviceProvider
                .GetStockInformation(Arg.Is<GetStockInformationRequest>(r => r.ShopName == "shopname" && r.Products.Single().ProductId == "1001"  && r.Products.Count() == 1 && r.DetailsLevel == StockDetailsLevel.All))
                .Returns(new GetStockInformationResult { StockInformation = stockInfos });

            // Act & Assert
            this._inventoryService.GetStockInformation("shopname", new List<string> { "1001" }, StockDetailsLevel.All, string.Empty, string.Empty).ShouldBeEquivalentTo(stockInfos);
        }

        /// <summary>
        /// Should return stock information if duplicated product ids passed.
        /// </summary>
        [Fact]
        public void ShouldReturnOnlyOneStockInformationIfDuplicatedProductIdsPassed()
        {
            // Arrange
            var stockInfo = new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock, AvailabilityDate = new DateTime(2014, 3, 12) };
            var invProduct1 = new InventoryProduct { ProductId = "1001" };

            this._serviceProvider
                .GetStockInformation(Arg.Is<GetStockInformationRequest>(r => r.ShopName == "shopname" && r.Products.Contains(invProduct1) && r.Products.Count() == 1 && r.DetailsLevel == StockDetailsLevel.All))
                .ReturnsForAnyArgs(new GetStockInformationResult { StockInformation = new List<StockInformation> { stockInfo } });

            // Act
            var stockInfos = this._inventoryService.GetStockInformation("shopname", new List<string> { "1001", "1001" }, StockDetailsLevel.All, string.Empty, string.Empty);

            // Assert
            stockInfos.Count.Should().Be(1);
            stockInfos.FirstOrDefault().ShouldBeEquivalentTo(stockInfo);
        }

        /// <summary>
        /// Should get the product pre-orderable information by product Ids.
        /// </summary>
        [Fact]
        public void ShouldGetPreOrderableInformation()
        {
            // Arrange
            var orderableInfo1 = new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock };
            var orderableInfo2 = new OrderableInformation { Product = new InventoryProduct { ProductId = "1002" }, Status = StockStatus.InStock };
            var orderableInfos = new List<OrderableInformation> { orderableInfo1, orderableInfo2 };
            var ids = new List<string> { "1001", "1002" };

            this._serviceProvider
                .GetPreOrderableInformation(Arg.Is<GetPreOrderableInformationRequest>(r => r.ShopName == "shopname" && r.Products.First().ProductId == "1001" && r.Products.Last().ProductId == "1002" && r.Products.Count() == 2))
                .Returns(new GetPreOrderableInformationResult { OrderableInformation = orderableInfos });

            // Act & Assert
            this._inventoryService.GetPreOrderableInformation("shopname", ids, string.Empty, string.Empty).ShouldBeEquivalentTo(orderableInfos);
        }
                
        /// <summary>
        /// Should return only product id orderable information if no orderable information found.
        /// </summary>
        [Fact]
        public void ShouldReturnOnlyProductIdIfNoPreOrderableInformationFound()
        {
            // Arrange
            var orderableInfo1 = new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" } };
            var orderableInfo2 = new OrderableInformation { Product = new InventoryProduct { ProductId = "1002" } };
            var orderableInfos = new List<OrderableInformation> { orderableInfo1, orderableInfo2 };
            var ids = new List<string> { "1001", "1002" };

            this._serviceProvider
               .GetPreOrderableInformation(Arg.Is<GetPreOrderableInformationRequest>(r => r.ShopName == "shopname" && r.Products.First().ProductId == "1001" && r.Products.Last().ProductId == "1002" && r.Products.Count() == 2))
               .Returns(new GetPreOrderableInformationResult { OrderableInformation = orderableInfos });

            // Act & Assert
            this._inventoryService.GetPreOrderableInformation("shopname", ids, string.Empty, string.Empty).ShouldBeEquivalentTo(orderableInfos);
        }

        /// <summary>
        /// Should return pre-orderable information if duplicated product ids passed.
        /// </summary>
        [Fact]
        public void ShouldReturnOnlyOnePreOrderableInformationIfDuplicatedProductIdsPassed()
        {
            // Arrange
            var orderableInfo = new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock };
            var invProduct1 = new InventoryProduct { ProductId = "1001" };

            this._serviceProvider
                .GetPreOrderableInformation(Arg.Is<GetPreOrderableInformationRequest>(r => r.ShopName == "shopname" && r.Products.Contains(invProduct1) && r.Products.Count() == 1))
                .ReturnsForAnyArgs(new GetPreOrderableInformationResult { OrderableInformation = new List<OrderableInformation> { orderableInfo } });

            // Act
            var orderableInfos = this._inventoryService.GetPreOrderableInformation("shopname", new List<string> { "1001", "1001" }, string.Empty, string.Empty);

            // Assert
            orderableInfos.Count.Should().Be(1);
            orderableInfos.FirstOrDefault().ShouldBeEquivalentTo(orderableInfo);
        }

        /// <summary>
        /// Should get the product back-orderable information by product Ids.
        /// </summary>
        [Fact]
        public void ShouldGetBackOrderableInformation()
        {
            // Arrange
            var orderableInfo1 = new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock };
            var orderableInfo2 = new OrderableInformation { Product = new InventoryProduct { ProductId = "1002" }, Status = StockStatus.InStock };
            var orderableInfos = new List<OrderableInformation> { orderableInfo1, orderableInfo2 };
            var ids = new List<string> { "1001", "1002" };

            this._serviceProvider
                .GetBackOrderableInformation(Arg.Is<GetBackOrderableInformationRequest>(r => r.ShopName == "shopname" && r.Products.First().ProductId == "1001" && r.Products.Last().ProductId == "1002" && r.Products.Count() == 2))
                .Returns(new GetBackOrderableInformationResult { OrderableInformation = orderableInfos });

            // Act & Assert
            this._inventoryService.GetBackOrderableInformation("shopname", ids, string.Empty, string.Empty).ShouldBeEquivalentTo(orderableInfos);
        }

        /// <summary>
        /// Should return only product id orderable information if no orderable information found.
        /// </summary>
        [Fact]
        public void ShouldReturnOnlyProductIdIfNoBackOrderableInformationFound()
        {
            // Arrange
            var orderableInfo1 = new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" } };
            var orderableInfo2 = new OrderableInformation { Product = new InventoryProduct { ProductId = "1002" } };
            var orderableInfos = new List<OrderableInformation> { orderableInfo1, orderableInfo2 };
            var ids = new List<string> { "1001", "1002" };

            this._serviceProvider
                 .GetBackOrderableInformation(Arg.Is<GetBackOrderableInformationRequest>(r => r.ShopName == "shopname" && r.Products.First().ProductId == "1001" && r.Products.Last().ProductId == "1002" && r.Products.Count() == 2))
                 .Returns(new GetBackOrderableInformationResult { OrderableInformation = orderableInfos });

            // Act & Assert
            this._inventoryService.GetBackOrderableInformation("shopname", ids, string.Empty, string.Empty).ShouldBeEquivalentTo(orderableInfos);
        }
        
        /// <summary>
        /// Should return back-orderable information if duplicated product ids passed.
        /// </summary>
        [Fact]
        public void ShouldReturnOnlyOneBackOrderableInformationIfDuplicatedProductIdsPassed()
        {
            // Arrange
            var orderableInfo = new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock };
            var invProduct1 = new InventoryProduct { ProductId = "1001" };

            this._serviceProvider
                .GetBackOrderableInformation(Arg.Is<GetBackOrderableInformationRequest>(r => r.ShopName == "shopname" && r.Products.Contains(invProduct1) && r.Products.Count() == 1))
                .ReturnsForAnyArgs(new GetBackOrderableInformationResult { OrderableInformation = new List<OrderableInformation> { orderableInfo } });

            // Act
            var orderableInfos = this._inventoryService.GetBackOrderableInformation("shopname", new List<string> { "1001", "1001" }, string.Empty, string.Empty);

            // Assert
            orderableInfos.Count.Should().Be(1);
            orderableInfos.FirstOrDefault().ShouldBeEquivalentTo(orderableInfo);
        }
    }
}
