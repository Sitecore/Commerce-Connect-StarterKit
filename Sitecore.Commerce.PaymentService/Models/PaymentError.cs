// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentError.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the PaymentError class.
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
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Defines a payment error.
    /// </summary>
    public class PaymentError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentError"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        public PaymentError(ErrorCode code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public ErrorCode Code { get; protected set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// Converts a list of errors into a trace string.
        /// </summary>
        /// <param name="errors">The list of errors.</param>
        /// <returns>A trace string.</returns>
        public static string GetErrorsAsTraceString(IList<PaymentError> errors)
        {
            if (errors == null)
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();
            foreach (PaymentError error in errors)
            {
                object[] objArray1 = new object[] { error.Code, error.Message };
                builder.AppendFormat(CultureInfo.InvariantCulture, "code: {0} message: {1}\n", (object[])objArray1);
            }

            return builder.ToString();
        }
    }
}
