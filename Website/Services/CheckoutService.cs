// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckoutService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the CheckoutService class.</summary>
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Commerce.Services.Customers;
    using Commerce.Services.Payments;
    using Commerce.Services.Shipping;
    using Diagnostics;
    using Entities;
    using Entities.Customers;
    using Entities.Payments;
    using Entities.Shipping;

    /// <summary>
    /// The checkout service.
    /// </summary>
    public class CheckoutService : ICheckoutService
    {
        /// <summary>
        /// The shipping service provider.
        /// </summary>
        private readonly ShippingServiceProvider _shippingServiceProvider;

        /// <summary>
        /// The payment service provider
        /// </summary>
        private readonly PaymentServiceProvider _paymentServiceProvider;

        /// <summary>
        /// The customer service provider
        /// </summary>
        private readonly CustomerServiceProvider _customerServiceProvider;

        /// <summary>
        /// The cart service
        /// </summary>
        private readonly ICartService _cartService;

        /// <summary>
        /// The shop name.
        /// </summary>
        private string shopName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutService"/> class.
        /// </summary>
        /// <param name="shippingServiceProvider">The shipping service provider.</param>
        /// <param name="paymentServiceProvider">The payment service provider.</param>
        /// <param name="customerServiceProvider">The customer service provider.</param>
        /// <param name="cartService">The cart service.</param>
        /// <param name="shopName">The shop name.</param>
        public CheckoutService(
            [NotNull]ShippingServiceProvider shippingServiceProvider,
            [NotNull] PaymentServiceProvider paymentServiceProvider,
            [NotNull] CustomerServiceProvider customerServiceProvider,
            [NotNull] ICartService cartService,
            [NotNull] string shopName)
        {
            Assert.ArgumentNotNull(shippingServiceProvider, "shippingServiceProvider");
            Assert.ArgumentNotNull(paymentServiceProvider, "paymentServiceProvider");
            Assert.ArgumentNotNull(customerServiceProvider, "customerServiceProvider");
            Assert.ArgumentNotNull(cartService, "cartService");
            Assert.ArgumentNotNullOrEmpty(shopName, "shopName");

            this._shippingServiceProvider = shippingServiceProvider;
            this._paymentServiceProvider = paymentServiceProvider;
            this._customerServiceProvider = customerServiceProvider;
            this._cartService = cartService;
            this.shopName = shopName;
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
        /// Get shipping options
        /// </summary>
        /// <returns>The shipping options.</returns>
        [NotNull]
        public IEnumerable<ShippingOption> GetShippingOptions()
        {
            var result = this._shippingServiceProvider.GetShippingOptions(new GetShippingOptionsRequest());
            if (result == null)
            {
                return Enumerable.Empty<ShippingOption>();
            }

            return result.ShippingOptions ?? Enumerable.Empty<Entities.Shipping.ShippingOption>();
        }

        /// <summary>
        /// Get payment options
        /// </summary>
        /// <returns>The payment options.</returns>
        [NotNull]
        public IEnumerable<Entities.Payments.PaymentOption> GetPaymentOptions()
        {
            var result = this._paymentServiceProvider.GetPaymentOptions(new GetPaymentOptionsRequest(this.ShopName));
            if (result == null)
            {
                return Enumerable.Empty<Entities.Payments.PaymentOption>();
            }

            return result.PaymentOptions ?? Enumerable.Empty<Entities.Payments.PaymentOption>();
        }

        /// <summary>
        /// Get shipping methods
        /// </summary>
        /// <param name="shippingOption">The shipping option.</param>
        /// <returns>The shipping methods.</returns>
        [NotNull]
        public IEnumerable<Entities.Shipping.ShippingMethod> GetShippingMethods([NotNull]ShippingOption shippingOption)
        {
            var result = this._shippingServiceProvider.GetShippingMethods(new GetShippingMethodsRequest(shippingOption)
            {
                Properties = new PropertyCollection()
                {
                  new PropertyItem()
                  {
                    Key = "cart",
                    Value = this._cartService.GetCart()
                  }
                }
            });

            if (!result.Success)
            {
                return Enumerable.Empty<Entities.Shipping.ShippingMethod>();
            }

            return result.ShippingMethods ?? Enumerable.Empty<Entities.Shipping.ShippingMethod>();
        }

        /// <summary>
        /// Get shipping methods
        /// </summary>
        /// <param name="paymentOption">The payment option.</param>
        /// <returns>The payment methods.</returns>
        [NotNull]
        public IEnumerable<PaymentMethod> GetPaymentMethods([NotNull]PaymentOption paymentOption)
        {
            paymentOption.ShopName = this.ShopName;

            var result = this._paymentServiceProvider.GetPaymentMethods(new GetPaymentMethodsRequest(paymentOption));
            if (result == null || !result.Success)
            {
                return Enumerable.Empty<PaymentMethod>();
            }

            return result.PaymentMethods ?? Enumerable.Empty<PaymentMethod>();
        }

        [NotNull]
        public string GetPaymentServiceUrl()
        {
            var cart = this._cartService.GetCart();
            var billingAddress = this._cartService.GetBillingAddress();

            var request = new GetPaymentServiceUrlRequest()
            {
                AllowPartialAuthorization = false,
                AllowVoiceAuthorization = false,
                CardType = CardType.AllCardTypes,
                City = billingAddress.City,
                ColumnNumber = 1,
                Country = billingAddress.Country,
                CurrencyCode = cart.CurrencyCode,
                DisabledTextBackgroundColor = "#E4E4E4",
                FontFamily = "\"Helvetica Neue\", Helvetica, Arial, sans-serif",
                FontSize = "14px",
                IndustryType = "Ecommerce",
                LabelColor = "black",
                Locale = Context.Culture.Name,
                PageBackgroundColor = "white",
                PageWidth = "429px",
                PostalCode = billingAddress.ZipPostalCode,
                PurchaseLevel = PurchaseLevel.Level1,
                ShowSameAsShippingAddress = false,
                State = billingAddress.State,
                StreetAddress = billingAddress.Address1,
                SupportCardSwipe = false,
                SupportCardTokenization = true,
                TextBackgroundColor = "white",
                TextColor = "black",
                TransactionType = TransactionType.Authorize,
            };

            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                request.HostPageOrigin = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "{0}://{1}",
                    HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.Authority);
            }

            if (!string.IsNullOrWhiteSpace(billingAddress.Address2))
            {
                request.StreetAddress = request.StreetAddress + Environment.NewLine + billingAddress.Address2;
            }

            if (string.IsNullOrWhiteSpace(request.CurrencyCode))
            {
                request.CurrencyCode = "USD";
            }

            var result = this._paymentServiceProvider.GetPaymentServiceUrl(request);

            return result.Url ?? string.Empty;
        }

        /// <summary>
        /// Gets the result of a payment service action.
        /// </summary>
        /// <param name="accessCode">The payment service access code.</param>
        /// <returns>The result of the payment service acction.</returns>
        public GetPaymentServiceActionResultResult GetPaymentServiceActionResult(string accessCode)
        {
            Assert.ArgumentNotNullOrEmpty(accessCode, "accessCode");

            var request = new GetPaymentServiceActionResultRequest()
            {
                Locale = Context.Culture.Name,
                PaymentAcceptResultAccessCode = accessCode
            };

            var result = this._paymentServiceProvider.GetPaymentServiceActionResult(request);
            return result;
        }
    }
}