// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentUtilities.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the PaymentUtilities class.
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
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Utility methods for payment processing.
    /// </summary>
    internal static class PaymentUtilities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Field is set to MinValue on purpose.")]
        internal const decimal DecimalValueNotPresent = decimal.MinValue;
        internal static readonly DateTime DateTimeValueNotPresent = DateTime.MinValue;

        internal static string GetPropertyStringValue(Dictionary<string, object> properties, string propertyNamespace, string propertyName, List<PaymentError> errors, ErrorCode errorCode)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            string value;
            if (!PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out value))
            {
                value = null;
                if (errors != null)
                {
                    errors.Add(new PaymentError(errorCode, MissingPropertyMessage(propertyNamespace, propertyName)));
                }
            }

            return value;
        }

        internal static string GetPropertyStringValue(Dictionary<string, object> properties, string propertyNamespace, string propertyName)
        {
            return GetPropertyStringValue(properties, propertyNamespace, propertyName, null, ErrorCode.InvalidRequest);
        }

        internal static decimal GetPropertyDecimalValue(Dictionary<string, object> properties, string propertyNamespace, string propertyName, List<PaymentError> errors, ErrorCode errorCode)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            decimal value;
            if (!PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out value))
            {
                value = DecimalValueNotPresent;
                if (errors != null)
                {
                    errors.Add(new PaymentError(errorCode, MissingPropertyMessage(propertyNamespace, propertyName)));
                }
            }

            return value;
        }

        internal static decimal GetPropertyDecimalValue(Dictionary<string, object> properties, string propertyNamespace, string propertyName)
        {
            return GetPropertyDecimalValue(properties, propertyNamespace, propertyName, null, ErrorCode.InvalidRequest);
        }

        internal static bool GetPropertyBooleanValue(Dictionary<string, object> properties, string propertyNamespace, string propertyName, bool defaultValue, List<PaymentError> errors, ErrorCode errorCode)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            bool value;
            if (!PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out value))
            {
                value = defaultValue;
                if (errors != null)
                {
                    errors.Add(new PaymentError(errorCode, MissingPropertyMessage(propertyNamespace, propertyName)));
                }
            }

            return value;
        }

        internal static bool GetPropertyBooleanValue(Dictionary<string, object> properties, string propertyNamespace, string propertyName, bool defaultValue)
        {
            return GetPropertyBooleanValue(properties, propertyNamespace, propertyName, defaultValue, null, ErrorCode.InvalidRequest);
        }

        internal static DateTime GetPropertyDateTimeValue(Dictionary<string, object> properties, string propertyNamespace, string propertyName, List<PaymentError> errors, ErrorCode errorCode)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            DateTime value;
            if (!PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out value))
            {
                value = DateTimeValueNotPresent;
                if (errors != null)
                {
                    errors.Add(new PaymentError(errorCode, MissingPropertyMessage(propertyNamespace, propertyName)));
                }
            }

            return value;
        }

        internal static DateTime GetPropertyDateTimeValue(Dictionary<string, object> properties, string propertyNamespace, string propertyName)
        {
            return GetPropertyDateTimeValue(properties, propertyNamespace, propertyName, null, ErrorCode.InvalidRequest);
        }
        
        internal static PaymentProperty AddPropertyIfPresent(IList<PaymentProperty> properties, string propertyNamespace, string propertyName, string value)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            if (value != null)
            {
                PaymentProperty property = new PaymentProperty(propertyNamespace, propertyName, value);
                properties.Add(property);
                return property;
            }
            else
            {
                return null;
            }
        }

        internal static PaymentProperty AddPropertyIfPresent(IList<PaymentProperty> properties, string propertyNamespace, string propertyName, decimal value)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            if (value != DecimalValueNotPresent)
            {
                PaymentProperty property = new PaymentProperty(propertyNamespace, propertyName, value);
                properties.Add(property);
                return property;
            }
            else
            {
                return null;
            }
        }

        internal static PaymentProperty AddPropertyIfPresent(IList<PaymentProperty> properties, string propertyNamespace, string propertyName, DateTime value)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            if (value != DateTimeValueNotPresent)
            {
                PaymentProperty property = new PaymentProperty(propertyNamespace, propertyName, value);
                properties.Add(property);
                return property;
            }
            else
            {
                return null;
            }
        }

        internal static Response CreateAndLogResponseForReturn(string methodName, string connectorName, string platform, IList<PaymentError> errors)
        {
            return CreateAndLogResponseForReturn(methodName, connectorName, platform, null, null, errors);
        }

        internal static Response CreateAndLogResponseForReturn(string methodName, string connectorName, string platform, string locale, IList<PaymentError> errors)
        {
            return CreateAndLogResponseForReturn(methodName, connectorName, platform, locale, null, errors);
        }

        internal static Response CreateAndLogResponseForReturn(string methodName, string connectorName, string platform, string locale, IList<PaymentProperty> properties, IList<PaymentError> errors)
        {
            var response = new Response();
            response.Locale = locale;

            if (properties != null && properties.Count > 0)
            {
                response.Properties = new PaymentProperty[properties.Count];
                properties.CopyTo(response.Properties, 0);
            }

            if (errors != null && errors.Count > 0)
            {
                response.Errors = new PaymentError[errors.Count];
                errors.CopyTo(response.Errors, 0);
            }

            LogBeforeReturn(methodName, connectorName, platform, response.Errors);
            return response;
        }

        internal static void LogResponseBeforeReturn(string methodName, string connectorName, string platform, Response response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            LogBeforeReturn(methodName, connectorName, platform, response.Errors);
        }

        internal static bool ValidateCardType(string supportedCardTypes, string cardType)
        {
            if (string.IsNullOrWhiteSpace(supportedCardTypes) || string.IsNullOrWhiteSpace(cardType))
            {
                return false;
            }

            string[] cardTypes = supportedCardTypes.Split(';');
            return cardTypes.Any(t => t.Equals(cardType, StringComparison.OrdinalIgnoreCase));
        }

        internal static bool ValidateCurrencyCode(string supportedCurrencies, string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(supportedCurrencies) || string.IsNullOrWhiteSpace(currencyCode))
            {
                return false;
            }

            string[] currencies = supportedCurrencies.Split(';');
            return currencies.Any(c => c.Equals(currencyCode, StringComparison.OrdinalIgnoreCase));
        }

        internal static bool ValidateExpirationDate(decimal expirationYear, decimal expirationMonth)
        {
            DateTime now = DateTime.Now;
            return expirationYear > now.Year
                || (expirationYear == now.Year && expirationMonth >= now.Month);
        }

        internal static string ParseTrack1ForCardNumber(string track1)
        {
            if (string.IsNullOrWhiteSpace(track1))
            {
                return null;
            }

            // Normally track 1 starts with %B but some hardware just send B as the start sentinel
            string symbolBeforeCardNumber = "B";
            string symbolAfterCardNumber = "^";
            int index1 = track1.IndexOf(symbolBeforeCardNumber, StringComparison.OrdinalIgnoreCase);
            int index2 = track1.IndexOf(symbolAfterCardNumber, StringComparison.OrdinalIgnoreCase);

            if (index1 < 0 || index2 < 0)
            {
                return null;
            }

            int begin = index1 + symbolBeforeCardNumber.Length;
            int end = index2 - 1;

            if (begin > end)
            {
                return null;
            }
            else
            {
                return track1.Substring(begin, end - begin + 1);
            }
        }

        internal static string ParseTrack2ForCardNumber(string track2)
        {
            if (string.IsNullOrWhiteSpace(track2))
            {
                return null;
            }

            string symbolBeforeCardNumber = ";";
            string symbolAfterCardNumber = "=";
            int index1 = track2.IndexOf(symbolBeforeCardNumber, StringComparison.OrdinalIgnoreCase);
            int index2 = track2.IndexOf(symbolAfterCardNumber, StringComparison.OrdinalIgnoreCase);

            // Normally track 2 starts with ; but some hardware remove the start sentinel, so ignore not finding it
            if (index2 < 0)
            {
                return null;
            }

            int begin = index1 + symbolBeforeCardNumber.Length;
            int end = index2 - 1;

            if (begin > end)
            {
                return null;
            }
            else
            {
                return track2.Substring(begin, end - begin + 1);
            }
        }

        private static void LogBeforeReturn(string methodName, string connectorName, string platform, IList<PaymentError> errors)
        {
            // TODO: currently not implemented. Add your custom logging code here.
        }

        private static string MissingPropertyMessage(string propertyNamespace, string propertyName)
        {
            return string.Format(CultureInfo.InvariantCulture, "Missing {0}:{1}.", propertyNamespace, propertyName);
        }
    }
}
