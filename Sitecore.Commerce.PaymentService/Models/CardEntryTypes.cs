// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CardEntryTypes.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CardEntryTypes enum.
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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Specifies a card entry type.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Justification = "Required for backwards compatibility.")]
    public enum CardEntryTypes
    {
        /// <summary>
        /// No type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Magnetic strip card read.
        /// </summary>
        MagneticStripeRead = 1,

        /// <summary>
        /// Manually entered card number.
        /// </summary>
        ManuallyEntered = 2,

        /// <summary>
        /// Chip entry card read.
        /// </summary>
        ChipEntry = 3,
    }
}