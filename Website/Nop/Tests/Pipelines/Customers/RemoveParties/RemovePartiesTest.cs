// ----------------------------------------------------------------------------------------------
// <copyright file="RemovePartiesTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The RemovePartiesTest class.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Tests.Pipelines.Customers.RemoveParties
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
    /// Test class. Removes parties to customer
    /// </summary>
    public class RemovePartiesTest
    {
        private readonly ICustomersServiceChannel _client;

        private readonly RemoveParties _processor;

        public RemovePartiesTest()
        {
          _client = Substitute.For<ICustomersServiceChannel>();
          var clientFactory = Substitute.For<ServiceClientFactory>();
          clientFactory.CreateClient<ICustomersServiceChannel>(Arg.Any<string>(), Arg.Any<string>()).Returns(_client);
          _processor = new RemoveParties(new EntityFactory()) { ClientFactory = clientFactory };
        }
        
        [Fact]
        public void ShouldCallServiceWithCorrectArgs()
        {
            var customerId = Guid.NewGuid();

            var request = new RemovePartiesRequest(
                new CommerceCustomer { ExternalId = customerId.ToString() },
                new List<Party> { new Party(), new Party() }
                );
            var result = new CustomerPartiesResult();
            var args = new ServicePipelineArgs(request, result);

            _processor.Process(args);

            _client.Received().RemoveAddresses(Arg.Is(customerId), Arg.Any<string[]>());
        }

        [Fact]
        public void ShouldHandleInvalidCustomerId()
        {
            var request = new RemovePartiesRequest(
                new CommerceCustomer { ExternalId = "1234" },
                new List<Party>(0)
                );
            var result = new CustomerPartiesResult();
            var args = new ServicePipelineArgs(request, result);

            _processor.Process(args);

            result.Success.Should().Be(false);
            result.SystemMessages.Should().HaveCount(x => x > 0);
        }



    }
}
