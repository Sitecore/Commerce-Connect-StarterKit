// ---------------------------------------------------------------------
// <copyright file="PartyExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The cart extensions.
// </summary>
// ---------------------------------------------------------------------
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
  using Sitecore.Commerce.Connectors.NopCommerce.NopOrdersService;
  using Sitecore.Commerce.Entities;
  using Sitecore.Diagnostics;

  /// <summary>
  /// The cart extensions.
  /// </summary>
  public static class PartyExtensions
  {
    /// <summary>
    /// Maps cart from model.
    /// </summary>
    public static void MapPartyFromNopAddress([NotNull] this Party party, [NotNull] AddressModel address)
    {
      Assert.ArgumentNotNull(party, "party");
      Assert.ArgumentNotNull(address, "address");

      party.Address1 = address.Address1;
      party.Address2 = address.Address2;
      party.City = address.City;
      party.Company = address.Company;
      party.Country = address.CountryThreeLetterIsoCode;
      party.Email = address.Email;
      party.ExternalId = address.Id;
      party.FirstName = address.FirstName;
      party.LastName = address.LastName;
      party.PartyId = address.Id;
      party.PhoneNumber = address.PhoneNumber;
      party.State = address.StateProvinceAbbreviation;
      party.ZipPostalCode = address.ZipPostalCode;
    }
  }
}