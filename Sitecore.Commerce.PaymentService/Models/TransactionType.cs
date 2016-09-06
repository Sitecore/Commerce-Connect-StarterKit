// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionType.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the TransactionType enum.
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
    /// Specifies a transaction type.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// No transaction type.
        /// </summary>
        None,

        /// <summary>
        /// Authorize transaction type.
        /// </summary>
        Authorize,

        /// <summary>
        /// Reauthorize transaction type.
        /// </summary>
        Reauthorize,

        /// <summary>
        /// Capture transaction type.
        /// </summary>
        Capture,

        /// <summary>
        /// Immediate capture transaction type.
        /// </summary>
        ImmediateCapture,

        /// <summary>
        /// Void transaction type.
        /// </summary>
        Void,

        /// <summary>
        /// Reversal transaction type.
        /// </summary>
        Reversal,

        /// <summary>
        /// Refund transaction type.
        /// </summary>
        Refund
    }
}
