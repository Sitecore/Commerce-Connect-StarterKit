// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the PaymentService class.</summary>
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
namespace Nop.Plugin.Sitecore.Commerce.Payments
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Services;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Sitecore.Commerce.Payments.Models;
    using Nop.Services.Stores;

    /// <summary>
    /// The payment service.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly Services.Payments.IPaymentService paymentService;

        private readonly IStoreService storeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        public PaymentService()
        {
            this.paymentService = EngineContext.Current.Resolve<Services.Payments.IPaymentService>();
            this.storeService = EngineContext.Current.Resolve<IStoreService>();
        }

        /// <summary>
        /// Get payment methods
        /// </summary>
        /// <param name="storeName">The store name.</param>
        /// <returns>Thhe payment methods.</returns>
        [WebMethod(EnableSession = false)]
        public ResponseModel<IEnumerable<PaymentMethodModel>> GetPaymentMethods(string storeName)
        {
            var store = this.storeService.GetAllStores().FirstOrDefault(s => s.Name == storeName);

            var storeId = (store == null) ? 0 : store.Id;
            var paymentMethods = this.paymentService.LoadActivePaymentMethods(storeId);

            var result = new List<PaymentMethodModel>();
            result.AddRange(paymentMethods.Select(paymentMethod => new PaymentMethodModel()
            {
                ShopName = storeName,
                MethodName = paymentMethod.PluginDescriptor.FriendlyName,
                SystemName = paymentMethod.PluginDescriptor.SystemName
            }));

            return new ResponseModel<IEnumerable<PaymentMethodModel>> { Success = true, Result = result };
        }
    }
}