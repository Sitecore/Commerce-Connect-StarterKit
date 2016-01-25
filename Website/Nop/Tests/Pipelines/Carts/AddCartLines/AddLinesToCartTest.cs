// ----------------------------------------------------------------------------------------------
// <copyright file="AddLinesToCartTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the AddLinesToCartTest type.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.AddCartLines
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Sitecore.Commerce.Connectors.NopCommerce;
    using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.AddCartLines;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Carts;
    using Xunit;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

    /// <summary>
    /// The add lines to cart test.
    /// </summary>
    public class AddLinesToCartTest
    {
        /// <summary>
        /// The processor.
        /// </summary>
        private readonly AddLinesToCart processor;

        /// <summary>
        /// The visitor id.
        /// </summary>
        private readonly Guid visitorId;

        /// <summary>
        /// The request.
        /// </summary>
        private readonly AddCartLinesRequest request;

        /// <summary>
        /// The result.
        /// </summary>
        private readonly CartResult result;

        /// <summary>
        /// The args.
        /// </summary>
        private readonly ServicePipelineArgs args;

        /// <summary>
        /// The client.
        /// </summary>
        private readonly ICartsServiceChannel client;

        /// <summary>
        /// The cart.
        /// </summary>
        private readonly Cart cart;

        /// <summary>
        /// The line to add
        /// </summary>
        private readonly CartLine lineToAdd;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddLinesToCartTest"/> class.
        /// </summary>
        public AddLinesToCartTest()
        {
          this.visitorId = Guid.NewGuid();

          this.cart = new Cart
          {
            ExternalId = this.visitorId.ToString(),
            Lines = new ReadOnlyCollection<CartLine>(new List<CartLine> { new CartLine() })
          };

          this.lineToAdd = new CartLine
          {
            Product = new CartProduct
            {
              ProductId = "100500",
              Price = new Price { Amount = 100 }
            },
            Quantity = 12
          };

          this.request = new AddCartLinesRequest(this.cart, new[] { this.lineToAdd });
          this.result = new CartResult();
          this.args = new ServicePipelineArgs(this.request, this.result);

          this.client = Substitute.For<ICartsServiceChannel>();

          var clientFactory = Substitute.For<ServiceClientFactory>();
          clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this.client);

          this.processor = new AddLinesToCart { ClientFactory = clientFactory };
        }

        /// <summary>
        /// Should add lines to cart.
        /// </summary>
        [Fact]
        public void ShouldAddLinesToCart()
        {
          // arrange
          var inintialLineModel = new ShoppingCartItemModel();
          var initialCartModel = new ShoppingCartModel { CustomerGuid = this.visitorId, ShoppingItems = new[] { inintialLineModel } };
          this.client.GetCart(this.visitorId,null).Returns(initialCartModel);

          var addedLineModel = new ShoppingCartItemModel { Id = "LineId2", LineTotal = 1010, Price = 50, ProductId = 100500, Quantity = 12 };
          var resultingCartModel = new ShoppingCartModel { CustomerGuid = this.visitorId, Total = 2233, ShoppingItems = new[] { inintialLineModel, addedLineModel } };
          this.client.AddProduct(this.visitorId, "100500", 12).Returns(resultingCartModel);

          // act
          this.processor.Process(this.args);

          // assert
          this.result.Cart.Lines.Count.Should().Be(2);
          this.result.Cart.Total.Amount.Should().Be(2233);
          this.result.Cart.Lines.Should().Contain(cl =>
            cl.ExternalCartLineId == "LineId2" &&
            cl.Product.ProductId == "100500" &&
            cl.Quantity == 12 &&
            cl.Product.Price.Amount == 50 &&
            cl.Total.Amount == 1010);
        }
    }
}