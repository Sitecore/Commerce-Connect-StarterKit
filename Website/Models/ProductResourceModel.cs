// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductResourceModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The product resource model.
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
namespace Sitecore.Commerce.StarterKit.Models
{
  using Glass.Sitecore.Mapper.Configuration.Attributes;
  using Glass.Sitecore.Mapper.FieldTypes;

  /// <summary>
  /// The product resource model.
  /// </summary>
  [SitecoreClass]
  public class ProductResourceModel
  {
    /// <summary>
    /// Gets or sets the media item identifier.
    /// </summary>
    /// <value>
    /// The media item identifier.
    /// </value>
    [SitecoreField]
    public virtual Image Resource { get; set; }

    /// <summary>
    /// Gets or sets the URI.
    /// </summary>
    /// <value>
    /// The URI.
    /// </value>
    [SitecoreField("URI")]
    public virtual string Uri { get; set; }
  }
}