//-----------------------------------------------------------------------
// <copyright file="AddPartiesToCartTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The AddPartiesToCartTest class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.AddParties
{

  using System;
  using System.Collections.Generic;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.AddParties;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Carts;
  using Xunit;

  /// <summary>
  /// Test class. Adds parties to Cart
  /// </summary>
  public class AddPartiesToCartTest
  {
    private readonly ICartsServiceChannel _client;

    private readonly AddPartiesToCart _processor;

    public AddPartiesToCartTest()
    {
      _client = Substitute.For<ICartsServiceChannel>();
      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(_client);
      _processor = new AddPartiesToCart() { ClientFactory = clientFactory }; 
    }

    [Fact]
    public void ShouldCallServiceWithCorrectArgs()
    {
      var cartId = Guid.NewGuid();

      var request = new AddPartiesRequest(
          new Cart { ExternalId = cartId.ToString(), BuyerCustomerParty = new CartParty(){ExternalId = "1"}}, // only one party to add
          new List<Party> { new Party(), new Party() }
          );

      var result = new AddPartiesResult();
      var args = new ServicePipelineArgs(request, result);
      
      _client.AddAddresses(Arg.Any<CustomerAddressModel[]>(), Arg.Any<Guid>()).Returns(new Response()
      {
        Success = true,
        Message = "Success"
      });
      
      _processor.Process(args);

      // asserts
     // result.Success.Should().Be(true);
     // _client.Received().AddAddressesByExternalCustomerId(Arg.Is<CustomerAddressModel[]>(x => x.Length > 0), Arg.Is(cartId));
      _client.Received().AddAddresses(Arg.Any<CustomerAddressModel[]>(), Arg.Any<Guid>());
    }

    [Fact]
    public void ShouldHandleInvalidCartId()
    {
      var request = new AddPartiesRequest(
          new Cart { ExternalId = "1234", BuyerCustomerParty = new CartParty() { ExternalId = "1" } },
          new List<Party>(0)
          );

      var result = new AddPartiesResult();
      var args = new ServicePipelineArgs(request, result);

      _processor.Process(args);

      result.Success.Should().Be(false);
      result.SystemMessages.Should().HaveCount(x => x > 0);
    }

    [Fact]
    public void ShouldAddPartiesToCartById()
    {
      var cartId = Guid.NewGuid();

      var request = new AddPartiesRequest(
        new Cart
        {
          ExternalId = cartId.ToString(), 
          Name = "Vasya"
        },
        new List<Party>
        {
          new Party
          {
            Address1 = "my address 1", 
            Address2 = "my address 2", 
            FirstName = "Koly", 
            LastName = "Ivanov"
          },
          new Party()
        });
      var result = new AddPartiesResult();
      var args = new ServicePipelineArgs(request, result);

      _client.AddAddresses(Arg.Any<CustomerAddressModel[]>(), Arg.Any<Guid>())
      .Returns(new Response()
      {
        Success = true,
        Message = "Success"
      });

      _processor.Process(args);

      result.Success.Should().BeTrue();
    }
  }

}
