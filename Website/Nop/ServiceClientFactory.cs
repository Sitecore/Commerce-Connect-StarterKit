// ----------------------------------------------------------------------------------------------
// <copyright file="ServiceClientFactory.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The service client helper.
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
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using Sitecore.IO;

namespace Sitecore.Commerce.Connectors.NopCommerce
{
    /// <summary>
  /// The service client factory.
  /// </summary>
  public class ServiceClientFactory
  {
    /// <summary>
    /// The channel factory cache.
    /// </summary>
    private static readonly IDictionary<Tuple<Type, string, string>, IChannelFactory> ChannelFactoryCache = new Dictionary<Tuple<Type, string, string>, IChannelFactory>();

    /// <summary>
    /// Creates the client.
    /// </summary>
    /// <typeparam name="T">Type of the client.</typeparam>
    /// <param name="configurationFile">Name of the configuration file.</param>
    /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
    /// <returns>Service client instance.</returns>
    public virtual T CreateClient<T>(string configurationFile, string endpointConfigurationName)
    {
      return this.GetChannelFactory<T>(configurationFile, endpointConfigurationName).CreateChannel();
    }

    /// <summary>
    /// Gets the channel factory.
    /// </summary>
    /// <typeparam name="T">Type of the client.</typeparam>
    /// <param name="configurationFile">The configuration file.</param>
    /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
    /// <returns>New channel factory.</returns>
    [NotNull]
    protected virtual ChannelFactory<T> GetChannelFactory<T>([NotNull] string configurationFile, [NotNull] string endpointConfigurationName)
    {
      Tuple<Type, string, string> key = new Tuple<Type, string, string>(typeof(T), configurationFile, endpointConfigurationName);
      IChannelFactory result;

      lock (ChannelFactoryCache)
      {
        if (!ChannelFactoryCache.TryGetValue(key, out result))
        {
          System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = FileUtil.MapPath(configurationFile) }, ConfigurationUserLevel.None);
          result = new ConfigurationChannelFactory<T>(endpointConfigurationName, configuration, null);

          ChannelFactoryCache.Add(key, result);
        }
      }

      return (ChannelFactory<T>)result;
    }
  }
}
