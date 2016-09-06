// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AVSResult.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the AVSResult enum.
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
    /// Specifies the AVS result.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By Design")]
    public enum AVSResult
    {
        /// <summary>
        /// no result.
        /// </summary>
        None = 0,

        /// <summary>
        /// AVS returned.
        /// </summary>
        Returned = 1,

        /// <summary>
        /// AVS not returned.
        /// </summary>
        NotReturned = 2,

        /// <summary>
        /// Verification not supported.
        /// </summary>
        VerificationNotSupported = 3,

        /// <summary>
        /// System unavailable.
        /// </summary>
        SystemUnavailable = 4,
    }
}