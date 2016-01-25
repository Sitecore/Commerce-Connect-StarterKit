// ---------------------------------------------------------------------
// <copyright file="PagedList.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The PagedList class.
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
  using System.Collections.Generic;

  public class PagedList<T>
  {
    public PagedList(IEnumerable<T> data, int page, int pageSize, int totalCount)
    {
      this.List = new List<T>(data);

      this.Page = page;
      this.PageSize = pageSize;
      this.TotalCount = totalCount;
    }

    public List<T> List { get; private set; }

    public int Page { get; private set; }

    public int PageSize { get; private set; }

    public int TotalCount { get; private set; } 
  }
}