// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the shopping cart controller type.
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
namespace Sitecore.Commerce.StarterKit.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Sitecore;
    using Diagnostics;
    using Entities.Carts;
    using Entities.Prices;
    using Helpers;
    using Models;
    using Services;
    using Texts = Texts;

    /// <summary>
    /// Defines the shopping cart controller type.
    /// </summary>
    public class CartController : Controller
    {
        /// <summary>
        /// The shopping cart service.
        /// </summary>
        private readonly ICartService cartService;

        /// <summary>
        /// The product service.
        /// </summary>
        private readonly IProductService productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController" /> class.
        /// </summary>
        /// <param name="cartService">The cart service.</param>
        /// <param name="productService">The product service.</param>
        public CartController([NotNull] ICartService cartService, [NotNull] IProductService productService)
        {
            Assert.ArgumentNotNull(cartService, "cartService");
            Assert.ArgumentNotNull(productService, "productService");

            this.cartService = cartService;
            this.productService = productService;
        }

        /// <summary>
        /// The Info view.
        /// </summary>
        /// <returns>Message string.</returns>
        [HttpGet]
        [ActionName("Info")]
        public string GetInfo()
        {
            var cart = this.cartService.GetCart();

            return this.GetCartWidgetInfo(cart);
        }

        /// <summary>
        /// Gets the cart.
        /// </summary>
        /// <returns>The cart.</returns>
        [HttpGet]
        [ActionName("Cart")]
        public JsonResult GetCart()
        {
            var cart = this.cartService.GetCart();
            var model = this.CreateCartModel(cart);
            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds the product to cart.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>The <see cref="string" />.</returns>
        [HttpPost]
        public string AddProduct(string id, string quantity)
        {
            uint quantityUint;
            if (!uint.TryParse(quantity, out quantityUint))
            {
                quantityUint = 1;
            }

            var cart = this.cartService.AddToCart(id, quantityUint);

            return this.GetCartWidgetInfo(cart);
        }

        /// <summary>
        /// Adds a product to the user wish list.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>The new wish list quantity.</returns>
        [HttpPost]
        public string AddProductToWishlist(string id, string quantity)
        {
            uint quantityUint;
            if (!uint.TryParse(quantity, out quantityUint))
            {
                quantityUint = 1;
            }

            var wishList = this.cartService.AddToWishList(id, quantityUint);

            return wishList.Lines.Sum(cl => cl.Quantity).ToString();
        }

        /// <summary>
        /// Gets the user wish list quantity.
        /// </summary>
        /// <returns>The user wish list quantity.</returns>
        [HttpGet]
        public string GetWishlistQuantity()
        {
            var wishList = this.cartService.GetWishList();
            if (wishList == null || wishList.Lines == null || !wishList.Lines.Any())
            {
                return "0";
            }

            return wishList.Lines.Sum(cl => cl.Quantity).ToString();
        }

        /// <summary>
        /// Gets the user wish list.
        /// </summary>
        /// <returns>The user wish list JSON.</returns>
        [HttpGet]
        [ActionName("wishlist")]
        public JsonResult GetWishlist()
        {
            var wishlist = this.cartService.GetWishList();
            var wishlistModel = wishlist.ToWishlistModel(this.productService);
            return this.Json(wishlistModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Changes a wish list item quantity.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>The Json result.</returns>
        [HttpPut]
        public JsonResult ChangeWishlistLineQuantity(string id, uint quantity)
        {
            var wishlist = this.cartService.ChangeWishlistLineQuantity(id, quantity);
            if (wishlist == null)
            {
                return Json(new QuantityResultModel());
            }

            var wishlistModel = wishlist.ToWishlistModel();
            var wishlistLineModel = wishlistModel.Lines.FirstOrDefault(l => l.ProductId == id);

            if (wishlistLineModel == null)
            {
                return Json(new QuantityResultModel());
            }

            var quantityResultModel = new QuantityResultModel()
            {
                TotalLineSum = (wishlistLineModel.UnitPrice * wishlistLineModel.Quantity).ToString("C", Context.Language.CultureInfo)
            };

            return Json(quantityResultModel);
        }

        /// <summary>
        /// Removes an item from the customer wish list.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The json result.</returns>
        [AcceptVerbs("DELETE")]
        public JsonResult RemoveLineFromeWishlist(string id)
        {
            var wishlist = this.cartService.RemoveLineFromWishlist(id);

            return Json(wishlist.ToWishlistModel(this.productService));
        }

        /// <summary>
        /// Removes a line from cart.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="string" />.</returns>
        [AcceptVerbs("DELETE")]
        public JsonResult RemoveFromCart(string id)
        {
            var cart = this.cartService.RemoveFromCart(id);
            var model = this.CreateCartModel(cart);
            return Json(model);
        }

        /// <summary>
        /// Change Line Quantity
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>The json result.</returns>
        [AcceptVerbs("PUT")]
        public JsonResult ChangeLineQuantity(string id, uint quantity)
        {
            var cart = this.cartService.ChangeLineQuantity(id, quantity);
            var cartModel = this.CreateCartModel(cart);

            if (cartModel.CartLines == null || !cart.Lines.Any())
            {
                return Json(new QuantityResultModel());
            }

            decimal cartLinesSum = cartModel.CartLines.Sum(cartLineModel => cartLineModel.UnitPrice * cartLineModel.Quantity);

            var cartLine = cartModel.CartLines.FirstOrDefault(cl => cl.ProductId.Equals(id));
            decimal curtLineSum = 0;
            if (cartLine != null)
            {
                curtLineSum = cartLine.UnitPrice * cartLine.Quantity;
            }

            var quantityResultModel = new QuantityResultModel()
            {
                TotalSum = cartLinesSum.ToString("C", Context.Language.CultureInfo),
                TotalLineSum = curtLineSum.ToString("C", Context.Language.CultureInfo)
            };

            return Json(quantityResultModel);
        }

        /// <summary>
        /// Gets the cart model.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns>The cart model.</returns>
        private string GetCartWidgetInfo(Cart cart)
        {
            var model = this.CreateCartModel(cart);

            if (model.CartLines == null || !cart.Lines.Any())
            {
                return Texts.NoProductsInCart;
            }

            //decimal cartLinesSum = model.CartLines.Sum(cartLineModel => cartLineModel.UnitPrice * cartLineModel.Quantity);
            //return string.Format(Texts.XProducts, cart.Lines.Sum(cl => cl.Quantity), cartLinesSum.ToString("c", new CultureInfo("en-US")).Replace("$",""));
            return string.Format(Texts.XProductsInfo, cart.Lines.Sum(cl => cl.Quantity));
        }

        /// <summary>
        /// Creates the cart model.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns>The cart model.</returns>
        [NotNull]
        private CartModel CreateCartModel([NotNull] Cart cart)
        {
            var cartModel = new CartModel();
            cartModel.CartLines = cart.Lines.Select(CreateCartLineModel);

            if (cartModel.CartLines != null)
            {
                cartModel.TotalSum = cartModel.CartLines.Sum(cl => cl.Quantity * cl.UnitPrice).ToString("C", Context.Language.CultureInfo);
            }

            return cartModel;
        }

        /// <summary>
        /// Creates the cart line model.
        /// </summary>
        /// <param name="l">The l.</param>
        /// <returns>Cart line model.</returns>
        private CartLineModel CreateCartLineModel(CartLine l)
        {
            if (l.Product == null || string.IsNullOrEmpty(l.Product.ProductId))
            {
                return new CartLineModel(l);
            }

            var productItem = productService.ReadProduct(l.Product.ProductId);
            if (productItem == null)
            {
                return new CartLineModel(l);
            }

            var productName = productItem.Name;

            string image = productService.GetImage(productItem);
            if (string.IsNullOrEmpty(image))
            {
                image = productService.GetImages(productItem).FirstOrDefault();
            }

            return new CartLineModel(l, productName)
            {
                Image = image
            };
        }
    }
}