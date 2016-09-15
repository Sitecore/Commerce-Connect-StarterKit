// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentSearchHelper.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The content search helper.
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
// ---------------------------------------------------------------------
namespace Sitecore.Commerce.StarterKit.Helpers
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using Sitecore;
  using Sitecore.Buckets.Util;
  using Sitecore.ContentSearch.Utilities;
  using Sitecore.Data.Items;

  /// <summary>
  /// The content search helper.
  /// </summary>
  public class ContentSearchHelper
  {
    /// <summary>
    /// The get rendering.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="device">The device.</param>
    /// <param name="renderingId">The rendering id.</param>
    /// <returns>The search sting models.</returns>
    [NotNull]
    public virtual IEnumerable<SearchStringModel> GetDataSourceQuery(Item item, DeviceItem device, string renderingId)
    {
      var rendering = item.Visualization.GetRenderings(device, false).FirstOrDefault(r => r.RenderingID.ToString() == renderingId);
      if (rendering == null)
      {
        return new Collection<SearchStringModel>();
      }

      string dataSource = rendering.Settings.DataSource;
      if (string.IsNullOrEmpty(dataSource))
      {
        return new Collection<SearchStringModel>();
      }

      IEnumerable<SearchStringModel> result = SearchStringModel.ParseDatasourceString(dataSource);

      foreach (SearchStringModel clause in result)
      {
        if (!clause.Type.Equals("location", StringComparison.InvariantCultureIgnoreCase))
        {
          continue;
        }

        var i = Context.Database.GetItem(clause.Value);
        if (i == null)
        {
          continue;
        }

        clause.Value = i.ID.ToString();
      }

      return result;
    }
  }
}