// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShippingInfoExtensions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The cart extensions.
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
namespace Sitecore.Commerce.Connectors.NopCommerce
{
    using System.Linq;
    using Sitecore.Commerce.Connectors.NopCommerce.NopOrdersService;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The cart extensions.
    /// </summary>
    public static class ShippingInfoExtensions
    {
        /// <summary>
        /// Maps cart from model.
        /// </summary>
        /// <param name="info">The shipping information.</param>
        /// <param name="shipmentModel">The shipment model.</param>
        public static void MapShipmentFromModel([NotNull] this ShippingInfo info, [NotNull] ShipmentModel shipmentModel)
        {
            Assert.ArgumentNotNull(info, "info");
            Assert.ArgumentNotNull(shipmentModel, "shipmentModel");

            info.ExternalId = shipmentModel.Id.ToString();
            info.LineIDs = shipmentModel.ItemsIDs.OfType<string>().ToList().AsReadOnly();

            if (shipmentModel.ShippingAddressId != null)
            {
                info.PartyID = shipmentModel.ShippingAddressId.Value.ToString();
            }

            info.ShippingMethodID = shipmentModel.ShippingMethod;
        }
    }
}