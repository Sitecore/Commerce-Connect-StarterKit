// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AVSDetail.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the AVSDetail enum.
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

namespace Sitecore.Commerce.PaymentService.Connector
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Specifies the AVS details
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By Design.")]
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Backward compatibility.")]
    public enum AVSDetail
    {
        /// <summary>
        /// No details.
        /// </summary>
        None = 5,

        /// <summary>
        /// Specifies the account hloder name.
        /// </summary>
        AccountholderName = 6,

        /// <summary>
        /// Specifies the billing address.
        /// </summary>
        BillingAddress = 7,

        /// <summary>
        /// Specifies the billing postal code.
        /// </summary>
        BillingPostalCode = 8,

        /// <summary>
        /// Specifies the billing and postal code.
        /// </summary>
        BillingAndPostalCode = 9,
    }
}