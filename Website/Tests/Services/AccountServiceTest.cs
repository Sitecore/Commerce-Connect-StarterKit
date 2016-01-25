// ----------------------------------------------------------------------------------------------
// <copyright file="AccountServiceTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the AccountServiceTest class.
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
namespace Sitecore.Commerce.StarterKit.Tests.Services
{
    using NSubstitute;
    using Sitecore.Analytics;
    using Sitecore.Analytics.Tracking;
    using Sitecore.Commerce.Services.Customers;
    using Sitecore.Commerce.StarterKit.Services;
    using Sitecore.Common;
    using Xunit.Extensions;

    /// <summary>
    /// Defines the AccountServiceTest class.
    /// </summary>
    public class AccountServiceTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldMergeCartsOnLogIn(bool persistent)
        {
            // Arrange
            var contactModel = new FakeContact();
            var contact = new ContactContext(contactModel);
            var tracker = Substitute.For<ITracker>();
            var testCart = new Sitecore.Commerce.Entities.Carts.Cart();
            tracker.Contact.Returns(contact);

            var cartService = Substitute.For<ICartService>();
            cartService.GetCart().Returns(testCart);

            var customerProvider = Substitute.For<CustomerServiceProvider>();
            var accountService = new AccountService(cartService, customerProvider);

            // Act
            using (new Switcher<ITracker, TrackerSwitcher>(tracker))
            {
                accountService.Login("user", "password", persistent);
            }

            // Assert
            cartService.Received().MergeCarts(testCart);
        }
    }
}