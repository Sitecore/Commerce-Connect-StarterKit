// ----------------------------------------------------------------------------------------------
// <copyright file="GetShippingMethodsTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   GetShippingMethodsTest test class
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Shipping.GetShippingMethods
{
  using System;
  using System.Linq;
  using System.Xml;
  using Commerce.Entities;
  using Commerce.Entities.Carts;
  using Commerce.Entities.Shipping;
  using Commerce.Pipelines;
  using Commerce.Services.Shipping;
  using FluentAssertions;
  using NopShippingService;
  using NSubstitute;
  using Xunit;
  using GetShippingMethodsRequest = Services.Shipping.GetShippingMethodsRequest;

  public class GetShippingMethodsTest
  {
    /// <summary>
    /// Shipping service channal
    /// </summary>
    private readonly IShippingServiceChannel _client;

    /// <summary>
    /// Get shipping methods processor
    /// </summary>
    private readonly NopCommerce.Pipelines.Shipping.GetShippingMethods.GetShippingMethods _processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetShippingMethodsTest" /> class.
    /// </summary>
    public GetShippingMethodsTest()
    {
      this._client = Substitute.For<IShippingServiceChannel>();
      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IShippingServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this._client);
      _processor = new NopCommerce.Pipelines.Shipping.GetShippingMethods.GetShippingMethods() { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should get shipping methods by shipping option
    /// </summary>
    [Fact]
    [Trait("Catygory", "Shipping")] 
    public void ShouldGetShippingMethodsByShippingOption()
    {
      var request =
        new GetShippingMethodsRequest(new ShippingOption()
        {
          ShippingOptionType = Commerce.Entities.Shipping.ShippingOptionType.ShipToAddress,
        }, new Party(){Country = "USA"});

      request.Properties.Add(new PropertyItem("cart", new Cart() {ExternalId = Guid.NewGuid().ToString()}));
      
      var result = new GetShippingMethodsResult();
      var args = new ServicePipelineArgs(request, result);

      _client.GetShippingMethods(Arg.Any<ShoppingCartModel>(), Arg.Any<string>(), Arg.Any<AddressModel>()).Returns(
        new ArrayOfShippingMethodModelResponse()
        {
          Success = true,
          Result = new[]
          {
            new ShippingMethodModel()
            {
              Name = "By Ground", 
              Description = "Compared to other shipping methods, like by flight or over seas, ground shipping is carried out closer to the earth",
              SystemName = "Shipping.FixedRate"
            }, 
            new ShippingMethodModel()
            {
              Name = "By Air",
              Description = "The one day air shipping",
              SystemName = "Shipping.FixedRate"
            }
          }
        });

      var xml = new XmlDocument();
      var rootNode = xml.CreateElement("map");
      xml.AppendChild(rootNode);

      var shippingOptionValue = xml.CreateAttribute("shippingOptionValue");
      var systemName = xml.CreateAttribute("systemName");
      var methodName = xml.CreateAttribute("methodName");

      shippingOptionValue.Value = "1";
      systemName.Value = "Shipping.FixedRate";
      methodName.Value = "By Ground";

      rootNode.Attributes.Append(shippingOptionValue);
      rootNode.Attributes.Append(systemName);
      rootNode.Attributes.Append(methodName);
      
      _processor.Map(rootNode);

      shippingOptionValue.Value = "1";
      systemName.Value = "Shipping.FixedRate";
      methodName.Value = "By Air";

      _processor.Map(rootNode);


      _processor.Process(args);

      result.Should().NotBeNull();
      result.Success.Should().BeTrue();
      result.ShippingMethods.Should().HaveCount(2);
      result.ShippingMethods.ElementAt(0).Name.Should().Be("By Ground");
    }
  }
}
