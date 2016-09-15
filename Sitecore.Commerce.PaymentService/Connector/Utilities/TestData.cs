// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestData.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the TestData class.
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
    /// Pre-defined test data for the payment connector.
    /// </summary>
    public static class TestData
    {
        // AVS test values

        /// <summary>
        /// The AVS returned billing name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By design.")]
        public const string AVSReturnedBillingName = "0.01";

        /// <summary>
        /// The AVS returned billing address.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By design.")]
        public const string AVSReturnedBillingAddress = "0.03";

        /// <summary>
        /// The AVS returned billing and postal code.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By design.")]
        public const string AVSReturnedBillingAndPostalCode = "0.05";

        /// <summary>
        /// The AVS returned billing postal code.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By design.")]
        public const string AVSReturnedBillingPostalCode = "0.07";

        /// <summary>
        /// The AVS returned none.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By design.")]
        public const string AVSReturnedNone = "0.09";

        /// <summary>
        /// The AVS not returned.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By design.")]
        public const string AVSNotReturned = "0.11";

        /// <summary>
        /// The AVS none.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By design.")]
        public const string AVSNone = "0.13";

        /// <summary>
        /// The AVS verification not supported.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By design.")]
        public const string AVSVerificationNotSupported = "0.15";

        /// <summary>
        /// The AVS system unavailable.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AVS", Justification = "By design.")]
        public const string AVSSystemUnavailable = "0.17";

        // CVV2 test values

        /// <summary>
        /// The cv v2 failure.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CVV", Justification = "By design.")]
        public const string CVV2Failure = "001";

        /// <summary>
        /// The cv v2 issuer not registered.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CVV", Justification = "By design.")]
        public const string CVV2IssuerNotRegistered = "003";

        /// <summary>
        /// The cv v2 not processed.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CVV", Justification = "By design.")]
        public const string CVV2NotProcessed = "005";

        /// <summary>
        /// The cv v2 unknown.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CVV", Justification = "By design.")]
        public const string CVV2Unknown = "007";

        // Authorization test values

        /// <summary>
        /// The authorization declined.
        /// </summary>
        public const string AuthorizationDeclined = "1.12";

        /// <summary>
        /// The authorization none.
        /// </summary>
        public const string AuthorizationNone = "1.14";

        /// <summary>
        /// The authorization referral.
        /// </summary>
        public const string AuthorizationReferral = "1.16";

        /// <summary>
        /// The authorization partial authorization.
        /// </summary>
        public const string AuthorizationPartialAuthorization = "1.18";

        /// <summary>
        /// The authorization immediate capture required.
        /// </summary>
        public const string AuthorizationImmediateCaptureRequired = "1.20";

        /// <summary>
        /// The authorization success with available balance returned.
        /// </summary>
        public const string AuthorizationAvailableBalance = "1.22";

        // Capture test values

        /// <summary>
        /// The capture failure.
        /// </summary>
        public const string CaptureFailure = "2.12";

        /// <summary>
        /// The capture queued for batch.
        /// </summary>
        public const string CaptureQueuedForBatch = "2.14";

        /// <summary>
        /// The capture none.
        /// </summary>
        public const string CaptureNone = "2.16";

        /// <summary>
        /// The capture failure and void failure.
        /// </summary>
        public const string CaptureFailureVoidFailure = "2.18";

        // Refund test values

        /// <summary>
        /// The refund failure.
        /// </summary>
        public const string RefundFailure = "3.12";

        /// <summary>
        /// The refund queue for batch.
        /// </summary>
        public const string RefundQueueForBatch = "3.14";

        /// <summary>
        /// The refund none.
        /// </summary>
        public const string RefundNone = "3.16";

        // Void test values

        /// <summary>
        /// The void failure.
        /// </summary>
        public const string VoidFailure = "4.12";

        /// <summary>
        /// The void none.
        /// </summary>
        public const string VoidNone = "4.14";

        /// <summary>
        /// The test string.
        /// </summary>
        public const string TestString = "Test string 1234567890 1234567890 End.";

        /// <summary>
        /// The test decimal.
        /// </summary>
        public const decimal TestDecimal = 12345.67M;

        /// <summary>
        /// The supported currencies.
        /// </summary>
        public const string SupportedCurrencies = "AUD;BRL;CAD;CHF;CNY;CZK;DKK;EUR;GBP;HKD;HUF;INR;JPY;KPW;KRW;MXN;NOK;NZD;PLN;SEK;SGD;TWD;USD;ZAR";

        /// <summary>
        /// The supported tender types.
        /// </summary>
        public const string SupportedTenderTypes = "Visa;MasterCard;Amex;Discover;Debit";

        /// <summary>
        /// The merchant identifier.
        /// </summary>
        public static readonly Guid MerchantId = new Guid("136E9C86-31A1-4177-B2B7-A027C63EDBE0");

        /// <summary>
        /// The provider identifier.
        /// </summary>
        public static readonly Guid ProviderId = new Guid("467079B4-1601-4F79-83C9-F569872EB94E");

        /// <summary>
        /// The test date.
        /// </summary>
        public static readonly DateTime TestDate = new DateTime(2011, 9, 22, 11, 3, 0);
    }
}
