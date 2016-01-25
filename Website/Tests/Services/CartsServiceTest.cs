// ----------------------------------------------------------------------------------------------
// <copyright file="CartServiceTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The cart service test.
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
  using Commerce.Services.Customers;
  using Commerce.Services.WishLists;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Analytics.Data.DataAccess;
  using Sitecore.Common;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Entities.Prices;
  using Sitecore.Commerce.Services.Carts;
  using Sitecore.Commerce.Services.Prices;
  using Sitecore.Commerce.StarterKit.Services;
  using Sitecore.Commerce.Services.Inventory;
  using Sitecore.Commerce.Entities.Inventory;
  using Xunit;
  using Sitecore.Commerce.Contacts;
  using Sitecore.Analytics.Tracking;
  using Sitecore.Data;
  using Sitecore.Analytics;
    using Sitecore.Commerce.Data.Products;

  /// <summary>
  /// The cart service test.
  /// </summary>
  public class CartsServiceTest
  {
    /// <summary>
    /// The service provider
    /// </summary>
    private readonly CartServiceProvider cartServiceProvider;

    /// <summary>
    /// The result
    /// </summary>
    private readonly CartResult result;

    /// <summary>
    /// The cart
    /// </summary>
    private readonly Cart cart;

    /// <summary>
    /// The cart from anonymous.
    /// </summary>
    private readonly Cart cartFromAnonymous;

    /// <summary>
    /// The pricing service provider.
    /// </summary>
    private readonly PricingServiceProvider pricingService;

    /// <summary>
    /// The contact id.
    /// </summary>
    private readonly Guid contactId;

    /// <summary>
    /// The result from anonymous.
    /// </summary>
    private readonly CartResult resultFromAnonymous;

    /// <summary>
    /// The service.
    /// </summary>
    private CartService service;

    /// <summary>
    /// The contact factory.
    /// </summary>
    private ContactFactory contactFactory;

    /// <summary>
    /// The inventory service provider.
    /// </summary>
    private readonly InventoryServiceProvider _inventoryService;

    /// <summary>
    /// The customer service provider.
    /// </summary>
    private readonly CustomerServiceProvider _customerService;

    /// <summary>
    /// The wishlist service provider.
    /// </summary>
    private readonly WishListServiceProvider _wishListServiceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartsServiceTest"/> class.
    /// </summary>
    public CartsServiceTest()
    {
      this.cart = new Cart();
      this.cartFromAnonymous = new Cart();

      this.result = new CartResult { Cart = this.cart };
      this.resultFromAnonymous = new CartResult { Cart = this.cartFromAnonymous };

      this.cartServiceProvider = Substitute.For<CartServiceProvider>();
      this.cartServiceProvider.CreateOrResumeCart(Arg.Is<CreateOrResumeCartRequest>(r => r.UserId == "John Carter")).Returns(this.result);

      var pricesResult = new GetProductPricesResult();
      pricesResult.Prices.Add("List", new Price(0, "USD"));
      this.pricingService = Substitute.For<PricingServiceProvider>();
      this.pricingService.GetProductPrices(Arg.Any<GetProductPricesRequest>()).Returns(pricesResult);

      this.contactId = Guid.NewGuid();
      this.cartServiceProvider.CreateOrResumeCart(Arg.Is<CreateOrResumeCartRequest>(r => r.UserId == ID.Parse(this.contactId).ToString())).Returns(this.resultFromAnonymous);

      this.cartServiceProvider.GetCarts(Arg.Any<GetCartsRequest>()).Returns(new GetCartsResult { Carts = Enumerable.Empty<CartBase>() });
      this.contactFactory = Substitute.For<ContactFactory>();
      this.contactFactory.GetContact().Returns("John Carter");

      var inventoryResult = new GetStockInformationResult();
      inventoryResult.StockInformation.ToList().Add(new StockInformation { Product = new InventoryProduct { ProductId = "1001" }, Status = StockStatus.InStock });
      this._inventoryService = Substitute.For<InventoryServiceProvider>();
      this._inventoryService.GetStockInformation(Arg.Any<GetStockInformationRequest>()).Returns(inventoryResult);
      
      this._customerService = Substitute.For<CustomerServiceProvider>();
      
      this._wishListServiceProvider = Substitute.For<WishListServiceProvider>();

      this.service = new CartService(this.cartServiceProvider, this._wishListServiceProvider, this.pricingService, "autohaus", this.contactFactory, this._inventoryService, this._customerService);
    }

    /// <summary>
    /// Should get cart model for contact.
    /// </summary>
    [Fact]
    public void ShouldGetCartModelForContact()
    {
      // arrange
      var cartLine = new CartLine();
      this.cart.Lines = new ReadOnlyCollectionAdapter<CartLine> { cartLine };

      // act
      var cartModel = this.service.GetCart();

      // assert
      cartModel.Lines.Should().ContainSingle(cl => cl == cartLine);
    }

    /// <summary>
    /// Should get price for product.
    /// </summary>
    [Fact]
    public void ShouldGetPriceForProduct()
    {
      // arrange
      var cartLine = new CartLine { Product = new CartProduct { ProductId = "TestProductId" } };
      this.cart.Lines = new ReadOnlyCollectionAdapter<CartLine> { cartLine };

      var result = new GetProductPricesResult();
      result.Prices.Add("List", new Price(10.50m, "USD"));
      this.pricingService.GetProductPrices(Arg.Is<GetProductPricesRequest>(r => r.ProductId == "TestProductId")).Returns(result);

      // act
      var cartModel = this.service.GetCart();

      // assert
      cartModel.Lines.Single().Product.Price.Amount.Should().Be(10.50m);
    }

    /// <summary>
    /// Should leave the price intact if failed to retrieve prices.
    /// </summary>
    [Fact]
    public void ShouldLeaveThePriceIntactIfFailedToRetrievePrices()
    {
      // arrange
      var cartLine = new CartLine { Product = new CartProduct { ProductId = "TestProductId" } };
      this.cart.Lines = new ReadOnlyCollectionAdapter<CartLine> { cartLine };

      var result = new GetProductPricesResult();
      this.pricingService.GetProductPrices(Arg.Is<GetProductPricesRequest>(r => r.ProductId == "TestProductId")).Returns(result);

      // act
      var cartModel = this.service.GetCart();

      // assert
      cartModel.Lines.Single().Product.Price.Should().BeNull();
    }

    /// <summary>
    /// Should add cart line to cart.
    /// </summary>
    [Fact]
    public void ShouldAddCartLineToCart()
    {
      // Arrange
      this.cartServiceProvider.AddCartLines(
        Arg.Is<AddCartLinesRequest>(r =>
                                    r.Cart == this.cart &&
                                    r.Lines.Single().Product.ProductId == "TestProduct" &&
                                    r.Lines.Single().Quantity == 1)).Returns(this.result);

      // act
      var expected = this.service.AddToCart("TestProduct", 1);

      // assert
      expected.Should().Be(this.cart);
    }

    /// <summary>
    /// Should increate first matching line quantity if lines with specified product exists.
    /// </summary>
    [Fact]
    public void ShouldUpdateExistingCartLineQuantityIfLinesWithSpecifiedProductExists()
    {
      // arragne
      const string ProductId = "Audi A8L";
      this.cart.Lines = new ReadOnlyCollectionAdapter<CartLine>()
      { 
        new CartLine { Product = new CartProduct { ProductId = ProductId }, Quantity = 3 },
        new CartLine { Product = new CartProduct { ProductId = ProductId }, Quantity = 1 }
      };

      this.cartServiceProvider.UpdateCartLines(
        Arg.Is<UpdateCartLinesRequest>(r =>
          r.Cart == this.cart &&
          r.Lines.Single().Product.ProductId == ProductId &&
          r.Lines.Single().Quantity == 4)).Returns(this.result);

      // act
      var expected = this.service.AddToCart(ProductId, 1);

      // assert
      expected.Should().Be(this.cart);
      this.cartServiceProvider.DidNotReceiveWithAnyArgs().AddCartLines(null);
    }

    /// <summary>
    /// Should change cart line quantity.
    /// </summary>
    [Fact]
    public void ShouldChangeCartLineQuantity()
    {
      // arrange
      var cartLine = new CartLine { ExternalCartLineId = "LineId", Product = new CartProduct { ProductId = "TestProduct" }, Quantity = 1 };
      this.cart.Lines = new ReadOnlyCollectionAdapter<CartLine> { cartLine };

      this.cartServiceProvider.UpdateCartLines(
        Arg.Is<UpdateCartLinesRequest>(r =>
          r.Cart == this.cart &&
          r.Lines.Single() == cartLine &&
          r.Lines.Single().Product.ProductId == "TestProduct" &&
          r.Lines.Single().Quantity == 10)).Returns(this.result);

      // act
      var expected = this.service.ChangeLineQuantity("TestProduct", 10);

      // assert
      expected.Should().Be(this.cart);
    }

    /// <summary>
    /// Should remove cart line.
    /// </summary>
    [Fact]
    public void ShouldRemoveCartLine()
    {
      // arrange
      var lineToRemove = new CartLine { ExternalCartLineId = "MyCartLine" };
      this.cart.Lines = new ReadOnlyCollectionAdapter<CartLine> { lineToRemove };

      this.cartServiceProvider.RemoveCartLines(
        Arg.Is<RemoveCartLinesRequest>(r =>
          r.Cart == this.cart &&
          r.Lines.Single() == lineToRemove)).Returns(this.result);

      // act
      var expected = this.service.RemoveFromCart("MyCartLine");

      // assert
      expected.Should().Be(this.cart);
    }

    /// <summary>
    /// Should throw exception during merge if current cart is null.
    /// </summary>
    [Fact]
    public void ShouldThrowExceptionDuringMergeIfCurrentCartIsNull()
    {
      // Act
      Action action = () => this.service.MergeCarts(null, new Cart());

      // Assert
      action.ShouldThrow<ArgumentNullException>();
    }

    /// <summary>
    /// Should throw exception during merge if anonymous cart is null.
    /// </summary>
    [Fact]
    public void ShouldThrowExceptionDuringMergeIfAnonymousCartIsNull()
    {
      // Act
      Action action = () => this.service.MergeCarts(new Cart(), null);

      // Assert
      action.ShouldThrow<ArgumentNullException>();
    }

    /// <summary>
    /// Should not try to merge if anonymous cart is null.
    /// </summary>
    [Fact]
    public void ShouldNotTryToMergeIfAnonymousCartIsNull()
    {
      // Arrange
      this.cartServiceProvider.CreateOrResumeCart(Arg.Is<CreateOrResumeCartRequest>(r => r.UserId == ID.Parse(this.contactId).ToString())).Returns(this.resultFromAnonymous);

      // Act
      var actualCart = this.service.GetCart();

      // Assert
      actualCart.ShouldBeEquivalentTo(this.cart);
      this.cartServiceProvider.DidNotReceive().AddCartLines(Arg.Any<AddCartLinesRequest>());
      this.cartServiceProvider.DidNotReceive().DeleteCart(Arg.Any<DeleteCartRequest>());
    }

    /// <summary>
    /// Should not try to load additional cart if current user anonymous.
    /// </summary>
    [Fact]
    public void ShouldNotTryToLoadAdditionalCartIfCurrentUserAnonymous()
    {
      // Arrange
      this.contactFactory.GetContact().Returns(ID.Parse(this.contactId).ToString());
      this.service = new CartService(this.cartServiceProvider, this._wishListServiceProvider, this.pricingService, "autohaus", this.contactFactory, this._inventoryService, this._customerService);

      // Act
      this.service.GetCart();

      // Assert
      // Should be called only once, but not two times.
      this.cartServiceProvider.Received(1).CreateOrResumeCart(Arg.Is<CreateOrResumeCartRequest>(r => r.UserId == ID.Parse(this.contactId).ToString()));
    }

    /// <summary>
    /// Should not try to merge carts if anonymous has no ones.
    /// </summary>
    [Fact]
    public void ShouldNotTryToMergeCartsIfAnonymousHasNoOnes()
    {
      // Arrange

      // Act
      this.service.GetCart();

      // Assert
      this.cartServiceProvider.DidNotReceive().CreateOrResumeCart(Arg.Is<CreateOrResumeCartRequest>(r => r.UserId == this.contactId.ToString()));
    }

    /// <summary>
    /// Should not save merged cart using public API if anonymous cart has no cart lines.
    /// </summary>
    [Fact]
    public void ShouldNotSaveMergedCartUsingPublicApiIfAnonymousCartHasNoCartLines()
    {
      // Arrange
      var currentCart = new Cart { ShopName = "My", ExternalId = "GUID", Lines = new ReadOnlyCollectionAdapter<CartLine> { new CartLine { ExternalCartLineId = "1", Product = new CartProduct { ProductId = "Audi" } } } };
      var cartAnonymous = new Cart { ShopName = "My" };

      this.cartServiceProvider.CreateOrResumeCart(Arg.Is<CreateOrResumeCartRequest>(r => r.UserId == "John Carter")).Returns(new CartResult { Cart = currentCart });
      this.cartServiceProvider.CreateOrResumeCart(Arg.Is<CreateOrResumeCartRequest>(r => r.UserId == ID.Parse(this.contactId).ToString())).Returns(new CartResult { Cart = cartAnonymous });

      var contactModel = new FakeContact();
      contactModel.Id = new ID(this.contactId);
      contactModel.Identifiers.Identifier = string.Empty;

      var contact = new ContactContext(contactModel);
      var tracker = Substitute.For<ITracker>();
      tracker.Contact.Returns(contact);

      var testCart = new Cart();

      // Act
      using (new Switcher<ITracker, TrackerSwitcher>(tracker))
      {
        this.service.MergeCarts(testCart);
      }

      // Assert
      this.cartServiceProvider.DidNotReceive().SaveCart(Arg.Is<SaveCartRequest>(r => r.Cart == currentCart));
    }
  }
}