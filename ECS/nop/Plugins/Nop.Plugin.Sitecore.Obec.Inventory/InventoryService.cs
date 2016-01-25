// -----------------------------------------------------------------
// <copyright file="InventoryService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the InventoryService type.
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
namespace Nop.Plugin.Sitecore.Commerce.Inventory
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.ServiceModel.Activation;
  using System.Web.Services;
  using Nop.Core.Domain.Catalog;
  using Nop.Core.Infrastructure;
  using Nop.Plugin.Sitecore.Commerce.Inventory.Models;
  using Nop.Services.Catalog;
  using Nop.Services.Stores;

  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  public class InventoryService : IInventoryService
  {
    /// <summary>
    ///   The product service.
    /// </summary>
    private readonly IProductService productService;

    /// <summary>
    ///   The store mapping service
    /// </summary>
    private readonly IStoreMappingService storeMappingService;

    /// <summary>
    ///   The store service
    /// </summary>
    private readonly IStoreService storeService;

    /// <summary>
    ///   Initializes a new instance of the <see cref="InventoryService" /> class.
    /// </summary>
    public InventoryService()
    {
      this.productService = EngineContext.Current.Resolve<IProductService>();
      this.storeMappingService = EngineContext.Current.Resolve<IStoreMappingService>();
      this.storeService = EngineContext.Current.Resolve<IStoreService>();
    }

    /// <summary>
    ///   Gets the stock information.
    /// </summary>
    /// <param name="shopName">Name of the shop.</param>
    /// <param name="externalproductIds">The externalproduct ids.</param>
    /// <param name="customerId">The customer identifier.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual IList<StockInformationModel> GetStocksInformation(string shopName, IList<string> externalproductIds, Guid customerId)
    {
      return externalproductIds.Select(externalProductId => this.GetStockInformation(shopName, externalProductId, customerId)).ToList();
    }

    /// <summary>
    ///   Gets the stock information.
    /// </summary>
    /// <param name="shopName">Name of the shop.</param>
    /// <param name="externalProductId">The external product identifier.</param>
    /// <param name="customerId">The customer identifier.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual StockInformationModel GetStockInformation(string shopName, string externalProductId, Guid customerId)
    {
      // Don't think customerId has any meaning here - will have to investigate further.
      // Not sure implementation on shopName detection is corret - needs to be verified.
      int productId;

      if (!int.TryParse(externalProductId, out productId))
      {
        return null;
      }
      var product = this.productService.GetProductById(productId);

      if (product == null || product.Deleted)
      {
        return null;
      }

      if (product.LimitedToStores && !this.IsProductInStore(product, shopName))
      {
        return null;
      }

      var nowUtc = DateTime.UtcNow;
      var availability = (product.StockQuantity <= 0) ? StockStatus.OutOfStock : StockStatus.InStock;

      if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock
          && product.StockQuantity <= 0)
      {
        switch (product.BackorderMode)
        {
          case BackorderMode.NoBackorders:
          {
            if (product.AvailableForPreOrder)
            {
              availability = StockStatus.PreOrderable;
              break;
            }
            availability = StockStatus.OutOfStock;
          }
            break;
          case BackorderMode.AllowQtyBelow0:
          case BackorderMode.AllowQtyBelow0AndNotifyCustomer:
          {
            availability = StockStatus.BackOrderable;
          }
            break;
          default:
            availability = (product.StockQuantity <= 0) ? StockStatus.OutOfStock : StockStatus.InStock;
            break;
        }
      }

      if (product.AvailableStartDateTimeUtc.HasValue && product.AvailableStartDateTimeUtc.Value < nowUtc && product.AvailableForPreOrder)
      {
        availability = StockStatus.PreOrderable;
      }

      return new StockInformationModel
      {
        ProductId = externalProductId,
        Count = product.StockQuantity,
        Status = availability,
        AvailabilityDate = product.AvailableStartDateTimeUtc.HasValue ? product.AvailableStartDateTimeUtc : null
      };
    }

    /// <summary>
    ///   Gets the pre orderable information by product list.
    /// </summary>
    /// <param name="shopName">Name of the shop.</param>
    /// <param name="externalproductIds">The externalproduct ids.</param>
    /// <param name="customerId">The customer identifier.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual IList<OrderableInformationModel> GetPreOrderableInformationList(string shopName, IList<string> externalproductIds, Guid customerId)
    {
      return externalproductIds.Select(x => this.GetPreOrderableInformation(shopName, x, customerId)).ToList();
    }

    /// <summary>
    ///   Gets the pre orderable information by product.
    /// </summary>
    /// <param name="shopName">Name of the shop.</param>
    /// <param name="externalProductId">The external product identifier.</param>
    /// <param name="customerId">The customer identifier.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual OrderableInformationModel GetPreOrderableInformation(string shopName, string externalProductId, Guid customerId)
    {
      int productId;
      if (!int.TryParse(externalProductId, out productId))
      {
        return null;
      }
      var product = this.productService.GetProductById(productId);

      if (product == null || product.Deleted)
      {
        return null;
      }

      if (product.LimitedToStores && !this.IsProductInStore(product, shopName))
      {
        return null;
      }

      var nowUtc = DateTime.UtcNow;
      var availability = (product.StockQuantity <= 0) ? StockStatus.OutOfStock : StockStatus.InStock;

      if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock && product.StockQuantity <= 0)
      {
        switch (product.BackorderMode)
        {
          case BackorderMode.NoBackorders:
          {
            if (product.AvailableForPreOrder)
            {
              availability = StockStatus.PreOrderable;
              break;
            }
            availability = StockStatus.OutOfStock;
          }
            break;
          case BackorderMode.AllowQtyBelow0:
          case BackorderMode.AllowQtyBelow0AndNotifyCustomer:
          {
            availability = StockStatus.BackOrderable;
          }
            break;
          default:
            availability = (product.StockQuantity <= 0) ? StockStatus.OutOfStock : StockStatus.InStock;
            break;
        }
      }

      if (product.AvailableStartDateTimeUtc.HasValue && product.AvailableStartDateTimeUtc.Value < nowUtc && product.AvailableForPreOrder)
      {
        availability = StockStatus.PreOrderable;
      }

      return new OrderableInformationModel
      {
        ProductId = externalProductId,
        Status = availability,
        InStockDate = product.AvailableStartDateTimeUtc.HasValue ? product.AvailableStartDateTimeUtc : null,
        ShippingDate = null,
        CartQuantityLimit = product.OrderMaximumQuantity,
        OrderableStartDate = product.AvailableStartDateTimeUtc,
        OrderableEndDate = product.AvailableEndDateTimeUtc,
        RemainingQuantity = product.StockQuantity
      };
    }

    /// <summary>
    ///   Gets the back orderable information by product list.
    /// </summary>
    /// <param name="shopName">Name of the shop.</param>
    /// <param name="externalproductIds">The externalproduct ids.</param>
    /// <param name="customerId">The customer identifier.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual IList<OrderableInformationModel> GetBackOrderableInformationList(string shopName,
      IList<string> externalproductIds, Guid customerId)
    {
      return externalproductIds.Select(x => this.GetBackOrderableInformation(shopName, x, customerId)).ToList();
    }

    /// <summary>
    ///   Gets the back orderable information by product.
    /// </summary>
    /// <param name="shopName">Name of the shop.</param>
    /// <param name="externalProductId">The external product identifier.</param>
    /// <param name="customerId">The customer identifier.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual OrderableInformationModel GetBackOrderableInformation(string shopName, string externalProductId, Guid customerId)
    {
      return this.GetPreOrderableInformation(shopName, externalProductId, customerId);
    }

    /// <summary>
    ///   Gets the back in stock information.
    /// </summary>
    /// <param name="shopName">Name of the shop.</param>
    /// <param name="externalproductIds">The externalproduct ids.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual IList<StockInformationUpdateModel> GetBackInStocksInformation(string shopName, IList<string> externalproductIds)
    {
      return externalproductIds.Select(x => this.GetBackInStockInformation(shopName, x)).ToList();
    }

    /// <summary>
    ///   Gets the back in stock information by product.
    /// </summary>
    /// <param name="shopName">Name of the shop.</param>
    /// <param name="externalProductId">The externalproduct identifier.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual StockInformationUpdateModel GetBackInStockInformation(string shopName, string externalProductId)
    {
      int productId;
      if (!int.TryParse(externalProductId, out productId))
      {
        return new StockInformationUpdateModel();
      }
      var product = this.productService.GetProductById(productId);

      if (product == null || product.Deleted)
      {
        return new StockInformationUpdateModel();
      }

      IList<string> productLocations = !product.LimitedToStores ? this.storeService.GetAllStores().Select(x => x.Name).ToList() : this.storeMappingService.GetStoreMappings(product).Select(x => x.EntityName).ToList();

      return new StockInformationUpdateModel
      {
        ProductId = externalProductId,
        StockInformationUpdateLocation = productLocations.Select(x => new StockInformationUpdateLocationModel
        {
          AvailabilityDate = product.AvailableStartDateTimeUtc.HasValue ? product.AvailableStartDateTimeUtc : null,
          Count = product.StockQuantity,
          Location = x
        }).ToList()
      };
    }

    /// <summary>
    ///   Stocks the status for indexing.
    /// </summary>
    /// <param name="externalproductIds">The externalproduct ids.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual IList<IndexStockInformationModel> StocksStatusForIndexing(IList<string> externalproductIds)
    {
      return externalproductIds.Select(this.StockStatusForIndexing).ToList();
    }

    /// <summary>
    ///   Stocks the status for indexing by product.
    /// </summary>
    /// <param name="externalProductId">The externalproduct identifier.</param>
    /// <returns></returns>
    [WebMethod(EnableSession = false)]
    public virtual IndexStockInformationModel StockStatusForIndexing(string externalProductId)
    {
      int productId;
      if (!int.TryParse(externalProductId, out productId))
      {
        return new IndexStockInformationModel();
      }
      var product = this.productService.GetProductById(productId);

      if (product == null || product.Deleted)
      {
        return new IndexStockInformationModel();
      }

      IList<string> productLocations = !product.LimitedToStores ? this.storeService.GetAllStores().Select(x => x.Name).ToList() : this.storeMappingService.GetStoreMappings(product).Select(x => x.EntityName).ToList();
      return new IndexStockInformationModel
      {
        ProductId = externalProductId,
        PreOrderable = product.AvailableForPreOrder,
        InStockLocations = (product.StockQuantity > 0) ? productLocations : new List<string>(),
        OutOfStockLocations = (product.StockQuantity <= 0) ? productLocations : new List<string>(),
        OrderableLocations = productLocations
      };
    }

    /// <summary>
    ///   Determines whether the specified product is available in the given store.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <param name="shopName">Name of the shop.</param>
    /// <returns></returns>
    private bool IsProductInStore(Product product, string shopName)
    {
      return this.storeMappingService.GetStoreMappings(product).Select(x => x.EntityName == shopName).Any();
    }
  }
}