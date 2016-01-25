// -----------------------------------------------------------------
// <copyright file="SpecificationModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the specification model used by remote services.
// </summary>
// -----------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -----------------------------------------------------------------
namespace Nop.Plugin.Sitecore.Commerce.Products.Models
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Defines the specification model used by remote services.
  /// </summary>
  public class SpecificationLookupModel
  {
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>
    /// The id.
    /// </value>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the specification options.
    /// </summary>
    /// <value>
    /// The specification options.
    /// </value>
    public IList<LookupValueModel> SpecificationOptions { get; set; }
    
    /// <summary>
    /// Gets or sets the created on UTC.
    /// </summary>
    /// <value>
    /// The created on UTC.
    /// </value>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the updated on UTC.
    /// </summary>
    /// <value>
    /// The updated on UTC.
    /// </value>
    public DateTime UpdatedOnUtc { get; set; }




  }
}
