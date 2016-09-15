// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestBase.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the RequestBase class.
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
    using System.Collections.Generic;

    internal abstract class RequestBase
    {
        internal RequestBase()
            : base()
        {
        }

        internal string Locale { get; set; }

        internal string ServiceAccountId { get; set; }

        internal string MerchantId { get; set; }

        internal string ProviderId { get; set; }

        internal string Environment { get; set; }

        internal string SupportedCurrencies { get; set; }

        internal string SupportedTenderTypes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal string IndustryType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By Design.")]
        internal bool IsTestMode { get; set; }

        protected void ReadBaseProperties(Request request, List<PaymentError> errors)
        {
            if (request == null)
            {
                throw new SampleException(ErrorCode.InvalidRequest, "Request is null.");
            }

            if (string.IsNullOrWhiteSpace(request.Locale))
            {
                throw new SampleException(ErrorCode.InvalidRequest, "Locale is null or whitespaces.");
            }
            else
            {
                this.Locale = request.Locale;
            }
            
            if (request.Properties == null || request.Properties.Length == 0)
            {
                throw new SampleException(ErrorCode.InvalidRequest, "Request properties is null or empty.");
            }

            var properties = PaymentProperty.ConvertToHashtable(request.Properties);
            this.ServiceAccountId = PaymentUtilities.GetPropertyStringValue(
                properties,
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.ServiceAccountId,
                errors,
                ErrorCode.InvalidMerchantProperty);
            this.MerchantId = PaymentUtilities.GetPropertyStringValue(
                properties,
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.MerchantId,
                errors,
                ErrorCode.InvalidMerchantProperty);
            this.ProviderId = PaymentUtilities.GetPropertyStringValue(
                properties,
                GenericNamespace.MerchantAccount,
                SampleMerchantAccountProperty.ProviderId,
                errors,
                ErrorCode.InvalidMerchantProperty);
            this.Environment = PaymentUtilities.GetPropertyStringValue(
                properties,
                GenericNamespace.MerchantAccount,
                SampleMerchantAccountProperty.Environment);
            this.SupportedCurrencies = PaymentUtilities.GetPropertyStringValue(
                properties,
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.SupportedCurrencies,
                errors,
                ErrorCode.InvalidMerchantProperty);
            this.SupportedTenderTypes = PaymentUtilities.GetPropertyStringValue(
                properties,
                GenericNamespace.MerchantAccount,
                MerchantAccountProperties.SupportedTenderTypes,
                errors,
                ErrorCode.InvalidMerchantProperty);
            this.IndustryType = PaymentUtilities.GetPropertyStringValue(
                properties,
                GenericNamespace.TransactionData,
                TransactionDataProperties.IndustryType);
            this.IsTestMode = PaymentUtilities.GetPropertyBooleanValue(
                properties,
                GenericNamespace.TransactionData,
                TransactionDataProperties.IsTestMode,
                false);
        }
    }
}
