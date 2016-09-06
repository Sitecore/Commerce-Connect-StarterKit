// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindCart.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the FindCart class.</summary>
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

// DOCDONE

namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Carts.GetCarts
{
    using System.Linq;
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Data.Carts;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Data.Products;
    using System;

    /// <summary>
    /// Searches for a visitor cart in its current Engagement Automation (EA) state with the following input parameters:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// UserID
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// CustomerID
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// CartName
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// ShopName
    /// </description>
    /// </item>
    /// </list>
    /// If a matching cart is found, the custom pipeline argument args.Request.Properties[“CartID”] is assigned to the the ID of the matching cart. 
    /// In the  CreateOrResumeCart pipeline, the RunLoadCart processor uses the cart ID stored in the custom data.
    /// You can use the FindCartInEaState processor to search and retrieve the ID of an existing cart before executing the RunLoadCart processor.
    /// </summary>
    public class FindCart : PipelineProcessor<ServicePipelineArgs>
    {
        /// <summary>
        /// Allows Create, Read, Update, and Delete operations to be performed on the shopping carts that are stored in the Engagement Automation states.
        /// </summary>
        private readonly ICartRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCart"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public FindCart([NotNull] ICartRepository repository)
        {
            Assert.ArgumentNotNull(repository, "repository");

            this.repository = repository;
        }

        /// <summary>
        /// Executes the business logic of the FindCartInEaState processor.
        /// </summary>
        /// <param name="args">The args.</param>
        public override void Process([NotNull] ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var request = (CreateOrResumeCartRequest)args.Request;
            
            args.Request.Properties["CartId"] = request.UserId;
        }
    }
}
