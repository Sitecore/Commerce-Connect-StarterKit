// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CardPaymentException.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CardPaymentException class.
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
    using System.Collections.Generic;

    /// <summary>
    /// Exception class for card payment errors.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "By Design.  Sample only.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Do not need serialization.")]
    public sealed class CardPaymentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardPaymentException" /> class.
        /// </summary>
        public CardPaymentException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardPaymentException" /> class.
        /// </summary>
        /// <param name="message">The message describing the error that occurred.</param>
        public CardPaymentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardPaymentException" /> class.
        /// </summary>
        /// <param name="message">The message describing the error that occurred.</param>
        /// <param name="paymentErrors">The payment errors causing the error.</param>
        public CardPaymentException(string message, IEnumerable<PaymentError> paymentErrors)
            : base(message)
        {
            this.PaymentErrors = paymentErrors;
        }

        /// <summary>
        /// Gets or sets payment errors.
        /// </summary>
        public IEnumerable<PaymentError> PaymentErrors { get; set; }
    }
}