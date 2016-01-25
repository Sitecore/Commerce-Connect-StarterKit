// -----------------------------------------------------------------
// <copyright file="OrderService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the OrderService type.
// </summary>
// -----------------------------------------------------------------
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

  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  public class OrdersService : IOrdersService
  {
    protected readonly IOrderService orderService;

    protected readonly ICustomerService customerService;

    protected readonly ICartsService cartsService;

    protected readonly IEncryptionService encryptionService;

    protected readonly IPaymentService paymentService;

    protected readonly IOrderTotalCalculationService orderTotalCalculationService;

    protected readonly ILanguageService languageService;

    protected readonly IWebHelper webHelper;

    protected readonly ITaxService taxService;

    protected readonly ICheckoutAttributeFormatter checkoutAttributeFormatter;

    protected readonly ICurrencyService currencyService;

    protected readonly IAffiliateService affiliateService;

    protected readonly IPriceCalculationService priceCalculationService;

    protected readonly IProductAttributeFormatter productAttributeFormatter;

    protected readonly IShippingService shippingService;


    public OrdersService()
    {
      this.orderService = EngineContext.Current.Resolve<IOrderService>();
      this.customerService = EngineContext.Current.Resolve<ICustomerService>();
      this.cartsService = new CartsService();
      this.encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
      this.paymentService = EngineContext.Current.Resolve<IPaymentService>();
      this.orderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
      this.languageService = EngineContext.Current.Resolve<ILanguageService>();
      this.webHelper = EngineContext.Current.Resolve<IWebHelper>();
      this.taxService = EngineContext.Current.Resolve<ITaxService>();
      this.checkoutAttributeFormatter = EngineContext.Current.Resolve<ICheckoutAttributeFormatter>();
      this.currencyService = EngineContext.Current.Resolve<ICurrencyService>();
      this.affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
      this.priceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();
      this.productAttributeFormatter = EngineContext.Current.Resolve<IProductAttributeFormatter>();
      this.shippingService = EngineContext.Current.Resolve<IShippingService>();
    }


    [WebMethod(EnableSession = false)]
    public OrderModel SubmitOrder(Guid customertGuid, string shippingMethod, string paymentMethod, Dictionary<string, string> properties)
    {
      var customer = this.customerService.GetCustomerByGuid(customertGuid);
      var serializer = new JavaScriptSerializer();
      var orderModel = serializer.Deserialize<OrderModel>(serializer.Serialize(properties));
      var cartModel = this.cartsService.GetCart(customertGuid);

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
      this.orderTotalCalculationService.GetShoppingCartSubTotal(
        orderItems, true, out discountAmountInclTax, out appliedDiscountInclTax, out subTotalWithoutDiscountInclTax, out subTotalWithDiscountInclTax);
      this.orderTotalCalculationService.GetShoppingCartSubTotal(
        orderItems, false, out discountAmountExclTax, out appliedDiscountExclTax, out subTotalWithoutDiscountExclTax, out subTotalWithDiscountExclTax);

      decimal taxRate;
      Discount shippingTotalDiscount;
      var orderShippingTotalInclTax = this.orderTotalCalculationService.GetShoppingCartShippingTotal(orderItems, true, out taxRate, out shippingTotalDiscount);
      var orderShippingTotalExclTax = this.orderTotalCalculationService.GetShoppingCartShippingTotal(orderItems, false);

      decimal paymentAdditionalFee = this.paymentService.GetAdditionalHandlingFee(orderItems, paymentMethod);
      var paymentAdditionalFeeInclTax = this.taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, true, customer);
      var paymentAdditionalFeeExclTax = this.taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, customer);

      string checkoutAttributesXml = customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, cartModel.StoreId);
      string checkoutAttributeDescription = this.checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, customer);

      var currencyTmp = this.currencyService.GetCurrencyById(customer.GetAttribute<int>(SystemCustomerAttributeNames.CurrencyId, cartModel.StoreId));

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
          CustomerIp = this.webHelper.GetCurrentIpAddress(),
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
          // TODO: Implement in future is necessary
          //AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId,
          //AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode,
          //AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult,
          //CaptureTransactionId = processPaymentResult.CaptureTransactionId,
          //CaptureTransactionResult = processPaymentResult.CaptureTransactionResult,
          //SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId,
          PaymentStatus = PaymentStatus.Pending,
          PaidDateUtc = null,
          ShippingStatus = ShippingStatus.NotYetShipped,
          ShippingMethod = shippingMethod,
          ShippingRateComputationMethodSystemName = shippingOption != null ? shippingOption.ShippingRateComputationMethodSystemName : string.Empty,
          VatNumber = vatNumber,
          CreatedOnUtc = DateTime.UtcNow
        };
        
        this.InitOrderItems(customer, order);

        order.CardType = order.AllowStoringCreditCardNumber ? this.encryptionService.EncryptText(orderModel.CardType) : string.Empty;
        order.CardName = order.AllowStoringCreditCardNumber ? this.encryptionService.EncryptText(orderModel.CardType) : string.Empty;
        order.CardNumber = order.AllowStoringCreditCardNumber ? this.encryptionService.EncryptText(orderModel.CardNumber) : string.Empty;
        order.MaskedCreditCardNumber = this.encryptionService.EncryptText(this.paymentService.GetMaskedCreditCardNumber(orderModel.CardNumber));
        order.CardCvv2 = order.AllowStoringCreditCardNumber ? this.encryptionService.EncryptText(orderModel.CardCvv2) : string.Empty;
        order.CardExpirationMonth = order.AllowStoringCreditCardNumber ? this.encryptionService.EncryptText(orderModel.CardExpirationMonth) : string.Empty;
        order.CardExpirationYear = order.AllowStoringCreditCardNumber ? this.encryptionService.EncryptText(orderModel.CardExpirationYear) : string.Empty;

        this.InitBilling(customer, order);
        this.InitShipping(customer, order);

        return this.InsertOrder(order);
      }

      return null;
    }

    [WebMethod(EnableSession = false)]
    public virtual OrderModel GetOrder(int orderId, string storeName)
    {
      var order = this.orderService.GetOrderById(orderId);
      
      return this.GetModel(order, storeName);
    }

    [WebMethod(EnableSession = false)]
    public virtual IEnumerable<OrderModel> GetOrders(Guid customerGuid, string storeName)
    {
      Customer customer = this.customerService.GetCustomerByGuid(customerGuid);

      var orders = this.orderService.SearchOrders(0, 0, customer.Id, null, null, null, null, null, null, null, 0, int.MaxValue);

      foreach (Order order in orders)
      {
        yield return this.GetModel(order, storeName);
      }
    }

    [WebMethod(EnableSession = false)]
    public virtual OrderModel CancelOrder(int orderId, string storeName)
    {
      var order = this.orderService.GetOrderById(orderId);
      order.OrderStatus = OrderStatus.Cancelled;
      this.orderService.UpdateOrder(order);
      order = this.orderService.GetOrderById(orderId);

      return this.GetModel(order, storeName);
    }


    protected virtual OrderModel GetModel(Order order, string storeName)
    {
      var model = new OrderModel();
      model.Map(order, storeName);

      return model;
    }

    protected virtual decimal GetTaxTotal(Customer customer, out SortedDictionary<decimal, decimal> taxRatesDictionary)
    {
      var orderTaxTotal = this.orderTotalCalculationService.GetTaxTotal(customer.ShoppingCartItems.ToList(), out taxRatesDictionary);
      
      return orderTaxTotal;
    }

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

    protected virtual Language GetCustomerLanguage(Customer customer, int storeId)
    {
      return this.languageService.GetLanguageById(customer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId, storeId));
    }

    protected virtual TaxDisplayType GetCustomerTaxDisplayType(Customer customer, int storeId)
    {
      var customerTaxDisplayType = TaxDisplayType.IncludingTax;
      customerTaxDisplayType = (TaxDisplayType)customer.GetAttribute<int>(SystemCustomerAttributeNames.TaxDisplayTypeId, storeId);

      return customerTaxDisplayType;
    }

    protected virtual int GetAffiliateId(Customer customer)
    {
      int affiliateId = 0;
      var affiliate = this.affiliateService.GetAffiliateById(customer.AffiliateId);
      if (affiliate != null && affiliate.Active && !affiliate.Deleted)
      {
        affiliateId = affiliate.Id;
      }

      return affiliateId;
    }

    protected virtual decimal GetCustomerCurrencyRate(Currency currencyTmp)
    {
      decimal customerCurrencyRate = 0;
      var primaryStoreCurrency = this.currencyService.GetCurrencyById((new CurrencySettings()).PrimaryStoreCurrencyId);
      if (primaryStoreCurrency != null && currencyTmp != null)
      {
        customerCurrencyRate = currencyTmp.Rate / primaryStoreCurrency.Rate;
      }

      return customerCurrencyRate;
    }

    protected virtual void InitOrderItems(Customer customer, Order order)
    {
      foreach (var item in customer.ShoppingCartItems.Where(i => i.ShoppingCartType == ShoppingCartType.ShoppingCart))
      {
        decimal taxRate;
        decimal itemTaxRate;
        decimal scUnitPrice = this.priceCalculationService.GetUnitPrice(item, true);
        decimal scSubTotal = this.priceCalculationService.GetSubTotal(item, true);
        decimal scUnitPriceInclTax = this.taxService.GetProductPrice(item.Product, scUnitPrice, true, customer, out itemTaxRate);
        decimal scUnitPriceExclTax = this.taxService.GetProductPrice(item.Product, scUnitPrice, false, customer, out itemTaxRate);
        decimal scSubTotalInclTax = this.taxService.GetProductPrice(item.Product, scSubTotal, true, customer, out itemTaxRate);
        decimal scSubTotalExclTax = this.taxService.GetProductPrice(item.Product, scSubTotal, false, customer, out itemTaxRate);

        string attributeDescription = this.productAttributeFormatter.FormatAttributes(item.Product, item.AttributesXml, customer);

        Discount scDiscount = null;
        decimal discountAmount = this.priceCalculationService.GetDiscountAmount(item, out scDiscount);
        decimal itemDiscountAmountInclTax = this.taxService.GetProductPrice(item.Product, discountAmount, true, customer, out taxRate);
        decimal itemDiscountAmountExclTax = this.taxService.GetProductPrice(item.Product, discountAmount, false, customer, out taxRate);

        var itemWeight = this.shippingService.GetShoppingCartItemWeight(item);

        order.OrderItems.Add(
          new OrderItem
          {
            OrderItemGuid = Guid.NewGuid(),
            Order = order,
            ProductId = item.ProductId,
            UnitPriceInclTax = scUnitPriceInclTax,
            UnitPriceExclTax = scUnitPriceExclTax,
            PriceInclTax = scSubTotalInclTax,
            PriceExclTax = scSubTotalExclTax,
            OriginalProductCost = this.priceCalculationService.GetProductCost(item.Product, item.AttributesXml),
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

    protected virtual void InitBilling(Customer customer, Order order)
    {
      if (customer.BillingAddress != null)
      {
        order.BillingAddress = (Address)customer.BillingAddress.Clone();
      }
    }

    protected virtual void InitShipping(Customer customer, Order order)
    {
      if (customer.ShippingAddress != null)
      {
        order.ShippingAddress = (Address)customer.ShippingAddress.Clone();
      }
    }

    protected virtual OrderModel InsertOrder(Order order)
    {
      this.orderService.InsertOrder(order);
      var result = this.GetOrder(order.Id, null);
      if (result != null)
      {
        this.cartsService.DeleteCart(result.CustomerGuid);
      }

      return result;
    }
  }
}