// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectorProperties.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ConnectorProperties class.
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

    /// <summary>
    /// Specifies the name of connector properties.
    /// </summary>
    public static class ConnectorProperties
    {
        /// <summary>
        /// Gets the CardKey property name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CardKey", Justification = "By Design.")]
        public static string CardKey
        {
            get
            {
                return "CardKey";
            }
        }

        /// <summary>
        /// Gets the ConnectorName property name.
        /// </summary>
        public static string ConnectorName
        {
            get
            {
                return "ConnectorName";
            }
        }
    }
}
