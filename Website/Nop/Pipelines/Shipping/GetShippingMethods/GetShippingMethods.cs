// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetShippingMethods.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the GetShippingMethods class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Shipping.GetShippingMethods
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Commerce.Entities.Carts;
    using Commerce.Entities.Shipping;
    using Commerce.Services;
    using Commerce.Services.Shipping;
    using Common;
    using Diagnostics;
    using NopShippingService;
    using Xml;

    /// <summary>
    /// Defines the pipeline processor that gets shipping methods.
    /// </summary>
    public class GetShippingMethods : NopProcessor<IShippingServiceChannel>
    {
        /// <summary>
        /// Map collection
        /// </summary>
        protected readonly HashSet<Tuple<int, string, string>> MappingCollectin;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetShippingMethods"/> class.
        /// </summary>
        public GetShippingMethods()
        {
            MappingCollectin = new HashSet<Tuple<int, string, string>>();
        }

        /// <summary>
        /// Processes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process([NotNull]Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var request = (GetShippingMethodsRequest)args.Request;
            var result = (GetShippingMethodsResult)args.Result;

            using (var client = this.GetClient())
            {
                try
                {
                    AddressModel addressModel = null;
                    if (request.Party != null)
                    {
                        addressModel = new AddressModel
                        {
                            Id = request.Party.ExternalId,
                            FirstName = request.Party.FirstName,
                            LastName = request.Party.LastName,
                            Email = request.Party.Email,
                            Address1 = request.Party.Address1,
                            Address2 = request.Party.Address2,
                            City = request.Party.City,
                            Company = request.Party.Company,
                            CountryThreeLetterIsoCode = request.Party.Country, //TODO: make sure it is threee letter code
                            CountryTwoLetterIsoCode = string.Empty,
                            FaxNumber = string.Empty,
                            PhoneNumber = request.Party.PhoneNumber,
                            StateProvinceAbbreviation = request.Party.State,
                            ZipPostalCode = request.Party.ZipPostalCode
                        };
                    }

                    var cart = (Cart)request.Properties["cart"];

                    //TODO: Change 'new ShoppingCartModel()'. Map request.Cart to ShoppingCartModel
                    var response = client.GetShippingMethods(new ShoppingCartModel { CustomerGuid = new Guid(cart.ExternalId) }, request.ShippingOption.ShopName, addressModel);

                    result.Success = response.Success;

                    var shippinMethods = new List<ShippingMethod>(0);
                    foreach (var shippingMethodModel in response.Result)
                    {
                        if (this.MappingCollectin.Contains(Tuple.Create(request.ShippingOption.ShippingOptionType.Value, shippingMethodModel.SystemName, shippingMethodModel.Name)))
                        {
                            shippinMethods.Add(new ShippingMethod()
                            {
                                Name = shippingMethodModel.Name,
                                Description = shippingMethodModel.Description,
                                ExternalId = shippingMethodModel.SystemName
                            });
                        }
                    }

                    result.ShippingMethods = shippinMethods.AsReadOnly();

                    if (!string.IsNullOrEmpty(response.Message))
                    {
                        result.SystemMessages.Add(new SystemMessage() { Message = response.Message });
                    }
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.SystemMessages.Add(new SystemMessage() { Message = e.Message });
                }
            }
        }

        /// <summary>
        /// Map data from configuration
        /// </summary>
        /// <param name="configNode">The configuration node.</param>
        public virtual void Map(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, "configNode");

            string shippingOptionValue = XmlUtil.GetAttribute("shippingOptionValue", configNode);
            string systemName = XmlUtil.GetAttribute("systemName", configNode);
            string methodName = XmlUtil.GetAttribute("methodName", configNode);

            Assert.IsNotNull(shippingOptionValue, "shippingOptionValue");
            Assert.IsNotNull(systemName, "systemName");
            Assert.IsNotNull(methodName, "methodName");

            this.MappingCollectin.Add(Tuple.Create(int.Parse(shippingOptionValue), systemName, methodName));
        }
    }
}