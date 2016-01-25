// ----------------------------------------------------------------------------------------------
// <copyright file="CartsControllerTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The carts controller test.
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
namespace Sitecore.Commerce.StarterKit.Tests.Controllers.Admin
{
  using System.Linq;
  using Castle.Core.Internal;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Services.Carts;
  using Sitecore.Commerce.StarterKit.Controllers.Admin;
  using Sitecore.Commerce.StarterKit.Filters;
  using Sitecore.Sites;
  using Sitecore.TestKit.Sites;
  using Xunit;

  /// <summary>
  /// The carts controller test.
  /// </summary>
  public class CartsControllerTest
  {
    /// <summary>
    /// Should get the list of carts for context shop.
    /// </summary>
    [Fact]
    public void ShouldGetListOfCartsForContextShop()
    {
      // Arrange
      var cartService = Substitute.For<CartServiceProvider>();
      var carts = new[] { new Cart { ExternalId = "1001" }, new Cart { ExternalId = "1002" } };

      cartService.GetCarts(Arg.Is<GetCartsRequest>(r => r.ShopName == "mystore")).Returns(new GetCartsResult { Carts = carts });

      CartsController controller = new CartsController(cartService);

      using (new SiteContextSwitcher(new TSiteContext("mystore")))
      {
        // Act
        var result = controller.Get();

        // Assert
        result.Count().Should().Be(2);
        result.ElementAt(0).ExternalId.Should().Be("1001");
        result.ElementAt(1).ExternalId.Should().Be("1002");
      }
    }

    /// <summary>
    /// Should filter list of carts by user id.
    /// </summary>
    [Fact]
    public void ShouldFilterListOfCartsByUserId()
    {
      // Arrange
      var cartService = Substitute.For<CartServiceProvider>();
      var carts1 = new[] { new Cart { ExternalId = "1001", UserId = "Bob" } };
      var carts2 = new[] { new Cart { ExternalId = "1001", UserId = "Stan" } };
      var carts = carts1.Union(carts2);

      cartService.GetCarts(Arg.Is<GetCartsRequest>(r => r.ShopName == "mystore")).Returns(new GetCartsResult { Carts = carts });
      cartService.GetCarts(Arg.Is<GetCartsRequest>(r => r.ShopName == "mystore" && r.UserIds.Contains("Bob"))).Returns(new GetCartsResult { Carts = carts1 });

      CartsController controller = new CartsController(cartService);

      using (new SiteContextSwitcher(new TSiteContext("mystore")))
      {
        // Act
        var result = controller.Get(userId: "Bob");

        // Assert
        result.Count().Should().Be(1);
        result.ElementAt(0).ExternalId.Should().Be("1001");
        result.ElementAt(0).UserId.Should().Be("Bob");
      }
    }

    /// <summary>
    /// Should filter list of carts by customer id.
    /// </summary>
    [Fact]
    public void ShouldFilterListOfCartsByCustomerId()
    {
      // Arrange
      var cartService = Substitute.For<CartServiceProvider>();
      var carts1 = new[] { new Cart { ExternalId = "1001", CustomerId = "Bob" } };
      var carts2 = new[] { new Cart { ExternalId = "1001", CustomerId = "Stan" } };
      var carts = carts1.Union(carts2);

      cartService.GetCarts(Arg.Is<GetCartsRequest>(r => r.ShopName == "mystore")).Returns(new GetCartsResult { Carts = carts });
      cartService.GetCarts(Arg.Is<GetCartsRequest>(r => r.ShopName == "mystore" && r.CustomerIds.Contains("Bob"))).Returns(new GetCartsResult { Carts = carts1 });

      CartsController controller = new CartsController(cartService);

      using (new SiteContextSwitcher(new TSiteContext("mystore")))
      {
        // Act
        var result = controller.Get(customerId: "Bob");

        // Assert
        result.Count().Should().Be(1);
        result.ElementAt(0).ExternalId.Should().Be("1001");
        result.ElementAt(0).CustomerId.Should().Be("Bob");
      }
    }

    /// <summary>
    /// Should filter list of carts by cart name.
    /// </summary>
    [Fact]
    public void ShouldFilterListOfCartsByCartName()
    {
      // Arrange
      var cartService = Substitute.For<CartServiceProvider>();
      var carts1 = new[] { new Cart { ExternalId = "1001", Name = "Bob" } };
      var carts2 = new[] { new Cart { ExternalId = "1001", Name = "Stan" } };
      var carts = carts1.Union(carts2);

      cartService.GetCarts(Arg.Is<GetCartsRequest>(r => r.ShopName == "mystore")).Returns(new GetCartsResult { Carts = carts });
      cartService.GetCarts(Arg.Is<GetCartsRequest>(r => r.ShopName == "mystore" && r.Names.Contains("Bob"))).Returns(new GetCartsResult { Carts = carts1 });

      CartsController controller = new CartsController(cartService);

      using (new SiteContextSwitcher(new TSiteContext("mystore")))
      {
        // Act
        var result = controller.Get(cartName: "Bob");

        // Assert
        result.Count().Should().Be(1);
        result.ElementAt(0).ExternalId.Should().Be("1001");
        result.ElementAt(0).Name.Should().Be("Bob");
      }
    }

    /// <summary>
    /// Should filter list of carts by cart status.
    /// </summary>
    [Fact]
    public void ShouldFilterListOfCartsByCartStatus()
    {
      // Arrange
      var cartService = Substitute.For<CartServiceProvider>();
      var carts1 = new[] { new Cart { ExternalId = "1001", Status = "Bob" } };
      var carts2 = new[] { new Cart { ExternalId = "1001", Status = "Stan" } };
      var carts = carts1.Union(carts2);

      cartService.GetCarts(Arg.Is<GetCartsRequest>(r => r.ShopName == "mystore")).Returns(new GetCartsResult { Carts = carts });
      cartService.GetCarts(Arg.Is<GetCartsRequest>(r => r.ShopName == "mystore" && r.Statuses.Contains("Bob"))).Returns(new GetCartsResult { Carts = carts1 });

      CartsController controller = new CartsController(cartService);

      using (new SiteContextSwitcher(new TSiteContext("mystore")))
      {
        // Act
        var result = controller.Get(cartStatus: "Bob");

        // Assert
        result.Count().Should().Be(1);
        result.ElementAt(0).ExternalId.Should().Be("1001");
        result.ElementAt(0).Status.Should().Be("Bob");
      }
    }

    /// <summary>
    /// Should apply require admin attribute.
    /// </summary>
    [Fact]
    public void ShouldApplyRequireAdminAttribute()
    {
      // arrange
      var methodInfo = typeof(CartsController).GetMethod("Get");

      // act
      var attribute = methodInfo.GetAttribute<RequireAdminAttribute>();

      // assert
      attribute.Should().NotBeNull();
    }
  }
}