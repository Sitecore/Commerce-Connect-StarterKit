// ----------------------------------------------------------------------------------------------
// <copyright file="RequireAdminAttributeTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The require admin attribute test.
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
namespace Sitecore.Commerce.StarterKit.Tests.Filters
{
  using System.Linq;
  using System.Net.Http;
  using System.Web.Http;
  using System.Web.Http.Controllers;
  using FluentAssertions;
  using Sitecore.Commerce.StarterKit.Filters;
  using Xunit;

  /// <summary>
  /// The require admin attribute test.
  /// </summary>
  public class RequireAdminAttributeTest
  {
    /// <summary>
    /// Should redirect to start path if user is not admin.
    /// </summary>
    [Fact]
    public void ShouldRedirectToStartPathIfUserIsNotAdmin()
    {
      // arrange
      var attribute = new RequireAdminAttribute { IsAdministrator = false };
      var httpActionContext = new HttpActionContext { ControllerContext = new HttpControllerContext { Request = new HttpRequestMessage() } };

      // act
      attribute.OnAuthorization(httpActionContext);

      // assert
      httpActionContext.Response.Should().NotBeNull();
      httpActionContext.Response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
      httpActionContext.Response.Headers.Count().Should().Be(0);
      var error = httpActionContext.Response.Content as ObjectContent<HttpError>;
      error.Should().NotBeNull();
      var errorValue = error.Value as HttpError;
      errorValue.Should().NotBeNull();
      errorValue.Message.Should().Be(Texts.PermissionIsDenied);
    }


    /// <summary>
    /// Should not redirect to start path if user is admin.
    /// </summary>
    [Fact]
    public void ShouldNotRedirectToStartPathIfUserIsAdmin()
    {
      // arrange
      var attribute = new RequireAdminAttribute { IsAdministrator = true };
      var httpActionContext = new HttpActionContext { ControllerContext = new HttpControllerContext { Request = new HttpRequestMessage() } };

      // act
      attribute.OnAuthorization(httpActionContext);

      // assert
      httpActionContext.Response.Should().BeNull();
    }
  }
}