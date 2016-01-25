//-----------------------------------------------------------------------
// <copyright file="AddPartiesTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The AddPartiesTest class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Customers.AddParties
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

  /// <summary>
  /// Test class. Adds parties to customer
  /// </summary>
  public class AddPartiesTest
  {
    private readonly ICustomersServiceChannel _client;

    private readonly AddParties _processor;

    public AddPartiesTest()
    {
      _client = Substitute.For<ICustomersServiceChannel>();
      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICustomersServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(_client);
      _processor = new AddParties(new EntityFactory()) { ClientFactory = clientFactory };
    }

    [Fact]
    public void ShouldCallServiceWithCorrectArgs()
    {
      var customerId = Guid.NewGuid();

      var request = new AddPartiesRequest(
          new CommerceCustomer { ExternalId = customerId.ToString() },
          new List<Party> { new Party(), new Party() }
          );
      var result = new AddPartiesResult();
      var args = new ServicePipelineArgs(request, result);

      _client.AddAddresses(Arg.Any<Guid>(), Arg.Any<AddressModel[]>()).Returns(new ArrayOfAddressModelResponse() { });
      _processor.Process(args);

      _client.Received().AddAddresses(Arg.Is(customerId), Arg.Is<AddressModel[]>(x => x.Length == request.Parties.Count));
    }

    [Fact]
    public void ShouldHandleInvalidCustomerId()
    {
      var request = new AddPartiesRequest(
          new CommerceCustomer { ExternalId = "1234" },
          new List<Party>(0)
          );
      var result = new AddPartiesResult();
      var args = new ServicePipelineArgs(request, result);

      _processor.Process(args);

      result.Success.Should().Be(false);
      result.SystemMessages.Should().HaveCount(x => x > 0);
    }

    /// <summary>
    /// Should Add parties by customer id.
    /// </summary>
    [Fact]
    public void ShouldAddPartiesToCustomerById()
    {
      var customerId = Guid.NewGuid();

      var request = new AddPartiesRequest(
        new CommerceCustomer
        {
          ExternalId = customerId.ToString(), 
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

      _client.AddAddresses(Arg.Any<Guid>(), Arg.Any<AddressModel[]>())
      .Returns(new ArrayOfAddressModelResponse()
      {
        Success = true,
        Message = "Success",
        Result = new[]
                {
                  new AddressModel()
                  {
                    Address1 = "my address 1",
                    Address2 = "my address 2",
                    FirstName = "Koly",
                    LastName = "Ivanov",
                    City = "Vinnitsa",
                    Company = "Sitecore",
                    CountryThreeLetterIsoCode = "USA",
                    CountryTwoLetterIsoCode = "US",
                    Email = "myemail@sitecore.net",
                    FaxNumber = "9999",
                    Id = "1",
                    PhoneNumber = "043298765",
                    ZipPostalCode = "21000"
                  },
                  new AddressModel()
                  {
                    Address1 = "test address 1",
                    Address2 = "test address 2",
                    FirstName = "Ivan",
                    LastName = "Petrovich"
                  }

                }

      });

      _processor.Process(args);

      result.Success.Should().BeTrue();
      result.Parties.Should().HaveCount(2);
    }
  }
}
