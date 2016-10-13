// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductGlobalSpecificationModel.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ProductGlobalSpecificationModel type.
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
// -----------------------------------------------------------------
namespace Sitecore.Commerce.Nop.Products.Models
{
  /// <summary>
  /// The product global specification model.
  /// </summary>
  public class ProductGlobalSpecificationModel
  {
    /// <summary>
    /// Gets or sets the specification lookup unique identifier.
    /// </summary>
    /// <value>
    /// The specification lookup unique identifier.
    /// </value>
    public string SpecificationLookupId { get; set; }

    /// <summary>
    /// Gets or sets the lookup value.
    /// </summary>
    /// <value>
    /// The lookup value.
    /// </value>
    public string LookupValueId { get; set; }

    /// <summary>
    /// Gets or sets the custom value.
    /// </summary>
    /// <value>
    /// The custom value.
    /// </value>
    public string CustomValue { get; set; }

    /// <summary>
    /// Gets or sets the name of the specification lookup.
    /// </summary>
    /// <value>
    /// The name of the specification lookup.
    /// </value>
    public string SpecificationLookupName { get; set; }

    /// <summary>
    /// Gets or sets the name of the lookup value.
    /// </summary>
    /// <value>
    /// The name of the lookup value.
    /// </value>
    public string LookupValueName { get; set; }
  }
}