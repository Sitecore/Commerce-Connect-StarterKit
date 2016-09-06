// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionDataProperties.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the TransactionDataProperties class.
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
    /// Specifies the transaction data property names.
    /// </summary>
    public static class TransactionDataProperties
    {
        /// <summary>
        /// Gets the AllowPartialAuthorization property name.
        /// </summary>
        public static string AllowPartialAuthorization
        {
            get
            {
                return "AllowPartialAuthorization";
            }
        }

        /// <summary>
        /// Gets the AllowVoiceAuthorization property name.
        /// </summary>
        public static string AllowVoiceAuthorization
        {
            get
            {
                return "AllowVoiceAuthorization";
            }
        }

        /// <summary>
        /// Gets the Amount property name.
        /// </summary>
        public static string Amount
        {
            get
            {
                return "Amount";
            }
        }

        /// <summary>
        /// Gets the AuthProviderTransactionId property name.
        /// </summary>
        public static string AuthorizationProviderTransactionId
        {
            get
            {
                return "AuthProviderTransactionId";
            }
        }

        /// <summary>
        /// Gets the CardNumber property name.
        /// </summary>
        public static string CardNumber
        {
            get
            {
                return "CardNumber";
            }
        }

        /// <summary>
        /// Gets the CurrencyCode property name.
        /// </summary>
        public static string CurrencyCode
        {
            get
            {
                return "CurrencyCode";
            }
        }

        /// <summary>
        /// Gets the Description property name.
        /// </summary>
        public static string Description
        {
            get
            {
                return "Description";
            }
        }

        /// <summary>
        /// Gets the ExternalCustomerId property name.
        /// </summary>
        public static string ExternalCustomerId
        {
            get
            {
                return "ExternalCustomerId";
            }
        }

        /// <summary>
        /// Gets the ExternalInvoiceNumber property name.
        /// </summary>
        public static string ExternalInvoiceNumber
        {
            get
            {
                return "ExternalInvoiceNumber";
            }
        }

        /// <summary>
        /// Gets the ExternalReferenceId property name.
        /// </summary>
        public static string ExternalReferenceId
        {
            get
            {
                return "ExternalReferenceId";
            }
        }

        /// <summary>
        /// Gets the HostPageOrigin property name.
        /// </summary>
        public static string HostPageOrigin
        {
            get
            {
                return "HostPageOrigin";
            }
        }

        /// <summary>
        /// Gets the IndustryType property name.
        /// </summary>
        public static string IndustryType
        {
            get
            {
                return "IndustryType";
            }
        }

        /// <summary>
        /// Gets the IsTestMode property name.
        /// </summary>
        public static string IsTestMode
        {
            get
            {
                return "IsTestMode";
            }
        }

        /// <summary>
        /// Gets the PaymentAcceptResultAccessCode property name.
        /// </summary>
        public static string PaymentAcceptResultAccessCode
        {
            get
            {
                return "PaymentAcceptResultAccessCode";
            }
        }

        /// <summary>
        /// Gets the PaymentAcceptUrl property name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design.")]
        public static string PaymentAcceptUrl
        {
            get
            {
                return "PaymentAcceptUrl";
            }
        }

        /// <summary>
        /// Gets the SupportCardSwipe property name.
        /// </summary>
        public static string PurchaseLevel
        {
            get
            {
                return "SupportCardSwipe";
            }
        }

        /// <summary>
        /// Gets the SupportCardSwipe property name.
        /// </summary>
        public static string SupportCardSwipe
        {
            get
            {
                return "SupportCardSwipe";
            }
        }

        /// <summary>
        /// Gets the SupportCardTokenization property name.
        /// </summary>
        public static string SupportCardTokenization
        {
            get
            {
                return "SupportCardTokenization";
            }
        }

        /// <summary>
        /// Gets the TerminalId property name.
        /// </summary>
        public static string TerminalId
        {
            get
            {
                return "TerminalId";
            }
        }

        /// <summary>
        /// Gets the TransactionType property name.
        /// </summary>
        public static string TransactionType
        {
            get
            {
                return "TransactionType";
            }
        }
    }
}
