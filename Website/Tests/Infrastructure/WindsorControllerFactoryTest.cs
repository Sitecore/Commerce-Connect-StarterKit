// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindsorControllerFactoryTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The windsor controller factory test.
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
namespace Sitecore.Commerce.StarterKit.Tests.Infrastructure
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Castle.MicroKernel;
    using FluentAssertions;
    using NSubstitute;
    using Sitecore;
    using Sitecore.Commerce.StarterKit.Controllers;
    using Sitecore.Commerce.StarterKit.Infrastructure;
    using Xunit;

    /// <summary>
    /// The windsor controller factory test.
    /// </summary>
    public class WindsorControllerFactoryTest
    {
        /// <summary>
        /// The kernel.
        /// </summary>
        private readonly IKernel kernel;

        /// <summary>
        /// The request context.
        /// </summary>
        private readonly RequestContext requestContext;

        /// <summary>
        /// The controller factory.
        /// </summary>
        private readonly OpenWindsorControllerFactory controllerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorControllerFactoryTest"/> class.
        /// </summary>
        public WindsorControllerFactoryTest()
        {
            this.kernel = Substitute.For<IKernel>();
            this.requestContext = new RequestContext(Substitute.For<HttpContextBase>(), new RouteData());

            this.controllerFactory = new OpenWindsorControllerFactory(this.kernel);
        }

        /// <summary>
        /// Should get controller instance.
        /// </summary>
        [Fact]
        public void ShouldGetControllerInstance()
        {
            // arrange
            var controllerType = typeof(HomeController);
            this.kernel.Resolve(controllerType).Returns(new HomeController());

            // act
            var controller = this.controllerFactory.GetControllerInstance(this.requestContext, controllerType);

            // assert
            controller.Should().BeOfType<HomeController>();
        }

        /// <summary>
        /// Should throw HTTP exception if controller type is null.
        /// </summary>
        [Fact]
        public void ShouldThrowHttpExceptionIfControllerTypeIsNull()
        {
            // arrange
            this.requestContext.HttpContext.Request.Path.Returns("/cart/info");

            // act
            Action a = () => this.controllerFactory.GetControllerInstance(this.requestContext, null);

            // assert
            a.ShouldThrow<HttpException>().WithMessage("The controller for path '/cart/info' was not found or does not implement IController.");
        }

        /// <summary>
        /// The open windsor controller factory.
        /// </summary>
        private class OpenWindsorControllerFactory : WindsorControllerFactory
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OpenWindsorControllerFactory"/> class.
            /// </summary>
            /// <param name="kernel">The kernel.</param>
            public OpenWindsorControllerFactory(IKernel kernel)
                : base(kernel)
            {
            }

            /// <summary>
            /// Retrieves the controller instance for the specified request context and controller type.
            /// </summary>
            /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
            /// <param name="controllerType">The type of the controller.</param>
            /// <returns>The controller instance.</returns>
            [CanBeNull]
            public new IController GetControllerInstance([CanBeNull] RequestContext requestContext, [CanBeNull] Type controllerType)
            {
                return base.GetControllerInstance(requestContext, controllerType);
            }
        }
    }
}