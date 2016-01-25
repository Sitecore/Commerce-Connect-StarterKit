// ----------------------------------------------------------------------------------------------
// <copyright file="NopProcessor.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The base NopCommerce cart processor.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common
{
  using Sitecore;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Pipelines;

  /// <summary>
  /// The base class for NopCommerce processors.
  /// </summary>
  /// <typeparam name="TChannel">The type of the channel.</typeparam>
  public abstract class NopProcessor<TChannel> : PipelineProcessor<ServicePipelineArgs> where TChannel : System.ServiceModel.IClientChannel
  {
    /// <summary>
    /// The nop commerce connector configuration file.
    /// </summary>
    private const string NopCommerceConnectorConfigurationFile = "/App_Config/Sitecore.Commerce.Connectors.NopCommerce.config";
    
    /// <summary>
    /// The WCF service client factory.
    /// </summary>
    private ServiceClientFactory clientFactory;

    /// <summary>
    /// Gets or sets the client factory.
    /// </summary>
    /// <value>
    /// The client factory.
    /// </value>
    [NotNull]
    public ServiceClientFactory ClientFactory
    {
      get
      {
        return this.clientFactory = this.clientFactory ?? new ServiceClientFactory();
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.clientFactory = value;
      }
    }

    /// <summary>
    /// Gets the instance of WCF cart service client.
    /// </summary>
    /// <returns> The instance of WCF cart service client.</returns>
    protected TChannel GetClient()
    {
      return this.ClientFactory.CreateClient<TChannel>(NopCommerceConnectorConfigurationFile, string.Format("BasicHttpBinding_{0}", typeof(TChannel).Name.Substring(0, typeof(TChannel).Name.Length - 7)));
    }
  }
}