// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryFacetModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the CategoryFacetModel class.</summary>
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
    using System.Collections.Generic;
    using Sitecore.Diagnostics;

    /// <summary>
    /// Represents category facet information.
    /// </summary>
    public class CategoryFacetModel
    {
        private readonly IReadOnlyCollection<CategoryModel> categories;

        private readonly string currentCategory;

        private readonly string currentSubCategory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryFacetModel"/> class.
        /// </summary>
        /// <param name="categories">The categories.</param>
        /// <param name="currentCategory">The current category.</param>
        /// <param name="currentSubCategory">The sub category.</param>
        public CategoryFacetModel([NotNull] IEnumerable<CategoryModel> categories, [NotNull] string currentCategory, [CanBeNull] string currentSubCategory)
        {
            Assert.ArgumentNotNull(categories, "classifications");
            Assert.ArgumentNotNullOrEmpty(currentCategory, "currentCategory");

            this.categories = new List<CategoryModel>(categories);

            this.currentCategory = currentCategory;
            this.currentSubCategory = currentSubCategory;
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<CategoryModel> Categories
        {
            get { return this.categories; }
        }

        /// <summary>
        /// Gets the current category.
        /// </summary>
        [NotNull]
        public string CurrentCategory
        {
            get { return this.currentCategory; }
        }

        /// <summary>
        /// Gets the current sub category.
        /// </summary>
        [CanBeNull]
        public string CurrentSubCategory
        {
            get { return this.currentSubCategory; }
        }
    }
}