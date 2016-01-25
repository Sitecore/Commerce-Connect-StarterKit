// -----------------------------------------------------------------
// <copyright file="StockInformationUpdateModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the StockInformationUpdateModel class.
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
namespace Nop.Plugin.Sitecore.Commerce.Inventory.Models
{
  using System.Collections.Generic;

  public class StockInformationUpdateModel
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="StockInformationUpdateModel"/> class.
    /// </summary>
    public StockInformationUpdateModel()
    {
      StockInformationUpdateLocation = new List<StockInformationUpdateLocationModel>();
    }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    /// <value>
    /// The product identifier.
    /// </value>
    public string ProductId { get; set; }

    /// <summary>
    /// Gets or sets the stock information update location.
    /// </summary>
    /// <value>
    /// The stock information update location.
    /// </value>
    public IList<StockInformationUpdateLocationModel> StockInformationUpdateLocation { get; set; }
  }
}
