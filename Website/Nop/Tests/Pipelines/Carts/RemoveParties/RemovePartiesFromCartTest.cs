// ----------------------------------------------------------------------------------------------
// <copyright file="RemovePartiesFromCartTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The RemovePartiesFromCartTest class.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Carts.RemoveParties
{
  using System;
  using System.Collections.Generic;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.Connectors.NopCommerce.NopCartsService;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Carts;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Pipelines.Carts.RemoveParties;
  using Sitecore.Commerce.Services.Carts;
  using Xunit;

  public class RemovePartiesFromCartTest
  {
    /// <summary>
    /// The processor.
    /// </summary>
    private readonly RemovePartiesFromCart processor;

    /// <summary>
    /// The visitor id.
    /// </summary>
    private readonly Guid visitorId;

    /// <summary>
    /// The request.
    /// </summary>
    private RemovePartiesRequest request;

    /// <summary>
    /// The result.
    /// </summary>
    private RemovePartiesResult result;

    /// <summary>
    /// The args.
    /// </summary>
    private ServicePipelineArgs args;

    /// <summary>
    /// The client.
    /// </summary>
    private readonly ICartsServiceChannel client;

    /// <summary>
    /// The cart (add party to add/remove here).
    /// </summary>
    private Cart cart;

    private List<Party> partiesList; 

    public RemovePartiesFromCartTest()
    {
      visitorId = Guid.NewGuid();

      client = Substitute.For<ICartsServiceChannel>();

      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICartsServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(client);

      processor = new RemovePartiesFromCart();
    }

    /// <summary>
    /// Should remove cart line.
    /// </summary>
    [Fact]
    public void ShouldNotRemovePartyFromCart()
    {
      cart = new Cart
      {
        ExternalId = visitorId.ToString(),
        Lines = new ReadOnlyCollectionAdapter<CartLine> { new CartLine() },
        AccountingCustomerParty = new CartParty() {PartyID = "1", ExternalId = "1"},
        BuyerCustomerParty = new CartParty() { PartyID = "1", ExternalId = "1" }
      };

      partiesList = new List<Party>();


      request = new RemovePartiesRequest(cart, partiesList);
      result = new RemovePartiesResult();
      args = new ServicePipelineArgs(request, result);
      
      var addressesToRemove = new[] { new CustomerAddressModel() { AddressType = AddressTypeModel.Billing} };
      var responseModel = new Response() {Message = "Success", Success = true};

      client.RemoveAddresses(addressesToRemove, visitorId).Returns(responseModel);

      // act
      processor.Process(args);

      // assert
      result.Success.Should().BeTrue();
    }


    /// <summary>
    /// Should remove cart line.
    /// </summary>
    [Fact]
    public void ShouldRemoveShippingPartyFromCart()
    {
      cart = new Cart
      {
        ExternalId = visitorId.ToString(),
        Lines = new ReadOnlyCollectionAdapter<CartLine> { new CartLine() },
        AccountingCustomerParty = new CartParty() { }
      };

      partiesList = new List<Party>();
      
      request = new RemovePartiesRequest(cart, partiesList);
      result = new RemovePartiesResult();
      args = new ServicePipelineArgs(request, result);
      
      var addressesToRemove = new[] { new CustomerAddressModel() { AddressType = AddressTypeModel.Billing } };
      var responseModel = new Response() { Message = "Success", Success = true };

      client.RemoveAddresses(addressesToRemove, visitorId).Returns(responseModel);

      // act
      processor.Process(args);

      // assert
      result.Success.Should().BeTrue();
    }
    
  }
}
