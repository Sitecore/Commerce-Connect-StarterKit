// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateMerchantAccountRequest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ValidateMerchantAccountRequest class.
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

    internal class ValidateMerchantAccountRequest : RequestBase
    {
        internal static ValidateMerchantAccountRequest ConvertFrom(Request request)
        {
            var validateRequest = new ValidateMerchantAccountRequest();
            var errors = new List<PaymentError>();
            validateRequest.ReadBaseProperties(request, errors);

            foreach (PaymentProperty property in request.Properties)
            {
                if (property.Namespace != GenericNamespace.MerchantAccount)
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format(CultureInfo.InvariantCulture, "Invalid namespace {0} for property {1}", property.Namespace, property.Name)));
                }
                else if (!SampleMerchantAccountProperty.ArrayList.Contains(property.Name))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidMerchantProperty, string.Format(CultureInfo.InvariantCulture, "Invalid property {0}", property.Name)));
                }
            }

            if (errors.Count > 0)
            {
                throw new SampleException(errors);
            }

            return validateRequest;
        }
    }
}
