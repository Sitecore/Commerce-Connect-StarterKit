//-----------------------------------------------------------------------
// <copyright file="GetPartiesByCustomerTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The GetPartiesByCustomerTest class.</summary>
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Customers.GetParties
{
  using System;
  using System.Linq;
  using Commerce.Entities;
  using Commerce.Entities.Customers;
  using Commerce.Pipelines;
  using Commerce.Services.Customers;
  using FluentAssertions;
  using NopCommerce.Pipelines.Customers;
  using NopCustomersService;
  using NSubstitute;
  using Xunit;

  /// <summary>
  /// Test class. Get parties by customer
  /// </summary>
  public class GetPartiesByCustomerTest
  {
    private readonly ICustomersServiceChannel _client;

    private readonly GetParties _processor;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPartiesByCustomerTest" /> class.
    /// </summary>
    public GetPartiesByCustomerTest()
    {
      this._client = Substitute.For<ICustomersServiceChannel>();
      var clientFactory = Substitute.For<ServiceClientFactory>();
      clientFactory.CreateClient<ICustomersServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(this._client);
      _processor = new GetParties(new EntityFactory()) {ClientFactory = clientFactory};
    }

    /// <summary>
    /// Should get all parties by customer id.
    /// </summary>
    [Fact]
    public void ShouldGetAllPartiesByCustomerId()
    {
      var customerId = new Guid();
      var request = new GetPartiesRequest(new CommerceCustomer() { ExternalId = customerId.ToString() });
      var result = new GetPartiesResult();
      var args = new ServicePipelineArgs(request, result);

      this._client.GetAllAddresses(customerId).Returns(new ArrayOfAddressModelResponse()
      {
        Success = true,
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

      result.Parties.Should().HaveCount(2);
      result.Success.Should().BeTrue();
      result.Parties.ElementAt(0).Address1.Should().Be("my address 1");
      result.Parties.ElementAt(0).Address2.Should().Be("my address 2");
      result.Parties.ElementAt(0).FirstName.Should().Be("Koly");
      result.Parties.ElementAt(0).LastName.Should().Be("Ivanov");
      result.Parties.ElementAt(0).City.Should().Be("Vinnitsa");
      result.Parties.ElementAt(0).Company.Should().Be("Sitecore");
      result.Parties.ElementAt(0).Country.Should().Be("US");
      result.Parties.ElementAt(0).Email.Should().Be("myemail@sitecore.net");
      result.Parties.ElementAt(0).PhoneNumber.Should().Be("043298765");
      result.Parties.ElementAt(0).ZipPostalCode.Should().Be("21000");

      result.Parties.ElementAt(1).Address1.Should().Be("test address 1");
      result.Parties.ElementAt(1).Address2.Should().Be("test address 2");
      result.Parties.ElementAt(1).FirstName.Should().Be("Ivan");
      result.Parties.ElementAt(1).LastName.Should().Be("Petrovich");
    }
  }
}
