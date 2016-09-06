// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependenciesInstallerTest.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The dependencies installer test.
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
  using Castle.MicroKernel.Registration;
  using Castle.Windsor;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Analytics.Data.DataAccess;
  using Sitecore.Collections;
  using Sitecore.Common;
  using Sitecore.Commerce.StarterKit.Controllers;
  using Sitecore.Commerce.StarterKit.Helpers;
  using Sitecore.Commerce.StarterKit.Infrastructure;
  using Sitecore.Commerce.StarterKit.Services;
  using Sitecore.Sites;
  using Sitecore.TestKit.Data.Items;
  using Sitecore.TestKit.Sites;
  using Xunit;
  using Sitecore.Commerce.Contacts;
  using Sitecore.Analytics.Tracking;
  using Sitecore.Data;
  using Sitecore.Analytics;
  using Sitecore.Commerce.Data.Products;

  /// <summary>
  /// The dependencies installer test.
  /// </summary>
  public class DependenciesInstallerTest : IDisposable
  {
    /// <summary>
    /// The container.
    /// </summary>
    private readonly IWindsorContainer container;

    /// <summary>
    /// The tree.
    /// </summary>
    private readonly TTree tree;

    /// <summary>
    /// The site context switcher.
    /// </summary>
    private readonly SiteContextSwitcher siteContextSwitcher;

    /// <summary>
    /// The _is disposed
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DependenciesInstallerTest" /> class.
    /// </summary>
    public DependenciesInstallerTest()
    {
      this.container = new WindsorContainer();
      this.tree = new TTree("web");
      this.siteContextSwitcher = new SiteContextSwitcher(new TSiteContext(new StringDictionary { { "name", "autohaus" }, { "database", "web" } }));

      Sitecore.Context.Database.Name.Should().Be("web");
    }

    /// <summary>
    /// Should resolve controllers.
    /// </summary>
    [Fact]
    public void ShouldResolveControllers()
    {
      // arrange
      this.container.Install(new DependenciesInstaller());

      // act
      var controller = this.container.Resolve<ProductController>();

      // assert
      controller.Should().NotBeNull();
    }

    /// <summary>
    /// Should resolve dependency from OBEC assembly.
    /// </summary>
    [Fact]
    public void ShouldResolveDependencyFromObecAssembly()
    {
      // arrange
      this.container.Install(new DependenciesInstaller());

      // act
      var service = this.container.Resolve<IProductService>();

      // assert
      service.Should().NotBeNull();
    }

    /// <summary>
    /// Should resolve dependency from sample store assembly.
    /// </summary>
    [Fact]
    public void ShouldResolveDependencyFromSampleStoreAssembly()
    {
      // arrange
      this.container.Install(new DependenciesInstaller());

      // act
      var contentSearchHelper = this.container.Resolve<ContentSearchHelper>();

      // assert
      contentSearchHelper.Should().NotBeNull();
    }

    /// <summary>
    /// Should inject context shop name and visitor into cart service.
    /// </summary>
    [Fact]
    public void ShouldInjectContextShopNameIntoCartService()
    {
      // arrange
      var contactFactory = Substitute.For<ContactFactory>();
      contactFactory.GetContact().Returns("Connor John");

      this.container.Register(Component.For<ContactFactory>().Instance(contactFactory));
      this.container.Install(new DependenciesInstaller());

      var contactId = new ID();
      var identifiers = Substitute.For<Sitecore.Analytics.Model.Entities.IContactIdentifiers>();
      var contactModel = Substitute.For<Sitecore.Analytics.Model.Entities.IContact>();
      contactModel.Identifiers.Returns(identifiers);
      contactModel.Identifiers.Identifier.Returns(string.Empty);
      contactModel.Id.Returns(contactId);
      var contact = new ContactContext(contactModel);

      var tracker = Substitute.For<ITracker>();
      tracker.Contact.Returns(contact);

      using (new Switcher<ITracker, TrackerSwitcher>(tracker))
      {
        // act
        var service = this.container.Resolve<ICartService>();

        // assert
        ((CartService)service).ShopName.Should().Be("autohaus");
        ((CartService)service).UserId.Should().Be("Connor John");
      }
    }

    /// <summary>
    /// Should inject context database into product service.
    /// </summary>
    [Fact]
    public void ShouldInjectContextDatabaseIntoProductService()
    {
      // arrange
      this.container.Install(new DependenciesInstaller());

      // act
      var service = this.container.Resolve<IProductService>();

      // assert
      ((ProductService)service).Database.Should().Be(this.tree.Database);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_isDisposed) return;
      if (disposing)
      {
        this.siteContextSwitcher.Dispose();
        this.tree.Dispose();
        this.container.Dispose();
      }
      _isDisposed = true;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DependenciesInstallerTest"/> class.
    /// </summary>
    ~DependenciesInstallerTest()
    {
      Dispose(false);
    }
  }
}