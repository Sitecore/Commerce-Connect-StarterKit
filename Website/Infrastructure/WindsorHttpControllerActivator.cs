﻿// ----------------------------------------------------------------------------------------------
// <copyright file="WindsorHttpControllerActivator.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the WindsorHttpControllerActivator type.
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
namespace Sitecore.Commerce.StarterKit.Infrastructure
{
    using System;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;
    using Castle.Windsor;

    /// <summary>
    /// Defines the WindsorHttpControllerActivator type.
    /// </summary>
    public class WindsorHttpControllerActivator : IHttpControllerActivator
    {
        /// <summary>
        /// The container.
        /// </summary>
        private readonly IWindsorContainer container;

        /// <summary>
        /// The default activator should this one be unable to find the requested controller
        /// </summary>
        private readonly IHttpControllerActivator defaultActivator;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorHttpControllerActivator"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="defaultActivator">The default activator to fallback to should the main container bee unable to create a controller.</param>
        public WindsorHttpControllerActivator(IWindsorContainer container, IHttpControllerActivator defaultActivator)
        {
            this.container = container;
            this.defaultActivator = defaultActivator;
        }

        /// <summary>
        /// Creates an <see cref="T:System.Web.Http.Controllers.IHttpController" /> object.
        /// </summary>
        /// <param name="request">The message request.</param>
        /// <param name="controllerDescriptor">The HTTP controller descriptor.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>
        /// An <see cref="T:System.Web.Http.Controllers.IHttpController" /> object.
        /// </returns>
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            IHttpController controller;

            try
            {
                controller = (IHttpController)this.container.Resolve(controllerType);

                request.RegisterForDispose(new Release(() => this.container.Release(controller)));
            }
            catch (Exception)
            {
                controller = this.defaultActivator.Create(request, controllerDescriptor, controllerType);
            }

            return controller;
        }

        /// <summary>
        /// Defines the WindsorHttpControllerActivator.Release type.
        /// </summary>
        private class Release : IDisposable
        {
            /// <summary>
            /// The release.
            /// </summary>
            private readonly Action release;

            /// <summary>
            /// Initializes a new instance of the <see cref="Release"/> class.
            /// </summary>
            /// <param name="release">The release.</param>
            public Release(Action release)
            {
                this.release = release;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.release();
            }
        }
    }
}