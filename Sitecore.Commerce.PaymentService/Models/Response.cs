// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Response.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the Response class.
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
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Defines a response.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Gets or sets the response errors.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "Allow AX access to these objects")]
        public PaymentError[] Errors { get; set; }

        /// <summary>
        /// Gets or sets the response locale.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the response properties.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "Allow AX access to these objects")]
        public PaymentProperty[] Properties { get; set; }
    }
}
