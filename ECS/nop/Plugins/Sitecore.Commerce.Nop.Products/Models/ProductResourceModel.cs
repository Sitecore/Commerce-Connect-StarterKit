// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductResourceModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the product resource model used by remote services.
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
namespace Sitecore.Commerce.Nop.Products.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a product resource.
    /// </summary>
    public class ProductResourceModel
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        public virtual string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public virtual IList<ResourceModel> Resources { get; set; }

        /// <summary>
        /// Gets or sets the type of the product resource.
        /// </summary>
        /// <value>
        /// The type of the product resource.
        /// </value>
        public virtual string ProductResourceType { get; set; }

        /// <summary>
        /// Gets or sets the created on UTC.
        /// </summary>
        /// <value>
        /// The created on UTC.
        /// </value>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the updated on UTC.
        /// </summary>
        /// <value>
        /// The updated on UTC.
        /// </value>
        public virtual DateTime UpdatedOnUtc { get; set; }
    }
}