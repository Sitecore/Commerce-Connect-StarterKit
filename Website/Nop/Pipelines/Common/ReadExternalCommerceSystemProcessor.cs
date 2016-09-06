// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadExternalCommerceSystemProcessor.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The base processor for all the processors that read data from Nop.
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
namespace Sitecore.Commerce.Connectors.NopCommerce.Pipelines.Common
{
  using Sitecore.Configuration;
  using Sitecore.Diagnostics;
  using Sitecore.Commerce.Entities;

  /// <summary>
  /// The base processor for all the processors that read data from Nop.
  /// </summary>
  /// <typeparam name="TChannel">Type of the channel.</typeparam>
  public abstract class ReadExternalCommerceSystemProcessor<TChannel> : NopProcessor<TChannel> where TChannel : System.ServiceModel.IClientChannel
  {
    /// <summary>
    /// The entity factory.
    /// </summary>
    private IEntityFactory entityFactory;

    /// <summary>
    /// Gets or sets the entity factory.
    /// </summary>
    public IEntityFactory EntityFactory
    {
      get
      {
        return this.entityFactory ?? (this.entityFactory = (IEntityFactory)Factory.CreateObject("entityFactory", true));
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.entityFactory = value;
      }
    }

    /// <summary>
    /// Instantiates the entity.
    /// </summary>
    /// <returns>The instantiated entity.</returns>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    protected virtual TEntity InstantiateEntity<TEntity>()
    {
      return this.EntityFactory.Create<TEntity>(typeof(TEntity).Name);
    }
  }
}