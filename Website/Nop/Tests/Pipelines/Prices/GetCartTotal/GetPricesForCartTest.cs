// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetPricesForCartTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The get cart total test.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Prices.GetCartTotal
{
    using System.Linq;
    using FluentAssertions;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Prices.GetCartTotal;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services.Prices;
    using Xunit;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

    /// <summary>
    /// The get cart total test.
    /// </summary>
    public class GetPricesForCartTest
    {
        /// <summary>
        /// Should calculate cart line totals.
        /// </summary>
        [Fact]
        public void ShouldCalculateCartLineTotals()
        {
            // arrange
            var cart = new Cart { Lines = new ReadOnlyCollection<CartLine>(new List<CartLine> { new CartLine { Product = new CartProduct { Price = new Price(1050, "USD") }, Quantity = 2 } }) };

            var processor = new GetPricesForCart();
            var request = new GetCartTotalRequest { Cart = cart };
            var args = new ServicePipelineArgs(request, new GetCartTotalResult());

            // act
            processor.Process(args);

            // assert
            var resultLine = ((GetCartTotalResult)args.Result).Cart.Lines.First();
            resultLine.Total.Amount.Should().Be(2100);
            resultLine.Total.CurrencyCode.Should().Be("USD");
        }

        /// <summary>
        /// Should calculate cart totals.
        /// </summary>
        [Fact]
        public void ShouldCalculateCartTotals()
        {
            // arrange
            var cart = new Cart
            {
                Lines = new ReadOnlyCollection<CartLine>(new List<CartLine>
        {
          new CartLine { Product = new CartProduct { Price = new Price(1050, "USD") }, Quantity = 2 },
          new CartLine { Product = new CartProduct { Price = new Price(2000, "USD") }, Quantity = 1 }
        })
            };

            var processor = new GetPricesForCart();
            var request = new GetCartTotalRequest { Cart = cart };
            var args = new ServicePipelineArgs(request, new GetCartTotalResult());

            // act
            processor.Process(args);

            // assert
            var resultCart = ((GetCartTotalResult)args.Result).Cart;
            resultCart.Total.Amount.Should().Be(4100);
            resultCart.Total.CurrencyCode.Should().Be("USD");
        }

        /// <summary>
        /// Shoulds calculate cart totals in specified currency.
        /// </summary>
        [Fact]
        public void ShouldCalculateCartTotalsInCurrency()
        {
            // arrange
            var cart = new Cart
            {
                Lines = new ReadOnlyCollectionAdapter<CartLine>
                {
                    new CartLine { Product = new CartProduct { Price = new Price(1050, "USD") }, Quantity = 2 },
                    new CartLine { Product = new CartProduct { Price = new Price(2000, "USD") }, Quantity = 1 }
                }
            };

            var processor = new GetPricesForCart();
            var request = new GetCartTotalRequest { Cart = cart, CurrencyCode = "CAD" };
            var args = new ServicePipelineArgs(request, new GetCartTotalResult());

            // act
            processor.Process(args);

            // assert
            var resultCart = ((GetCartTotalResult)args.Result).Cart;
            resultCart.Total.Amount.Should().Be(4100);
            resultCart.Total.CurrencyCode.Should().Be("CAD");
        }
    }
}