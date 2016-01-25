// ----------------------------------------------------------------------------------------------
// <copyright file="HttpUserAgentMessageInspector.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The HttpUserAgentMessageInspector.
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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Sitecore.Commerce.Connectors.NopCommerce.ServiceModel.Dispatcher.ClientRuntime
{
    public class HttpUserAgentMessageInspector : IClientMessageInspector
  {
    /// <summary>
    /// The user agent HTTP header
    /// </summary>
    private const string UserAgentHttpHeader = "user-agent";

    /// <summary>
    /// The user agent
    /// </summary>
    private readonly string userAgent;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpUserAgentMessageInspector"/> class.
    /// </summary>
    /// <param name="userAgent">The user agent.</param>
    public HttpUserAgentMessageInspector(string userAgent)
    {
      this.userAgent = userAgent;
    }

    #region IClientMessageInspector Members

    /// <summary>
    /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
    /// </summary>
    /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
    /// <param name="correlationState">Correlation state data.</param>
    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
    }


    /// <summary>
    /// Enables inspection or modification of a message before a request message is sent to a service.
    /// </summary>
    /// <param name="request">The message to be sent to the service.</param>
    /// <param name="channel">The WCF client object channel.</param>
    /// <returns>
    /// The object that is returned as the <paramref name="correlationState " />argument of the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)" /> method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid" /> to ensure that no two <paramref name="correlationState" /> objects are the same.
    /// </returns>
    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
      HttpRequestMessageProperty httpRequestMessage;
      object httpRequestMessageObject;
      if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
      {
        httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
        if (httpRequestMessage != null && string.IsNullOrEmpty(httpRequestMessage.Headers[UserAgentHttpHeader]))
        {
          httpRequestMessage.Headers[UserAgentHttpHeader] = this.userAgent;
        }
      }
      else
      {
        httpRequestMessage = new HttpRequestMessageProperty();
        httpRequestMessage.Headers.Add(UserAgentHttpHeader, this.userAgent);
        request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
      }
      return null;
    }

    #endregion
  }
}
