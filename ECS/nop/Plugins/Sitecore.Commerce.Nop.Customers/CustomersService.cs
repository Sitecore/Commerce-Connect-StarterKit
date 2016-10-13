// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomersService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The implementation of Customers Service.
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
namespace Sitecore.Commerce.Nop.Customers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Activation;
    using System.Web.Services;

    using global::Nop.Core.Domain.Common;
    using global::Nop.Core.Domain.Customers;
    using global::Nop.Core.Infrastructure;
    using Common.Models;
    using global::Nop.Services.Common;
    using global::Nop.Services.Directory;

    /// <summary>
    /// The customer service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CustomersService : ICustomersService
    {
        /// <summary>
        /// The customer service.
        /// </summary>
        private readonly global::Nop.Services.Customers.ICustomerService customerService;

        private readonly IAddressService addressService;

        private readonly IGenericAttributeService genericAttributeService;

        /// <summary>
        /// The country service
        /// </summary>
        private readonly ICountryService countryService;

        /// <summary>
        /// The state province service
        /// </summary>
        private readonly IStateProvinceService stateProvinceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersService"/> class.
        /// </summary>
        public CustomersService()
        {
            this.customerService = EngineContext.Current.Resolve<global::Nop.Services.Customers.ICustomerService>();
            this.countryService = EngineContext.Current.Resolve<ICountryService>();
            this.stateProvinceService = EngineContext.Current.Resolve<IStateProvinceService>();
            this.addressService = EngineContext.Current.Resolve<IAddressService>();
            this.genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        }

        /// <summary>
        /// Gets the customer by customer unique identifier.
        /// </summary>
        /// <param name="customerId">The customer unique identifier.</param>
        /// <returns>The Connect customer model.</returns>
        public virtual CustomerModel GetCustomer(Guid customerId)
        {
            var customer = this.customerService.GetCustomerByGuid(customerId);
            if (customer == null)
            {
                return null;
            }

            return this.MapCustomerToCustomerModel(customer);
        }

        /// <summary>
        /// Creates the customer by customer unique identifier.
        /// </summary>
        /// <param name="customerId">The customer unique identifier.</param>
        /// <param name="customerModel">The customer information.</param>
        /// <returns>The customer model.</returns>
        [WebMethod(EnableSession = false)]
        public virtual CustomerModel CreateCustomer(Guid customerId, CustomerModel customerModel = null)
        {
            var customer = this.customerService.GetCustomerByGuid(customerId);

            if (customer == null && customerModel != null)
            {
                customerModel.CustomerGuid = customerId;
                this.CreateCustomer(customerModel);
            }
            else if (customerModel != null)
            {
                // customer actually exists - lets update it.
                customerModel.CustomerGuid = customerId;
                this.UpdateCustomer(customerModel, customer);
            }

            return customerModel;
        }

        /// <summary>
        /// Adds an address.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="addressModels">The address model.</param>
        /// <returns>A service response.</returns>
        [WebMethod(EnableSession = false)]
        public virtual Response<IEnumerable<AddressModel>> AddAddresses(Guid customerId, IList<AddressModel> addressModels)
        {
            var customer = this.customerService.GetCustomerByGuid(customerId);
            var result = new List<AddressModel>(0);
            if (customer == null)
            {
                return new Response<IEnumerable<AddressModel>>()
                {
                    Success = false,
                    Result = result,
                    Message = string.Format("Customer not foud. Customer Id: {0}", customerId)
                };
            }

            foreach (var addressModel in addressModels)
            {
                customer.Addresses.Add(this.MapAddressModelToAdress(addressModel));
            }

            try
            {
                customer.LastActivityDateUtc = DateTime.UtcNow;
                this.customerService.UpdateCustomer(customer);
            }
            catch (Exception)
            {
                return new Response<IEnumerable<AddressModel>>()
                {
                    Message = string.Format("Error during updating customer. Customer Id: {0}", customerId),
                    Result = result,
                    Success = false
                };
            }

            return this.GetAllAddresses(customerId);
        }

        /// <summary>
        /// Removes a list of addresses by customer unique identifier.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="addressIds">The IDs of the addresses to remove.</param>
        /// <returns>The service response.</returns>
        [WebMethod(EnableSession = false)]
        public virtual Response RemoveAddresses(Guid customerId, IList<string> addressIds)
        {
            var customer = this.customerService.GetCustomerByGuid(customerId);
            if (customer == null)
            {
                return new Response()
                {
                    Success = false,
                    Message = string.Format("Customer not foud. Customer Id: {0}", customerId)
                };
            }

            if (addressIds != null && customer.Addresses != null)
            {
                var addresses = customer.Addresses.Where(a => addressIds.Contains(a.Id.ToString()));
                addresses = addresses as IList<Address> ?? addresses.ToList();
                foreach (var address in addresses)
                {
                    customer.Addresses.Remove(address);
                }
            }

            try
            {
                customer.LastActivityDateUtc = DateTime.UtcNow;
                this.customerService.UpdateCustomer(customer);
            }
            catch (Exception)
            {
                return new Response()
                {
                    Message = string.Format("Error during updating customer. Customer Id: {0}", customerId),
                    Success = false
                };
            }

            return new Response() { Success = true };
        }

        /// <summary>
        /// Updates an address.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="addressModels">The address model.</param>
        /// <returns>A service response.</returns>
        [WebMethod(EnableSession = false)]
        public virtual Response UpdateAddresses(Guid customerId, IList<AddressModel> addressModels)
        {
            var customer = this.customerService.GetCustomerByGuid(customerId);
            if (customer == null)
            {
                return new Response()
                {
                    Success = false,
                    Message = string.Format("Customer not foud. Customer Id: {0}", customerId)
                };
            }

            if (addressModels != null && addressModels.Any() && customer.Addresses != null && customer.Addresses.Any())
            {
                foreach (var addressModel in addressModels)
                {
                    var address = customer.Addresses.FirstOrDefault(a => a.Id.ToString() == addressModel.Id);
                    if (address != null)
                    {
                        this.MapAddressModelToAdress(addressModel, address);
                    }
                }
            }

            try
            {
                customer.LastActivityDateUtc = DateTime.UtcNow;
                this.customerService.UpdateCustomer(customer);
            }
            catch (Exception)
            {
                return new Response()
                {
                    Message = string.Format("Error during updating customer. Customer Id: {0}", customerId),
                    Success = false
                };
            }

            return new Response() { Success = true };
        }

        /// <summary>
        /// Get all addresses by customer id
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>The service response.</returns>
        [WebMethod(EnableSession = false)]
        public virtual Response<IEnumerable<AddressModel>> GetAllAddresses(Guid customerId)
        {
            var customer = this.customerService.GetCustomerByGuid(customerId);
            if (customer == null)
            {
                return new Response<IEnumerable<AddressModel>>()
                {
                    Success = false,
                    Result = null,
                    Message = string.Format("Customer not foud. Customer Id: {0}", customerId)
                };
            }

            var result = new List<AddressModel>(0);
            foreach (var address in customer.Addresses)
            {
                result.Add(this.MapAddressToAdressModel(address));
            }

            return new Response<IEnumerable<AddressModel>>() { Result = result, Success = true };
        }

        #region Mappings

        /// <summary>
        /// Maps a NOP address to a Connect address model.
        /// </summary>
        /// <param name="address">The NOP address.</param>
        /// <returns>The Connect address model.</returns>
        protected virtual AddressModel MapAddressToAdressModel(Address address)
        {
            return new AddressModel
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                Email = address.Email,
                Company = address.Company,
                City = address.City,
                Address1 = address.Address1,
                Address2 = address.Address2,
                ZipPostalCode = address.ZipPostalCode,
                PhoneNumber = address.PhoneNumber,
                FaxNumber = address.FaxNumber,
                CreatedOnUtc = address.CreatedOnUtc,
                CountryThreeLetterIsoCode = address.CountryId.HasValue ? this.countryService.GetCountryById(address.CountryId.Value).ThreeLetterIsoCode : string.Empty,
                CountryTwoLetterIsoCode = address.CountryId.HasValue ? this.countryService.GetCountryById(address.CountryId.Value).TwoLetterIsoCode : string.Empty,
                StateProvinceAbbreviation = address.StateProvinceId.HasValue ? this.stateProvinceService.GetStateProvinceById(address.StateProvinceId.Value).Abbreviation : string.Empty,
                Id = address.Id.ToString()
            };
        }

        /// <summary>
        /// Maps a Connect address model to a NOP address.
        /// </summary>
        /// <param name="addressModel">The  Connect address model.</param>
        /// <param name="address">The NOP address.</param>
        /// <returns>The mapped NOP address.</returns>
        protected virtual Address MapAddressModelToAdress(AddressModel addressModel, Address address = null)
        {
            if (addressModel == null)
            {
                return null;
            }

            if (address == null)
            {
                address = new Address();
            }

            address.CreatedOnUtc = addressModel.CreatedOnUtc ?? DateTime.UtcNow;

            address.FirstName = addressModel.FirstName;
            address.LastName = addressModel.LastName;
            address.Email = addressModel.Email;
            address.Company = addressModel.Company;
            address.City = addressModel.City;
            address.Address1 = addressModel.Address1;
            address.Address2 = addressModel.Address2;
            address.ZipPostalCode = addressModel.ZipPostalCode;
            address.PhoneNumber = addressModel.PhoneNumber;
            address.FaxNumber = addressModel.FaxNumber;

            address.CountryId = this.GetCountryId(addressModel);
            address.StateProvinceId = this.GetStateProvinceId(addressModel);

            return address;
        }

        /// <summary>
        /// Map Customer to CustomerModel
        /// </summary>
        /// <param name="customer">The NOP customer.</param>
        /// <returns>The Connect customer model.</returns>
        protected virtual CustomerModel MapCustomerToCustomerModel(Customer customer)
        {
            var customerModel = new CustomerModel
            {
                FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                CustomerGuid = customer.CustomerGuid,
                Active = customer.Active,
                CreatedOnUtc = customer.CreatedOnUtc,
                AdminContent = customer.AdminComment,
                BillingAddress = new AddressModel
                {
                    FirstName = customer.BillingAddress.FirstName,
                    LastName = customer.BillingAddress.LastName,
                    Email = customer.BillingAddress.Email,
                    Company = customer.BillingAddress.Company,
                    City = customer.BillingAddress.City,
                    Address1 = customer.BillingAddress.Address1,
                    Address2 = customer.BillingAddress.Address2,
                    ZipPostalCode = customer.BillingAddress.ZipPostalCode,
                    PhoneNumber = customer.BillingAddress.PhoneNumber,
                    FaxNumber = customer.BillingAddress.FaxNumber,
                    CountryTwoLetterIsoCode = customer.BillingAddress.CountryId.HasValue ? this.countryService.GetCountryById(customer.BillingAddress.CountryId.Value).TwoLetterIsoCode : string.Empty,
                    CountryThreeLetterIsoCode = customer.BillingAddress.CountryId.HasValue ? this.countryService.GetCountryById(customer.BillingAddress.CountryId.Value).ThreeLetterIsoCode : string.Empty,
                    StateProvinceAbbreviation = customer.BillingAddress.StateProvinceId.HasValue ? this.stateProvinceService.GetStateProvinceById(customer.BillingAddress.StateProvinceId.Value).Abbreviation : string.Empty,
                    CreatedOnUtc = customer.BillingAddress.CreatedOnUtc
                },
                ShippingAddress = new AddressModel
                {
                    FirstName = customer.ShippingAddress.FirstName,
                    LastName = customer.ShippingAddress.LastName,
                    Email = customer.ShippingAddress.Email,
                    Company = customer.ShippingAddress.Company,
                    City = customer.ShippingAddress.City,
                    Address1 = customer.ShippingAddress.Address1,
                    Address2 = customer.ShippingAddress.Address2,
                    ZipPostalCode = customer.ShippingAddress.ZipPostalCode,
                    PhoneNumber = customer.ShippingAddress.PhoneNumber,
                    FaxNumber = customer.ShippingAddress.FaxNumber,
                    CountryTwoLetterIsoCode = customer.ShippingAddress.CountryId.HasValue ? this.countryService.GetCountryById(customer.ShippingAddress.CountryId.Value).TwoLetterIsoCode : string.Empty,
                    CountryThreeLetterIsoCode = customer.ShippingAddress.CountryId.HasValue ? this.countryService.GetCountryById(customer.ShippingAddress.CountryId.Value).ThreeLetterIsoCode : string.Empty,
                    StateProvinceAbbreviation = customer.ShippingAddress.StateProvinceId.HasValue ? this.stateProvinceService.GetStateProvinceById(customer.ShippingAddress.StateProvinceId.Value).Abbreviation : string.Empty,
                    CreatedOnUtc = customer.ShippingAddress.CreatedOnUtc
                }
            };
            return customerModel;
        }

        #endregion

        /// <summary>
        /// Creates the customer.
        /// </summary>
        /// <param name="customerModel">The customer model.</param>
        /// <exception cref="Nop.Core.NopException">Role could not be loaded</exception>
        private void CreateCustomer(CustomerModel customerModel)
        {
            if (customerModel == null)
            {
                throw new ArgumentNullException("customerModel");
            }

            // Creates new instance of customer.
            var customer = new Customer
            {
                CustomerGuid = customerModel.CustomerGuid,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                BillingAddress = this.MapAddressModelToAdress(customerModel.BillingAddress),
                ShippingAddress = this.MapAddressModelToAdress(customerModel.ShippingAddress),
                AdminComment = customerModel.AdminContent
            };

            customer.Addresses.Add(customer.BillingAddress);
            customer.Addresses.Add(customer.ShippingAddress);

            var role = this.customerService.GetCustomerRoleBySystemName(customerModel.IsRegistered ? SystemCustomerRoleNames.Registered : SystemCustomerRoleNames.Guests);
            if (role != null)
            {
                customer.CustomerRoles.Clear();
                customer.CustomerRoles.Add(role);
            }

            this.customerService.InsertCustomer(customer);

            this.genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, customerModel.FirstName);
            this.genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, customerModel.LastName);
            this.genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, customerModel.Company);
        }

        private void UpdateCustomer(CustomerModel customerModel, Customer existingCustomer)
        {
            if (customerModel == null)
            {
                throw new ArgumentNullException("customerModel");
            }

            if (existingCustomer == null)
            {
                throw new ArgumentNullException("existingCustomer");
            }

            this.UpdateAddress(existingCustomer.BillingAddress, customerModel.BillingAddress);
            this.UpdateAddress(existingCustomer.ShippingAddress, customerModel.ShippingAddress);

            existingCustomer.LastActivityDateUtc = DateTime.UtcNow;
            existingCustomer.AdminComment = customerModel.AdminContent;

            var role = this.customerService.GetCustomerRoleBySystemName(customerModel.IsRegistered ? SystemCustomerRoleNames.Registered : SystemCustomerRoleNames.Guests);
            if (role != null)
            {
                existingCustomer.CustomerRoles.Clear();
                existingCustomer.CustomerRoles.Add(role);
            }

            this.customerService.UpdateCustomer(existingCustomer);

            this.genericAttributeService.SaveAttribute(existingCustomer, SystemCustomerAttributeNames.FirstName, customerModel.FirstName);
            this.genericAttributeService.SaveAttribute(existingCustomer, SystemCustomerAttributeNames.LastName, customerModel.LastName);
            this.genericAttributeService.SaveAttribute(existingCustomer, SystemCustomerAttributeNames.Company, customerModel.Company);
        }

        private void UpdateAddress(Address currentAddress, AddressModel addressModel)
        {
            if (currentAddress == null)
            {
                return;
            }

            if (addressModel == null)
            {
                return;
            }

            currentAddress = this.addressService.GetAddressById(currentAddress.Id);

            this.MapAddressModelToAdress(addressModel, currentAddress);

            this.addressService.UpdateAddress(currentAddress);
        }

        private int? GetCountryId(AddressModel address)
        {
            if (address == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(address.CountryTwoLetterIsoCode))
            {
                return this.countryService.GetCountryByTwoLetterIsoCode(address.CountryTwoLetterIsoCode).Id;
            }

            if (!string.IsNullOrEmpty(address.CountryThreeLetterIsoCode))
            {
                return this.countryService.GetCountryByThreeLetterIsoCode(address.CountryThreeLetterIsoCode).Id;
            }

            return null;
        }

        private int? GetStateProvinceId(AddressModel address)
        {
            if (address == null)
            {
                return null;
            }

            try
            {
                return this.stateProvinceService.GetStateProvinceByAbbreviation(address.StateProvinceAbbreviation).Id;
            }
            catch
            {
                return null;
            }
        }
    }
}