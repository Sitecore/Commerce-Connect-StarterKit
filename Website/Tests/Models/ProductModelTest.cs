// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductModelTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The product model test.
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
using System.Globalization;

namespace Sitecore.Commerce.StarterKit.Tests.Models
{
    using System;
    using FluentAssertions;
    using Sitecore.Commerce.StarterKit.Models;
    using Xunit;
    using Xunit.Extensions;
    using Sitecore.Commerce.Entities.Inventory;

    /// <summary>
    /// The product model test.
    /// </summary>
    public class ProductModelTest
    {
        /// <summary>
        /// Should get manufacturers from non empty list of models.
        /// </summary>
        [Fact]
        public void ShouldGetManufacturersFromNonEmptyListOfModels()
        {
            // arrange
            var manufacturers = new[] { new ManufacturerModel { Name = "Mercedes" }, new ManufacturerModel { Name = "AMG" } };
                        
            // act
            var model = new ProductModel(10m, new StockInformation(), manufacturers);

            // assert
            model.Manufacturer.Should().Be("Mercedes, AMG");
            model.Categories.Should().Be("-");
        }

        /// <summary>
        /// Should get dash from empty list of models.
        /// </summary>
        [Fact]
        public void ShouldGetDashFromEmptyListOfModels()
        {
            // arrange

            // act
            var model = new ProductModel(10m, new StockInformation(), new ManufacturerModel[0]);

            // assert
            model.Manufacturer.Should().Be("-");
            model.Categories.Should().Be("-");
        }

        /// <summary>
        /// Should get categories from non empty list of models.
        /// </summary>
        [Fact]
        public void ShouldGetCategoriesFromNonEmptyListOfModels()
        {
            // arrange
            var productClasses = new[] { new CategoryModel { Name = "Cars" }, new CategoryModel { Name = "Luxury" } };

            // act
            var model = new ProductModel(10m, new StockInformation(), productClasses: productClasses);

            // assert
            model.Categories.Should().Be("Cars, Luxury");
            model.Manufacturer.Should().Be("-");
        }

        /// <summary>
        /// Should get dash from non empty list of models.
        /// </summary>
        [Fact]
        public void ShouldGetDashFromEmptyListOfClassificationModels()
        {
            // arrange

            // act
            var model = new ProductModel(10m, new StockInformation(), productClasses: new CategoryModel[0]);

            // assert
            model.Categories.Should().Be("-");
            model.Manufacturer.Should().Be("-");
        }

        /// <summary>
        /// Should get formatted price.
        /// </summary>
        [Fact]
        public void ShouldGetFormattedPrice()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation());

            // assert
            model.FormattedPrice.Should().Be(5m.ToString("c", new CultureInfo("en-US")));
        }

        /// <summary>
        /// Should get dash if price is zero.
        /// </summary>
        [Fact]
        public void ShouldGetDashIfPriceIsZero()
        {
            // arrange

            // act
            var model = new ProductModel(0m, new StockInformation());

            // assert
            model.FormattedPrice.Should().Be("-");
        }

        /// <summary>
        /// Should get formatted short description.
        /// </summary>
        /// <param name="shortDescription"></param>
        /// <param name="expectedResult"></param>
        [Theory]
        [InlineData("Short description", "Short description")]
        [InlineData(null, "-")]
        [InlineData("", "-")]
        public void ShouldGetFormattedShortDescription(string shortDescription, string expectedResult)
        {
            // arrange

            // act
            var model = new ProductModel(0m, new StockInformation()) { ShortDescription = shortDescription };

            // assert
            model.FormattedShortDescription.Should().Be(expectedResult);
        }

        /// <summary>
        /// Should get availability date.
        /// </summary>
        [Fact]
        public void ShouldGetAvailabilityDate()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock, AvailabilityDate = new DateTime(2014, 3, 12) });

            // assert
            model.AvailabilityDate.Should().Be((new DateTime(2014, 3, 12)));
        }

        /// <summary>
        /// Should get formatted availability date.
        /// </summary>
        [Fact]
        public void ShouldGetFormattedAvailabilityDate()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock, AvailabilityDate = new DateTime(2014, 3, 12) });

            // assert
            model.FormattedAvailabilityDate.Should().Be((new DateTime(2014, 3, 12)).ToShortDateString());
        }

        /// <summary>
        /// Should get dash if availability date is null.
        /// </summary>
        [Fact]
        public void ShouldGetDashStringIfAvailabilityDateIsNull()
        {
            // arrange

            // act
            var model = new ProductModel(0m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock });

            // assert
            model.FormattedAvailabilityDate.Should().Be("-");
        }

        /// <summary>
        /// Should get status.
        /// </summary>
        [Fact]
        public void ShouldGetStatus()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock });

            // assert
            model.Status.Should().Be(StockStatus.InStock);
        }

        /// <summary>
        /// Should get status name.
        /// </summary>  
        [Fact]
        public void ShouldGetStatusName()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock });

            // assert
            model.StatusName.Should().Be("InStock");
        }

        /// <summary>
        /// Should get dash if status is null.
        /// </summary>
        [Fact]
        public void ShouldGetDashIfStatusIsNull()
        {
            // arrange

            // act
            var model = new ProductModel(0m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" } });

            // assert
            model.StatusName.Should().Be("-");
        }

        /// <summary>
        /// Should get in-stock date.
        /// </summary>
        [Fact]
        public void ShouldGetInStockDate()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" } }, new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.PreOrderable, InStockDate = new DateTime(2014, 3, 12) });

            // assert
            model.InStockDate.Should().Be((new DateTime(2014, 3, 12)));
        }
        
        /// <summary>
        /// Should get formatted in-stock date.
        /// </summary>
        [Fact]
        public void ShouldGetFormattedInStockDate()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" } }, new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.PreOrderable, InStockDate = new DateTime(2014, 3, 12) });

            // assert
            model.FormattedInStockDate.Should().Be((new DateTime(2014, 3, 12)).ToShortDateString());
        }

        /// <summary>
        /// Should get dash if in-stock date is null.
        /// </summary>
        [Fact]
        public void ShouldGetDashStringIfInStockDateIsNull()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" } }, new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.PreOrderable });

            // assert
            model.FormattedInStockDate.Should().Be("-");
        }

        /// <summary>
        /// Should get remaining quantity.
        /// </summary>
        [Fact]
        public void ShouldGetRemainingQuantity()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" } }, new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, RemainingQuantity = 10 });

            // assert
            model.RemainingQuantity.Should().Be(10);
        }

        /// <summary>
        /// Should get formatted remaining quantity.
        /// </summary>
        [Fact]
        public void ShouldGetFormattedRemainingQuantity()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" } }, new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" }, RemainingQuantity = 10 });

            // assert
            model.FormattedRemainingQuantity.Should().Be("10");
        }

        /// <summary>
        /// Should get dash if formatted remaining quantity is null.
        /// </summary>
        [Fact]
        public void ShouldGetDashStringIfFormattedRemainingQuantityIsNull()
        {
            // arrange

            // act
            var model = new ProductModel(5m, new StockInformation { Product = new InventoryProduct { ProductId = "1001" } }, new OrderableInformation { Product = new InventoryProduct { ProductId = "1001" } });

            // assert
            model.FormattedRemainingQuantity.Should().Be("-");
        }
    }
}