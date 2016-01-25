// -----------------------------------------------------------------
// <copyright file="ShoppingCartModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Class for Nop shopping cart data
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
namespace Nop.Plugin.Sitecore.Commerce.Common.Models
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Runtime.Serialization;
  using Nop.Core.Domain.Common;
  using Nop.Core.Domain.Customers;
  using Nop.Core.Domain.Orders;
  using Nop.Core.Domain.Shipping;
  using Nop.Core.Infrastructure;
  using Nop.Services.Common;
  using Nop.Services.Orders;
  using Nop.Services.Stores;

  /// <summary>
  /// Class for Nop shopping cart data
  /// </summary>
  [DataContract]
  public class ShoppingCartModel
  {
    /// <summary>
    /// The order total calculation service.
    /// </summary>
    protected readonly IOrderTotalCalculationService orderTotalCalculationService;

    /// <summary>
    /// The store service
    /// </summary>
    private readonly IStoreService storeService;

    /// <summary>
    /// The generic attribute service
    /// </summary>
    private readonly IGenericAttributeService genericAttributeService;


    [DataMember]
    public int StoreId { get; set; }

    [DataMember]
    public int TotalItems { get; set; }

    [DataMember]
    public int CustomerId { get; set; }

    [DataMember]
    public Guid CustomerGuid { get; set; }

    [DataMember]
    public decimal? Total { get; set; }

    [DataMember]
    public IList<ShoppingCartItemModel> ShoppingItems { get; set; }

    [DataMember]
    public bool IsAnonymous { get; set; }

    [DataMember]
    public string CustomerEmail { get; set; }

    [DataMember]
    public bool IsDeleted { get; set; }

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public int? ShippingAddressId { get; set; }

    [DataMember]
    public AddressModel ShippingAddress { get; set; }

    [DataMember]
    public string ShippingName { get; set; }

    [DataMember]
    public int? BillingAddressId { get; set; }

    [DataMember]
    public AddressModel BillingAddress { get; set; }

    [DataMember]
    public string BillingName { get; set; }


    [DataMember]
    public ICollection<AddressModel> Addresses { get; set; }

    [DataMember]
    public ICollection<ShipmentModel> Shipments { get; set; }

    [DataMember]
    public decimal Discount { get; set; }

    [DataMember]
    public string PaymentMethod { get; set; }

    [DataMember]
    public string PaymentStatus { get; set; }
    
    [DataMember]
    public string ShippingMethod { get; set; }

    [DataMember]
    public string ShippingStatus { get; set; }

    [DataMember]
    public PaymentInfoModel PaymentInfo { get; set; }

    [DataMember]
    public ShippingMethodModel ShippingInfo { get; set; }


    public ShoppingCartModel()
    {
      this.orderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
      this.storeService = EngineContext.Current.Resolve<IStoreService>();
      this.genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
    }

    public void Map(Customer customer, ShoppingCartType cartType, string storeName = null)
    {
      this.CustomerGuid = customer.CustomerGuid;
      this.CustomerId = customer.Id;
      this.Total = this.orderTotalCalculationService.GetShoppingCartTotal(customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == cartType).ToList(), true, false);
      this.CustomerEmail = customer.IsGuest() ? string.Empty : customer.Email;
      this.ShoppingItems = this.GetShoppingItemsModels(customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == cartType).ToList());
      this.IsAnonymous = customer.IsGuest();
      this.IsDeleted = customer.Deleted;
      this.ShippingAddressId = (customer.ShippingAddress != null) ? customer.ShippingAddress.Id : (int?)null;
      if (customer.ShippingAddress != null)
      {
        this.ShippingName = customer.ShippingAddress.FirstName + " " + customer.ShippingAddress.LastName;
        var model = new AddressModel();
        model.Map(customer.ShippingAddress);
        this.ShippingAddress = model;
      }
      this.BillingAddressId = (customer.BillingAddress != null) ? customer.BillingAddress.Id : (int?)null;
      if (customer.BillingAddress != null)
      {
        this.BillingName = customer.BillingAddress.FirstName + " " + customer.BillingAddress.LastName;
        var model = new AddressModel();
        model.Map(customer.BillingAddress);
        this.BillingAddress = model;
      }

      this.Addresses = this.GetAddressModels(customer.Addresses);

      /////////////////////
      this.StoreId = this.GetStoreId(storeName);
      this.PaymentInfo = this.GetPaymentInfoModel(customer, this.StoreId);
      this.ShippingInfo = this.GetShippingMethodModel(customer, this.StoreId);
    }


    protected IList<ShoppingCartItemModel> GetShoppingItemsModels(ICollection<ShoppingCartItem> items)
    {
      var cartItems = new List<ShoppingCartItemModel>();

      foreach (ShoppingCartItem item in items)
      {
        if (item.Product != null)
        {
          cartItems.Add(new ShoppingCartItemModel
          {
            Id = item.Id.ToString(),
            ProductId = item.ProductId,
            Price = item.Product.Price,
            LineTotal = item.Product.Price * (uint)item.Quantity,
            Sku = item.Product.Sku,
            Quantity = (uint)item.Quantity
          });
        }
      }

      return cartItems;
    }

    protected IList<AddressModel> GetAddressModels(ICollection<Address> addresses)
    {
      var models = new List<AddressModel>();

      foreach (Address address in addresses)
      {
        var model = new AddressModel();
        model.Map(address);
        models.Add(model);
      }

      return models;
    }

    protected int GetStoreId(string storeName)
    {
      int storeId;

      if (string.IsNullOrEmpty(storeName))
      {
        storeId = 0;
      }
      else
      {
        var store =
          this.storeService.GetAllStores()
            .FirstOrDefault(s => s.Name.Equals(storeName, StringComparison.InvariantCultureIgnoreCase));

        storeId = store == null ? 0 : store.Id;
      }

      return storeId;
    }

    protected PaymentInfoModel GetPaymentInfoModel(Customer customer, int storeId)
    {
      string selectedPaymentMethodSystemName = null;

      try
      {
        selectedPaymentMethodSystemName = customer.GetAttribute<string>(SystemCustomerAttributeNames.SelectedPaymentMethod, this.genericAttributeService, storeId);
      }
      catch
      {
        // ignored
      }

      var paymentMethod = !String.IsNullOrEmpty(selectedPaymentMethodSystemName) ?
        new PaymentInfoModel
        {
          SystemName = selectedPaymentMethodSystemName
        } : null;

      return paymentMethod;
    }

    protected ShippingMethodModel GetShippingMethodModel(Customer customer, int storeId)
    {
      ShippingOption selectedShippingOption = null;

      try
      {
        selectedShippingOption = customer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, storeId);
      }
      catch
      {
        // ignored
      }

      var shippingOption = selectedShippingOption != null ?
        new ShippingMethodModel
        {
          SystemName = selectedShippingOption.ShippingRateComputationMethodSystemName,
          Name = selectedShippingOption.Name,
          Description = selectedShippingOption.Description
        } : null;

      return shippingOption;
    }
  }
}