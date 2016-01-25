// ---------------------------------------------------------------------
// <copyright file="ProductModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ProductModel type.
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
using System.Globalization;
using Glass.Sitecore.Mapper.Configuration;

namespace Sitecore.Commerce.StarterKit.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Glass.Sitecore.Mapper.Configuration.Attributes;
    using Sitecore;
    using Sitecore.Commerce.Entities.Inventory;
    using System;

    /// <summary>
    /// The product.
    /// </summary>
    [SitecoreClass]
    public class ProductModel
    {
        /// <summary>
        /// The price.
        /// </summary>
        private readonly decimal price;

        /// <summary>
        /// The stock information.
        /// </summary>
        private readonly StockInformation stockInformation;

        /// <summary>
        /// The orderable information.
        /// </summary>
        private readonly OrderableInformation orderableInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModel" /> class.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="stockInformation">The stock information.</param>
        public ProductModel(decimal price, StockInformation stockInformation)
        {
            this.price = price;
            this.stockInformation = stockInformation;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModel" /> class.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="stockInformation">The stock information.</param>
        /// <param name="orderableInformation">The pre-orderable information.</param>
        public ProductModel(decimal price, StockInformation stockInformation, OrderableInformation orderableInformation)
            : this(price, stockInformation)
        {
            this.orderableInformation = orderableInformation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModel" /> class.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="stockInformation">The stock information.</param>
        /// <param name="manufacturers">The manufacturers.</param>
        /// <param name="productClasses">The product classes.</param>
        internal ProductModel(decimal price, StockInformation stockInformation, IEnumerable<ManufacturerModel> manufacturers = null, IEnumerable<CategoryModel> productClasses = null)
            : this(price, stockInformation)
        {
            this.Manufacturers = manufacturers;
            this.ProductClasses = productClasses;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModel" /> class.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="stockInformation">The stock information.</param>
        /// <param name="productClasses">The product classes.</param>
        internal ProductModel(decimal price, StockInformation stockInformation, IEnumerable<CategoryModel> productClasses)
            : this(price, stockInformation)
        {
            this.ProductClasses = productClasses;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [SitecoreField]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the external ID.
        /// </summary>
        /// <value>
        /// The external ID.
        /// </value>
        [SitecoreField]
        public string ExternalID { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The body type.</value>
        [SitecoreField("Short Description")]
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the Full description.
        /// </summary>
        /// <value>The body type.</value>
        [SitecoreField("{25FE6930-D1F7-4924-95F9-770AA320A1F9}",SitecoreFieldType.RichText)]
        public string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public string Image { get; set; }
      
        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>The image.</value>
        public List<string> Images { get; set; }

        /// <summary>
        /// Gets the formatted short description.
        /// </summary>
        /// <value>The formatted short description.</value>
        public string FormattedShortDescription
        {
            get
            {
                return string.IsNullOrEmpty(this.ShortDescription) ? "-" : this.ShortDescription;
            }
        }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string Manufacturer
        {
            get
            {
                if (this.Manufacturers != null && this.Manufacturers.Any())
                {
                    return string.Join(", ", this.Manufacturers.Select(m => m.Name));
                }

                return "-";
            }
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public string Categories
        {
            get
            {
                if (this.ProductClasses != null && this.ProductClasses.Any())
                {
                    return string.Join(", ", this.ProductClasses.Select(m => m.Name));
                }

                return "-";
            }
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        /// <value>The price.</value>
        public decimal Price
        {
            get { return this.price; }
        }

        /// <summary>
        /// Gets the formatted price.
        /// </summary>
        /// <value>The formatted price.</value>
        public string FormattedPrice
        {
          get { return this.Price != 0 ? this.Price.ToString("c", new CultureInfo("en-US")) : "-"; }
        }

        /// <summary>
        /// Gets the inventory status.
        /// </summary>
        /// <value>The inventory status.</value>
        public StockStatus Status
        {
            get { return this.stockInformation != null && this.stockInformation.Status != null ? this.stockInformation.Status : null; }
        }

        /// <summary>
        /// Gets the inventory status name.
        /// </summary>
        /// <value>The inventory status name.</value>
        public string StatusName
        {
            get { return this.Status != null ? this.Status.Name : "-"; }
        }

        /// <summary>
        /// Gets the inventory availability date.
        /// </summary>
        /// <value>The inventory availability date.</value>
        public DateTime? AvailabilityDate
        {
            get { return this.stockInformation != null && this.stockInformation.AvailabilityDate.HasValue ? this.stockInformation.AvailabilityDate : null; }
        }

        /// <summary>
        /// Gets the inventory formatted availability date.
        /// </summary>
        /// <value>The inventory formatted availability date.</value>
        public string FormattedAvailabilityDate
        {
            get { return this.AvailabilityDate.HasValue ? this.AvailabilityDate.Value.Date.ToShortDateString() : "-"; }
        }

        /// <summary>
        /// Gets the inventory in-stock date.
        /// </summary>
        /// <value>The inventory in-stock date.</value>
        public DateTime? InStockDate
        {
            get { return this.orderableInformation != null && this.orderableInformation.InStockDate.HasValue ? this.orderableInformation.InStockDate : null; }
        }

        /// <summary>
        /// Gets the inventory formatted in-stock date.
        /// </summary>
        /// <value>The inventory formatted in-stock date.</value>
        public string FormattedInStockDate
        {
            get { return this.InStockDate.HasValue ? this.InStockDate.Value.Date.ToShortDateString() : "-"; }
        }

        /// <summary>
        /// Gets the inventory remaining quantity.
        /// </summary>
        /// <value>The inventory remaining quantity.</value>
        public double RemainingQuantity 
        {
            get { return this.orderableInformation != null ? this.orderableInformation.RemainingQuantity : 0; }
        }

        /// <summary>
        /// Gets the inventory formatted remaining quantity.
        /// </summary>
        /// <value>The inventory formatted remaining quantity.</value>
        public string FormattedRemainingQuantity
        {
            get { return this.RemainingQuantity != 0 ? this.RemainingQuantity.ToString(CultureInfo.InvariantCulture) : "-"; }
        }        
        
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [SitecoreField("Manufacturer")]
        private IEnumerable<ManufacturerModel> Manufacturers { get; [UsedImplicitly] set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [SitecoreField]
        private IEnumerable<CategoryModel> ProductClasses { get; [UsedImplicitly] set; }
    }
}