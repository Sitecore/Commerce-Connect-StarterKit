// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddressHelper.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the AddressHelper class.</summary>
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
namespace Sitecore.Commerce.StarterKit.Helpers
{
    using Entities;
    using Models;

    /// <summary>
    /// The address helper 
    /// </summary>
    public static class AddressHelper
    {
        /// <summary>
        /// Map party to address model
        /// </summary>
        /// <param name="party">The party.</param>
        /// <returns>The mapped address model.</returns>
        public static AddressModel ToAddressModel(this Party party)
        {
            var addressModel = new AddressModel()
            {
                Address1 = party.Address1,
                Address2 = party.Address2,
                FirstName = party.FirstName,
                LastName = party.LastName,
                City = party.City,
                Country = party.Country,
                PhoneNumber = party.PhoneNumber,
                State = party.State,
                Company = party.Company,
                ZipPostalCode = party.ZipPostalCode,
                Email = party.Email,
                PartyId = party.PartyId ?? party.ExternalId
            };

            return addressModel;
        }

        /// <summary>
        /// Map address model to party
        /// </summary>
        /// <param name="addressModel">The address.</param>
        /// <returns>The party.</returns>
        public static Party ToParty(this AddressModel addressModel)
        {
            var party = new Party()
            {
                Address1 = addressModel.Address1,
                Address2 = addressModel.Address2,
                City = addressModel.City,
                Company = addressModel.Company,
                Country = addressModel.Country,
                Email = addressModel.Email,
                FirstName = addressModel.FirstName,
                LastName = addressModel.LastName,
                PhoneNumber = addressModel.PhoneNumber,
                State = addressModel.State,
                ZipPostalCode = addressModel.ZipPostalCode,
                PartyId = addressModel.PartyId,
                ExternalId = addressModel.PartyId
            };

            return party;
        }

        /// <summary>
        /// Display name
        /// </summary>
        /// <param name="addressModel">The address.</param>
        /// <returns>
        /// The formated display name
        /// </returns>
        public static string DisplayName(this AddressModel addressModel)
        {
            return string.Format("{0} {1}, {2} {3}, {4} {5}, {6}", addressModel.FirstName, addressModel.LastName, addressModel.Address1, addressModel.Address2, addressModel.City, addressModel.ZipPostalCode, addressModel.Country);
        }

        /// <summary>
        /// Is changed
        /// </summary>
        /// <param name="addressModel">The first address model.</param>
        /// <param name="address">The second addrress model.</param>
        /// <returns>True if the address are different, otherwise false.</returns>
        public static bool IsChanged(this AddressModel addressModel, AddressModel address)
        {
            return !addressModel.Equals(address);
        }
    }
}