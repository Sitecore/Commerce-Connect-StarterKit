// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelperUtilities.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the HelperUtilities class.
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
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines helper utilities.
    /// </summary>
    public static class HelperUtilities
    {
        /// <summary>
        /// Parses the track data for expiration date.
        /// </summary>
        /// <param name="track1Data">The track data.</param>
        /// <param name="track2Data">The second track data.</param>
        /// <param name="expirationYear">Recieves the expiration year.</param>
        /// <param name="expirationMonth">Recieves the expiration month.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Decimal.TryParse(System.String,System.Decimal@)", Justification = "By Design.")]
        public static void ParseTrackDataForExpirationDate(string track1Data, string track2Data, out decimal expirationYear, out decimal expirationMonth)
        {
            Match match = new Regex("^;?(?<PrimaryAccountNumber>[0-9]{1,19})=((?<ExpiryDate>[0-9]{4})|((?<ExpiryDate>)=)).*$").Match(track2Data);
            Group group = new Regex(@"^%?(?<FC>[A-Z]{1})(?<PrimaryAccountNumber>[0-9]{1,19})\^(?<Name>[^\^]{2,26})\^((?<ExpiryDate>[0-9]{4})|((?<ExpiryDate>)\^)).*$").Match(track1Data).Groups["ExpiryDate"];
            Group group2 = match.Groups["ExpiryDate"];
            if (group.Success && (group.Length >= 4))
            {
                decimal.TryParse(group.Value.Substring(0, 2), out expirationYear);
            }
            else if (group2.Success && (group2.Length >= 4))
            {
                decimal.TryParse(group2.Value.Substring(0, 2), out expirationYear);
            }
            else
            {
                expirationYear = new decimal();
            }

            expirationYear += 2000M;
            if (group.Success && (group.Length >= 4))
            {
                decimal.TryParse(group.Value.Substring(2, 2), out expirationMonth);
            }
            else if (group2.Success && (group2.Length >= 4))
            {
                decimal.TryParse(group2.Value.Substring(2, 2), out expirationMonth);
            }
            else
            {
                expirationMonth = new decimal();
            }
        }

        /// <summary>
        /// Validates a bank card number.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <returns>True if the card number is valid, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BankCard", Justification = "By Design.")]
        public static bool ValidateBankCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
            {
                return false;
            }

            string number = cardNumber.Replace("-", string.Empty);
            int length = number.Length;
            return (((length >= 13) && (length <= 0x10)) && CheckMod10(number));
        }

        private static bool CheckMod10(string number)
        {
            int num = 0;
            bool flag = false;
            int num2 = number.Length - 1;
            while (num2 >= 0)
            {
                int num3 = number[num2] - '0';
                if ((num3 < 0) || (num3 > 9))
                {
                    return false;
                }

                if (flag)
                {
                    num3 *= 2;
                    if (num3 > 9)
                    {
                        num3 -= 9;
                    }
                }
                
                num += num3;
                num2--;
                flag = !flag;
            }

            return ((num % 10) == 0);
        }
    }
}
