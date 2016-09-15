// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the product model used by remote services.
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
namespace Nop.Plugin.Sitecore.Commerce.Products.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the product model used by remote services.
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// The global specifications.
        /// </summary>
        private IList<ProductGlobalSpecificationModel> productGlobalSpecifications;

        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the full description.
        /// </summary>
        /// <value>
        /// The full description.
        /// </value>
        public string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the admin comment.
        /// </summary>
        /// <value>
        /// The admin comment.
        /// </value>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show on home page].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show on home page]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords.
        /// </summary>
        /// <value>
        /// The meta keywords.
        /// </value>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description.
        /// </summary>
        /// <value>
        /// The meta description.
        /// </value>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title.
        /// </summary>
        /// <value>
        /// The meta title.
        /// </value>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the name of the se.
        /// </summary>
        /// <value>
        /// The name of the se.
        /// </value>
        public string SeName { get; set; }

        /// <summary>
        /// Gets or sets the product variant ID.
        /// </summary>
        public string ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the name of the product variant.
        /// </summary>
        /// <value>
        /// The name of the product variant.
        /// </value>
        public string ProductVariantName { get; set; }

        /// <summary>
        /// Gets or sets the sku.
        /// </summary>
        /// <value>
        /// The sku.
        /// </value>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer part number.
        /// </summary>
        /// <value>
        /// The manufacturer part number.
        /// </value>
        public string ManufacturerPartNumber { get; set; }

        /// <summary>
        /// Gets or sets the gtin.
        /// </summary>
        /// <value>
        /// The gtin.
        /// </value>
        public string Gtin { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public decimal Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ProductModel"/> is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if deleted; otherwise, <c>false</c>.
        /// </value>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ProductModel"/> is published.
        /// </summary>
        /// <value>
        ///   <c>true</c> if published; otherwise, <c>false</c>.
        /// </value>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the created on UTC.
        /// </summary>
        /// <value>
        /// The created on UTC.
        /// </value>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the updated on UTC.
        /// </summary>
        /// <value>
        /// The updated on UTC.
        /// </value>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer ids.
        /// </summary>
        /// <value>
        /// The manufacturer ids.
        /// </value>
        public IList<string> ManufacturerIds { get; set; }

        /// <summary>
        /// Gets or sets the category ids.
        /// </summary>
        /// <value>
        /// The category ids.
        /// </value>
        public IList<string> CategoryIds { get; set; }

        /// <summary>
        /// Gets or sets the product specifications.
        /// </summary>
        /// <value>
        /// The product specifications.
        /// </value>
        public IList<ProductGlobalSpecificationModel> ProductGlobalSpecifications
        {
            get
            {
                return this.productGlobalSpecifications ?? (this.productGlobalSpecifications = new List<ProductGlobalSpecificationModel>());
            }

            set
            {
                this.productGlobalSpecifications = value;
            }
        }
    }
}
