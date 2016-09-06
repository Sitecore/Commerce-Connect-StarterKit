// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckoutController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the checkout controller type.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Entities.Payments;
    using Filters;
    using Helpers;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Commerce.StarterKit.Models;
    using Sitecore.Commerce.StarterKit.Services;
    using Sitecore.Diagnostics;
    using System.Globalization;
    using Sitecore.Commerce.PaymentService.Connector;

    /// <summary>
    /// The checkout controller.
    /// </summary>
    public class CheckoutController : Controller
    {
        /// <summary>
        /// The checkout service.
        /// </summary>
        private readonly ICheckoutService checkoutService;

        /// <summary>
        /// The cart service.
        /// </summary>
        private readonly ICartService cartService;

        /// <summary>
        /// The cart service.
        /// </summary>
        private readonly IOrderService orderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutController"/> class.
        /// </summary>
        /// <param name="checkoutService">The checkout service.</param>
        /// <param name="cartService">The cart service.</param>
        /// <param name="orderService">The orders service.</param>
        public CheckoutController([NotNull] ICheckoutService checkoutService, [NotNull] ICartService cartService, [NotNull] IOrderService orderService)
        {
            Assert.ArgumentNotNull(checkoutService, "checkoutService");
            Assert.ArgumentNotNull(cartService, "cartService");
            Assert.ArgumentNotNull(orderService, "orderService");

            this.checkoutService = checkoutService;
            this.cartService = cartService;
            this.orderService = orderService;
        }

        /// <summary>
        /// Views the index.
        /// </summary>
        /// <returns>The action result.</returns>
        [RequireCartLine]
        public ActionResult Index()
        {
            ViewBag.Countries = Context.Database.Items["/sitecore/system/Settings/Analytics/Lookups/Countries/"];
            return View("Index");
        }

        /// <summary>
        /// Views the billing address.
        /// </summary>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult BillingAddress()
        {
            ViewBag.Countries = Context.Database.Items["/sitecore/system/Settings/Analytics/Lookups/Countries/"];

            var address = this.cartService.GetBillingAddress();
            if (address == null)
            {
                address = this.cartService.GetAddresses().FirstOrDefault() ?? new Party();
            }

            ViewBag.Addresses = this.cartService.GetAddresses().Select(a => a.ToAddressModel());

            var billingAddress = address.ToAddressModel();

            return View("BillingAddress", billingAddress);
        }

        /// <summary>
        /// Views the billing address.
        /// </summary>
        /// <param name="billingAddress">The user address.</param>
        /// <param name="changeStep">The change step type.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public ActionResult BillingAddress(AddressModel billingAddress, ChangeStepType changeStep)
        {
            ViewBag.Countries = Context.Database.Items["/sitecore/system/Settings/Analytics/Lookups/Countries/"];
            ViewBag.Addresses = this.cartService.GetAddresses().Select(a => a.ToAddressModel());

            if (changeStep != ChangeStepType.Next || !this.ModelState.IsValid)
            {
                return View(billingAddress);
            }

            // If address is not new
            if (!string.IsNullOrEmpty(billingAddress.PartyId))
            {
                if (billingAddress.IsChanged((AddressModel)TempData["billing_address"]))
                {
                    this.cartService.UpdateAddresses(new List<Party>() { billingAddress.ToParty() });
                }

                this.cartService.SetBillingAddressToCart(new CartParty()
                {
                    ExternalId = billingAddress.PartyId,
                    PartyID = billingAddress.PartyId
                });

                // proceed to next step
                return this.RedirectToAction("ShippingAddress");
            }

            // Add a new address to customer
            var party = billingAddress.ToParty();
            var addresses = this.cartService.AddAddress(party);

            var address = addresses.FirstOrDefault(a => a.FirstName == party.FirstName && a.LastName == party.LastName && a.Email == party.Email);

            if (address == null)
            {
                this.ModelState.AddModelError(string.Empty, "Address could not be added to cart");
                return View(billingAddress);
            }

            var result = this.cartService.SetBillingAddressToCart(new CartParty
            {
                ExternalId = address.ExternalId,
                PartyID = address.PartyId
            });

            if (!result)
            {
                this.ModelState.AddModelError(string.Empty, "Could not update billing address");
                return View(billingAddress);
            }

            // proceed to next step
            return this.RedirectToAction("ShippingAddress");
        }

        /// <summary>
        /// Gets a billing address.
        /// </summary>
        /// <param name="id">The address party ID.</param>
        /// <returns>The Json content.</returns>
        [HttpGet]
        public JsonResult GetBillingAddress(string id)
        {
            var addressMmodel = this.cartService.GetAddresses().Select(a => a.ToAddressModel());
            return Json(addressMmodel.FirstOrDefault(m => m.PartyId == id), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Views the Shipping address.
        /// </summary>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult ShippingAddress()
        {
            ViewBag.Countries = Context.Database.Items["/sitecore/system/Settings/Analytics/Lookups/Countries/"];
            var address = this.cartService.GetShippingAddress() ?? this.cartService.GetBillingAddress();
            if (address == null)
            {
                address = this.cartService.GetAddresses().FirstOrDefault() ?? new Party();
            }

            ViewBag.Addresses = this.cartService.GetAddresses().Select(a => a.ToAddressModel());

            var shippingAddress = address.ToAddressModel();
            TempData["shipping_address"] = shippingAddress;

            return View("ShippingAddress", shippingAddress);
        }

        /// <summary>
        /// Views the shipping address.
        /// </summary>
        /// <param name="shippingAddress">The address model.</param>
        /// <param name="changeStep">The change step type.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public ActionResult ShippingAddress(AddressModel shippingAddress, ChangeStepType changeStep)
        {
            ViewBag.Countries = Context.Database.Items["/sitecore/system/Settings/Analytics/Lookups/Countries/"];
            ViewBag.Addresses = this.cartService.GetAddresses().Select(a => a.ToAddressModel());

            if (changeStep == ChangeStepType.Previous)
            {
                this.ModelState.Clear();

                return this.RedirectToAction("BillingAddress");
            }

            if (!this.ModelState.IsValid)
            {
                return View(shippingAddress);
            }

            // If address is not new
            if (!string.IsNullOrEmpty(shippingAddress.PartyId))
            {
                if (shippingAddress.IsChanged((AddressModel)TempData["shipping_address"]))
                {
                    this.cartService.UpdateAddresses(new List<Party>() { shippingAddress.ToParty() });
                }

                this.cartService.SetShippingAddressToCart(new CartParty()
                {
                    ExternalId = shippingAddress.PartyId,
                    PartyID = shippingAddress.PartyId
                });

                // proceed to next step
                return this.RedirectToAction("ShippingMethod");
            }

            var party = shippingAddress.ToParty();
            var addresses = this.cartService.AddAddress(party);

            var address = addresses.FirstOrDefault(a => a.FirstName == party.FirstName && a.LastName == party.LastName && a.Email == party.Email);

            if (address == null)
            {
                this.ModelState.AddModelError(string.Empty, "Address could not be added to cart");
                return View(shippingAddress);
            }

            var result = this.cartService.SetShippingAddressToCart(new CartParty()
            {
                ExternalId = address.ExternalId,
                PartyID = address.PartyId
            });

            if (!result)
            {
                this.ModelState.AddModelError(string.Empty, "Could not update shipping address");
                return View(shippingAddress);
            }

            // proceed to next step
            return this.RedirectToAction("ShippingMethod");
        }

        /// <summary>
        /// Views shipping methods.
        /// </summary>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult ShippingMethod()
        {
            var shippingOptions = this.checkoutService.GetShippingOptions();

            var shippingInfos = new List<ShippingInfoModel>();

            foreach (var sho in shippingOptions)
            {
                var option = new ShippingInfoModel()
                {
                    OptionName = sho.Name,
                    OptionType = sho.ShippingOptionType.Value,
                    Selected = sho.ShippingOptionType.Value == 0
                };

                shippingInfos.Add(option);
            }

            return View("ShippingMethod", shippingInfos);
        }

        /// <summary>
        /// Views the shipping method list.
        /// </summary>
        /// <param name="id">The shipping option ID.</param>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult ShippingMethodList(int id)
        {
            var selectedSippingInfo = this.cartService.GetShippingInfo() ?? new ShippingInfo();

            var shippingMethods = new List<ShippingMethodModel>();

            var shippingOption = this.checkoutService.GetShippingOptions().FirstOrDefault(i => i.ShippingOptionType.Value == id);
            if (shippingOption != null)
            {
                foreach (var method in this.checkoutService.GetShippingMethods(shippingOption))
                {
                    shippingMethods.Add(new ShippingMethodModel
                    {
                        Name = method.Name,
                        SystemName = method.ExternalId,
                        Selected = method.Name == selectedSippingInfo.ShippingMethodID
                    });
                }
            }

            return this.View(shippingMethods);
        }

        /// <summary>
        /// Handles a shipping method selection.
        /// </summary>
        /// <param name="shippingMethod">The shipping method.</param>
        /// <param name="changeStep">The change step.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public ActionResult ShippingMethod(string shippingMethod, ChangeStepType changeStep)
        {
            if (changeStep == ChangeStepType.Previous)
            {
                return this.RedirectToAction("ShippingAddress");
            }

            if (changeStep == ChangeStepType.Next && !string.IsNullOrEmpty(shippingMethod))
            {
                var values = shippingMethod.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (values.Length == 2)
                {
                    this.cartService.SetShippingMethodToCart(new ShippingInfo()
                    {
                        ShippingProviderID = values[0],
                        ShippingMethodID = values[1]
                    });

                    Session["shipping_method"] = values[1];

                    return this.RedirectToAction("PaymentMethod");
                }
            }

            return RedirectToAction("ShippingMethod");
        }

        /// <summary>
        /// Views payment methods.
        /// </summary>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult PaymentMethod()
        {
            var paymentOptions = this.checkoutService.GetPaymentOptions() ?? Enumerable.Empty<PaymentOption>();

            var paymentOptinModel = new List<PaymentOptionModel>();

            foreach (var sho in paymentOptions)
            {
                var option = new PaymentOptionModel()
                {
                    OptionName = sho.Name,
                    OptionType = sho.PaymentOptionType.Value,
                    Selected = sho.PaymentOptionType.Value == 0
                };

                paymentOptinModel.Add(option);
            }

            return View("PaymentMethod", paymentOptinModel);
        }

        /// <summary>
        /// Views a payment method list.
        /// </summary>
        /// <param name="id">The selected payment option type.</param>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult PaymentMethodList(int id)
        {
            var selectedPaymentInfo = this.cartService.GetPaymentInfo() ?? new PaymentInfo();

            var paymentMethods = new List<PaymentMethodModel>();

            var paymentOption = this.checkoutService.GetPaymentOptions().FirstOrDefault(i => i.PaymentOptionType.Value == id);
            if (paymentOption != null)
            {
                foreach (var method in this.checkoutService.GetPaymentMethods(paymentOption))
                {
                    paymentMethods.Add(new PaymentMethodModel
                    {
                        Name = method.Name,
                        SystemName = method.ExternalId,
                        Selected = method.ExternalId == selectedPaymentInfo.PaymentProviderID
                    });
                }
            }

            return View(paymentMethods);
        }

        /// <summary>
        /// Processes the next step of the payment method page.
        /// </summary>
        /// <param name="paymentMethod">The payment method.</param>
        /// <param name="changeStep">The change step type.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public ActionResult PaymentMethod(string paymentMethod, ChangeStepType changeStep)
        {
            if (changeStep == ChangeStepType.Previous)
            {
                return this.RedirectToAction("ShippingMethod");
            }

            if (changeStep == ChangeStepType.Next && !string.IsNullOrEmpty(paymentMethod))
            {
                var values = paymentMethod.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (values.Length == 2)
                {
                    this.cartService.SetPaymentMethodToCart(new PaymentInfo()
                    {
                        PaymentProviderID = values[0],
                        PaymentMethodID = values[1]
                    });

                    Session["payment_method"] = values[1];

                    Session["payment_method_provider"] = values[0];

                    return this.RedirectToAction("PaymentInformation", new { paymentMethodProvider = values[0] });
                }
            }

            return RedirectToAction("PaymentMethod");
        }

        /// <summary>
        /// Views payment information.
        /// </summary>
        /// <param name="paymentMethodProvider">The payment method provider.</param>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult PaymentInformation(string paymentMethodProvider)
        {
            Session["payment_method_provider"] = paymentMethodProvider;

            return PaymentInformationViewSelector(paymentMethodProvider);
        }

        /// <summary>
        /// Processes payment information.
        /// </summary>
        /// <param name="cardInfo">The credit card information.</param>
        /// <param name="changeStep">The change step type.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public ActionResult PaymentInformation(CreditCardModel cardInfo, ChangeStepType changeStep)
        {
            if (changeStep == ChangeStepType.Previous)
            {
                return this.RedirectToAction("PaymentMethod");
            }

            string paymentMethod = Session["payment_method_provider"].ToString();

            if (paymentMethod.Equals("Payments.SamplePaymentService", StringComparison.OrdinalIgnoreCase))
            {
                var actionResult = this.checkoutService.GetPaymentServiceActionResult(cardInfo.ResultAccessCode);
                if (!actionResult.Success ||
                    !string.Equals(actionResult.AuthorizationResult, AuthorizationResult.Success.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    // authorization failed.
                    return PaymentInformationViewSelector(paymentMethod);
                }

                cardInfo.CardToken = actionResult.CardToken;
            }

            if (changeStep == ChangeStepType.Next && cardInfo != null)
            {

                if (!paymentMethod.Equals("Payments.Manual") || this.ModelState.IsValid)
                {
                    //session save
                    Session["order_payment_info"] = cardInfo;
                    return RedirectToAction("Confirmation");
                }
            }

            return PaymentInformationViewSelector(paymentMethod);
        }

        /// <summary>
        /// Views the confirmation page.
        /// </summary>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult Confirmation()
        {
            var confirmModel = new ConfirmModel();

            var shippingAddress = this.cartService.GetShippingAddress();
            confirmModel.ShippingAddress = shippingAddress.ToAddressModel();

            var billingAddress = this.cartService.GetBillingAddress();
            confirmModel.BillingAddress = billingAddress.ToAddressModel();

            confirmModel.ShippingMethod = (string)Session["shipping_method"];
            confirmModel.PaymentMethod = (string)Session["payment_method"];

            return View("Confirmation", confirmModel);
        }

        /// <summary>
        /// Processes the confirmation page.
        /// </summary>
        /// <param name="changeStep">The change step type.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public ActionResult Confirmation(ChangeStepType changeStep)
        {
            if (changeStep == ChangeStepType.Previous)
            {
                return this.RedirectToAction("PaymentInformation", new { paymentMethodProvider = Session["payment_method_provider"].ToString() });
            }

            if (changeStep == ChangeStepType.Confirm)
            {
                string paymentMethodProvider = Session["payment_method_provider"].ToString();

                CreditCardModel cardInfo = (CreditCardModel)Session["order_payment_info"];

                var paymentInformation = new PropertyCollection();

                if (paymentMethodProvider.Equals("Payments.Manual", StringComparison.OrdinalIgnoreCase))
                {
                    paymentInformation = new PropertyCollection
                    {
                      { "CardType", cardInfo.CreditCardType },
                      { "CardName", cardInfo.CardholderName },
                      { "CardNumber", cardInfo.CardNumber },
                      { "CardExpirationMonth", cardInfo.ExpireMonth },
                      { "CardExpirationYear", cardInfo.ExpireYear },
                      { "CardCvv2", cardInfo.CardCode }
                    };
                }
                else if (paymentMethodProvider.Equals("Payments.SamplePaymentService", StringComparison.OrdinalIgnoreCase))
                {
                    paymentInformation = new PropertyCollection
                    {
                        { "CardType", cardInfo.CreditCardType },
                        { "CardPrefix", cardInfo.CreditCardPrefix },
                        { "ResultAccessCode", cardInfo.ResultAccessCode },
                        { "CardToken", cardInfo.CardToken }
                    };
                }

                var orderID = orderService.SetPaymentInformationAndSubmit(paymentInformation, paymentMethodProvider);

                return RedirectToAction("Success", new { id = orderID });
            }

            return RedirectToAction("Confirmation");
        }

        /// <summary>
        /// Views the success page.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <returns>The action result.</returns>
        [HttpGet]
        public ActionResult Success(string id)
        {
            ViewBag.OrderId = id;
            return View("Success");
        }

        /// <summary>
        /// Selects the payment information view.
        /// </summary>
        /// <param name="paymentMethod">The payment method name.</param>
        /// <returns>The action result.</returns>
        protected ActionResult PaymentInformationViewSelector(string paymentMethod)
        {
            if (paymentMethod.Equals("Payments.Manual"))
            {
                CreditCardModel cardInfo = (CreditCardModel)Session["order_payment_info"];

                if (cardInfo == null)
                {
                    cardInfo = new CreditCardModel();
                }

                if (cardInfo.ExpireMonth == null)
                {
                    cardInfo.ExpireMonth = DateTime.Now.Month.ToString();
                }

                if (cardInfo.ExpireYear == null)
                {
                    cardInfo.ExpireYear = DateTime.Now.Year.ToString();
                }

                //CC types
                cardInfo.CreditCardTypes.Clear();

                cardInfo.CreditCardTypes.Add(new SelectListItem()
                {
                    Text = "Visa",
                    Value = "Visa",
                });
                cardInfo.CreditCardTypes.Add(new SelectListItem()
                {
                    Text = "Master card",
                    Value = "MasterCard",
                });
                cardInfo.CreditCardTypes.Add(new SelectListItem()
                {
                    Text = "Discover",
                    Value = "Discover",
                });
                cardInfo.CreditCardTypes.Add(new SelectListItem()
                {
                    Text = "Amex",
                    Value = "Amex",
                });

                return View("PaymentMethods/CreditCard", cardInfo);
            }

            if (paymentMethod.Equals("Payments.SamplePaymentService"))
            {
                var serviceUrl = new Uri(this.checkoutService.GetPaymentServiceUrl());
                var cart = this.cartService.GetCart();

                var serviceModel = new SamplePaymentServiceModel();
                serviceModel.PaymentServiceUrl = serviceUrl.ToString();
                serviceModel.CardPageOrigin = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", serviceUrl.Scheme, serviceUrl.Authority);
                serviceModel.PaymentAmount = cart.Total.Amount;

                return View("PaymentMethods/SamplePaymentService", serviceModel);
            }

            if (paymentMethod.Equals("COD"))
            {
                return View("PaymentMethods/Static");
            }

            return View("PaymentMethods/Static");
        }
    }
}