// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICartsService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The CartsService interface.
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
namespace Sitecore.Commerce.Nop.Carts
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using Common.Models;

    /// <summary>
    /// The CartService interface.
    /// </summary>
    [ServiceContract]
    public interface ICartsService
    {
        /// <summary>
        /// Gets the carts.
        /// </summary>
        /// <returns>List of <see cref="ShoppingCartModel"/></returns>
        [OperationContract]
        IQueryable<ShoppingCartModel> GetCarts();

        /// <summary>
        /// Gets the carts by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>List of <see cref="ShoppingCartModel"/></returns>
        [OperationContract]
        ShoppingCartModel GetCart(Guid customerId, string storeName = null);

        /// <summary>
        /// Deletes the cart by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        [OperationContract]
        void DeleteCart(Guid customerId);

        /// <summary>
        /// Creates the cart by customer id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns>The shopping cart.</returns>
        [OperationContract]
        ShoppingCartModel CreateCart(Guid customerId);

        /// <summary>
        /// Adds the product by product variant id to cart.
        /// </summary>
        /// <param name="customerId">The custome Id.</param>
        /// <param name="externalProductId">The external product id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Instance of <see cref="ShoppingCartModel"/></returns>
        [OperationContract]
        ShoppingCartModel AddProduct(Guid customerId, string externalProductId, uint quantity);

        /// <summary>
        /// Removes the product by product variant id from cart.
        /// </summary>
        /// <param name="customerId">The custome Id.</param>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>Instance of <see cref="ShoppingCartModel"/></returns>
        [OperationContract]
        ShoppingCartModel RemoveProduct(Guid customerId, string externalProductId);

        /// <summary>
        /// Updates the quantity by external product id on cart.
        /// </summary>
        /// <param name="customerId">The custome Id.</param>
        /// <param name="externalProductId">The external product id.</param>
        /// <param name="newQuantity">The new quantity.</param>
        /// <returns>Instance of <see cref="ShoppingCartModel"/></returns>
        [OperationContract]
        ShoppingCartModel UpdateQuantity(Guid customerId, string externalProductId, int newQuantity);

        /// <summary>
        /// Adds the Address customer addresses and customer id.
        /// </summary>
        /// <param name="addresses">Customer addresses collection.</param>
        /// <param name="customerId">Customer Id.</param>
        /// <returns>A service response.</returns>
        [OperationContract]
        Response AddAddresses(CustomerAddressModel[] addresses, Guid customerId);

        /// <summary>
        /// Removes the Address customer addresses and customer id.
        /// </summary>
        /// <param name="addresses">Customer addresses collection.</param>
        /// <param name="customerId">Customer Id.</param>
        /// <returns>A service response.</returns>
        [OperationContract]
        Response RemoveAddresses(CustomerAddressModel[] addresses, Guid customerId);

        /// <summary>
        /// Add payment info
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="paymentInfoModel">The payment information.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>A service response.</returns>
        [OperationContract]
        Response<PaymentInfoModel> AddPaymentInfo(Guid customerId, PaymentInfoModel paymentInfoModel, string storeName);

        /// <summary>
        /// Remove payment info
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="paymentInfoModel">The payment information.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>A service response.</returns>
        [OperationContract]
        Response RemovePaymentInfo(Guid customerId, PaymentInfoModel paymentInfoModel, string storeName);

        /// <summary>
        /// Add shipping info
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="shippingMethodModel">The shipping method information.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>A service response.</returns>
        [OperationContract]
        Response AddShippingInfo(Guid customerId, ShippingMethodModel shippingMethodModel, string storeName);

        /// <summary>
        /// Remove shipping info
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>A service response.</returns>
        [OperationContract]
        Response RemoveShippingInfo(Guid customerId, string storeName);

        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromCustomerId">The source customer ID.</param>
        /// <param name="toCustomerId">The destination customer ID.</param>
        /// <param name="includeCouponCodes">Specifies wither coupon codes will be included in the migrated shopping cart.</param>
        /// <returns>A service response.</returns>
        [OperationContract]
        Response MigrateShoppingCart(Guid fromCustomerId, Guid toCustomerId, bool includeCouponCodes);
    }
}