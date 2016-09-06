// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateCardTokenResponse.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GenerateCardTokenResponse class.
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
    using System.Collections.Generic;

    internal class GenerateCardTokenResponse : ResponseBase
    {
        internal GenerateCardTokenResponse(string locale, string serviceAccountId, string connectorName)
            : base(locale, serviceAccountId, connectorName)
        {
            this.ExpirationYear = PaymentUtilities.DecimalValueNotPresent;
            this.ExpirationMonth = PaymentUtilities.DecimalValueNotPresent;
        }

        internal string CardType { get; set; }

        internal string Last4Digit { get; set; }

        internal string CardToken { get; set; }

        internal string UniqueCardId { get; set; }

        internal decimal ExpirationYear { get; set; }

        internal decimal ExpirationMonth { get; set; }

        internal string Name { get; set; }

        internal string StreetAddress { get; set; }

        internal string StreetAddress2 { get; set; }

        internal string City { get; set; }

        internal string State { get; set; }

        internal string PostalCode { get; set; }

        internal string Country { get; set; }

        internal string Phone { get; set; }

        internal string AccountType { get; set; }

        internal IList<PaymentProperty> OtherCardProperties { get; set; }

        internal static Response ConvertTo(GenerateCardTokenResponse tokenizeResponse)
        {
            var response = new Response();
            tokenizeResponse.WriteBaseProperties(response);

            var properties = new List<PaymentProperty>();
            if (response.Properties != null)
            {
                properties.AddRange(response.Properties);
            }

            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.CardType, tokenizeResponse.CardType);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.Last4Digits, tokenizeResponse.Last4Digit);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.CardToken, tokenizeResponse.CardToken);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.UniqueCardId, tokenizeResponse.UniqueCardId);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.ExpirationYear, tokenizeResponse.ExpirationYear);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.ExpirationMonth, tokenizeResponse.ExpirationMonth);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.Name, tokenizeResponse.Name);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.StreetAddress, tokenizeResponse.StreetAddress);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.StreetAddress2, tokenizeResponse.StreetAddress2);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.City, tokenizeResponse.City);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.State, tokenizeResponse.State);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.PostalCode, tokenizeResponse.PostalCode);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.Country, tokenizeResponse.Country);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.Phone, tokenizeResponse.Phone);
            PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.AccountType, tokenizeResponse.AccountType);
            properties.AddRange(tokenizeResponse.OtherCardProperties);

            response.Properties = properties.ToArray();
            return response;
        }
    }
}
