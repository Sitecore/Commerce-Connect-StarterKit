// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorCode.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ErrorCode enum.
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
    /// Specifies an error code.
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// No error.
        /// </summary>
        None = 0,

        /// <summary>
        /// Invalid operation error.
        /// </summary>
        InvalidOperation = 20001,

        /// <summary>
        /// Applicaton error.
        /// </summary>
        ApplicationError = 20002,

        /// <summary>
        /// Generic check details for error.
        /// </summary>
        GenericCheckDetailsForError = 20003,

        /// <summary>
        /// Not authorized error.
        /// </summary>
        DONotAuthorized = 20004,

        /// <summary>
        /// User aborted error.
        /// </summary>
        UserAborted = 20005,

        /// <summary>
        /// Invalid argument - tender account number error.
        /// </summary>
        InvalidArgumentTenderAccountNumber = 20119,

        /// <summary>
        /// Locale not supported error.
        /// </summary>
        LocaleNotSupported = 21001,

        /// <summary>
        /// Invalid merchant property error.
        /// </summary>
        InvalidMerchantProperty = 21002,

        /// <summary>
        /// Communication error.
        /// </summary>
        CommunicationError = 22001,

        /// <summary>
        /// Invalid argument - card type not supported error.
        /// </summary>
        InvalidArgumentCardTypeNotSupported = 22010,

        /// <summary>
        /// Voice authorization not supported error.
        /// </summary>
        VoiceAuthorizationNotSupported = 22011,

        /// <summary>
        /// Reauthorization not supported error.
        /// </summary>
        ReauthorizationNotSupported = 22012,

        /// <summary>
        /// Multiple capture not supported error.
        /// </summary>
        MultipleCaptureNotSupported = 22013,

        /// <summary>
        /// Batch capture not supported error.
        /// </summary>
        BatchCaptureNotSupported = 22014,

        /// <summary>
        /// Unsupported currency error.
        /// </summary>
        UnsupportedCurrency = 22015,

        /// <summary>
        /// Unsupported country error.
        /// </summary>
        UnsupportedCountry = 22016,

        /// <summary>
        /// Cannot reauthorize post capture error.
        /// </summary>
        CannotReauthorizePostCapture = 22017,

        /// <summary>
        /// Cannot reauthorize post void error.
        /// </summary>
        CannotReauthorizePostVoid = 22018,

        /// <summary>
        /// Immediate capture not supported error.
        /// </summary>
        ImmediateCaptureNotSupported = 22019,

        /// <summary>
        /// Card expired error.
        /// </summary>
        CardExpired = 22050,

        /// <summary>
        /// Refer to issuer error.
        /// </summary>
        ReferToIssuer = 22051,

        /// <summary>
        /// No reply error.
        /// </summary>
        NoReply = 22052,

        /// <summary>
        /// Hold call or pickup card error.
        /// </summary>
        HoldCallOrPickupCard = 22053,

        /// <summary>
        /// Invalid amount error.
        /// </summary>
        InvalidAmount = 22054,

        /// <summary>
        /// Account length error.
        /// </summary>
        AccountLengthError = 22055,

        /// <summary>
        /// Already reserved error.
        /// </summary>
        AlreadyReversed = 22056,

        /// <summary>
        /// Cannot verify pin error.
        /// </summary>
        CannotVerifyPin = 22057,

        /// <summary>
        /// Invalid card number error.
        /// </summary>
        InvalidCardNumber = 22058,

        /// <summary>
        /// Invallid CVV2 error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CVV", Justification = "By Design.")]
        InvalidCVV2 = 22059,

        /// <summary>
        /// Cash back not available error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CashBack", Justification = "By Design.")]
        CashBackNotAvailable = 22060,

        /// <summary>
        /// Card type verification error.
        /// </summary>
        CardTypeVerificationError = 22061,

        /// <summary>
        /// Declined error.
        /// </summary>
        Decline = 22062,

        /// <summary>
        /// Encryption error.
        /// </summary>
        EncryptionError = 22063,

        /// <summary>
        /// No action taken error.
        /// </summary>
        NoActionTaken = 22065,

        /// <summary>
        /// No such issuer error.
        /// </summary>
        NoSuchIssuer = 22066,

        /// <summary>
        /// Pin tries exceeded error.
        /// </summary>
        PinTriesExceeded = 22067,

        /// <summary>
        /// Security violation error.
        /// </summary>
        SecurityViolation = 22068,

        /// <summary>
        /// Service not allowed error.
        /// </summary>
        ServiceNotAllowed = 22069,

        /// <summary>
        /// Stop Recurring error.
        /// </summary>
        StopRecurring = 22070,

        /// <summary>
        /// Wrong PIN error.
        /// </summary>
        WrongPin = 22071,

        /// <summary>
        /// CVV2 mismatch error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CVV", Justification = "By design.")]
        CVV2Mismatch = 22072,

        /// <summary>
        /// Duplicate transaction error.
        /// </summary>
        DuplicateTransaction = 22073,

        /// <summary>
        /// Reenter error.
        /// </summary>
        Reenter = 22074,

        /// <summary>
        /// Amount exceeded limit error.
        /// </summary>
        AmountExceedLimit = 22075,

        /// <summary>
        /// Authorization expired error.
        /// </summary>
        AuthorizationExpired = 22076,

        /// <summary>
        /// Authorization already completed error.
        /// </summary>
        AuthorizationAlreadyCompleted = 22077,

        /// <summary>
        /// Authorization is voided error.
        /// </summary>
        AuthorizationIsVoided = 22078,

        /// <summary>
        /// Processor duplicate batch error.
        /// </summary>
        ProcessorDuplicateBatch = 22090,

        /// <summary>
        /// Authorization failure error.
        /// </summary>
        AuthorizationFailure = 22100,

        /// <summary>
        /// Invalid merchant configuration error.
        /// </summary>
        InvalidMerchantConfiguration = 22102,

        /// <summary>
        /// Invalid expiration date error.
        /// </summary>
        InvalidExpirationDate = 22103,

        /// <summary>
        /// Invalid cardholder name - first name required error.
        /// </summary>
        InvalidCardholderNameFirstNameRequired = 22104,

        /// <summary>
        /// Invalid cardholder name - last name required error.
        /// </summary>
        InvalidCardholderNameLastNameRequired = 22105,

        /// <summary>
        /// Filter decline error.
        /// </summary>
        FilterDecline = 22106,

        /// <summary>
        /// Invalid address error.
        /// </summary>
        InvalidAddress = 22107,

        /// <summary>
        /// CVV2 required error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CVV", Justification = "By Design.")]
        CVV2Required = 22108,

        /// <summary>
        /// Card type not supported error.
        /// </summary>
        CardTypeNotSupported = 22109,

        /// <summary>
        /// Unique invoice number required error.
        /// </summary>
        UniqueInvoiceNumberRequired = 22110,

        /// <summary>
        /// Possible duplicate error.
        /// </summary>
        PossibleDuplicate = 22111,

        /// <summary>
        /// Processor requires linked refund error.
        /// </summary>
        ProcessorRequiresLinkedRefund = 22112,

        /// <summary>
        /// Crypto box unavailable error.
        /// </summary>
        CryptoBoxUnavailable = 22113,

        /// <summary>
        /// CVV2 declined error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CVV", Justification = "By Design.")]
        CVV2Declined = 22114,

        /// <summary>
        /// Merchant ID invalid error.
        /// </summary>
        MerchantIdInvalid = 22115,

        /// <summary>
        /// Tran not allowed error.
        /// </summary>
        TranNotAllowed = 22116,

        /// <summary>
        /// Terminal not found error.
        /// </summary>
        TerminalNotFound = 22117,

        /// <summary>
        /// Invalid effective date error.
        /// </summary>
        InvalidEffectiveDate = 22118,

        /// <summary>
        /// Insufficient funds error.
        /// </summary>
        InsufficientFunds = 22119,

        /// <summary>
        /// Reauthorization max reached error.
        /// </summary>
        ReauthorizationMaxReached = 22120,

        /// <summary>
        /// Reauthorization not allowed error.
        /// </summary>
        ReauthorizationNotAllowed = 22121,

        /// <summary>
        /// Date of birth error.
        /// </summary>
        DateOfBirthError = 22122,

        /// <summary>
        /// Enter leser amount error
        /// </summary>
        EnterLesserAmount = 22123,

        /// <summary>
        /// Host key error.
        /// </summary>
        HostKeyError = 22124,

        /// <summary>
        /// Invalid cash back amount error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CashBack", Justification = "By Design.")]
        InvalidCashBackAmount = 22125,

        /// <summary>
        /// Invalid transaction error.
        /// </summary>
        InvalidTransaction = 22126,

        /// <summary>
        /// Immediate capture required error.
        /// </summary>
        ImmediateCaptureRequired = 22127,

        /// <summary>
        /// Immediate capture required MAC error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MAC", Justification = "By Design.")]
        ImmediateCaptureRequiredMAC = 22128,

        /// <summary>
        /// MAC required error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MAC", Justification = "By Design.")]
        MACRequired = 22129,

        /// <summary>
        /// Bank card not set error.
        /// </summary>
        BankcardNotSet = 22130,

        /// <summary>
        /// Invalid request error.
        /// </summary>
        InvalidRequest = 22131,

        /// <summary>
        /// Invalid transaction fee error.
        /// </summary>
        InvalidTransactionFee = 22132,

        /// <summary>
        /// No checking account error.
        /// </summary>
        NoCheckingAccount = 22133,

        /// <summary>
        /// No savings account error.
        /// </summary>
        NoSavingsAccount = 22134,

        /// <summary>
        /// Restricted card temporarily disallowed from interchange error.
        /// </summary>
        RestrictedCardTemporarilyDisallowedFromInterchange = 22135,

        /// <summary>
        /// MAC security failure error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MAC", Justification = "By Design.")]
        MACSecurityFailure = 22136,

        /// <summary>
        /// Exceeds withdrawal frequency limit error.
        /// </summary>
        ExceedsWithdrawalFrequencyLimit = 22137,

        /// <summary>
        /// Invalid capture date error.
        /// </summary>
        InvalidCaptureDate = 22138,

        /// <summary>
        /// No keys available error.
        /// </summary>
        NoKeysAvailable = 22139,

        /// <summary>
        /// KME sync error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "KME", Justification = "By Design.")]
        KMESyncError = 22140,

        /// <summary>
        /// KPE sync error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "KPE", Justification = "By Design.")]
        KPESyncError = 22141,

        /// <summary>
        /// KMAC sync error.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "KMAC", Justification = "By Design.")]
        KMACSyncError = 22142,

        /// <summary>
        /// Resubmit exceeds limit error.
        /// </summary>
        ResubmitExceedsLimit = 22143,

        /// <summary>
        /// System problem error.
        /// </summary>
        SystemProblemError = 22144,

        /// <summary>
        /// Account number not found for row error.
        /// </summary>
        AccountNumberNotFoundForRow = 22145,

        /// <summary>
        /// Invalid token info parameter for row error.
        /// </summary>
        InvalidTokenInfoParameterForRow = 22146,

        /// <summary>
        /// Exception thrown for row error.
        /// </summary>
        ExceptionThrownForRow = 22147,

        /// <summary>
        /// Transaction amount exceeds remaining error.
        /// </summary>
        TransactionAmountExceedsRemaining = 22148,

        /// <summary>
        /// General exception error.
        /// </summary>
        GeneralException = 22149,

        /// <summary>
        /// Invalid card track data error.
        /// </summary>
        InvalidCardTrackData = 22150,

        /// <summary>
        /// Invalid result access code error.
        /// </summary>
        InvalidResultAccessCode = 22151,
    }
}
