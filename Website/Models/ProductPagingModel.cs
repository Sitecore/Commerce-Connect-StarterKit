// ---------------------------------------------------------------------
// <copyright file="ProductPagingModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The ProductPagingModel class.
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
  public class ProductPagingModel
  {
    public ProductPagingModel(int pageIndex, int pageSize, int totalCount)
    {
      this.PageIndex = pageIndex;
      this.PageSize = pageSize;
      this.TotalCount = totalCount;
    }

    public int PageIndex { get; private  set; }

    public int PageSize { get; private set; }

    public int TotalCount { get; private set; }
  }
}