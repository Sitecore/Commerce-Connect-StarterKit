// ----------------------------------------------------------------------------------------------
// <copyright file="ManufacturerModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The manufacturer model.
// </summary>
// ----------------------------------------------------------------------------------------------
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
  using Glass.Sitecore.Mapper.Configuration.Attributes;

  /// <summary>
  /// The manufacturer model.
  /// </summary>
  [SitecoreClass]
  public class ManufacturerModel
  {
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>
    /// The id.
    /// </value>
    [SitecoreId]
    public virtual Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    [SitecoreField]
    public virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the external ID.
    /// </summary>
    /// <value>
    /// The external ID.
    /// </value>
    [SitecoreField]
    public virtual string ExternalID { get; set; }
  }
}