// ----------------------------------------------------------------------------------------------
// <copyright file="HttpUserAgentBehaviorExtensionElement.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The HttpUserAgentBehaviorExtensionElement.
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
using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using Sitecore.Commerce.Connectors.NopCommerce.ServiceModel.Description;

namespace Sitecore.Commerce.Connectors.NopCommerce.ServiceModel.Configuration
{
    public class HttpUserAgentBehaviorExtensionElement : BehaviorExtensionElement
  {

    private ConfigurationPropertyCollection properties;

    /// <summary>
    /// Gets the type of behavior.
    /// </summary>
    /// <returns>The type of behavior.</returns>
    public override Type BehaviorType
    {
      get
      {
        return typeof(HttpUserAgentEndpointBehavior);
      }
    }

    /// <summary>
    /// Creates a behavior extension based on the current configuration settings.
    /// </summary>
    /// <returns>
    /// The behavior extension.
    /// </returns>
    protected override object CreateBehavior()
    {
      return new HttpUserAgentEndpointBehavior(this.UserAgent);
    }

    /// <summary>
    /// Gets the collection of properties.
    /// </summary>
    /// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> of properties for the element.</returns>
    protected override ConfigurationPropertyCollection Properties
    {
      get
      {
        return this.properties
               ?? (this.properties =
                   new ConfigurationPropertyCollection
                     {
                       new ConfigurationProperty(
                         "userAgent",
                         typeof(string),
                         "",
                         ConfigurationPropertyOptions.IsRequired)
                     });
      }
    }

    /// <summary>
    /// Copies the content of the specified configuration element to this configuration element.
    /// </summary>
    /// <param name="from">The configuration element to be copied.</param>
    public override void CopyFrom(ServiceModelExtensionElement from)
    {
      base.CopyFrom(from);
      var element = (HttpUserAgentBehaviorExtensionElement)from;
      this.UserAgent = element.UserAgent;
    }

    /// <summary>
    /// Gets or sets the user agent.
    /// </summary>
    /// <value>
    /// The user agent.
    /// </value>
    [ConfigurationProperty("userAgent", IsRequired = true)]
    public string UserAgent
    {
      get { return (string)base["userAgent"]; }
      set { base["userAgent"] = value; }
    }
  }
}