// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Request.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the Request class.
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
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a request.
    /// </summary>
    [DataContract]
    public class Request : IExtensibleDataObject
    {
        private PaymentProperty[] properties;

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Gets or sets the request locale.
        /// </summary>
        [DataMember]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the request properties.
        /// </summary>
        [DataMember, SuppressMessage("Microsoft.Performance", "CA1819", Justification = "Allow AX access to these objects")]
        public PaymentProperty[] Properties
        {
            get
            {
                return this.properties;
            }

            set
            {
                this.properties = value;
            }
        }

        /// <summary>
        /// Sets the request properties.
        /// </summary>
        /// <param name="value">The property array.</param>
        public void SetProperties(object[] value)
        {
            List<PaymentProperty> list = new List<PaymentProperty>();
            if (value != null)
            {
                object[] objArray = value;
                for (int i = 0; i < objArray.Length; i++)
                {
                    PaymentProperty property = objArray[i] as PaymentProperty;
                    if (property != null)
                    {
                        list.Add(property);
                    }
                }
            }

            this.properties = list.ToArray();
        }
    }
}
