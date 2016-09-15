// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonUtility.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CommonUtility class.
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
    using System.Security.Cryptography;

    /// <summary>
    /// Common utilities.
    /// </summary>
    public static class CommonUtility
    {
        /// <summary>
        /// Creates a secure <c>Guid</c> which is harder to predict.
        /// </summary>
        /// <returns>The <c>Guid</c>.</returns>
        public static Guid NewGuid()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[16];
            rng.GetBytes(randomBytes);
            return new Guid(randomBytes);
        }
    }
}