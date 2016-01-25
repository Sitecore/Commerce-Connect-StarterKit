// ----------------------------------------------------------------------------------------------
// <copyright file="DependenciesInstaller.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The dependencies installer.
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
  using System.Web.Http.Controllers;
  using System.Web.Mvc;
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using Sitecore;
  using Sitecore.Globalization;
  using Sitecore.Commerce.Services.Carts;
  using Sitecore.Commerce.StarterKit.Services;

  /// <summary>
  /// The dependencies installer.
  /// </summary>
  public class DependenciesInstaller : IWindsorInstaller
  {
    /// <summary>
    /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="store">The configuration store.</param>
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(Classes.FromThisAssembly()
        .BasedOn<IController>()
        .LifestyleTransient());

      container.Register(Classes.FromThisAssembly()
        .BasedOn<IHttpController>()
        .LifestyleTransient());

      container.Register(Component.For<ICartService>()
        .ImplementedBy<CartService>()
        .DynamicParameters((k, p) => { p["shopName"] = Context.Site.Name; })
        .LifestyleTransient());

      container.Register(Component.For<IProductService>()
        .ImplementedBy<ProductService>()
        .DynamicParameters((k, p) => { p["database"] = Context.Database; p["language"] = Language.Current; })
        .LifestyleTransient());

      container.Register(Component.For<ICheckoutService>()
        .ImplementedBy<CheckoutService>()
        .DynamicParameters((k, p) => { p["shopName"] = Context.Site.Name; })
        .LifestyleTransient());

      container.Register(Component.For<IOrderService>()
        .ImplementedBy<OrderService>()
        .DynamicParameters((k, p) => { p["shopName"] = Context.Site.Name; })
        .LifestyleTransient());

      container.Register(Classes.FromAssemblyContaining<ICartService>()
        .Pick()
        .WithService.DefaultInterfaces());

      // TODO: synchronize dependency with one registered under "/sitecore/cartServiceProvider" in Sitecore.Commerce.config.
      container.Register(Classes.FromAssemblyContaining<CartServiceProvider>()
        .Pick()
        .WithService.DefaultInterfaces());

      container.Register(Classes.FromThisAssembly()
        .Pick()
        .WithService.DefaultInterfaces());
    }
  }
}