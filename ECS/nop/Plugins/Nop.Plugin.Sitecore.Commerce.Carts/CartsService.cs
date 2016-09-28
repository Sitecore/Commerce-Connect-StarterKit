// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartsService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The implementation of Carts Service.
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
namespace Nop.Plugin.Sitecore.Commerce.Carts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web.Services;
    using Nop.Core.Domain.Customers;
    using Nop.Core.Domain.Orders;
    using Nop.Core.Domain.Seo;
    using Nop.Core.Domain.Shipping;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Sitecore.Commerce.Common;
    using Nop.Plugin.Sitecore.Commerce.Common.Models;
    using Nop.Services.Common;
    using Nop.Services.Payments;
    using Nop.Services.Stores;

    /// <summary>
    /// The implementation of Cart Service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CartsService : CartsServiceBase, ICartsService
    {
        /// <summary>
        /// The store service
        /// </summary>
        private readonly IStoreService storeService;

        /// <summary>
        /// The generic attribute service
        /// </summary>
        private readonly IGenericAttributeService genericAttributeService;

        /// <summary>
        /// The payment service
        /// </summary>
        private readonly IPaymentService paymentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartsService"/> class.
        /// </summary>
        public CartsService()
        {
            this.CartType = ShoppingCartType.ShoppingCart;
            this.storeService = EngineContext.Current.Resolve<IStoreService>();
            this.genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            this.paymentService = EngineContext.Current.Resolve<IPaymentService>();
        }

        /// <summary>
        /// Gets all carts.
        /// </summary>
        /// <returns>List of <see cref="ShoppingCartModel"/></returns>
        [WebGet]
        public virtual IQueryable<ShoppingCartModel> GetCarts()
        {
            return base.GetCarts();
        }

        /// <summary>
        /// Gets the carts by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>List of <see cref="ShoppingCartModel"/></returns>
        [WebMethod(EnableSession = false)]
        public virtual ShoppingCartModel GetCart(Guid customerId, string storeName = null)
        {
            return base.GetCart(customerId, storeName);
        }

        /// <summary>
        /// Deletes the cart by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        [WebMethod(EnableSession = false)]
        public void DeleteCart(Guid customerId)
        {
            base.DeleteCart(customerId);
        }

        /// <summary>
        /// Creates the cart by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns>The shopping cart.</returns>
        [WebMethod(EnableSession = false)]
        public virtual ShoppingCartModel CreateCart(Guid customerId)
        {
            return base.CreateCart(customerId);
        }

        /// <summary>
        /// Adds the product by product variant id to cart.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="externalProductId">The external product id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>
        /// Instance of <see cref="ShoppingCartModel" />
        /// </returns>
        [WebMethod(EnableSession = false)]
        public virtual ShoppingCartModel AddProduct(Guid customerId, string externalProductId, uint quantity)
        {
            return base.AddProduct(customerId, externalProductId, quantity);
        }

        /// <summary>
        /// Removes the product by product variant id from cart.
        /// </summary>
        /// <param name="customerId">The custommer ID.</param>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>
        /// Instance of <see cref="ShoppingCartModel" />
        /// </returns>
        [WebMethod(EnableSession = false)]
        public virtual ShoppingCartModel RemoveProduct(Guid customerId, string externalProductId)
        {
            return base.RemoveProduct(customerId, externalProductId);
        }

        /// <summary>
        /// Updates the quantity by external product id on cart.
        /// </summary>
        /// <param name="customerId">THe customer ID.</param>
        /// <param name="externalProductId">The external product id.</param>
        /// <param name="newQuantity">The new quantity.</param>
        /// <returns>
        /// Instance of <see cref="ShoppingCartModel" />
        /// </returns>
        [WebMethod(EnableSession = false)]
        public virtual ShoppingCartModel UpdateQuantity(Guid customerId, string externalProductId, int newQuantity)
        {
            return base.UpdateQuantity(customerId, externalProductId, newQuantity);
        }

        /// <summary>
        /// Adds the Address customer addresses and customer id.
        /// </summary>
        /// <param name="addresses">The address model.</param>
        /// <param name="customerId">Customer Id.</param>
        /// <returns>
        /// Instance of <see cref="Response" />
        /// </returns>
        [WebMethod(EnableSession = false)]
        public virtual Response AddAddresses(CustomerAddressModel[] addresses, Guid customerId)
        {
            var customer = this.CustomerService.GetCustomerByGuid(customerId);
            if (customer == null)
            {
                return new Response()
                {
                    Success = false,
                    Message = string.Format("Customer not foud. Customer Id: {0}", customerId)
                };
            }

            foreach (var address in addresses)
            {
                int addressId;

                if (!int.TryParse(address.Id, out addressId))
                {
                    return new Response() { Message = string.Format("Cannot parse address id : {0}", address.Id), Success = false };
                }

                if (address.AddressType == AddressTypeModel.Shipping)
                {
                    customer.ShippingAddress = customer.Addresses.FirstOrDefault(x => (x.Id == addressId));
                }

                if (address.AddressType == AddressTypeModel.Billing)
                {
                    customer.BillingAddress = customer.Addresses.FirstOrDefault(x => (x.Id == addressId));
                }
            }

            try
            {
                this.CustomerService.UpdateCustomer(customer);
            }
            catch (Exception)
            {
                return new Response<IEnumerable<AddressModel>>()
                {
                    Message = string.Format("Error during updating customer. Customer Id: {0}", customerId),
                    Success = false
                };
            }

            return new Response() { Message = "Success", Success = true };
        }

        /// <summary>
        /// Removes the Address customer addresses and customer id.
        /// </summary>
        /// <param name="addresses">The address model.</param>
        /// <param name="customerId">Customer Id.</param>
        /// <returns>
        /// Instance of <see cref="Response" />
        /// </returns>
        [WebMethod(EnableSession = false)]
        public Response RemoveAddresses(CustomerAddressModel[] addresses, Guid customerId)
        {
            var customer = this.CustomerService.GetCustomerByGuid(customerId);
            if (customer == null)
            {
                return new Response()
                {
                    Success = false,
                    Message = string.Format("Customer not foud. Customer Id: {0}", customerId)
                };
            }

            foreach (var address in addresses)
            {
                if ((address.AddressType == AddressTypeModel.Shipping) && (customer.ShippingAddress != null))
                {
                    customer.ShippingAddress = null;
                }
                else if ((address.AddressType == AddressTypeModel.Billing) && (customer.BillingAddress != null))
                {
                    customer.BillingAddress = null;
                }
            }

            try
            {
                this.CustomerService.UpdateCustomer(customer);
            }
            catch (Exception)
            {
                return new Response<IEnumerable<AddressModel>>()
                {
                    Message = string.Format("Error during updating customer. Customer Id: {0}", customerId),
                    Success = false
                };
            }

            return new Response() { Message = "Success", Success = true };
        }

        /// <summary>
        /// Add payment method info
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="paymentInfoModel">The payment information.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>A service response.</returns>
        [WebMethod(EnableSession = false)]
        public Response<PaymentInfoModel> AddPaymentInfo(Guid customerId, PaymentInfoModel paymentInfoModel, string storeName)
        {
            if (string.IsNullOrEmpty(storeName))
            {
                return new Response<PaymentInfoModel>
                {
                    Success = false,
                    Message = "storeName is null or empty"
                };
            }

            var customer = this.CustomerService.GetCustomerByGuid(customerId);
            if (customer == null)
            {
                return new Response<PaymentInfoModel>
                {
                    Success = false,
                    Message = string.Format("Customer not found. Customer Id: {0}", customerId)
                };
            }

            var store = this.storeService.GetAllStores().FirstOrDefault(s => s.Name.Equals(storeName, StringComparison.InvariantCultureIgnoreCase));

            var storeId = (store == null) ? 0 : store.Id;

            var cart = customer
                        .ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == this.CartType && (sci.StoreId == 0 || sci.StoreId == storeId))
                        .ToList();
            if (cart.Count == 0)
            {
                return new Response<PaymentInfoModel>
                {
                    Success = false,
                    Message = "Couldn't find cart."
                };
            }

            var paymentMethod = this.paymentService.LoadActivePaymentMethods()
              .FirstOrDefault(
                pm =>
                  pm.PluginDescriptor.SystemName.Equals(paymentInfoModel.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                  pm.PluginDescriptor.FriendlyName.Equals(paymentInfoModel.MethodName, StringComparison.InvariantCultureIgnoreCase));

            if (paymentMethod == null)
            {
                return new Response<PaymentInfoModel>
                {
                    Success = false,
                    Message = string.Format(
                        "Couldn't find payment method. System Name: {0}, Method Name: {1}",
                        paymentInfoModel.SystemName,
                        paymentInfoModel.MethodName)
                };
            }

            try
            {
                this.genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.SelectedPaymentMethod, paymentMethod.PluginDescriptor.SystemName, storeId);
            }
            catch (Exception e)
            {
                return new Response<PaymentInfoModel>
                {
                    Success = false,
                    Message = e.Message
                };
            }

            return new Response<PaymentInfoModel>
            {
                Success = true,
                Result = new PaymentInfoModel
                {
                    SystemName = paymentMethod.PluginDescriptor.SystemName,
                    MethodName = paymentMethod.PluginDescriptor.FriendlyName
                }
            };
        }

        /// <summary>
        /// Remove payment info
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="paymentInfoModel">The payment information.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>A service response.</returns>
        [WebMethod(EnableSession = false)]
        public virtual Response RemovePaymentInfo(Guid customerId, PaymentInfoModel paymentInfoModel, string storeName)
        {
            if (string.IsNullOrEmpty(storeName))
            {
                return new Response
                {
                    Success = false,
                    Message = "storeName is null or empty"
                };
            }

            var customer = this.CustomerService.GetCustomerByGuid(customerId);
            if (customer == null)
            {
                return new Response
                {
                    Success = false,
                    Message = string.Format("Customer not found. Customer Id: {0}", customerId)
                };
            }

            var store = this.storeService.GetAllStores()
             .FirstOrDefault(s => s.Name.Equals(storeName, StringComparison.InvariantCultureIgnoreCase));

            var storeId = (store == null) ? 0 : store.Id;

            var cart = customer
                        .ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart && sci.StoreId == storeId)
                        .ToList();

            if (cart.Count == 0)
            {
                return new Response
                {
                    Success = false,
                    Message = "Couldn't find cart."
                };
            }

            try
            {
                this.genericAttributeService.SaveAttribute<string>(customer, SystemCustomerAttributeNames.SelectedPaymentMethod, null, storeId);
            }
            catch (Exception e)
            {
                return new Response
                {
                    Success = false,
                    Message = e.Message
                };
            }

            return new Response
            {
                Success = true
            };
        }

        /// <summary>
        /// adds shipping information.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="shippingMethodModel">The shipping method model.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>A service response.</returns>
        [WebMethod(EnableSession = false)]
        public Response AddShippingInfo(Guid customerId, ShippingMethodModel shippingMethodModel, string storeName)
        {
            var option = new ShippingOption
            {
                ShippingRateComputationMethodSystemName = shippingMethodModel.SystemName,
                Name = shippingMethodModel.Name,
                Description = shippingMethodModel.Description
            };

            return this.UpdateShippingInfo(customerId, option, storeName);
        }

        /// <summary>
        /// Removes shipping information.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>A service response.</returns>
        [WebMethod(EnableSession = false)]
        public Response RemoveShippingInfo(Guid customerId, string storeName)
        {
            return this.UpdateShippingInfo(customerId, null, storeName);
        }

        /// <summary>
        /// Migrates a shopping cart from one customer to another.
        /// </summary>
        /// <param name="fromCustomerId">The source customer ID.</param>
        /// <param name="toCustomerId">The destination customer ID.</param>
        /// <param name="includeCouponCodes">Specifies whether coupon codes will be included in the migrated cart.</param>
        /// <returns>A service response.</returns>
        [WebMethod(EnableSession = false)]
        public Response MigrateShoppingCart(Guid fromCustomerId, Guid toCustomerId, bool includeCouponCodes)
        {
            var fromCustomer = this.CustomerService.GetCustomerByGuid(fromCustomerId);
            var toCustomer = this.CustomerService.GetCustomerByGuid(toCustomerId);

            if (fromCustomer == null)
            {
                return new Response { Success = false, Message = "fromCustomer could not be found" };
            }

            if (toCustomer == null)
            {
                return new Response { Success = false, Message = "toCustomer could not be found" };
            }

            try
            {
                this.ShoppingCartService.MigrateShoppingCart(fromCustomer, toCustomer, includeCouponCodes);
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }

            return new Response() { Success = true };
        }

        /// <summary>
        /// Updates shipping information.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <param name="shippingOption">The shipping option.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>A service response.</returns>
        protected virtual Response UpdateShippingInfo(Guid customerId, ShippingOption shippingOption, string storeName)
        {
            if (string.IsNullOrEmpty(storeName))
            {
                return new Response
                {
                    Success = false,
                    Message = "storeName is null or empty"
                };
            }

            var customer = this.CustomerService.GetCustomerByGuid(customerId);
            if (customer == null)
            {
                return new Response
                {
                    Success = false,
                    Message = string.Format("Customer not found. Customer Id: {0}", customerId)
                };
            }

            var store = this.storeService.GetAllStores()
               .FirstOrDefault(s => s.Name.Equals(storeName, StringComparison.InvariantCultureIgnoreCase));

            var storeId = (store == null) ? 0 : store.Id;

            try
            {
                this.genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, storeId);
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = ex.Message
                };
            }

            return new Response
            {
                Success = true
            };
        }
    }
}