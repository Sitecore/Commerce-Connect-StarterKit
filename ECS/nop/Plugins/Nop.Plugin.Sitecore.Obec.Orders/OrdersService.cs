// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrdersService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the OrdersService type.
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
namespace Nop.Plugin.Sitecore.Commerce.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel.Activation;
    using System.Web.Script.Serialization;
    using System.Web.Services;
    using Nop.Core;
    using Nop.Core.Domain.Common;
    using Nop.Core.Domain.Customers;
    using Nop.Core.Domain.Directory;
    using Nop.Core.Domain.Discounts;
    using Nop.Core.Domain.Localization;
    using Nop.Core.Domain.Orders;
    using Nop.Core.Domain.Payments;
    using Nop.Core.Domain.Shipping;
    using Nop.Core.Domain.Tax;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Sitecore.Commerce.Carts;
    using Nop.Plugin.Sitecore.Commerce.Orders.Models;
    using Nop.Services.Affiliates;
    using Nop.Services.Catalog;
    using Nop.Services.Common;
    using Nop.Services.Customers;
    using Nop.Services.Directory;
    using Nop.Services.Localization;
    using Nop.Services.Orders;
    using Nop.Services.Payments;
    using Nop.Services.Security;
    using Nop.Services.Shipping;
    using Nop.Services.Tax;

    /// <summary>
    /// The orders service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class OrdersService : IOrdersService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersService"/> class.
        /// </summary>
        public OrdersService()
        {
            this.OrderService = EngineContext.Current.Resolve<IOrderService>();
            this.CustomerService = EngineContext.Current.Resolve<ICustomerService>();
            this.CartsService = new CartsService();
            this.EncryptionService = EngineContext.Current.Resolve<IEncryptionService>();
            this.PaymentService = EngineContext.Current.Resolve<IPaymentService>();
            this.OrderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
            this.LanguageService = EngineContext.Current.Resolve<ILanguageService>();
            this.WebHelper = EngineContext.Current.Resolve<IWebHelper>();
            this.TaxService = EngineContext.Current.Resolve<ITaxService>();
            this.CheckoutAttributeFormatter = EngineContext.Current.Resolve<ICheckoutAttributeFormatter>();
            this.CurrencyService = EngineContext.Current.Resolve<ICurrencyService>();
            this.AffiliateService = EngineContext.Current.Resolve<IAffiliateService>();
            this.PriceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();
            this.ProductAttributeFormatter = EngineContext.Current.Resolve<IProductAttributeFormatter>();
            this.ShippingService = EngineContext.Current.Resolve<IShippingService>();
        }

        /// <summary>
        /// Gets or sets the orders service interface.
        /// </summary>
        protected IOrderService OrderService { get; set; }

        /// <summary>
        /// Gets or sets the customer service.
        /// </summary>
        protected ICustomerService CustomerService { get; set; }

        /// <summary>
        /// Gets or sets the carts service
        /// </summary>
        protected ICartsService CartsService { get; set; }

        /// <summary>
        /// Gets or sets the encryption service.
        /// </summary>
        protected IEncryptionService EncryptionService { get; set; }

        /// <summary>
        /// Gets or sets the payment service.
        /// </summary>
        protected IPaymentService PaymentService { get; set; }

        /// <summary>
        /// Gets the order calculation service.
        /// </summary>
        protected IOrderTotalCalculationService OrderTotalCalculationService { get; private set; }

        /// <summary>
        /// Gets the language service.
        /// </summary>
        protected ILanguageService LanguageService { get; private set; }

        /// <summary>
        /// Gets the web helper.
        /// </summary>
        protected IWebHelper WebHelper { get; private set; }

        /// <summary>
        /// Gets the tax service.
        /// </summary>
        protected ITaxService TaxService { get; private set; }

        /// <summary>
        /// Gets the checkout attribute formatter.
        /// </summary>
        protected ICheckoutAttributeFormatter CheckoutAttributeFormatter { get; private set; }

        /// <summary>
        /// Gets the currency service.
        /// </summary>
        protected ICurrencyService CurrencyService { get; private set; }

        /// <summary>
        /// Gets the affiliate service.
        /// </summary>
        protected IAffiliateService AffiliateService { get; private set; }

        /// <summary>
        /// Gets the price calculation service.
        /// </summary>
        protected IPriceCalculationService PriceCalculationService { get; private set; }

        /// <summary>
        /// Gets the product attribute formatter.
        /// </summary>
        protected IProductAttributeFormatter ProductAttributeFormatter { get; private set; }

        /// <summary>
        /// Gets the shipping service.
        /// </summary>
        protected IShippingService ShippingService { get; private set; }

        /// <summary>
        /// Submits an order.
        /// </summary>
        /// <param name="customerGuid">The customer GUID.</param>
        /// <param name="shippingMethod">The shipping method.</param>
        /// <param name="paymentMethod">The payment method.</param>
        /// <param name="properties">The order properties.</param>
        /// <returns>The submitted order informtion.</returns>
        [WebMethod(EnableSession = false)]
        public OrderModel SubmitOrder(Guid customerGuid, string shippingMethod, string paymentMethod, Dictionary<string, string> properties)
        {
            var customer = this.CustomerService.GetCustomerByGuid(customerGuid);
            var serializer = new JavaScriptSerializer();
            var orderModel = serializer.Deserialize<OrderModel>(serializer.Serialize(properties));
            var cartModel = this.CartsService.GetCart(customerGuid);

            SortedDictionary<decimal, decimal> taxRatesDictionary;
            var orderTaxTotal = this.GetTaxTotal(customer, out taxRatesDictionary);
            var vatNumber = this.GetVatNumber(customer);
            var taxRates = this.GetTaxRates(taxRatesDictionary);
            Language customerLanguage = this.GetCustomerLanguage(customer, cartModel.StoreId);
            var customerTaxDisplayType = this.GetCustomerTaxDisplayType(customer, cartModel.StoreId);

            decimal discountAmountInclTax, subTotalWithoutDiscountInclTax, subTotalWithDiscountInclTax;
            decimal discountAmountExclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountExclTax;
            Discount appliedDiscountInclTax, appliedDiscountExclTax;
            var orderItems = customer.ShoppingCartItems.ToList();
            this.OrderTotalCalculationService.GetShoppingCartSubTotal(
              orderItems, true, out discountAmountInclTax, out appliedDiscountInclTax, out subTotalWithoutDiscountInclTax, out subTotalWithDiscountInclTax);
            this.OrderTotalCalculationService.GetShoppingCartSubTotal(
              orderItems, false, out discountAmountExclTax, out appliedDiscountExclTax, out subTotalWithoutDiscountExclTax, out subTotalWithDiscountExclTax);

            decimal taxRate;
            Discount shippingTotalDiscount;
            var orderShippingTotalInclTax = this.OrderTotalCalculationService.GetShoppingCartShippingTotal(orderItems, true, out taxRate, out shippingTotalDiscount);
            var orderShippingTotalExclTax = this.OrderTotalCalculationService.GetShoppingCartShippingTotal(orderItems, false);

            decimal paymentAdditionalFee = this.PaymentService.GetAdditionalHandlingFee(orderItems, paymentMethod);
            var paymentAdditionalFeeInclTax = this.TaxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, true, customer);
            var paymentAdditionalFeeExclTax = this.TaxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, customer);

            string checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, cartModel.StoreId);
            string checkoutAttributeDescription = this.CheckoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, customer);

            var currencyTmp = this.CurrencyService.GetCurrencyById(customer.GetAttribute<int>(SystemCustomerAttributeNames.CurrencyId, cartModel.StoreId));

            decimal customerCurrencyRate = this.GetCustomerCurrencyRate(currencyTmp);

            var shippingOption = customer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, cartModel.StoreId);

            if (cartModel != null)
            {
                var order = new Order
                {
                    StoreId = cartModel.StoreId,
                    OrderGuid = Guid.NewGuid(),
                    CustomerId = cartModel.CustomerId,
                    CustomerLanguageId = customerLanguage != null ? customerLanguage.Id : 0,
                    CustomerTaxDisplayType = customerTaxDisplayType,
                    CustomerIp = this.WebHelper.GetCurrentIpAddress(),
                    OrderSubtotalInclTax = subTotalWithoutDiscountInclTax,
                    OrderSubtotalExclTax = subTotalWithoutDiscountExclTax,
                    OrderSubTotalDiscountInclTax = discountAmountInclTax,
                    OrderSubTotalDiscountExclTax = discountAmountExclTax,
                    OrderShippingInclTax = orderShippingTotalInclTax ?? 0,
                    OrderShippingExclTax = orderShippingTotalExclTax ?? 0,
                    PaymentMethodAdditionalFeeInclTax = paymentAdditionalFeeInclTax,
                    PaymentMethodAdditionalFeeExclTax = paymentAdditionalFeeExclTax,
                    TaxRates = taxRates,
                    OrderTax = orderTaxTotal,
                    OrderTotal = cartModel.Total ?? 0,
                    RefundedAmount = decimal.Zero,
                    OrderDiscount = cartModel.Discount,
                    CheckoutAttributeDescription = checkoutAttributeDescription,
                    CheckoutAttributesXml = checkoutAttributesXml,
                    CustomerCurrencyCode = currencyTmp != null ? currencyTmp.CurrencyCode : string.Empty,
                    CurrencyRate = customerCurrencyRate,
                    AffiliateId = this.GetAffiliateId(customer),
                    OrderStatus = OrderStatus.Pending,
                    AllowStoringCreditCardNumber = true,
                    PaymentMethodSystemName = paymentMethod,
                    //// TODO: Implement in future is necessary
                    ////AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId,
                    ////AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode,
                    ////AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult,
                    ////CaptureTransactionId = processPaymentResult.CaptureTransactionId,
                    ////CaptureTransactionResult = processPaymentResult.CaptureTransactionResult,
                    ////SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId,
                    PaymentStatus = PaymentStatus.Pending,
                    PaidDateUtc = null,
                    ShippingStatus = ShippingStatus.NotYetShipped,
                    ShippingMethod = shippingMethod,
                    ShippingRateComputationMethodSystemName = shippingOption != null ? shippingOption.ShippingRateComputationMethodSystemName : string.Empty,
                    VatNumber = vatNumber,
                    CreatedOnUtc = DateTime.UtcNow
                };

                this.InitOrderItems(customer, order);

                order.CardType = order.AllowStoringCreditCardNumber ? this.EncryptionService.EncryptText(orderModel.CardType) : string.Empty;
                order.CardName = order.AllowStoringCreditCardNumber ? this.EncryptionService.EncryptText(orderModel.CardType) : string.Empty;
                order.CardNumber = order.AllowStoringCreditCardNumber ? this.EncryptionService.EncryptText(orderModel.CardNumber) : string.Empty;
                order.MaskedCreditCardNumber = this.EncryptionService.EncryptText(this.PaymentService.GetMaskedCreditCardNumber(orderModel.CardNumber));
                order.CardCvv2 = order.AllowStoringCreditCardNumber ? this.EncryptionService.EncryptText(orderModel.CardCvv2) : string.Empty;
                order.CardExpirationMonth = order.AllowStoringCreditCardNumber ? this.EncryptionService.EncryptText(orderModel.CardExpirationMonth) : string.Empty;
                order.CardExpirationYear = order.AllowStoringCreditCardNumber ? this.EncryptionService.EncryptText(orderModel.CardExpirationYear) : string.Empty;

                this.InitBilling(customer, order);
                this.InitShipping(customer, order);

                return this.InsertOrder(order);
            }

            return null;
        }

        /// <summary>
        /// Gets an order.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>The order model.</returns>
        [WebMethod(EnableSession = false)]
        public virtual OrderModel GetOrder(int orderId, string storeName)
        {
            var order = this.OrderService.GetOrderById(orderId);

            return this.GetModel(order, storeName);
        }

        /// <summary>
        /// Gets the orders for a customer.
        /// </summary>
        /// <param name="customerGuid">The customer ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>The customer orders.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IEnumerable<OrderModel> GetOrders(Guid customerGuid, string storeName)
        {
            Customer customer = this.CustomerService.GetCustomerByGuid(customerGuid);

            var orders = this.OrderService.SearchOrders(0, 0, customer.Id, null, null, null, null, null, null, null, 0, int.MaxValue);

            foreach (Order order in orders)
            {
                yield return this.GetModel(order, storeName);
            }
        }

        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>The cancelled order mdodel.</returns>
        [WebMethod(EnableSession = false)]
        public virtual OrderModel CancelOrder(int orderId, string storeName)
        {
            var order = this.OrderService.GetOrderById(orderId);
            order.OrderStatus = OrderStatus.Cancelled;
            this.OrderService.UpdateOrder(order);
            order = this.OrderService.GetOrderById(orderId);

            return this.GetModel(order, storeName);
        }

        /// <summary>
        /// Gets a Connect order model.
        /// </summary>
        /// <param name="order">The NOP order.</param>
        /// <param name="storeName">The store name.</param>
        /// <returns>The Connect order model.</returns>
        protected virtual OrderModel GetModel(Order order, string storeName)
        {
            var model = new OrderModel();
            model.Map(order, storeName);

            return model;
        }

        /// <summary>
        /// Gets the tax total.
        /// </summary>
        /// <param name="customer">The custommer.</param>
        /// <param name="taxRatesDictionary">The tax rates.</param>
        /// <returns>The tax total.</returns>
        protected virtual decimal GetTaxTotal(Customer customer, out SortedDictionary<decimal, decimal> taxRatesDictionary)
        {
            var orderTaxTotal = this.OrderTotalCalculationService.GetTaxTotal(customer.ShoppingCartItems.ToList(), out taxRatesDictionary);

            return orderTaxTotal;
        }

        /// <summary>
        /// Gets the customer VAT number.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <returns>The customer VAT number.</returns>
        protected virtual string GetVatNumber(Customer customer)
        {
            var vatNumber = string.Empty;
            var customerVatStatus = (VatNumberStatus)customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId);
            if (customerVatStatus == VatNumberStatus.Valid)
            {
                vatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);
            }

            return vatNumber;
        }

        /// <summary>
        /// Gets the tax rates.
        /// </summary>
        /// <param name="taxRatesDictionary">The tax rates.</param>
        /// <returns>A string representing the tax rates.</returns>
        protected virtual string GetTaxRates(SortedDictionary<decimal, decimal> taxRatesDictionary)
        {
            var taxRates = string.Empty;
            foreach (var kvp in taxRatesDictionary)
            {
                var taxRate = kvp.Key;
                var taxValue = kvp.Value;
                taxRates += string.Format("{0}:{1};   ", taxRate.ToString(CultureInfo.InvariantCulture), taxValue.ToString(CultureInfo.InvariantCulture));
            }

            return taxRates;
        }

        /// <summary>
        /// Gets the cutomer languate.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="storeId">The store ID.</param>
        /// <returns>The customer languate.</returns>
        protected virtual Language GetCustomerLanguage(Customer customer, int storeId)
        {
            return this.LanguageService.GetLanguageById(customer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId, storeId));
        }

        /// <summary>
        /// Gets the customer tax display type.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="storeId">The store ID.</param>
        /// <returns>The customer tax display type.</returns>
        protected virtual TaxDisplayType GetCustomerTaxDisplayType(Customer customer, int storeId)
        {
            var customerTaxDisplayType = TaxDisplayType.IncludingTax;
            customerTaxDisplayType = (TaxDisplayType)customer.GetAttribute<int>(SystemCustomerAttributeNames.TaxDisplayTypeId, storeId);

            return customerTaxDisplayType;
        }

        /// <summary>
        /// Gets the customer affiliate ID.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <returns>The customer affiliated ID.</returns>
        protected virtual int GetAffiliateId(Customer customer)
        {
            int affiliateId = 0;
            var affiliate = this.AffiliateService.GetAffiliateById(customer.AffiliateId);
            if (affiliate != null && affiliate.Active && !affiliate.Deleted)
            {
                affiliateId = affiliate.Id;
            }

            return affiliateId;
        }

        /// <summary>
        /// Gets the customer currency rate.
        /// </summary>
        /// <param name="currencyTmp">The currency.</param>
        /// <returns>The customer currency rate.</returns>
        protected virtual decimal GetCustomerCurrencyRate(Currency currencyTmp)
        {
            decimal customerCurrencyRate = 0;
            var primaryStoreCurrency = this.CurrencyService.GetCurrencyById((new CurrencySettings()).PrimaryStoreCurrencyId);
            if (primaryStoreCurrency != null && currencyTmp != null)
            {
                customerCurrencyRate = currencyTmp.Rate / primaryStoreCurrency.Rate;
            }

            return customerCurrencyRate;
        }

        /// <summary>
        /// Initializes the customer order items.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="order">The order.</param>
        protected virtual void InitOrderItems(Customer customer, Order order)
        {
            foreach (var item in customer.ShoppingCartItems.Where(i => i.ShoppingCartType == ShoppingCartType.ShoppingCart))
            {
                decimal taxRate;
                decimal itemTaxRate;
                decimal unitPrice = this.PriceCalculationService.GetUnitPrice(item, true);
                decimal subTotal = this.PriceCalculationService.GetSubTotal(item, true);
                decimal unitPriceInclTax = this.TaxService.GetProductPrice(item.Product, unitPrice, true, customer, out itemTaxRate);
                decimal unitPriceExclTax = this.TaxService.GetProductPrice(item.Product, unitPrice, false, customer, out itemTaxRate);
                decimal subTotalInclTax = this.TaxService.GetProductPrice(item.Product, subTotal, true, customer, out itemTaxRate);
                decimal subTotalExclTax = this.TaxService.GetProductPrice(item.Product, subTotal, false, customer, out itemTaxRate);

                string attributeDescription = this.ProductAttributeFormatter.FormatAttributes(item.Product, item.AttributesXml, customer);

                Discount discount = null;
                decimal discountAmount = this.PriceCalculationService.GetDiscountAmount(item, out discount);
                decimal itemDiscountAmountInclTax = this.TaxService.GetProductPrice(item.Product, discountAmount, true, customer, out taxRate);
                decimal itemDiscountAmountExclTax = this.TaxService.GetProductPrice(item.Product, discountAmount, false, customer, out taxRate);

                var itemWeight = this.ShippingService.GetShoppingCartItemWeight(item);

                order.OrderItems.Add(
                  new OrderItem
                  {
                      OrderItemGuid = Guid.NewGuid(),
                      Order = order,
                      ProductId = item.ProductId,
                      UnitPriceInclTax = unitPriceInclTax,
                      UnitPriceExclTax = unitPriceExclTax,
                      PriceInclTax = subTotalInclTax,
                      PriceExclTax = subTotalExclTax,
                      OriginalProductCost = this.PriceCalculationService.GetProductCost(item.Product, item.AttributesXml),
                      AttributeDescription = attributeDescription,
                      AttributesXml = item.AttributesXml,
                      Quantity = item.Quantity,
                      DiscountAmountInclTax = itemDiscountAmountInclTax,
                      DiscountAmountExclTax = itemDiscountAmountExclTax,
                      DownloadCount = 0,
                      IsDownloadActivated = false,
                      LicenseDownloadId = 0,
                      ItemWeight = itemWeight
                  });
            }
        }

        /// <summary>
        /// Initializes billing.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="order">The order.</param>
        protected virtual void InitBilling(Customer customer, Order order)
        {
            if (customer.BillingAddress != null)
            {
                order.BillingAddress = (Address)customer.BillingAddress.Clone();
            }
        }

        /// <summary>
        /// Initializes shipping.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="order">The order.</param>
        protected virtual void InitShipping(Customer customer, Order order)
        {
            if (customer.ShippingAddress != null)
            {
                order.ShippingAddress = (Address)customer.ShippingAddress.Clone();
            }
        }

        /// <summary>
        /// Inserts an order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>The order model.</returns>
        protected virtual OrderModel InsertOrder(Order order)
        {
            this.OrderService.InsertOrder(order);
            var result = this.GetOrder(order.Id, null);
            if (result != null)
            {
                this.CartsService.DeleteCart(result.CustomerGuid);
            }

            return result;
        }
    }
}