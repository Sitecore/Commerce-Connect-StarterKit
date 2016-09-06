﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedProduct.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   The extended product.
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
namespace Sitecore.Obec.StarterKit.Entities.Products
{
  using Sitecore.Obec.Entities.Products;

  /// <summary>
  /// The extended product.
  /// </summary>
  public class ExtendedProduct : Product
  {
    /// <summary>
    /// Gets or sets the name of the brand.
    /// </summary>
    /// <value>
    /// The name of the brand.
    /// </value>
    public string MetaDescription { get; set; }
  }
}