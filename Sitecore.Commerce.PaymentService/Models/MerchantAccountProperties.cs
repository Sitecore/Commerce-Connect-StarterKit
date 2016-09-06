// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MerchantAccountProperties.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the MerchantAccountProperties class.
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
    /// Specifies merchant account property names.
    /// </summary>
    public static class MerchantAccountProperties
    {
        /// <summary>
        /// Gets the Environment property name.
        /// </summary>
        public static string Environment
        {
            get
            {
                return "Environment";
            }
        }

        /// <summary>
        /// Gets the GatewayClientId property name.
        /// </summary>
        public static string GatewayClientId
        {
            get
            {
                return "GatewayClientId";
            }
        }

        /// <summary>
        /// Gets the GatewayMerchantKey property name.
        /// </summary>
        public static string GatewayMerchantKey
        {
            get
            {
                return "GatewayMerchantKey";
            }
        }

        /// <summary>
        /// Gets the GatewayPassword property name.
        /// </summary>
        public static string GatewayPassword
        {
            get
            {
                return "GatewayPassword";
            }
        }

        /// <summary>
        /// Gets the GatewayUrl property name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By Design.")]
        public static string GatewayUrl
        {
            get
            {
                return "GatewayUrl";
            }
        }

        /// <summary>
        /// Gets the GatewayUserId property name.
        /// </summary>
        public static string GatewayUserId
        {
            get
            {
                return "GatewayUserId";
            }
        }

        /// <summary>
        /// Gets the IsAVSRequired property name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By Design.")]
        public static string IsAVSRequired
        {
            get
            {
                return "IsAVSRequired";
            }
        }

        /// <summary>
        /// Gets the MerchantId property name.
        /// </summary>
        public static string MerchantId
        {
            get
            {
                return "MerchantId";
            }
        }

        /// <summary>
        /// Gets the OperatorId property name.
        /// </summary>
        public static string OperatorId
        {
            get
            {
                return "OperatorId";
            }
        }

        /// <summary>
        /// Gets the PaymentAcceptKeyId property name.
        /// </summary>
        public static string PaymentAcceptKeyId
        {
            get
            {
                return "PaymentAcceptKeyId";
            }
        }

        /// <summary>
        /// Gets the PaymentAcceptKeyValue property name.
        /// </summary>
        public static string PaymentAcceptKeyValue
        {
            get
            {
                return "PaymentAcceptKeyValue";
            }
        }

        /// <summary>
        /// Gets the PaymentDeviceKey property name.
        /// </summary>
        public static string PaymentDeviceKey
        {
            get
            {
                return "PaymentDeviceKey";
            }
        }

        /// <summary>
        /// Gets the PaymentDeviceName property name.
        /// </summary>
        public static string PaymentDeviceName
        {
            get
            {
                return "PaymentDeviceName";
            }
        }

        /// <summary>
        /// Gets the PaymentDeviceSerialNumber property name.
        /// </summary>
        public static string PaymentDeviceSerialNumber
        {
            get
            {
                return "PaymentDeviceSerialNumber";
            }
        }

        /// <summary>
        /// Gets the PortableAssemblyName property name.
        /// </summary>
        public static string PortableAssemblyName
        {
            get
            {
                return "PortableAssemblyName";
            }
        }

        /// <summary>
        /// Gets the ProhibitMultiplePayments property name.
        /// </summary>
        public static string ProhibitMultiplePayments
        {
            get
            {
                return "ProhibitMultiplePayments";
            }
        }

        /// <summary>
        /// Gets the ProviderId property name.
        /// </summary>
        public static string ProviderId
        {
            get
            {
                return "ProviderId";
            }
        }

        /// <summary>
        /// Gets the ServiceAccountId property name.
        /// </summary>
        public static string ServiceAccountId
        {
            get
            {
                return "ServiceAccountId";
            }
        }

        /// <summary>
        /// Gets the SupportedCurrencies property name.
        /// </summary>
        public static string SupportedCurrencies
        {
            get
            {
                return "SupportedCurrencies";
            }
        }

        /// <summary>
        /// Gets the SupportedTenderTypes property name.
        /// </summary>
        public static string SupportedTenderTypes
        {
            get
            {
                return "SupportedTenderTypes";
            }
        }
    }
}
