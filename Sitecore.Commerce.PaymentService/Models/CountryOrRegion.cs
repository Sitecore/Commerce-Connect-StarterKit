// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountryOrRegion.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CountryOrRegion class.
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

namespace Sitecore.Commerce.PaymentService
{
    /// <summary>
    /// Represents a country or region.
    /// </summary>
    public class CountryOrRegion
    {
        /// <summary>
        /// Gets or sets the two-letter ISO code, e.g. US.
        /// </summary>
        public string TwoLetterCode { get; set; }

        /// <summary>
        /// Gets or sets the locale in which the names are, e.g. en-US.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the long name.
        /// </summary>
        public string LongName { get; set; }
    }
}