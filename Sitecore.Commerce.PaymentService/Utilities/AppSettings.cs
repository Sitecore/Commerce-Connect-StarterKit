// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the AppSettings class.
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
    using System.Configuration;
    using System.Globalization;

    /// <summary>
    /// The application settings from Web.config file.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Gets the connector assembly name.
        /// </summary>
        public static string ConnectorAssembly
        {
            get
            {
                return GetSetting<string>("ConnectorAssembly");
            }
        }

        /// <summary>
        /// Gets the connector name.
        /// </summary>
        public static string ConnectorName
        {
            get
            {
                return GetSetting<string>("ConnectorName");
            }
        }

        /// <summary>
        /// Gets the valid period of the payment entry (in minutes).
        /// </summary>
        public static int PaymentEntryValidPeriodInMinutes
        {
            get
            {
                return GetSetting<int>("PaymentEntryValidPeriodInMinutes");
            }
        }

        /// <summary>
        /// Gets the payment accept base address for configurable environments.
        /// </summary>
        public static string PaymentAcceptBaseAddress
        {
            get
            {
                return GetSetting<string>("PaymentAcceptBaseAddress");
            }
        }

        private static T GetSetting<T>(string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Could not find setting '{0}' in the configuration file.", name);
                throw new ArgumentException(name, message);
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}