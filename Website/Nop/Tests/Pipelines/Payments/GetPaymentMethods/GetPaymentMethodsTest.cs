//-----------------------------------------------------------------------
// <copyright file="GetPaymentMethodsTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The GetPaymentMethodsTest class.</summary>
//-----------------------------------------------------------------------
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Payments.GetPaymentMethods
{
  using System.Linq;
  using System.Xml;
  using Commerce.Entities.Payments;
  using Commerce.Pipelines;
  using Commerce.Services.Payments;
  using FluentAssertions;
  using NopPaymentService;
  using NSubstitute;
  using Xunit;

  public class GetPaymentMethodsTest
  {

    private readonly IPaymentServiceChannel _client;

    private readonly NopCommerce.Pipelines.Payments.GetPaymentMethods.GetPaymentMethods _processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPaymentMethodsTest" /> class.
    /// </summary>
    public GetPaymentMethodsTest()
    {
      this._client = Substitute.For<IPaymentServiceChannel>();
      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<IPaymentServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this._client);
      _processor = new NopCommerce.Pipelines.Payments.GetPaymentMethods.GetPaymentMethods() { ClientFactory = clientFactory };
    }

    /// <summary>
    /// Should get payment methods by payment option
    /// </summary>
    [Fact]
    [Trait("Catygory","Payment")] 
    public void ShouldGetPaymentMethodsByPaymentOption()
    {
      var request = new GetPaymentMethodsRequest(new PaymentOption()
      {
        ShopName = "My Store",
        PaymentOptionType = PaymentOptionType.PayCard
      });

      var result = new GetPaymentMethodsResult();
      var args = new ServicePipelineArgs(request, result);

      _client.GetPaymentMethods(request.PaymentOption.ShopName).Returns(new ResponseModelOfArrayOfPaymentMethodModelbQtzsV6D()
      {
        Success = true,
        Result = new []
        {
          new PaymentMethodModel()
          {
            MethodName = "Credit Card",
            ShopName = "My Store",
            SystemName = "Payments.Manual"
          },
          new PaymentMethodModel()
          {
            MethodName = "Credit Card",
            ShopName = "My Store",
            SystemName = "Payments.AuthorizeNet"
          },
        }
      });


      var xml = new XmlDocument();
      var rootNode = xml.CreateElement("map");
      xml.AppendChild(rootNode);

      var shippingOptionValue = xml.CreateAttribute("paymentOptionValue");
      var systemName = xml.CreateAttribute("systemName");
      var methodName = xml.CreateAttribute("methodName");

      shippingOptionValue.Value = "1";
      systemName.Value = "Payments.Manual";
      methodName.Value = "Credit Card";

      rootNode.Attributes.Append(shippingOptionValue);
      rootNode.Attributes.Append(systemName);
      rootNode.Attributes.Append(methodName);

      _processor.Map(rootNode);

      shippingOptionValue.Value = "1";
      systemName.Value = "Payments.AuthorizeNet";
      methodName.Value = "Credit Card";

      _processor.Map(rootNode);

      _processor.Process(args);

      result.Should().NotBeNull();
      result.Success.Should().BeTrue();
      result.PaymentMethods.Should().HaveCount(2);
      result.PaymentMethods.ElementAt(0).ExternalId.Should().Be("Payments.Manual");
      result.PaymentMethods.ElementAt(0).Name.Should().Be("Credit Card");
      result.PaymentMethods.ElementAt(1).ExternalId.Should().Be("Payments.AuthorizeNet");
      result.PaymentMethods.ElementAt(1).Name.Should().Be("Credit Card");
    }
  }
}
