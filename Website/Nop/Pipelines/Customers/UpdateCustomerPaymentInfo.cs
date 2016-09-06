// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCustomerPaymentInfo.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the UpdateCustomerPaymentInfo class.</summary>
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

namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Customers
{
    using Newtonsoft.Json;
    using Sitecore.Commerce.Connectors.NopCommerce.NopCustomersService;
    using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common;
    using Sitecore.Commerce.Data.Converters;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Customers;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Customers;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Pipeline processor that adds customer payment information.
    /// </summary>
    public class UpdateCustomerPaymentInfo : NopProcessor<ICustomersServiceChannel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCustomerPaymentInfo"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public UpdateCustomerPaymentInfo(IEntityFactory entityFactory)
        {
            this.EntityFactory = entityFactory;
        }

        /// <summary>
        /// Gets the entity factory.
        /// </summary>
        public IEntityFactory EntityFactory { get; private set; }

        /// <summary>
        /// Processes the arguments.
        /// </summary>
        /// <param name="args">The pipeline arguments.</param>
        public override void Process(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var addresses = new List<AddressModel>();
            Guid customerId;

            var request = (UpdateCustomerPaymentInfoRequest)args.Request;
            var result = (CustomerPaymentInfoResult)args.Result;

            using (ICustomersServiceChannel channel = this.GetClient())
            {
                try
                {
                    customerId = new Guid(request.CommerceCustomer.ExternalId);
                }
                catch
                {
                    result.Success = false;
                    result.SystemMessages.Add(new SystemMessage { Message = "Cannot parse customer Guid " + request.CommerceCustomer.ExternalId });
                    return;
                }

                var customerModel = channel.GetCustomer(customerId);
                var existingPayments = JsonConvert.DeserializeObject<List<CustomerPaymentInfo>>(customerModel.AdminContent, new EntityJsonConverter<CommerceCustomer>(this.EntityFactory));
                for (int i = 0; i < existingPayments.Count; i++)
                {
                    var updatedPayment = request.Payments.FirstOrDefault(p => string.Equals(p.ExternalId, existingPayments[i].ExternalId, StringComparison.OrdinalIgnoreCase));
                    if (updatedPayment != null)
                    {
                        existingPayments[i] = updatedPayment;
                    }
                }

                customerModel.AdminContent = JsonConvert.SerializeObject(existingPayments);

                try
                {
                    var nopResponse = channel.CreateCustomer(customerId, customerModel);
                    if (nopResponse.IsRegistered)
                    {
                        request.Properties.Add("UserId", customerId);
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.SystemMessages.Add(new SystemMessage { Message = "Error occuder while updating payment information for Cusomer " + request.CommerceCustomer.ExternalId });
                    }
                }
                catch (Exception)
                {
                    result.Success = false;
                    result.SystemMessages.Add(new SystemMessage { Message = "Communication error, while updating payment information for Cusomer " + request.CommerceCustomer.ExternalId });
                }
            }
        }
    }
}
