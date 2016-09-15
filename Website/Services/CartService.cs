// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Provides basic cart operations for the "Autohaus" web-store visitors.
//   This service is aimed to simplify the Test Driven Development (TDD) and
//   allows MVC controllers to use lite version of the cart management API
//   that satisfies their needs.
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
namespace Sitecore.Commerce.StarterKit.Services
{
    using System.Linq;
    using Sitecore;
    using Sitecore.Analytics;
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Commerce.Services.Prices;
    using Sitecore.Commerce.Contacts;
    using System.Collections.Generic;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Services.Inventory;
    using Sitecore.Data;
    using System.Globalization;
    using Commerce.Services.Customers;
    using Commerce.Services.WishLists;
    using Commerce.Services.WishLists.Generics;
    using Entities;
    using Entities.Customers;
    using Entities.WishLists;
    using Shell.Framework.Commands.Masters;
    using AddPartiesRequest = Commerce.Services.Carts.AddPartiesRequest;
    using System;
    using Sitecore.Commerce.Data.Products;

    /// <summary>
    /// Provides basic cart operations for the Autohaus web-store visitors.
    /// This service is aimed to simplify the Test Driven Development (TDD) and 
    /// allows MVC controllers to use lite version of the cart management API
    /// that satisfies their needs.
    /// </summary>
    public class CartService : ICartService
    {
        /// <summary>
        /// The inventory service provider.
        /// </summary>
        private readonly InventoryServiceProvider _inventoryServiceProvider;

        /// <summary>
        /// The customer service provider
        /// </summary>
        private readonly CustomerServiceProvider _customerServiceProvider;

        /// <summary>
        /// The wishlist service provider
        /// </summary>
        private readonly WishListServiceProvider _wishListServiceProvider;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly CartServiceProvider _cartServiceProvider;

        /// <summary>
        /// The pricing service provider.
        /// </summary>
        private readonly PricingServiceProvider _pricingServiceProvider;

        /// <summary>
        /// The visitor factory.
        /// </summary>
        private readonly ContactFactory contactFactory;

        /// <summary>
        /// The shop name.
        /// </summary>
        private string shopName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartService" /> class.
        /// </summary>
        /// <param name="cartServiceProvider">The service provider.</param>
        /// <param name="wishListServiceProvider">The wish list service provider.</param>
        /// <param name="pricingServiceProvider">The pricing service provider.</param>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="contactFactory">The visitor factory.</param>
        /// <param name="inventoryServiceProvider">The inventory service provider.</param>
        /// <param name="customerServiceProvider">The customer service provider.</param>
        public CartService([NotNull] CartServiceProvider cartServiceProvider, [NotNull] WishListServiceProvider wishListServiceProvider, [NotNull] PricingServiceProvider pricingServiceProvider, [NotNull] string shopName, ContactFactory contactFactory, [NotNull] InventoryServiceProvider inventoryServiceProvider, [NotNull]CustomerServiceProvider customerServiceProvider)
        {
            Assert.ArgumentNotNull(cartServiceProvider, "cartServiceProvider");
            Assert.ArgumentNotNull(wishListServiceProvider, "wishListServiceProvider");
            Assert.ArgumentNotNull(pricingServiceProvider, "pricingServiceProvider");
            Assert.ArgumentNotNull(customerServiceProvider, "customerServiceProvider");
            Assert.ArgumentNotNullOrEmpty(shopName, "shopName");

            this._cartServiceProvider = cartServiceProvider;
            this._pricingServiceProvider = pricingServiceProvider;
            this.shopName = shopName;
            this.contactFactory = contactFactory;
            this._inventoryServiceProvider = inventoryServiceProvider;
            this._customerServiceProvider = customerServiceProvider;
            this._wishListServiceProvider = wishListServiceProvider;
        }

        /// <summary>
        /// Gets or sets the name of the shop.
        /// </summary>
        /// <value>The name of the shop.</value>
        [NotNull]
        public string ShopName
        {
            get { return this.shopName; }
            set { this.shopName = value; }
        }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <value>The user id.</value>
        [NotNull]
        public string UserId
        {
            get
            {
                return this.contactFactory.GetContact();
            }
        }

        /// <summary>
        /// Gets the visitor id.
        /// </summary>
        /// <value>The visitor id.</value>
        [NotNull]
        public string VisitorId
        {
            get
            {
                var visitorId = this.contactFactory.GetContact();

                Guid temp;
                if (!Guid.TryParse(visitorId, out temp))
                {
                    var idGenerator = new Md5IdGenerator();
                    visitorId = idGenerator.StringToID(visitorId, string.Empty).ToString();
                }

                return visitorId;
            }
        }

        /// <summary>
        /// Gets the shopping cart for visitor.
        /// </summary>
        /// <returns>The visitor's cart.</returns>
        public Cart GetCart()
        {
            var currentCart = this.GetCart(this.UserId);

            return this.UpdatePrices(currentCart);
        }

        /// <summary>
        /// Adds product to the visitor's cart.
        /// </summary>
        /// <param name="productId">
        /// The product id.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <returns>
        /// The <see cref="Cart"/>.
        /// </returns>
        public Cart AddToCart(string productId, uint quantity)
        {
            Assert.ArgumentNotNull(productId, "productId");

            var cart = this.GetCart();
            if (string.IsNullOrWhiteSpace(cart.ShopName))
            {
                cart.ShopName = this.ShopName;
            }

            CartResult cartResult;
            var cartLineToChange = cart.Lines.FirstOrDefault(cl => cl.Product != null && cl.Product.ProductId == productId);
            if (cartLineToChange != null)
            {
                cartLineToChange.Quantity += quantity;
                var updateRequest = new UpdateCartLinesRequest(cart, new[] { cartLineToChange });
                cartResult = this._cartServiceProvider.UpdateCartLines(updateRequest);
            }
            else
            {
                var cartLine = new CartLine { Product = new CartProduct { ProductId = productId }, Quantity = quantity };
                this.UpdateStockInformation(cartLine);
                var request = new AddCartLinesRequest(cart, new[] { cartLine });
                cartResult = this._cartServiceProvider.AddCartLines(request);
            }

            return this.UpdatePrices(cartResult.Cart);
        }

        /// <summary>
        /// Removes product from the visitor's cart.
        /// </summary>
        /// <param name="externalCartLineId">
        /// The product id.
        /// </param>
        /// <returns>
        /// The <see cref="Cart"/>.
        /// </returns>
        public Cart RemoveFromCart(string externalCartLineId)
        {
            Assert.ArgumentNotNull(externalCartLineId, "externalCartLineId");

            var cart = this.GetCart();
            if (string.IsNullOrWhiteSpace(cart.ShopName))
            {
                cart.ShopName = this.ShopName;
            }

            var lineToRemove = cart.Lines.SingleOrDefault(cl => cl.ExternalCartLineId == externalCartLineId);
            if (lineToRemove == null)
            {
                return cart;
            }

            var request = new RemoveCartLinesRequest(cart, new[] { lineToRemove });

            var cartResult = this._cartServiceProvider.RemoveCartLines(request);
            return this.UpdatePrices(cartResult.Cart);
        }

        /// <summary>
        /// Changes the visitor's cart line quantity.
        /// </summary>
        /// <param name="productId">
        /// The product id.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <returns>
        /// The <see cref="Cart"/>.
        /// </returns>
        public Cart ChangeLineQuantity(string productId, uint quantity)
        {
            Assert.ArgumentNotNull(productId, "productId");

            var cart = this.GetCart();
            if (string.IsNullOrWhiteSpace(cart.ShopName))
            {
                cart.ShopName = this.ShopName;
            }

            var cartLineToChange = cart.Lines.SingleOrDefault(cl => cl.Product != null && cl.Product.ProductId == productId);
            if (cartLineToChange == null)
            {
                return cart;
            }

            cartLineToChange.Quantity = quantity;

            var updateRequest = new UpdateCartLinesRequest(cart, new[] { cartLineToChange });

            var cartResult = this._cartServiceProvider.UpdateCartLines(updateRequest);
            return this.UpdatePrices(cartResult.Cart);
        }

        /// <summary>
        /// Merges the carts.
        /// </summary>
        /// <param name="cartFromAnonymous">The anonymous visitor cart.</param>
        /// <returns>
        /// The <see cref="Cart"/>.
        /// </returns>
        public Cart MergeCarts(Cart cartFromAnonymous)
        {
            Assert.ArgumentNotNull(cartFromAnonymous, "anonymousCart");

            var currentCart = this.GetCart(this.UserId);
            if (this.UserId != cartFromAnonymous.UserId)
            {
                currentCart = this.EnsureCorrectCartUserId(currentCart);
                if (cartFromAnonymous != null && cartFromAnonymous.Lines.Any())
                {
                    return this.UpdatePrices(this.MergeCarts(currentCart, cartFromAnonymous));
                }
            }

            return this.UpdatePrices(currentCart);
        }

        /// <summary>
        /// Merges the carts.
        /// </summary>
        /// <param name="userCart">The user cart.</param>
        /// <param name="anonymousCart">The anonymous cart.</param>
        /// <returns>
        /// The merged cart.
        /// </returns>
        public Cart MergeCarts([NotNull] Cart userCart, [NotNull] Cart anonymousCart)
        {
            Assert.ArgumentNotNull(userCart, "userCart");
            Assert.ArgumentNotNull(anonymousCart, "anonymousCart");

            userCart = this.EnsureCorrectCartUserId(userCart);

            if ((userCart.ShopName == anonymousCart.ShopName) && (userCart.ExternalId != anonymousCart.ExternalId))
            {
                var mergeCartRequest = new MergeCartRequest(anonymousCart, userCart);
                var result = this._cartServiceProvider.MergeCart(mergeCartRequest);

                this._cartServiceProvider.DeleteCart(new DeleteCartRequest(anonymousCart));

                return result.Cart;
            }

            return userCart;
        }

        /// <summary>
        /// Add address
        /// </summary>
        /// <param name="party">The party to add.</param>
        /// <returns>
        /// The addresses
        /// </returns>
        [NotNull]
        public IEnumerable<Party> AddAddress(Party party)
        {
            var result = this._customerServiceProvider.AddParties(new Commerce.Services.Customers.AddPartiesRequest(
              new CommerceCustomer()
              {
                  ExternalId = this.GetCart().ExternalId
              },
              new List<Party>() { party }));

            if (!result.Success)
            {
                return Enumerable.Empty<Party>();
            }

            return result.Parties ?? Enumerable.Empty<Party>();
        }

        /// <summary>
        /// Set billing address to cart
        /// </summary>
        /// <param name="cartParty">The cart party.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        public bool SetBillingAddressToCart(CartParty cartParty)
        {
            var cart = this.GetCart();

            cart.AccountingCustomerParty = cartParty;

            var result = this._cartServiceProvider.AddParties(new AddPartiesRequest(
                cart,
                new List<Party>(0)));

            return result.Success;
        }

        /// <summary>
        /// Set shipping address to cart
        /// </summary>
        /// <param name="cartParty">The cart party.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        public bool SetShippingAddressToCart(CartParty cartParty)
        {
            var cart = this.GetCart();

            cart.BuyerCustomerParty = cartParty;

            var result = this._cartServiceProvider.AddParties(new AddPartiesRequest(
              cart,
              new List<Party>(0)));

            return result.Success;
        }

        /// <summary>
        /// Get addresses
        /// </summary>
        /// <returns>The addresses.</returns>
        [NotNull]
        public IEnumerable<Party> GetAddresses()
        {
            var result = this._customerServiceProvider.GetParties(new GetPartiesRequest(new CommerceCustomer() { ExternalId = this.GetCart().ExternalId }));
            if (result == null)
            {
                return Enumerable.Empty<Party>();
            }

            return result.Parties ?? Enumerable.Empty<Party>();
        }

        /// <summary>
        /// Get billing address
        /// </summary>
        /// <returns>
        /// The billing address
        /// </returns>
        [CanBeNull]
        public Party GetBillingAddress()
        {
            var cart = this.GetCart();
            if (cart == null || cart.AccountingCustomerParty == null)
            {
                return null;
            }

            return this.GetAddresses().FirstOrDefault(a => a.ExternalId == cart.AccountingCustomerParty.PartyID);
        }

        /// <summary>
        /// Get shipping address
        /// </summary>
        /// <returns>
        /// The shipping address
        /// </returns>
        [CanBeNull]
        public Party GetShippingAddress()
        {
            var cart = this.GetCart();
            if (cart == null || cart.BuyerCustomerParty == null)
            {
                return null;
            }

            return this.GetAddresses().FirstOrDefault(a => a.ExternalId == cart.BuyerCustomerParty.PartyID);
        }

        /// <summary>
        /// Set shipping method to cart
        /// </summary>
        /// <param name="shippingInfo">The shipping information.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        public bool SetShippingMethodToCart(ShippingInfo shippingInfo)
        {
            var cart = this.GetCart();
            if (string.IsNullOrWhiteSpace(cart.ShopName))
            {
                cart.ShopName = this.ShopName;
            }

            var result = this._cartServiceProvider.AddShippingInfo(new AddShippingInfoRequest(cart, new List<ShippingInfo>() { shippingInfo }));

            return (result.Success && result.ShippingInfo.Any(sh => sh.ShippingProviderID == shippingInfo.ShippingProviderID && sh.ShippingMethodID == shippingInfo.ShippingMethodID));
        }

        /// <summary>
        /// Set payment method to cart
        /// </summary>
        /// <param name="paymentInfo">The payment information.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        public bool SetPaymentMethodToCart(PaymentInfo paymentInfo)
        {
            var cart = this.GetCart();
            if (string.IsNullOrWhiteSpace(cart.ShopName))
            {
                cart.ShopName = this.ShopName;
            }

            var result = this._cartServiceProvider.AddPaymentInfo(new AddPaymentInfoRequest(cart, new List<PaymentInfo>() { paymentInfo }));

            return (result.Success && result.Payments.Any(p => p.PaymentProviderID == paymentInfo.PaymentProviderID && p.PaymentMethodID == paymentInfo.PaymentMethodID));
        }

        /// <summary>
        /// Update addresses
        /// </summary>
        /// <param name="parties">The list of addresses.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        public bool UpdateAddresses(List<Party> parties)
        {
            var result = this._customerServiceProvider.UpdateParties(new Sitecore.Commerce.Services.Customers.UpdatePartiesRequest(new CommerceCustomer() { ExternalId = this.GetCart().ExternalId }, parties));

            return (result != null && result.Success);
        }

        /// <summary>
        /// Set shipping method from cart
        /// </summary>
        /// <returns>the updated shipping information.</returns>
        public ShippingInfo GetShippingInfo()
        {
            var cart = this.GetCart();
            if (cart == null || cart.Shipping == null || cart.Shipping.Count == 0)
            {
                return null;
            }

            var r = cart.Shipping.FirstOrDefault();
            return r;
        }

        /// <summary>
        /// Get payment method from cart
        /// </summary>
        /// <returns>The payment information.</returns>
        public PaymentInfo GetPaymentInfo()
        {
            var cart = this.GetCart();
            if (cart == null || cart.Payment == null || !cart.Payment.Any())
            {
                return null;
            }

            return cart.Payment[0];
        }

        /// <summary>
        /// Get wishlist
        /// </summary>
        /// <returns>The wish list for the current visitor.</returns>
        public WishList GetWishList()
        {
            var currentWishList = this.GetWishList(this.VisitorId);

            return currentWishList;
        }

        /// <summary>
        /// Create wishlist
        /// </summary>
        /// <returns>The new wish list.</returns>
        public WishList CreateWishList()
        {
            var result = this._wishListServiceProvider.CreateWishList(new CreateWishListRequest(this.VisitorId, this.VisitorId, this.shopName));

            if (result == null || !result.Success)
            {
                return null;
            }

            return result.WishList;
        }

        /// <summary>
        /// Add to wishlist
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>The updated wish list.</returns>
        public WishList AddToWishList(string productId, uint quantity)
        {
            var wishList = this.GetWishList() ?? this.CreateWishList();

            var result = this._wishListServiceProvider.AddLinesToWishList(new AddLinesToWishListRequest(wishList, new List<WishListLine>() { new WishListLine() { Product = new CartProduct() { ProductId = productId }, Quantity = quantity } }));

            return result.WishList;
        }

        /// <summary>
        /// Change visitor's wishlist line property
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>The updated wish list.</returns>
        public WishList ChangeWishlistLineQuantity(string productId, uint quantity)
        {
            var wishList = this.GetWishList() ?? this.CreateWishList();
            var result = this._wishListServiceProvider.UpdateWishListLines(
                new UpdateWishListLinesRequest(
                    new WishList() { ExternalId = wishList.ExternalId },
                    new List<WishListLine>()
                    {
                        new WishListLine()
                        {
                            Product = new CartProduct()
                            {
                                ProductId = productId
                            }, 
                            Quantity = quantity
                        }
                    }));

            return this.GetWishList();
        }

        /// <summary>
        /// Remove line from wishlist
        /// </summary>
        /// <param name="lineId">The line ID.</param>
        /// <returns>The updated wishlist.</returns>
        public WishList RemoveLineFromWishlist(string lineId)
        {
            var result = this._wishListServiceProvider.RemoveWishListLines(
                new RemoveWishListLinesRequest(
                    this.GetWishList(),
                    new List<string>() { lineId }
                )
            );

            return this.GetWishList();
        }

        /// <summary>
        /// Merges the wishlist.
        /// </summary>
        /// <param name="anonymousWishlist">The anonymous visitor wishlist.</param>
        /// <returns>
        /// The <see cref="Cart"/>.
        /// </returns>
        public WishList MergeWishlist(WishList anonymousWishlist)
        {
            var currentWishlist = this.GetWishList(this.VisitorId);
            if (this.UserId != anonymousWishlist.UserId)
            {
                //currentWishlist = this.EnsureCorrectCartUserId(currentWishlist);
                if (anonymousWishlist != null && anonymousWishlist.Lines.Any())
                {
                    return this.MergeWishlist(currentWishlist, anonymousWishlist);
                }
            }

            return currentWishlist;
        }

        /// <summary>
        /// Merges the carts.
        /// </summary>
        /// <param name="userWishlist">The user wishlist.</param>
        /// <param name="anonymousWishlist">The anonymous wishlist.</param>
        /// <returns>
        /// The merged wishlist.
        /// </returns>
        public WishList MergeWishlist(WishList userWishlist, WishList anonymousWishlist)
        {
            foreach (var line in anonymousWishlist.Lines)
            {
                this.AddToWishList(line.Product.ProductId, line.Quantity);
            }

            return this.GetWishList();
        }

        /// <summary>
        /// Get wishlist
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns>The wish list.</returns>
        protected virtual WishList GetWishList(string userName)
        {
            var result = this._wishListServiceProvider.GetWishList(new GetWishListRequest(userName, userName, this.shopName));

            if (result == null || !result.Success)
            {
                return null;
            }

            return result.WishList;
        }

        /// <summary>
        /// The promotion cart.
        /// </summary>
        /// <param name="userName">The user Name.</param>
        /// <returns>
        /// The <see cref="GetCart" />.
        /// </returns>
        private Cart GetCart(string userName)
        {
            var request = new CreateOrResumeCartRequest(this.shopName, userName);
            var result = this._cartServiceProvider.CreateOrResumeCart(request);

            return result.Cart;
        }

        /// <summary>
        /// The update prices.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns>
        /// The <see cref="Cart" />.
        /// </returns>
        private Cart UpdatePrices([NotNull] Cart cart)
        {
            Assert.ArgumentNotNull(cart, "cart");

            // TODO: [Low] Consider using standard approach for getting prices when it will be implemented.
            foreach (var cartLine in cart.Lines)
            {
                if (cartLine.Product != null && cartLine.Product.ProductId != null)
                {
                    var priceRequest = new GetProductPricesRequest(cartLine.Product.ProductId, "List");
                    var priceResult = this._pricingServiceProvider.GetProductPrices(priceRequest);
                    Price price;

                    if (priceResult.Prices.TryGetValue("List", out price))
                    {
                        cartLine.Product.Price = price;
                    }
                }
            }

            return cart;
        }

        /// <summary>
        /// The update stock informationS.
        /// </summary>
        /// <param name="cartLine">The cart line.</param>
        private void UpdateStockInformation([NotNull] CartLine cartLine)
        {
            Assert.ArgumentNotNull(cartLine, "cartLine");

            var products = new List<InventoryProduct> { new InventoryProduct { ProductId = cartLine.Product.ProductId } };
            var stockInfoRequest = new GetStockInformationRequest(this.ShopName, products, StockDetailsLevel.Status);
            var stockInfoResult = this._inventoryServiceProvider.GetStockInformation(stockInfoRequest);

            if (stockInfoResult.StockInformation == null || !stockInfoResult.StockInformation.Any())
            {
                return;
            }

            var stockInfo = stockInfoResult.StockInformation.FirstOrDefault();
            var orderableInfo = new OrderableInformation();
            if (stockInfo != null && stockInfo.Status != null)
            {
                if (Equals(stockInfo.Status, StockStatus.PreOrderable))
                {
                    var preOrderableRequest = new GetPreOrderableInformationRequest(this.ShopName, products);
                    var preOrderableResult = this._inventoryServiceProvider.GetPreOrderableInformation(preOrderableRequest);
                    if (preOrderableResult.OrderableInformation != null && preOrderableResult.OrderableInformation.Any())
                    {
                        orderableInfo = preOrderableResult.OrderableInformation.FirstOrDefault();
                    }
                }
                else if (Equals(stockInfo.Status, StockStatus.BackOrderable))
                {
                    var backOrderableRequest = new GetBackOrderableInformationRequest(this.ShopName, products);
                    var backOrderableResult = this._inventoryServiceProvider.GetBackOrderableInformation(backOrderableRequest);
                    if (backOrderableResult.OrderableInformation != null && backOrderableResult.OrderableInformation.Any())
                    {
                        orderableInfo = backOrderableResult.OrderableInformation.FirstOrDefault();
                    }
                }
            }

            if (stockInfo != null)
            {
                cartLine.Product.StockStatus = stockInfo.Status;
            }

            if (orderableInfo == null)
            {
                return;
            }

            cartLine.Product.InStockDate = orderableInfo.InStockDate;
            cartLine.Product.ShippingDate = orderableInfo.ShippingDate;
        }

        private Cart EnsureCorrectCartUserId(Cart cart)
        {
            if (cart.UserId != this.UserId)
            {
                Cart changes = new Cart { UserId = this.UserId };
                var updateCartRequest = new UpdateCartRequest(cart, changes);
                var result = this._cartServiceProvider.UpdateCart(updateCartRequest);
                if (result != null && result.Success)
                {
                    return result.Cart;
                }
            }

            return cart;
        }
    }
}