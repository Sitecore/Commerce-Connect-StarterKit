// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountControllerTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the AccountControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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
namespace Sitecore.Commerce.StarterKit.Tests.Controllers
{
  using System;
  using System.Web.Mvc;
  using System.Web.Security;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Commerce.StarterKit.Controllers;
  using Sitecore.Commerce.StarterKit.Models;
  using Sitecore.Commerce.StarterKit.Services;
  using Sitecore.Sites;
  using Sitecore.TestKit.Sites;
  using Xunit;

  /// <summary>
  /// Defines the AccountControllerTest type.
  /// </summary>
  public class AccountControllerTest : IDisposable
  {
    /// <summary>
    /// The customer service.
    /// </summary>
    private readonly IAccountService accountService;

    private readonly IOrderService orderService;

    /// <summary>
    /// The controller.
    /// </summary>
    private readonly AccountController controller;

    /// <summary>
    /// The _is disposed
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountControllerTest" /> class.
    /// </summary>
    public AccountControllerTest()
    {
      this.accountService = Substitute.For<IAccountService>();
      this.orderService = Substitute.For<IOrderService>();

      this.controller = new AccountController(this.accountService, this.orderService);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            if (null != controller)
            {
                controller.Dispose();
            }
        }
        _isDisposed = true;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="AccountControllerTest"/> class.
    /// </summary>
    ~AccountControllerTest()
    {
        Dispose(false);
    }

    /// <summary>
    /// Should return corresponding view when register page is requested.
    /// </summary>
    [Fact]
    public void ShouldReturnCorrespondingViewWhenRegisterPageIsRequested()
    {
      // Act
      ActionResult actionResult = this.controller.Register();

      // Assert
      actionResult.Should().BeOfType<ViewResult>();
    }

    /// <summary>
    /// Should register, login user and redirect him to default page on customer registration attempt.
    /// </summary>
    [Fact]
    public void ShouldRegisterLoginUserAndRedirectHimToDefaultPageOnCustomerRegistrationAttempt()
    {
      // Arrange
      RegisterModel registerModel = new RegisterModel { UserName = "customer", Email = "email", Password = "password", ConfirmPassword = "password" };

      // Act
      using (new SiteContextSwitcher(new TSiteContext("shop")))
      {
        ActionResult actionResult = this.controller.Register(registerModel);

        // Assert
        ((RedirectResult)actionResult).Url.Should().Be("/");

        this.accountService.Received().Register(@"CommerceUsers\customer", "password", "email", "shop");
        this.accountService.Received().Login(@"CommerceUsers\customer", "password", false);
      }
    }

    /// <summary>
    /// Should do nothing on customer registration attempt with incorrect parameters.
    /// </summary>
    [Fact]
    public void ShouldDoNothingOnCustomerRegistrationAttemptWithIncorrectParameters()
    {
      // Arrange
      this.controller.ModelState.AddModelError("testError", "Test error message");

      // Act
      ActionResult actionResult = this.controller.Register(new RegisterModel());

      // Assert
      actionResult.Should().BeOfType<ViewResult>();

      this.accountService.DidNotReceiveWithAnyArgs().Register(null, null, null, null);
      this.accountService.DidNotReceiveWithAnyArgs().Login(null, null, false);
    }

    /// <summary>
    /// Should add model error if failed to register customer.
    /// </summary>
    [Fact]
    public void ShouldAddModelErrorIfFailedToRegisterCustomer()
    {
      // Arrange
      this.accountService.WhenForAnyArgs(customerService => customerService.Register(null, null, null, null)).Do(callInfo => { throw new MembershipCreateUserException(); });

      // Act
      using (new SiteContextSwitcher(new TSiteContext("shop")))
      {
        ActionResult actionResult = this.controller.Register(new RegisterModel());

        // Assert
        actionResult.Should().BeOfType<ViewResult>();

        this.controller.ModelState.Should().HaveCount(1);
      }
    }

    /// <summary>
    /// Should return corresponding view when login page requested.
    /// </summary>
    [Fact]
    public void ShouldReturnCorrespondingViewWhenLoginPageRequested()
    {
      // Act
      ActionResult actionResult = this.controller.Login("returnUrl");

      // Assert
      bool isReturnUrlCorrect = this.controller.ViewBag.ReturnUrl == "returnUrl";
      isReturnUrlCorrect.Should().BeTrue();

      actionResult.Should().BeOfType<ViewResult>();
    }

    /// <summary>
    /// Should login customer and redirect him to return URL when login page submitted.
    /// </summary>
    [Fact]
    public void ShouldLoginCustomerAndRedirectHimToReturnUrlWhenLoginPageSubmitted()
    {
      // Arrange
      this.accountService.Login(null, null, true).ReturnsForAnyArgs(true);

      // Act
      ActionResult actionResult = this.controller.Login(new LoginModel { UserName = "customer", Password = "password", RememberMe = true }, "/");

      // Assert
      ((RedirectResult)actionResult).Url.Should().Be("/");

      this.accountService.Received().Login(@"CommerceUsers\customer", "password", true);
    }

    /// <summary>
    /// Should add model error if failed to login.
    /// </summary>
    [Fact]
    public void ShouldAddModelErrorIfFailedToLogin()
    {
      // Act
      ActionResult actionResult = this.controller.Login(new LoginModel { UserName = "customer", Password = "password", RememberMe = true }, "/");

      // Assert
      actionResult.Should().BeOfType<ViewResult>();

      this.controller.ModelState.Should().HaveCount(1);

      this.accountService.Received().Login(@"CommerceUsers\customer", "password", true);
    }

    /// <summary>
    /// Should logout when customer attempts to log off.
    /// </summary>
    [Fact]
    public void ShouldLogoutWhenCustomerAttemptsToLogOff()
    {
      // Act
      ActionResult actionResult = this.controller.Logout();

      // Assert
      ((RedirectResult)actionResult).Url.Should().Be("/");

      this.accountService.Received().Logout();
    }
  }
}