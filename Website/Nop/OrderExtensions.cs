// ---------------------------------------------------------------------
// <copyright file="WishlistExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Helpful order extensions.
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
  using System.Collections.Generic;
  using System.Globalization;
  using Sitecore.Commerce.Connectors.NopCommerce.NopOrdersService;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Entities.Prices;
  using Sitecore.Common;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Entities.Orders;

  /// <summary>
  /// The order extensions.
  /// </summary>
  public static class OrderExtensions
  {
    /// <summary>
    /// Converts NopCommerce order line model to the OBEC cart line.
    /// </summary>
    /// <param name="model">The model.</param>
    [NotNull]
    public static CartLine ToObecOrderLine([NotNull] this ShoppingCartItemModel model)
    {
      Assert.ArgumentNotNull(model, "model");
      return new CartLine
               {
                 ExternalCartLineId = model.Id,
                 Product = new CartProduct
                 {
                   ProductId = model.ProductId.ToString(CultureInfo.InvariantCulture), 
                   Price = new Price
                   {
                     Amount = model.Price
                   }
                 },
                 Quantity = model.Quantity,
                 Total = new Total { Amount = model.LineTotal }
               };
    }

    /// <summary>
    /// Maps order from model.
    /// </summary>
    public static void MapOrderFromModel([NotNull] this Order order, [NotNull] OrderModel orderModel)
    {
      Assert.ArgumentNotNull(order, "order");
      Assert.ArgumentNotNull(orderModel, "orderModel");
      
      order.MapCartBaseFromModel(orderModel);

      order.OrderDate = orderModel.CreatedDateTime;
      order.OrderID = orderModel.Id.ToString();

      if (orderModel.Discount > 0)
      {
        var adjusment = new CartAdjustment
        {
          Amount = orderModel.Discount
        };

        order.Adjustments = (new List<CartAdjustment>
        {
          adjusment
        }).AsReadOnly();
      }

      var payment = new PaymentInfo
      {
        PaymentMethodID = orderModel.PaymentMethod
      };

      payment.Properties.Add(new PropertyItem("status", orderModel.PaymentStatus));
      order.Payment = (new List<PaymentInfo>
      {
        payment
      }).AsReadOnly();

      var shipping = new List<ShippingInfo>();
      foreach (ShipmentModel model in orderModel.Shipments)
      {
        var info = new ShippingInfo();
        info.MapShipmentFromModel(model);
        shipping.Add(info);
      }

      order.Shipping = shipping.AsReadOnly();

      var orderLines = new List<CartLine>();
      foreach (var model in orderModel.OrderItems)
      {
        CartLine line = model.ToObecOrderLine();
        orderLines.Add(
          new CartLine
          {
            ExternalCartLineId = line.ExternalCartLineId,
            Product = line.Product,
            Properties = line.Properties,
            Quantity = line.Quantity,
            Total = line.Total
          });
      }

      order.Lines = orderLines.AsReadOnly();

      var parties = new List<Party>();
      foreach (AddressModel address in orderModel.Addresses)
      {
        var party = new Party();
        party.MapPartyFromNopAddress(address);
        parties.Add(party);
      }

      order.Parties = parties.AsReadOnly();

      if (orderModel.Total != null)
      {
        order.Total = new Total
        {
          Amount = orderModel.Total.Value
        };
      }
    }

    public static void MapOrderHeaderFromModel([NotNull] this OrderHeader orderHeader, [NotNull] OrderModel orderModel)
    {
      orderHeader.MapCartBaseFromModel(orderModel);
      orderHeader.OrderDate = orderModel.CreatedDateTime;
      orderHeader.OrderID = orderModel.Id.ToString();
      orderHeader.Status = orderModel.Status;
    }

    public static void MapCartBaseFromModel([NotNull] this CartBase orderHeader, [NotNull] OrderModel orderModel)
    {
      Assert.ArgumentNotNull(orderHeader, "orderHeader");
      Assert.ArgumentNotNull(orderModel, "orderModel");

      if (orderModel.BillingAddressId != null)
      {
        orderHeader.AccountingCustomerParty = new CartParty
        {
          ExternalId = orderModel.BillingAddressId.Value.ToString(),
          PartyID = orderModel.BillingAddressId.Value.ToString(),
          Name = orderModel.BillingName
        }; // Billing
      }

      orderHeader.Email = orderModel.CustomerEmail;
      if (orderModel.ShippingAddressId != null)
      {
        orderHeader.BuyerCustomerParty = new CartParty
        {
          ExternalId = orderModel.ShippingAddressId.Value.ToString(),
          PartyID = orderModel.ShippingAddressId.Value.ToString(),
          Name = orderModel.ShippingName
        }; // Shipping
      }

      orderHeader.CartType = orderModel.CardType;
      orderHeader.CustomerId = orderModel.CustomerId.ToString();
      orderHeader.ExternalId = orderModel.OrderGuid.ToID().ToString();
      orderHeader.IsLocked = orderModel.IsDeleted;
      orderHeader.Name = orderModel.Name;
      orderHeader.Status = orderModel.Status;
      orderHeader.UserId = orderModel.CustomerGuid.ToID().ToString().ToUpper();
    }
  }
}