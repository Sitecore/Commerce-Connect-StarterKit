// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefundResponseProperties.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the RefundResponseProperties class.
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
    /// Specifies the names of the refund response properties.
    /// </summary>
    public static class RefundResponseProperties
    {
        /// <summary>
        /// Gets the ApprovalCode property name.
        /// </summary>
        public static string ApprovalCode
        {
            get
            {
                return "ApprovalCode";
            }
        }

        /// <summary>
        /// Gets the ApprovedAmount property name.
        /// </summary>
        public static string ApprovedAmount
        {
            get
            {
                return "ApprovedAmount";
            }
        }

        /// <summary>
        /// Gets the CardToken property name.
        /// </summary>
        public static string CardToken
        {
            get
            {
                return "CardToken";
            }
        }

        /// <summary>
        /// Gets the CardType property name.
        /// </summary>
        public static string CardType
        {
            get
            {
                return "CardType";
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
        /// Gets the ExternalReceipt property name.
        /// </summary>
        public static string ExternalReceipt
        {
            get
            {
                return "ExternalReceipt";
            }
        }

        /// <summary>
        /// Gets the IsCardDataRead property name.
        /// </summary>
        public static string IsCardDataRead
        {
            get
            {
                return "IsCardDataRead";
            }
        }

        /// <summary>
        /// Gets the IsImmediateCapture property name.
        /// </summary>
        public static string IsImmediateCapture
        {
            get
            {
                return "IsImmediateCapture";
            }
        }

        /// <summary>
        /// Gets the Last4Digits property name.
        /// </summary>
        public static string Last4Digits
        {
            get
            {
                return "Last4Digits";
            }
        }

        /// <summary>
        /// Gets the Properties property name.
        /// </summary>
        public static string Properties
        {
            get
            {
                return "Properties";
            }
        }

        /// <summary>
        /// Gets the ProviderMessage property name.
        /// </summary>
        public static string ProviderMessage
        {
            get
            {
                return "ProviderMessage";
            }
        }

        /// <summary>
        /// Gets the ProviderTransactionId property name.
        /// </summary>
        public static string ProviderTransactionId
        {
            get
            {
                return "ProviderTransactionId";
            }
        }

        /// <summary>
        /// Gets the RefundResult property name.
        /// </summary>
        public static string RefundResult
        {
            get
            {
                return "RefundResult";
            }
        }

        /// <summary>
        /// Gets the ResponseCode property name.
        /// </summary>
        public static string ResponseCode
        {
            get
            {
                return "ResponseCode";
            }
        }

        /// <summary>
        /// Gets the TransactionDateTime property name.
        /// </summary>
        public static string TransactionDateTime
        {
            get
            {
                return "TransactionDateTime";
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

        /// <summary>
        /// Gets the UniqueCardId property name.
        /// </summary>
        public static string UniqueCardId
        {
            get
            {
                return "UniqueCardId";
            }
        }
    }
}
