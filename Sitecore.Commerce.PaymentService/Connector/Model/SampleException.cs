// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleException.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the SampleException class.
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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Sample exception class for input errors and payment errors.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "By design - sample only.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "By design - sample only.")]
    public class SampleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleException"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        public SampleException(ErrorCode code, string message)
        {
            this.Errors = new List<PaymentError>();
            this.Errors.Add(new PaymentError(code, message));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleException"/> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public SampleException(List<PaymentError> errors)
        {
            this.Errors = errors;
        }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "By design.")]
        public List<PaymentError> Errors { get; set; }
    }
}
