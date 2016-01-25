// ---------------------------------------------------------------------
// <copyright file="ProductSortingModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The ProductSortingModel class.
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
  using System.Linq;

  using Sitecore.Diagnostics;

  public class ProductSortingModel
  {
    private const string DirectionAsc = "asc";
    private const string DirectionDesc = "desc";

    public ProductSortingModel([NotNull] string field, string direction)
    {
      Assert.ArgumentNotNullOrEmpty(field, "field");

      this.Field = field.ToLowerInvariant();

      if (new []{DirectionAsc,DirectionDesc}.Contains(direction.ToLowerInvariant()))
      {
        this.Direction = direction.ToLowerInvariant();
      }
      else
      {
        this.Direction = DirectionAsc;
      }
    }

    public string Field { get; private set; }

    public string Direction { get; private set; }

    public string Value
    {
      get
      {
        return this.Field + ":" + this.Direction;
      }
    }

    public static ProductSortingModel Parse(string sorting)
    {
      string field = "name";
      string direction = DirectionAsc;

      string[] array = sorting.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
      if (array.Length > 0)
      {
        field = array[0];
      }
      if (array.Length > 1)
      {
        direction = array[1];
      }

      return new ProductSortingModel(field, direction);
    }
  }
}