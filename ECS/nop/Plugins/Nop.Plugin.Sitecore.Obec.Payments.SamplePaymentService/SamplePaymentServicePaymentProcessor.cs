// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplePaymentServicePaymentProcessor.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the SamplePaymentServicePaymentProcessor class.
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

namespace Nop.Plugin.Sitecore.Obec.Payments.SamplePaymentService
{
    using Nop.Core.Domain.Orders;
    using Nop.Core.Domain.Payments;
    using Nop.Core.Plugins;
    using Nop.Plugin.Sitecore.Obec.Payments.SamplePaymentService.Controllers;
    using Nop.Services.Configuration;
    using Nop.Services.Localization;
    using Nop.Services.Orders;
    using Nop.Services.Payments;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Web.Routing;

    /// <summary>
    /// Defines the sample payment service payment method processor.
    /// </summary>
    public class SamplePaymentServicePaymentProcessor : BasePlugin, IPaymentMethod, IPlugin
    {
        private readonly SamplePaymentServicePaymentSettings paymentSettings;
        private readonly IOrderTotalCalculationService orderTotalCalculationService;
        private readonly ISettingService settingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SamplePaymentServicePaymentProcessor"/> class.
        /// </summary>
        /// <param name="paymentSettings">The payment settings.</param>
        /// <param name="settingService">The settings service.</param>
        /// <param name="orderTotalCalculationService">The order total calculation service.</param>
        public SamplePaymentServicePaymentProcessor(SamplePaymentServicePaymentSettings paymentSettings, ISettingService settingService, IOrderTotalCalculationService orderTotalCalculationService)
        {
            this.paymentSettings = paymentSettings;
            this.settingService = settingService;
            this.orderTotalCalculationService = orderTotalCalculationService;
        }

        /// <summary>
        /// Cancels a recurring payment.  This method is not supported.
        /// </summary>
        /// <param name="cancelPaymentRequest">The request.</param>
        /// <returns>The result.</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            CancelRecurringPaymentResult result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Checks if a payment can be reposted.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>A valud indicating if the payment can be reposted.</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            return false;
        }

        /// <summary>
        /// Captures an order payment.  This method is not supported.
        /// </summary>
        /// <param name="capturePaymentRequest">The request.</param>
        /// <returns>The result.</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            CapturePaymentResult result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Gets the additional handling fee for a cart.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns>The additional handling fee.</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return this.CalculateAdditionalFee(this.orderTotalCalculationService, cart, this.paymentSettings.AdditionalFee, this.paymentSettings.AdditionalFeePercentage);
        }

        /// <summary>
        /// Gets a configuration route.
        /// </summary>
        /// <param name="actionName">Recieves the action name.</param>
        /// <param name="controllerName">Recieves the controller name.</param>
        /// <param name="routeValues">Recieves the route value dicationary.</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentSamplePaymentService";
            RouteValueDictionary dictionary = new RouteValueDictionary();
            dictionary.Add("Namespaces", "Nop.Plugin.Sitecore.Obec.Payments.SamplePaymentService.Controllers");
            dictionary.Add("area", null);
            routeValues = dictionary;
        }

        /// <summary>
        /// Gets the type of the controller associated with this processor.
        /// </summary>
        /// <returns>The controller type.</returns>
        public Type GetControllerType()
        {
            return typeof(PaymentSamplePaymentServiceController);
        }

        /// <summary>
        /// Gets  the payment info route.
        /// </summary>
        /// <param name="actionName">Recieves the action name.</param>
        /// <param name="controllerName">Recieves the controller name.</param>
        /// <param name="routeValues">Recieves the route value dictionary.</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentSamplePaymentService";
            RouteValueDictionary dictionary = new RouteValueDictionary();
            dictionary.Add("Namespaces", "Nop.Plugin.Sitecore.Obec.Payments.SamplePaymentService.Controllers");
            dictionary.Add("area", null);
            routeValues = dictionary;
        }

        /// <summary>
        /// Installs the processor.
        /// </summary>
        public override void Install()
        {
            SamplePaymentServicePaymentSettings settings = new SamplePaymentServicePaymentSettings
            {
                DescriptionText = "<p>Sitecore Commerce Connect sample payment service.</p>"
            };

            this.settingService.SaveSetting<SamplePaymentServicePaymentSettings>(settings, 0);
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.SamplePaymentService.DescriptionText", "Description");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.SamplePaymentService.DescriptionText.Hint", "Enter info that will be shown to customers during checkout");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.SamplePaymentService.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.SamplePaymentService.AdditionalFee.Hint", "The additional fee.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.SamplePaymentService.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.SamplePaymentService.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            base.Install();
        }

        /// <summary>
        /// Executes payment post-processing.
        /// </summary>
        /// <param name="postProcessPaymentRequest">The request.</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
        }

        /// <summary>
        /// Processes a payment.
        /// </summary>
        /// <param name="processPaymentRequest">The request.</param>
        /// <returns>The result.</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending };
        }

        /// <summary>
        /// Processes a recurring payment.  This method is not supported.
        /// </summary>
        /// <param name="processPaymentRequest">The request.</param>
        /// <returns>The result.</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Processes a refund.  This method is not supported.
        /// </summary>
        /// <param name="refundPaymentRequest">The request.</param>
        /// <returns>The result.</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            RefundPaymentResult result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Uninstalls the plugin.
        /// </summary>
        public override void Uninstall()
        {
            this.settingService.DeleteSetting<SamplePaymentServicePaymentSettings>();
            this.DeletePluginLocaleResource("Plugins.Payment.SamplePaymentService.DescriptionText");
            this.DeletePluginLocaleResource("Plugins.Payment.SamplePaymentService.DescriptionText.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.SamplePaymentService.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payment.SamplePaymentService.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.SamplePaymentService.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payment.SamplePaymentService.AdditionalFeePercentage.Hint");
            base.Uninstall();
        }

        /// <summary>
        /// Voids a payment.  This method is not supported.
        /// </summary>
        /// <param name="voidPaymentRequest">The request.</param>
        /// <returns>The result.</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            VoidPaymentResult result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Gets the payment method type.
        /// </summary>
        public Nop.Services.Payments.PaymentMethodType PaymentMethodType
        {
            get
            {
                return Nop.Services.Payments.PaymentMethodType.Standard;
            }
        }

        /// <summary>
        /// Gets the recurring payment type.
        /// </summary>
        public Nop.Services.Payments.RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return Nop.Services.Payments.RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a value indicating whether capture is supported.
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refunds are supported.
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refunds are supported.
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a valud indicating whether voided payments are supported.
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }
    }
}
