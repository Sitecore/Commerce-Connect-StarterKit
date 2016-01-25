// ----------------------------------------------------------------------------------------------
// <copyright file="CartControllerTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ShoppingCartControllerTest type.
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
namespace Sitecore.Commerce.StarterKit.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Threading;
    using FluentAssertions;
    using NSubstitute;
    using Sitecore;
    using Sitecore.Collections;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Commerce.StarterKit.Controllers;
    using Sitecore.Commerce.StarterKit.Models;
    using Sitecore.Commerce.StarterKit.Services;
    using Sitecore.Sites;
    using Sitecore.TestKit.Data.Items;
    using Sitecore.TestKit.Sites;
    using Xunit;
    using Texts = Sitecore.Commerce.StarterKit.Texts;

    /// <summary>
    ///   Defines the ShoppingCartControllerTest type.
    /// </summary>
    public class CartControllerTest : IDisposable
    {
        /// <summary>
        /// The service.
        /// </summary>
        private readonly ICartService service;

        /// <summary>
        /// The controller.
        /// </summary>
        private readonly CartController controller;

        /// <summary>
        /// The cart.
        /// </summary>
        private readonly Cart cart;

        /// <summary>
        /// The product service
        /// </summary>
        private readonly IProductService productService;

        /// <summary>
        /// The tree.
        /// </summary>
        private readonly TTree tree;

        /// <summary>
        /// The first item.
        /// </summary>
        private readonly TItem firstItem;

        /// <summary>
        /// The second item.
        /// </summary>
        private readonly TItem secondItem;

        /// <summary>
        /// The _is disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CartControllerTest" /> class.
        /// </summary>
        public CartControllerTest()
        {
            this.cart = new Cart();
            this.service = Substitute.For<ICartService>();
            this.productService = Substitute.For<IProductService>();
            this.service.GetCart().Returns(this.cart);

            this.controller = new CartController(this.service, this.productService);

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            this.firstItem = new TItem("Terminator") { { "ExternalID", "T-800" } };
            this.secondItem = new TItem("Test") { { "ExternalID", "TestProduct" } };
            this.tree = new TTree("web") { this.firstItem, this.secondItem };
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
            {
                if (null != controller)
                {
                    controller.Dispose();
                }
                if (null != tree)
                {
                    tree.Dispose();
                }
            }
            _isDisposed = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CartControllerTest"/> class.
        /// </summary>
        ~CartControllerTest()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Should add product to cart.
        /// </summary>
        [Fact]
        public void ShouldAddProductToCart()
        {
            // Arrange
            using (new SiteContextSwitcher(new TSiteContext("webshop")))
            {
                var returnedCart = new Cart { Lines = new ReadOnlyCollectionAdapter<CartLine> { new CartLine { Product = new CartProduct { ProductId = "T-800", Price = new Price(10.20m, "USD") }, Quantity = 1 } } };
                this.service.AddToCart("T-800", 1).Returns(returnedCart);

                // Act
                var result = this.controller.AddProduct("T-800", "1");

                // Assert
                result.Should().Be("1 item(s) in your cart.");
            }
        }

        /// <summary>
        ///   Should change product quantity.
        /// </summary>
        [Fact]
        public void ShouldChangeProductQuantity()
        {
            // arrange
            using (new SiteContextSwitcher(new TSiteContext("webshop")))
            {
                var returnedCart = new Cart { Lines = new ReadOnlyCollectionAdapter<CartLine> { new CartLine { Product = new CartProduct { ProductId = "TestProduct", Price = new Price(5.10m, "USD") }, Quantity = 5 } } };
                this.service.ChangeLineQuantity("TestProduct", 4).Returns(returnedCart);

                // act
                var result = this.controller.ChangeLineQuantity("TestProduct", 4);

                // assert
              result.Should().NotBeNull();
              var resultModel = (QuantityResultModel)result.Data;
              resultModel.TotalLineSum.Should().Be("$25.50");
              resultModel.TotalSum.Should().Be("$25.50");
            }
        }

        /// <summary>
        ///   Should get cart.
        /// </summary>
        [Fact]
        public void ShouldGetCart()
        {
            // Arrange
            using (new SiteContextSwitcher(new TSiteContext("webshop")))
            {
                this.cart.Lines = new ReadOnlyCollectionAdapter<CartLine>
                {
                    new CartLine { Product = new CartProduct { ProductId = "Line1" } }, 
                    new CartLine { Product = new CartProduct { ProductId = "Line2" } }
                };

                // Act
                var result = this.controller.GetCart();

                // Assert
                var resultModel = (CartModel)result.Data;
                resultModel.CartLines.Should().Contain(model => model.ProductId == "Line1");
                resultModel.CartLines.Should().Contain(model => model.ProductId == "Line2");
            }
        }

        ///// <summary>
        /////   Should get cart.
        ///// </summary>
        //[Fact]
        //public void ShouldGetCartProvidingNamesForProducts()
        //{
        //    // Arrange
        //    var stringDictionary = new StringDictionary { { "database", "web" } };
        //    using (new SiteContextSwitcher(new TSiteContext(stringDictionary)))
        //    {
        //        this.cart.Lines = new ReadOnlyCollectionAdapter<CartLine>
        //        {
        //            new CartLine { Product = new CartProduct { ProductId = "T-800" } }, 
        //            new CartLine { Product = new CartProduct { ProductId = "TestProduct" } }
        //        };

        //        this.productService.ReadProduct("T-800").Returns(Context.Database.GetItem(this.firstItem.ID));
        //        this.productService.ReadProduct("TestProduct").Returns(Context.Database.GetItem(this.secondItem.ID));

        //        // Act
        //        var result = this.controller.GetCart();

        //        // Assert
        //        var resultModel = (CartModel)result.Data;
        //        resultModel.CartLines.Should().Contain(model => model.ProductName == "Terminator");
        //        resultModel.CartLines.Should().Contain(model => model.ProductName == "Test");
        //    }
        //}

        /// <summary>
        ///   Should get cart from service.
        /// </summary>
        [Fact]
        public void ShouldGetCartFromService()
        {
            // Act
            this.controller.GetInfo();

            // Assert
            this.service.Received().GetCart();
        }

        ///// <summary>
        /////   Should remove product from cart.
        ///// </summary>
        //[Fact]
        //public void ShouldRemoveProductFromCart()
        //{
        //    // Arrange
        //    var returnedCart = new Cart { Lines = new ReadOnlyCollectionAdapter<CartLine>() };
        //    this.service.RemoveFromCart("TestProduct").Returns(returnedCart);

        //    // Act
        //    var result = this.controller.RemoveFromCart("TestProduct");

        //    // Assert
        //    result.Should().Be(Texts.NoProductsInCart);
        //}

        /// <summary>
        ///   Should display empty cart text.
        /// </summary>
        [Fact]
        public void ShouldReturnNoProductsInCartInfo()
        {
            // Act
            var result = this.controller.GetInfo();

            // Assert
            result.Should().Be(Texts.NoProductsInCart);
        }

        /// <summary>
        ///   Should return products count in cart.
        /// </summary>
        [Fact]
        public void ShouldReturnProductsCountAndTotalPriceInCartInfo()
        {
            // Arrange
            using (new SiteContextSwitcher(new TSiteContext("webshop")))
            {
                var cartLines = new ReadOnlyCollectionAdapter<CartLine>
                {
                    new CartLine { Product = new CartProduct { ProductId = "T-800", Price = new Price(10.20m, "USD") }, Quantity = 1 }, 
                    new CartLine { Product = new CartProduct { ProductId = "T-850", Price = new Price(5.15m, "USD") }, Quantity = 2 }, 
                };

                this.cart.Lines = cartLines;

                // Act
                var result = this.controller.GetInfo();

                // Assert
                result.Should().Be("3 item(s) in your cart.");
            }
        }


    }
}