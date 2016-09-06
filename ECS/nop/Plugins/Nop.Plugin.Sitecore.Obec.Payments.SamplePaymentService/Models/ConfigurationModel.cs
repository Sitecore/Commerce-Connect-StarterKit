// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ConfigurationModel class.
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

namespace Nop.Plugin.Sitecore.Obec.Payments.SamplePaymentService.Models
{
    using Nop.Web.Framework;
    using Nop.Web.Framework.Mvc;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    /// <summary>
    /// Defines the configuration model for the sample payment service payment method.
    /// </summary>
    public class ConfigurationModel : BaseNopModel
    {
        /// <summary>
        /// Gets or sets the active store scope configuration.
        /// </summary>
        public int ActiveStoreScopeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the additonal fee associated with this payment method.
        /// </summary>
        [NopResourceDisplayName("Plugins.Payment.SamplePaymentService.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets the additional fee associated with this payment method for a store.
        /// </summary>
        public bool AdditionalFee_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets the additonal fee percentage associated with this payment method.
        /// </summary>
        [NopResourceDisplayName("Plugins.Payment.SamplePaymentService.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets the additional fee percentage associated with this payment method for a store.
        /// </summary>
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets the payment method description.
        /// </summary>
        [AllowHtml, NopResourceDisplayName("Plugins.Payment.SamplePaymentService.DescriptionText")]
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets or sets the payment method description for a store.
        /// </summary>
        public bool DescriptionText_OverrideForStore { get; set; }
    }
}
