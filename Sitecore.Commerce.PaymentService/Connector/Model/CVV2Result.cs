// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CVV2Result.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CVV2Result enum.
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
    /// <summary>
    /// Specifies a CVV2 result.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CVV", Justification = "By Design.")]
    public enum CVV2Result
    {
        /// <summary>
        /// Unknown result.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Success result.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Failures result.
        /// </summary>
        Failure = 2,

        /// <summary>
        /// Issuer not registered result.
        /// </summary>
        IssuerNotRegistered = 3,

        /// <summary>
        /// Not processed result.
        /// </summary>
        NotProcessed = 4,
    }
}