// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartExtensionsTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CartExtensionsTest type.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests
{
  using System;
  using FluentAssertions;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Entities.Carts;
  using Xunit;
  using Xunit.Extensions;

  /// <summary>
  /// The cart extensions test.
  /// </summary>
  public class CartExtensionsTest
  {
    /// <summary>
    /// Should map cart base from model.
    /// </summary>
    [Fact]
    public void ShouldMapCartBaseFromModel()
    {
      // arrange
      var id = Guid.NewGuid();
      var cartModel = new ShoppingCartModel { CustomerGuid = id };
      var cartBase = new CartBase();

      // act
      cartBase.MapCartBaseFromModel(cartModel);

      // assert
      cartBase.ExternalId.Should().Be(id.ToString("B").ToUpper());
    }

    /// <summary>
    /// Should map cart total.
    /// </summary>
    [Fact]
    public void ShouldMapCartTotal()
    {
      // arrange
      var cartModel = new ShoppingCartModel { Total = 15.00m, ShoppingItems = new ShoppingCartItemModel[0] };
      var cart = new Cart();

      // act
      cart.MapCartFromModel(cartModel);

      // assert
      cart.Total.Amount.Should().Be(15.00m);
    }

    /// <summary>
    /// Should not map total if it is zero or null.
    /// </summary>
    /// <param name="value">The value.</param>
    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void ShouldNotMapTotalIfItIsZeroOrNull(int? value)
    {
      var cartModel = new ShoppingCartModel { Total = value, ShoppingItems = new ShoppingCartItemModel[0] };
      var cart = new Cart();

      // act
      cart.MapCartFromModel(cartModel);

      // assert
      cart.Total.Should().BeNull();
    }
  }
}