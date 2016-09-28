// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddressModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The implementation of AddressModel.
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
namespace Nop.Plugin.Sitecore.Commerce.Common.Models
{
    using System;
    using System.Runtime.Serialization;
    using Nop.Core.Domain.Common;
    using Nop.Core.Domain.Directory;
    using Nop.Core.Infrastructure;
    using Nop.Services.Directory;
    using Nop.Services.Orders;

    /// <summary>
    /// Address model.
    /// </summary>
    [DataContract]
    public class AddressModel
    {
        /// <summary>
        /// The country service provider.
        /// </summary>
        protected ICountryService countryService;

        /// <summary>
        /// The state/province service provider.
        /// </summary>
        protected IStateProvinceService stateProvinceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressModel"/> class.
        /// </summary>
        public AddressModel()
        {
            this.countryService = EngineContext.Current.Resolve<ICountryService>();
            this.stateProvinceService = EngineContext.Current.Resolve<IStateProvinceService>();
        }

        /// <summary>
        /// Gets or sets the state province AddressId.
        /// </summary>
        /// <value>
        /// The address id
        /// </value>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>
        /// The company.
        /// </value>
        [DataMember]
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        [DataMember]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the address1.
        /// </summary>
        /// <value>
        /// The address1.
        /// </value>
        [DataMember]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address2.
        /// </summary>
        /// <value>
        /// The address2.
        /// </value>
        [DataMember]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the zip postal code.
        /// </summary>
        /// <value>
        /// The zip postal code.
        /// </value>
        [DataMember]
        public string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        [DataMember]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the fax number.
        /// </summary>
        /// <value>
        /// The fax number.
        /// </value>
        [DataMember]
        public string FaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the created configuration UTC.
        /// </summary>
        /// <value>
        /// The created configuration UTC.
        /// </value>
        [DataMember]
        public DateTime? CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the two letter iso code.
        /// </summary>
        /// <value>
        /// The two letter iso code.
        /// </value>
        [DataMember]
        public string CountryTwoLetterIsoCode { get; set; }

        /// <summary>
        /// Gets or sets the three letter iso code.
        /// </summary>
        /// <value>
        /// The three letter iso code.
        /// </value>
        [DataMember]
        public string CountryThreeLetterIsoCode { get; set; }

        /// <summary>
        /// Gets or sets the state province abbreviation.
        /// </summary>
        /// <value>
        /// The state province abbreviation.
        /// </value>
        [DataMember]
        public string StateProvinceAbbreviation { get; set; }

        /// <summary>
        /// maps an address into this instance.
        /// </summary>
        /// <param name="address">The NOP address.</param>
        public void Map(Address address)
        {
            this.Address1 = address.Address1;
            this.Address2 = address.Address2;
            this.City = address.City;
            this.Company = address.Company;
            this.CountryThreeLetterIsoCode = address.Country != null ? address.Country.ThreeLetterIsoCode : null;
            this.CountryTwoLetterIsoCode = address.Country != null ? address.Country.TwoLetterIsoCode : null;
            this.CreatedOnUtc = address.CreatedOnUtc;
            this.Email = address.Email;
            this.FaxNumber = address.FaxNumber;
            this.FirstName = address.FirstName;
            this.LastName = address.LastName;
            this.Id = address.Id.ToString();
            this.PhoneNumber = address.PhoneNumber;
            this.StateProvinceAbbreviation = address.StateProvince != null ? address.StateProvince.Abbreviation : null;
            this.ZipPostalCode = address.ZipPostalCode;
        }

        /// <summary>
        /// Maps this instance to a NOP address.
        /// </summary>
        /// <returns>The NOP address.</returns>
        public Address MapToAddress()
        {
            int? countryId = null;
            if (!string.IsNullOrEmpty(this.CountryTwoLetterIsoCode))
            {
                countryId = this.countryService.GetCountryByTwoLetterIsoCode(this.CountryTwoLetterIsoCode).Id;
            }
            else if (!string.IsNullOrEmpty(this.CountryThreeLetterIsoCode))
            {
                countryId = this.countryService.GetCountryByThreeLetterIsoCode(this.CountryThreeLetterIsoCode).Id;
            }

            int? provinceId = null;
            if (!string.IsNullOrEmpty(this.StateProvinceAbbreviation))
            {
                provinceId = this.stateProvinceService.GetStateProvinceByAbbreviation(this.StateProvinceAbbreviation).Id;
            }

            return new Address
            {
                Id = int.Parse(this.Id),
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                Company = this.Company,
                City = this.City,
                Address1 = this.Address1,
                Address2 = this.Address2,
                ZipPostalCode = this.ZipPostalCode,
                PhoneNumber = this.PhoneNumber,
                FaxNumber = this.FaxNumber,
                CreatedOnUtc = this.CreatedOnUtc ?? DateTime.UtcNow,
                CountryId = countryId,
                Country = new Country
                  {
                      ThreeLetterIsoCode = this.CountryThreeLetterIsoCode,
                      TwoLetterIsoCode = this.CountryTwoLetterIsoCode
                  },
                StateProvinceId = provinceId,
                StateProvince = new StateProvince
                  {
                      Abbreviation = this.StateProvinceAbbreviation
                  },
            };
        }
    }
}