// ----------------------------------------------------------------------------------------------
// <copyright file="Texts.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   List of general strings
// </summary>
// ----------------------------------------------------------------------------------------------
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
namespace Sitecore.Commerce.Connectors.NopCommerce
{
  using Globalization;

  [LocalizationTexts(ModuleName = "Sitecore.Commerce.Connectors.NopCommerce")]
  public class Texts
  {
    /// <summary>
    /// Defines the message: "Ship items"
    /// </summary>
    public const string NoDeliveryPreference = "No delivery preference";

    /// <summary>
    /// Defines the message: "Ship items"
    /// </summary>
    public const string ShipToNewAddress = "Ship items";

    /// <summary>
    /// Defines the message: "Pick up items in store"
    /// </summary>
    public const string PickupFromStore = "Pick up items in store";

    /// <summary>
    /// Defines the message: "Select delivery options by item"
    /// </summary>
    public const string ShipItemsIndividually = "Select delivery options by item";

    /// <summary>
    /// Defines the message: "Email"
    /// </summary>
    public const string EmailDelivery = "Email";

    /// <summary>
    /// Defines the message: "Pay card"
    /// </summary>
    public const string PayCard = "Pay card";

    /// <summary>
    /// Defines the message: "Pay loyalty card"
    /// </summary>
    public const string PayLoyaltyCard = "Pay loyalty card";

    /// <summary>
    /// Defines the message: "Pay gift card"
    /// </summary>
    public const string PayGiftCard = "Pay gift card";

    /// <summary>
    /// Defines the message: "Online payment"
    /// </summary>
    public const string OnlinePayment = "Online payment";

    /// <summary>
    /// Defines the message: "No payment preference"
    /// </summary>
    public const string NoPaymentPreference = "No payment preference";
  }
}