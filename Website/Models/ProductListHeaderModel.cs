// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductListHeaderModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the ProductListHeaderModel class.</summary>
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
    using System;
    using System.Collections.Generic;
    using Glass.Sitecore.Mapper;
    using Sitecore.Commerce.StarterKit.Helpers;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;

    /// <summary>
    /// represents product list header information.
    /// </summary>
    public class ProductListHeaderModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductListHeaderModel"/> class.
        /// </summary>
        /// <param name="paging">The paging information.</param>
        /// <param name="sorting">The sorting information.</param>
        public ProductListHeaderModel([NotNull] ProductPagingModel paging, [NotNull] ProductSortingModel sorting)
        {
            Assert.ArgumentNotNull(paging, "paging");
            Assert.ArgumentNotNull(sorting, "sorting");

            this.Paging = paging;

            this.PageSizes = new List<int> { 6, 12, 18 };

            this.Sorting = sorting;
            this.SortingItems = this.GetSortingItems();
        }

        /// <summary>
        /// Gets the paging information.
        /// </summary>
        [NotNull]
        public ProductPagingModel Paging { get; private set; }

        /// <summary>
        /// Gets the page sizes.
        /// </summary>
        [NotNull]
        public List<int> PageSizes { get; private set; }

        /// <summary>
        /// Gets the sorting items.
        /// </summary>
        [NotNull]
        public List<ProductSortingItem> SortingItems { get; private set; }

        /// <summary>
        /// Gets the sorting information.
        /// </summary>
        [NotNull]
        public ProductSortingModel Sorting { get; private set; }

        /// <summary>
        /// Gets the item number the page starts with.
        /// </summary>
        public int ShowingFrom
        {
            get
            {
                return Math.Min(this.Paging.TotalCount, (this.Paging.PageIndex * this.Paging.PageSize) + 1);
            }
        }

        /// <summary>
        /// Gets the item number the page ends with.
        /// </summary>
        public int ShowingTo
        {
            get
            {
                return Math.Min(this.Paging.TotalCount, (this.Paging.PageIndex + 1) * this.Paging.PageSize);
            }
        }

        /// <summary>
        /// Gets the sorting items.
        /// </summary>
        /// <returns>The sorting items.</returns>
        protected List<ProductSortingItem> GetSortingItems()
        {
            var result = new List<ProductSortingItem>();

            ISitecoreService glassMapper = GlassMapperService.Current;

            var sortingRoot = Sitecore.Context.Database.GetItem("/sitecore/content/Global/Product Sorting");
            if (sortingRoot != null)
            {
                foreach (Item sortingItem in sortingRoot.Children)
                {
                    result.Add(glassMapper.CreateClass<ProductSortingItem>(false, false, sortingItem));
                }
            }

            return result;
        }
    }
}
