// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataType.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the DataType enum.
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

    /// <summary>
    /// Specifies a data type.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// String data type.
        /// </summary>
        String,

        /// <summary>
        /// Decimal data type.
        /// </summary>
        Decimal,

        /// <summary>
        /// DateTime data type.
        /// </summary>
        DateTime,

        /// <summary>
        /// Property list data type.
        /// </summary>
        PropertyList
    }
}
