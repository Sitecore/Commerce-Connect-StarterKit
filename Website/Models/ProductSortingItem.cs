// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductSortingItem.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the ProductSortingItem class.</summary>
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
// ---------------------------------------------------------------------
namespace Sitecore.Commerce.StarterKit.Models
{
    using Glass.Sitecore.Mapper.Configuration;
    using Glass.Sitecore.Mapper.Configuration.Attributes;

    /// <summary>
    /// Represents product sorting information.
    /// </summary>
    [SitecoreClass]
    public class ProductSortingItem
    {
        private string field;
        private string direction;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [SitecoreInfo(SitecoreInfoType.DisplayName)]
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        [SitecoreField]
        public virtual string Field
        {
            get
            {
                return this.field;
            }

            set
            {
                this.field = value.ToLowerInvariant();
            }
        }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        [SitecoreField]
        public virtual string Direction
        {
            get
            {
                if (!string.IsNullOrEmpty(this.direction))
                {
                    return this.direction;
                }

                return "asc";
            }

            set
            {
                this.direction = value.ToLowerInvariant();
            }
        }
    }
}