// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentSamplePaymentServiceController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the PaymentSamplePaymentServiceController class.
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

namespace Sitecore.Commerce.Nop.Payments.SamplePaymentService.Controllers
{
    using global::Nop.Core;
    using Sitecore.Commerce.Nop.Payments.SamplePaymentService.Models;
    using global::Nop.Services.Configuration;
    using global::Nop.Services.Payments;
    using global::Nop.Services.Stores;
    using global::Nop.Web.Framework.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// The NOP payment controller for the sample federated payment service payment method.
    /// </summary>
    public class PaymentSamplePaymentServiceController : BaseNopPaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentSamplePaymentServiceController"/> class.
        /// </summary>
        /// <param name="workContext">The work context.</param>
        /// <param name="storeService">The store service.</param>
        /// <param name="settingService">The settings service.</param>
        /// <param name="storeContext">The store context.</param>
        public PaymentSamplePaymentServiceController(IWorkContext workContext, IStoreService storeService, ISettingService settingService, IStoreContext storeContext)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._storeContext = storeContext;
        }

        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <returns>The action result.</returns>
        [ChildActionOnly, AdminAuthorize]
        public ActionResult Configure()
        {
            int storeId = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
            SamplePaymentServicePaymentSettings settings = this._settingService.LoadSetting<SamplePaymentServicePaymentSettings>(storeId);
            ConfigurationModel model = new ConfigurationModel
            {
                DescriptionText = settings.DescriptionText,
                AdditionalFee = settings.AdditionalFee,
                AdditionalFeePercentage = settings.AdditionalFeePercentage,
                ActiveStoreScopeConfiguration = storeId
            };
            if (storeId > 0)
            {
                model.DescriptionText_OverrideForStore = this._settingService.SettingExists<SamplePaymentServicePaymentSettings, string>(settings, x => x.DescriptionText, storeId);
                model.AdditionalFee_OverrideForStore = this._settingService.SettingExists<SamplePaymentServicePaymentSettings, decimal>(settings, x => x.AdditionalFee, storeId);
                model.AdditionalFeePercentage_OverrideForStore = this._settingService.SettingExists<SamplePaymentServicePaymentSettings, bool>(settings, x => x.AdditionalFeePercentage, storeId);
            }
            return base.View("Sitecore.Commerce.Nop.Payments.SamplePaymentService.Views.PaymentSamplePaymentService.Configure", model);
        }


        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <param name="model">The payment service configuration model.</param>
        /// <returns>The action result.</returns>
        [AdminAuthorize, HttpPost, ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (base.ModelState.IsValid)
            {
                int storeId = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
                SamplePaymentServicePaymentSettings settings = this._settingService.LoadSetting<SamplePaymentServicePaymentSettings>(storeId);
                settings.DescriptionText = model.DescriptionText;
                settings.AdditionalFee = model.AdditionalFee;
                settings.AdditionalFeePercentage = model.AdditionalFeePercentage;
                if (model.DescriptionText_OverrideForStore || (storeId == 0))
                {
                    this._settingService.SaveSetting<SamplePaymentServicePaymentSettings, string>(settings, x => x.DescriptionText, storeId, false);
                }
                else if (storeId > 0)
                {
                    this._settingService.DeleteSetting<SamplePaymentServicePaymentSettings, string>(settings, x => x.DescriptionText, storeId);
                }
                if (model.AdditionalFee_OverrideForStore || (storeId == 0))
                {
                    this._settingService.SaveSetting<SamplePaymentServicePaymentSettings, decimal>(settings, x => x.AdditionalFee, storeId, false);
                }
                else if (storeId > 0)
                {
                    this._settingService.DeleteSetting<SamplePaymentServicePaymentSettings, decimal>(settings, x => x.AdditionalFee, storeId);
                }
                if (model.AdditionalFeePercentage_OverrideForStore || (storeId == 0))
                {
                    this._settingService.SaveSetting<SamplePaymentServicePaymentSettings, bool>(settings, x => x.AdditionalFeePercentage, storeId, false);
                }
                else if (storeId > 0)
                {
                    this._settingService.DeleteSetting<SamplePaymentServicePaymentSettings, bool>(settings, x => x.AdditionalFeePercentage, storeId);
                }
                this._settingService.ClearCache();
            }
            return this.Configure();
        }

        /// <summary>
        /// Retrieves payment information.
        /// </summary>
        /// <param name="form">The form collection.</param>
        /// <returns>An empty payment processing request.</returns>
        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        /// <summary>
        /// Retrieves payment information.
        /// </summary>
        /// <returns>The action result.</returns>
        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            SamplePaymentServicePaymentSettings settings = this._settingService.LoadSetting<SamplePaymentServicePaymentSettings>(this._storeContext.CurrentStore.Id);
            PaymentInfoModel model = new PaymentInfoModel
            {
                DescriptionText = settings.DescriptionText
            };

            return base.View("Sitecore.Commerce.Nop.Payments.SamplePaymentService.Views.PaymentSamplePaymentService.PaymentInfo", model);
        }

        /// <summary>
        /// Validates payment form data.
        /// </summary>
        /// <param name="form">The form collection.</param>
        /// <returns>A validation result.</returns>
        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            return new List<string>();
        }
    }
}
