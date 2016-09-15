// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartServiceTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The cart service test.
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
// -----------------------------------------------------------------
namespace Nop.Plugin.Sitecore.Commerce.Carts.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using Nop.Core;
  using Nop.Core.Domain.Catalog;
  using Nop.Core.Domain.Customers;
  using Nop.Core.Domain.Orders;
  using Nop.Core.Infrastructure;
  using Nop.Plugin.Sitecore.Commerce.Carts;
  using Nop.Plugin.Sitecore.Commerce.Common.Models;
  using Nop.Services.Catalog;
  using Nop.Services.Customers;
  using Nop.Services.Orders;
  using NSubstitute;
  using Xunit;

  /// <summary>
  /// The cart service test.
  /// </summary>
  public class CartsServiceTest
  {
    /// <summary>
    /// The cart service.
    /// </summary>
    private readonly ICartsService cartService;

    /// <summary>
    /// The customer service.
    /// </summary>
    private readonly ICustomerService customerService;

    /// <summary>
    /// The product service
    /// </summary>
    private readonly IProductService productService;

    /// <summary>
    /// The shopping cart service.
    /// </summary>
    private readonly IShoppingCartService shoppingCartService;

    /// <summary>
    /// The order total calculation service.
    /// </summary>
    private readonly IOrderTotalCalculationService orderTotalCalculationService;

    /// <summary>
    /// The customer.
    /// </summary>
    private readonly Customer customer;

    /// <summary>
    /// The customer GUID.
    /// </summary>
    private readonly Guid customerGuid;

    /// <summary>
    /// The shopping cart item.
    /// </summary>
    private readonly ShoppingCartItem shoppingCartItem;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartsServiceTest"/> class.
    /// </summary>
    public CartsServiceTest()
    {
      var serviceLocator = Substitute.For<IEngine>();
      EngineContext.Replace(serviceLocator);

      this.customerService = Substitute.For<ICustomerService>();
      this.productService = Substitute.For<IProductService>();
      this.shoppingCartService = Substitute.For<IShoppingCartService>();
      this.orderTotalCalculationService = Substitute.For<IOrderTotalCalculationService>();

      serviceLocator.Resolve<ICustomerService>().Returns(this.customerService);
      serviceLocator.Resolve<IProductService>().Returns(this.productService);
      serviceLocator.Resolve<IShoppingCartService>().Returns(this.shoppingCartService);
      serviceLocator.Resolve<IOrderTotalCalculationService>().Returns(this.orderTotalCalculationService);

      this.cartService = new CartsService();

      this.shoppingCartItem = new ShoppingCartItem
      {
        Id = 1050,
        ShoppingCartType = ShoppingCartType.ShoppingCart,
        ProductId = 1051,
        Product = new Product { Price = 12.55m, Id = 100500 },
        Quantity = 2,
        StoreId = 100500
      };

      this.customerGuid = Guid.NewGuid();

      this.customer = new Customer { CustomerGuid = this.customerGuid, Id = 111222 };
      this.customer.ShoppingCartItems.Add(this.shoppingCartItem);
      this.customer.ShoppingCartItems.Add(new ShoppingCartItem { Id = 9999, ShoppingCartType = ShoppingCartType.Wishlist });
    }

    /// <summary>
    /// Should get all carts.
    /// </summary>
    [Fact]
    public void ShouldGetAllCarts()
    {
      // arrange
      this.customerService.GetAllCustomers(loadOnlyWithShoppingCart: true, sct: ShoppingCartType.ShoppingCart)
        .Returns(new PagedList<Customer>(new[] { this.customer }, 0, 10));

      // act
      var carts = this.cartService.GetCarts();

      // assert
      var resultCart = carts.Single();
      resultCart.CustomerGuid.Should().Be(this.customerGuid);
      resultCart.CustomerId.Should().Be(111222);

      var resultCartLine = resultCart.ShoppingItems.Single();

      resultCartLine.Id.Should().Be("1050");
      resultCartLine.ProductId.Should().Be(1051);
      resultCartLine.Price.Should().Be(12.55m);
      resultCartLine.Quantity.Should().Be(2);
    }

    /// <summary>
    /// Should get carts by customer id.
    /// </summary>
    [Fact]
    public void ShouldGetCart()
    {
      // arrange
      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);

      // act
      var resultCart = this.cartService.GetCart(this.customerGuid);

      // assert
      resultCart.CustomerGuid.Should().Be(this.customerGuid);
      resultCart.CustomerId.Should().Be(111222);

      var resultCartLine = resultCart.ShoppingItems.Single();

      resultCartLine.Id.Should().Be("1050");
    }

    /// <summary>
    /// Should get new cart if customer not exist.
    /// </summary>
    [Fact]
    public void ShouldGetNewCartIfCustomerNotExist()
    {
      // arrange
      var nonExistentCustomerGuid = Guid.NewGuid();
      this.customerService.GetCustomerByGuid(Arg.Any<Guid>()).Returns(x => null);
      this.customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests).Returns(new CustomerRole());

      // act
      var cart = this.cartService.GetCart(nonExistentCustomerGuid);

      // assert
      cart.CustomerGuid.Should().Be(nonExistentCustomerGuid);
    }

    /// <summary>
    /// Should delete cart by id.
    /// </summary>
    [Fact]
    public void ShouldDeleteCartById()
    {
      // arrange
      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);

      // act
      this.cartService.DeleteCart(this.customerGuid);

      // assert
      this.shoppingCartService.Received(1).DeleteShoppingCartItem(this.shoppingCartItem);
    }

    /// <summary>
    /// Should create cart by id.
    /// </summary>
    [Fact]
    public void ShouldCreateCartById()
    {
      // arrange
      this.customerService.GetCustomerByGuid(Arg.Any<Guid>()).Returns(x => null);
      this.customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests).Returns(new CustomerRole());

      // act
      this.cartService.CreateCart(this.customerGuid);

      // assert
      this.customerService.Received(1).InsertCustomer(Arg.Is<Customer>(c => c.CustomerGuid == this.customerGuid));
    }

    /// <summary>
    /// Should add cart line.
    /// </summary>
    [Fact]
    public void ShouldAddNewProductToCart()
    {
      // arrange
      var newCartItem = new ShoppingCartItem
      {
        Id = 9999,
        ShoppingCartType = ShoppingCartType.ShoppingCart,
        Product = new Product { Id = 100500 },
        Quantity = 5
      };

      var customerWithNewCartItem = new Customer { CustomerGuid = this.customerGuid, Id = 111222 };
      customerWithNewCartItem.ShoppingCartItems.Add(this.shoppingCartItem);
      customerWithNewCartItem.ShoppingCartItems.Add(newCartItem);

      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer, customerWithNewCartItem);

      this.shoppingCartService.FindShoppingCartItemInTheCart(
        Arg.Any<IList<ShoppingCartItem>>(),
        ShoppingCartType.ShoppingCart,
        Arg.Is<Product>(pv => pv.Id == 100500)).Returns(newCartItem);

      this.productService.GetProductById(100500).Returns(new Product { Id = 100500 });

      // act
      var resultCartModel = this.cartService.AddProduct(this.customerGuid, "100500", 5);

      // assert
      resultCartModel.ShoppingItems.Count.Should().Be(2);
      resultCartModel.ShoppingItems.Should().Contain(sci => sci.Quantity == 5);
    }

    /// <summary>
    /// Should temporary add all cart to zero store if it is not specified.
    /// </summary>
    [Fact]
    public void ShouldTemporaryAddAllCartToZeroStoreIfItIsNotSpecified()
    {
      // arrange
      ShoppingCartItem cartItem = null;

      var customerWithCarts = new Customer { CustomerGuid = this.customerGuid, Id = 111222 };
      customerWithCarts.ShoppingCartItems.Add(this.shoppingCartItem);
      this.shoppingCartService.FindShoppingCartItemInTheCart(Arg.Any<IList<ShoppingCartItem>>(), ShoppingCartType.ShoppingCart, Arg.Is<Product>(pv => pv.Id == 100500)).Returns(cartItem);

      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer, customerWithCarts);
      var productVariant = new Product { Id = 100500 };
      this.productService.GetProductById(100500).Returns(productVariant);

      // act
      var resultCartModel = this.cartService.AddProduct(this.customerGuid, "100500", 5);

      // Assert
      this.shoppingCartService.Received().GetShoppingCartItemWarnings(customerWithCarts, ShoppingCartType.ShoppingCart, productVariant, 0, string.Empty, decimal.Zero, 5, false, true, false, false, false);
      this.shoppingCartService.Received().AddToCart(customerWithCarts, productVariant, ShoppingCartType.ShoppingCart, 0, string.Empty, decimal.Zero, 5, true);
    }

    /// <summary>
    /// Should add existing product to cart.
    /// </summary>
    [Fact]
    public void ShouldAddExistingProductToCart()
    {
      // arrange
      var updatedCartItem = new ShoppingCartItem
      {
        Id = 9999,
        ShoppingCartType = ShoppingCartType.ShoppingCart,
        Product = new Product { Id = 100500 },
        Quantity = 7
      };

      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);

      this.productService.GetProductById(100500).Returns(new Product { Id = 100500 });

      this.shoppingCartService.FindShoppingCartItemInTheCart(
        Arg.Any<IList<ShoppingCartItem>>(),
        ShoppingCartType.ShoppingCart,
        Arg.Is<Product>(pv => pv.Id == 100500)).Returns(this.shoppingCartItem);

      var customerWithUpdatedCartItem = new Customer { CustomerGuid = this.customerGuid, Id = 111222 };
      customerWithUpdatedCartItem.ShoppingCartItems.Add(updatedCartItem);

      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer, customerWithUpdatedCartItem);

      // act
      var resultCartModel = this.cartService.AddProduct(this.customerGuid, "100500", 5);

      // assert
      this.shoppingCartService.Received(1).AddToCart(
        this.customer,
        Arg.Is<Product>(pv => pv.Id == 100500),
        ShoppingCartType.ShoppingCart,
        100500,
        string.Empty,
        decimal.Zero,
        5,
        true);

      resultCartModel.ShoppingItems.Count.Should().Be(1);
      resultCartModel.ShoppingItems.Should().Contain(sci => sci.Quantity == 7);
    }

    /// <summary>
    /// Should not add product if product variant doesn't exist.
    /// </summary>
    [Fact]
    public void ShouldNotAddProductIfProductVariantNotExist()
    {
      // arrange
      var currentCartModel = new ShoppingCartModel { CustomerGuid = this.customerGuid,
        ShoppingItems = new List<ShoppingCartItemModel>() { new ShoppingCartItemModel() { Id = "1111"} } };

      this.productService.GetProductById(100500).Returns(x => null);
      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);

      // act
      var resultCartModel = this.cartService.AddProduct(this.customerGuid, "100500", 5);

      // assert
      resultCartModel.CustomerGuid.Should().Be(currentCartModel.CustomerGuid);
      resultCartModel.ShoppingItems.Count.Should().Be(currentCartModel.ShoppingItems.Count);
    }

    /// <summary>
    /// Should not update cart if product to remove doesn't exist.
    /// </summary>
    [Fact]
    public void ShouldNotUpdateCartIfProductToUpdateNotExist()
    {
      // arrange
      var currentCartModel = new ShoppingCartModel
      {
        CustomerGuid = this.customerGuid,
        ShoppingItems = new List<ShoppingCartItemModel>() { new ShoppingCartItemModel() { Id = "1111" } }
      };

      this.productService.GetProductById(100500).Returns(x => null);
      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);

      // act
      var resultCartModel = this.cartService.UpdateQuantity(this.customerGuid, "100500", 5);

      // assert
      resultCartModel.ShoppingItems.Count.Should().Be(currentCartModel.ShoppingItems.Count);
    }

    /// <summary>
    /// Should the update quantity on cart.
    /// </summary>
    [Fact]
    public void ShouldUpdateQuantityOnCart()
    {
      // arrange
      var cartItem = new ShoppingCartItem
      {
        Id = 9999,
        ShoppingCartType = ShoppingCartType.ShoppingCart,
        Product = new Product { Id = 100500 },
        Quantity = 3
      };

      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);

      this.productService.GetProductById(100500).Returns(new Product { Id = 100500 });

      this.shoppingCartService.FindShoppingCartItemInTheCart(
        Arg.Any<IList<ShoppingCartItem>>(),
        ShoppingCartType.ShoppingCart,
        Arg.Is<Product>(pv => pv.Id == 100500)).Returns(cartItem);

      var customerWithUpdatedCartItem = new Customer { CustomerGuid = this.customerGuid, Id = 111222 };
      customerWithUpdatedCartItem.ShoppingCartItems.Add(cartItem);

      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer, customerWithUpdatedCartItem);

      // act
      var resultCartModel = this.cartService.UpdateQuantity(this.customerGuid, "100500", 3);

      // assert
      this.shoppingCartService.Received(1).UpdateShoppingCartItem(this.customer, 9999, 3, true);

      resultCartModel.ShoppingItems.Count.Should().Be(1);
      resultCartModel.ShoppingItems.Should().Contain(sci => sci.Quantity == 3);
    }

    /// <summary>
    /// Should return the same cart if product variant id can not be found when trying to add to existing cart.
    /// </summary>
    [Fact]
    public void ShouldReturnSameCart1()
    {
      // arrange
      var shoppingCartModel = new ShoppingCartModel { CustomerGuid = this.customerGuid, ShoppingItems = new[] { new ShoppingCartItemModel() { Id = "123" } } };
      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);
      // act
      var resultCartModel = this.cartService.UpdateQuantity(this.customerGuid, "-1", 3);

      resultCartModel.CustomerGuid.ShouldBeEquivalentTo(shoppingCartModel.CustomerGuid);
      resultCartModel.ShoppingItems.Count.ShouldBeEquivalentTo(shoppingCartModel.ShoppingItems.Count);
    }

    /// <summary>
    /// Should return the same cart if product variant id is not integer when trying to add to existing cart.
    /// </summary>
    [Fact]
    public void ShouldReturnSameCart2()
    {
      // arrange
      var shoppingCartModel = new ShoppingCartModel { CustomerGuid = this.customerGuid, ShoppingItems = new []{ new ShoppingCartItemModel() { Id = "123" } }};
      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);
      // act
      var resultCartModel = this.cartService.UpdateQuantity(this.customerGuid, "thisisnotainteger", 3);

      resultCartModel.CustomerGuid.ShouldBeEquivalentTo(shoppingCartModel.CustomerGuid);
      resultCartModel.ShoppingItems.Count.ShouldBeEquivalentTo(shoppingCartModel.ShoppingItems.Count);
    }

    /// <summary>
    /// Should return the same cart if the product variant can not be found when trying to remove from existing cart.
    /// </summary>
    [Fact]
    public void ShouldReturnTheSameCart3()
    {
      // arrange
      var shoppingCartModel = new ShoppingCartModel { CustomerGuid = this.customerGuid, ShoppingItems = new[] { new ShoppingCartItemModel() { Id = "123" } } };
      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);
      // act
      var resultCartModel = this.cartService.RemoveProduct(this.customerGuid, "-1");

      resultCartModel.CustomerGuid.ShouldBeEquivalentTo(shoppingCartModel.CustomerGuid);
      resultCartModel.ShoppingItems.Count.ShouldBeEquivalentTo(shoppingCartModel.ShoppingItems.Count);
    }

    /// <summary>
    /// Should return the same cart if product variant id is not integer when trying to remove from existing cart.
    /// </summary>
    [Fact]
    public void ShouldReturnSameCart4()
    {
      // arrange
      var shoppingCartModel = new ShoppingCartModel { CustomerGuid = this.customerGuid, ShoppingItems = new[] { new ShoppingCartItemModel() { Id = "123" } } };
      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);
      // act
      var resultCartModel = this.cartService.RemoveProduct(this.customerGuid, "thisisnotainteger");

      resultCartModel.CustomerGuid.ShouldBeEquivalentTo(shoppingCartModel.CustomerGuid);
      resultCartModel.ShoppingItems.Count.ShouldBeEquivalentTo(shoppingCartModel.ShoppingItems.Count);
    }

    /// <summary>
    /// Should not remove non existent cart item.
    /// </summary>
    [Fact]
    public void ShouldNotRemoveNonExistentCartItem()
    {
      // arrange
      this.productService.GetProductById(100500).Returns(x => null);
      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer);

      // act
      this.cartService.RemoveProduct(this.customerGuid, "100500");

      // assert
      this.shoppingCartService.DidNotReceiveWithAnyArgs().DeleteShoppingCartItem(Arg.Any<ShoppingCartItem>());
    }

    /// <summary>
    /// Should remove cart item.
    /// </summary>
    [Fact]
    public void ShouldRemoveCartItem()
    {
      // arrange
      var resultCartItem = new ShoppingCartItem
      {
        Id = 9999,
        ShoppingCartType = ShoppingCartType.ShoppingCart,
        Product = new Product { Id = 100500 },
        Quantity = 5
      };

      var resultCustomer = new Customer { CustomerGuid = this.customerGuid, Id = 111222 };
      resultCustomer.ShoppingCartItems.Add(resultCartItem);

      this.customerService.GetCustomerByGuid(this.customerGuid).Returns(this.customer, resultCustomer);

      this.shoppingCartService.FindShoppingCartItemInTheCart(
        Arg.Any<IList<ShoppingCartItem>>(),
        ShoppingCartType.ShoppingCart,
        Arg.Is<Product>(pv => pv.Id == 100500)).Returns(this.shoppingCartItem);

      this.productService.GetProductById(100500).Returns(new Product { Id = 100500 });

      // act
      var resultCartModel = this.cartService.RemoveProduct(this.customerGuid, "100500");

      // assert
      resultCartModel.ShoppingItems.Count.Should().Be(1);
    }
  }
}