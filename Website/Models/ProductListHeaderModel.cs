// ---------------------------------------------------------------------
// <copyright file="ProductListHeaderModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The ProductListHeaderModel class.
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
namespace Sitecore.Commerce.StarterKit.Models
{
  using System;
  using System.Collections.Generic;

  using Glass.Sitecore.Mapper;

  using Sitecore.Commerce.StarterKit.Helpers;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;

  public class ProductListHeaderModel
  {
    public ProductListHeaderModel([NotNull] ProductPagingModel paging, [NotNull] ProductSortingModel sorting)
    {
      Assert.ArgumentNotNull(paging, "paging");
      Assert.ArgumentNotNull(sorting, "sorting");

      this.Paging = paging;

      this.PageSizes = new List<int> {6,12 ,18};

      this.Sorting = sorting;
      this.SortingItems = this.GetSortingItems();
    }

    [NotNull]
    public ProductPagingModel Paging { get; private set; }

    [NotNull]
    public List<int> PageSizes { get; private set; }

    [NotNull]
    public List<ProductSortingItem> SortingItems { get; private set; }

    [NotNull]
    public ProductSortingModel Sorting { get; private set; }

    public int ShowingFrom
    {
      get
      {
        return Math.Min(this.Paging.TotalCount, this.Paging.PageIndex * this.Paging.PageSize + 1);
      }
    }

    public int ShowingTo
    {
      get
      {
        return Math.Min(this.Paging.TotalCount, (this.Paging.PageIndex + 1) * this.Paging.PageSize);
      }
    }

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
