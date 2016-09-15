// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetShippingOptions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the GetShippingOptions class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Shipping.GetShippingOptions
{
  using System.Collections.ObjectModel;
  using System.Xml;
  using Commerce.Entities.Shipping;
  using Commerce.Pipelines;
  using Commerce.Services.Shipping;
  using Common;
  using Diagnostics;
  using NopShippingService;
  using Xml;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// Defines the pipeline processor that gets shipping options.
  /// </summary>
  public class GetShippingOptions : NopProcessor<IShippingServiceChannel>
  {
    /// <summary>
    /// The shipping option list.
    /// </summary>
    protected readonly Dictionary<int, ShippingOption> ShippingOptionList;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetShippingOptions"/> class.
    /// </summary>
    public GetShippingOptions()
    {
      this.ShippingOptionList = new Dictionary<int, ShippingOption>(0); 
    }

    /// <summary>
    /// Processes the specified args.
    /// </summary>
    /// <param name="args">The args.</param>
    public override void Process(ServicePipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      var result = (GetShippingOptionsResult)args.Result;

      result.ShippingOptions = new ReadOnlyCollection<ShippingOption>(this.ShippingOptionList.Values.ToList());
    }

    /// <summary>
    /// Add shipping optin by configuration setting
    /// </summary>
    /// <param name="configNode">The configuration node.</param>
    public virtual void AddShippingOption(XmlNode configNode)
    {
      Assert.ArgumentNotNull(configNode, "configNode");

      string value = XmlUtil.GetAttribute("value", configNode);

      Assert.IsNotNullOrEmpty(value, "value");

      int key = MainUtil.GetInt(value, 0);

      if (!this.ShippingOptionList.ContainsKey(key))
      {
        var shippingOption = new ShippingOption();

        switch (key)
        {
          case 1:
            shippingOption.ShippingOptionType = ShippingOptionType.ShipToAddress;
            shippingOption.Name = Globalization.Translate.Text(Texts.ShipToNewAddress);
            shippingOption.Description = Globalization.Translate.Text(Texts.ShipToNewAddress);
            break;
          case 2:
            shippingOption.ShippingOptionType = ShippingOptionType.PickupFromStore;
            shippingOption.Name = Globalization.Translate.Text(Texts.PickupFromStore);
            shippingOption.Description = Globalization.Translate.Text(Texts.PickupFromStore);
            break;
          case 3:
            shippingOption.ShippingOptionType = ShippingOptionType.ElectronicDelivery;
            shippingOption.Name = Globalization.Translate.Text(Texts.EmailDelivery);
            shippingOption.Description = Globalization.Translate.Text(Texts.EmailDelivery);
            break;
          default:
            shippingOption.ShippingOptionType = ShippingOptionType.None;
            shippingOption.Name = Globalization.Translate.Text(Texts.NoDeliveryPreference);
            shippingOption.Description = Globalization.Translate.Text(Texts.NoDeliveryPreference);
            break;
        }

        this.ShippingOptionList[key] = shippingOption;
      }
    }
  }
}
