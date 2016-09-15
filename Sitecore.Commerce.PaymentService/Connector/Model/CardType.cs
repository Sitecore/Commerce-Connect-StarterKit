// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CardType.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CardType enum.
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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Specifies the card type.
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// Amex card.
        /// </summary>
        Amex = 5,

        /// <summary>
        /// Debit card.
        /// </summary>
        Debit = 1,

        /// <summary>
        /// Discover card.
        /// </summary>
        Discover = 4,

        /// <summary>
        /// Gift card.
        /// </summary>
        Gift = 6,

        /// <summary>
        /// Maestro card.
        /// </summary>
        Maestro = 7,

        /// <summary>
        /// Master card.
        /// </summary>
        MasterCard = 3,

        /// <summary>
        /// Pro100 card.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PRO", Justification = "By Design.")]
        PRO100 = 9,

        /// <summary>
        /// Unknown card.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Visa card.
        /// </summary>
        Visa = 2,

        /// <summary>
        /// Visa elec card.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Elec", Justification = "Should be mapped with 10-symbol table key (RetailTenderTypeCardTable.CardTypeId)")]
        VisaElec = 8
    }
}
