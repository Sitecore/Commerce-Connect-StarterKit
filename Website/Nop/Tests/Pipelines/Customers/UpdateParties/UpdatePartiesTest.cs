// ----------------------------------------------------------------------------------------------
// <copyright file="UpdatePartiesTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The UpdatePartiesTest class.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Customers.UpdateParties
{
      using System;
  using System.Collections.Generic;

  using FluentAssertions;

  using NSubstitute;

  using Sitecore.Commerce.Connectors.NopCommerce.NopCustomersService;
  using Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Customers;
  using Sitecore.Commerce.Entities;
  using Sitecore.Commerce.Entities.Customers;
  using Sitecore.Commerce.Pipelines;
  using Sitecore.Commerce.Services.Customers;

  using Xunit;
    public class UpdatePartiesTest
    {
           private readonly ICustomersServiceChannel _client;

           private readonly UpdateParties _processor;

    public UpdatePartiesTest()
    {
      _client = Substitute.For<ICustomersServiceChannel>();
      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICustomersServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(_client);
      _processor = new UpdateParties(new EntityFactory()) { ClientFactory = clientFactory };
    }

    [Fact]
    public void ShouldCallServiceWithCorrectArgs()
    {
      var customerId = Guid.NewGuid();

      var request = new UpdatePartiesRequest(
          new CommerceCustomer { ExternalId = customerId.ToString() },
          new List<Party> { new Party(), new Party() }
          );
      var result = new CustomerResult();
      var args = new ServicePipelineArgs(request, result);

      _client.UpdateAddresses(Arg.Any<Guid>(), Arg.Any<AddressModel[]>()).Returns(new Response());
      _processor.Process(args);

      _client.Received().UpdateAddresses(Arg.Is(customerId), Arg.Is<AddressModel[]>(x => x.Length == request.Parties.Count));
    }

    [Fact]
    public void ShouldHandleInvalidCustomerId()
    {
        var request = new UpdatePartiesRequest(
          new CommerceCustomer { ExternalId = "1234" },
          new List<Party>(0)
          );
        var result = new CustomerResult();
      var args = new ServicePipelineArgs(request, result);

      _processor.Process(args);

      result.Success.Should().Be(false);
      result.SystemMessages.Should().HaveCount(x => x > 0);
    }

    }
}
