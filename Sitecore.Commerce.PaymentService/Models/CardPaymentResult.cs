// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CardPaymentResult.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CardPaymentResult class.
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
    /// <summary>
    /// The data object that maps to the CardPaymentResult table.
    /// </summary>
    public class CardPaymentResult
    {
        /// <summary>
        /// Gets or sets the service account ID.
        /// </summary>
        public string ServiceAccountId { get; set; }

        /// <summary>
        /// Gets or sets the entry ID of the card payment.
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// Gets or sets the result access code.
        /// </summary>
        public string ResultAccessCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the token has been retrieved.
        /// </summary>
        public bool Retrieved { get; set; }

        /// <summary>
        /// Gets or sets the payment result XML data.
        /// </summary>
        public string ResultData { get; set; }
    }
}