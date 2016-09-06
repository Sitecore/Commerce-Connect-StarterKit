// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetPaymentOptions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the GetPaymentOptions class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Payments.GetPaymentOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Xml;
    using Commerce.Entities.Payments;
    using Commerce.Pipelines;
    using Commerce.Services.Payments;
    using Common;
    using Diagnostics;
    using Entities.Payments;
    using NopPaymentService;
    using Xml;

    /// <summary>
    /// The pipeline processor that gets payment options.
    /// </summary>
    public class GetPaymentOptions : NopProcessor<IPaymentServiceChannel>
    {
        /// <summary>
        /// The payment option list.
        /// </summary>
        protected readonly Dictionary<int, PaymentOption> PaymentOptionList;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPaymentOptions"/> class.
        /// </summary>
        public GetPaymentOptions()
        {
            this.PaymentOptionList = new Dictionary<int, PaymentOption>(0);
        }

        /// <summary>
        /// Processes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            var result = (GetPaymentOptionsResult)args.Result;

            result.PaymentOptions = new ReadOnlyCollection<PaymentOption>(this.PaymentOptionList.Values.ToList());
        }

        /// <summary>
        /// Add shipping optin by configuration setting
        /// </summary>
        /// <param name="configNode">The configuration node.</param>
        public virtual void AddPaymentOption(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, "configNode");

            string value = XmlUtil.GetAttribute("value", configNode);

            Assert.IsNotNullOrEmpty(value, "value");

            int key = MainUtil.GetInt(value, 0);

            if (!this.PaymentOptionList.ContainsKey(key))
            {
                var paymentOption = new PaymentOption();

                switch (key)
                {
                    case 1:
                        paymentOption.PaymentOptionType = PaymentOptionType.PayCard;
                        paymentOption.Name = Globalization.Translate.Text(Texts.PayCard);
                        paymentOption.Description = Globalization.Translate.Text(Texts.PayCard);
                        break;
                    case 2:
                        paymentOption.PaymentOptionType = PaymentOptionType.PayLoyaltyCard;
                        paymentOption.Name = Globalization.Translate.Text(Texts.PayLoyaltyCard);
                        paymentOption.Description = Globalization.Translate.Text(Texts.PayLoyaltyCard);
                        break;
                    case 3:
                        paymentOption.PaymentOptionType = PaymentOptionType.PayGiftCard;
                        paymentOption.Name = Globalization.Translate.Text(Texts.PayGiftCard);
                        paymentOption.Description = Globalization.Translate.Text(Texts.PayGiftCard);
                        break;
                    case 4:
                        paymentOption.PaymentOptionType = NopPaymentOptionType.OnlinePayment;
                        paymentOption.Name = Globalization.Translate.Text(Texts.OnlinePayment);
                        paymentOption.Description = Globalization.Translate.Text(Texts.OnlinePayment);
                        break;
                    case 0:
                        paymentOption.PaymentOptionType = PaymentOptionType.None;
                        paymentOption.Name = Globalization.Translate.Text(Texts.NoPaymentPreference);
                        paymentOption.Description = Globalization.Translate.Text(Texts.NoPaymentPreference);
                        break;
                }

                this.PaymentOptionList[key] = paymentOption;
            }
        }
    }
}