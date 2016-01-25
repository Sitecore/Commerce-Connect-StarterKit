// ---------------------------------------------------------------------
// <copyright file="ProductService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The default product service.
// </summary>
// ---------------------------------------------------------------------
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
namespace Sitecore.Commerce.StarterKit.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using Data.Products;
  using Resources.Media;

  using Sitecore.Commerce.StarterKit.Models;
  using Sitecore.Configuration;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Linq;
  using Sitecore.ContentSearch.SearchTypes;
  using Sitecore.ContentSearch.Utilities;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Globalization;
  using Sitecore.Data.Fields;
  using Sitecore.Diagnostics;

  /// <summary>
  /// The default product service.
  /// </summary>
  public class ProductService : IProductService
  {
    /// <summary>
    /// The product template identifier.
    /// </summary>
    private readonly ID productTemplateId = ID.Parse(Configuration.Settings.GetSetting("IDs.ProductTemplateId", "{47D1A39E-3B4B-4428-A9F8-B446256C9581}"));

    /// <summary>
    /// Product classification template ID
    /// </summary>
    private readonly ID _productClassificationTemplateId;

    /// <summary>
    /// The database.
    /// </summary>
    private readonly Database database;

    /// <summary>
    /// The language.
    /// </summary>
    private readonly Language language;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService" /> class.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="language">The language.</param>
    public ProductService(Database database, Language language)
    {
      this.database = database;
      this.language = language;

      string id = Factory.CreateObject("includeTemplates/ProductClassificationTemplateId", false) as string;

      this._productClassificationTemplateId = !string.IsNullOrEmpty(id)? new ID(id) : new ID("{D5B1659A-83E7-485F-AD9A-555EECB564BF}");
    }

    /// <summary>
    /// Gets the database.
    /// </summary>
    /// <value>The database.</value>
    public Database Database
    {
      get { return this.database; }
    }

    /// <summary>
    /// Gets the language.
    /// </summary>
    /// <value>The language.</value>
    public Language Language
    {
      get { return this.language; }
    }

    /// <summary>
    /// The read product.
    /// </summary>
    /// <param name="productId">The product id.</param>
    /// <returns>The <see cref="Item" />.</returns>
    public Item ReadProduct(string productId)
    {
      using (var context = CreateSearchContext())
      {
        var searchResultItem = context.GetQueryable<SearchResultItem>()
          .Where(p => p.Language == this.Language.Name && p["_latestversion"] == "1")
          .FirstOrDefault(item => item["ExternalID"] == productId && item.TemplateId == this.productTemplateId);

        return searchResultItem != null ? searchResultItem.GetItem() : null;
      }
    }

    public IEnumerable<Item> GetProducts(IEnumerable<SearchStringModel> searchStringModel)
    {
      var templateId = this.productTemplateId;//.ToGuid().ToString("N");
      using (var context = CreateSearchContext())
      {
        return LinqHelper.CreateQuery<SearchResultItem>(context, searchStringModel)
        .Where(p => p.Language == this.Language.Name && p["_latestversion"] == "1" && p.TemplateId == templateId)
        .Select(searchItem => searchItem.GetItem()).ToList();
      }
    }

    /// <summary>
    /// Gets the products.
    /// </summary>
    /// <param name="searchStringModel">The search string model.</param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>The list of the product items.</returns>
    public PagedList<Item> GetProducts(IEnumerable<SearchStringModel> searchStringModel, int page, int pageSize)
    {
      Assert.ArgumentCondition(page >= 0, "page","page is less than 0");
      Assert.ArgumentCondition(pageSize > 1, "pageSize", "pageSize is less than 1");

      var templateId = this.productTemplateId;//.ToGuid().ToString("N");
      using (var context = CreateSearchContext())
      {
        var query = LinqHelper.CreateQuery<SearchResultItem>(context, searchStringModel)
          .Where(p => p.Language == this.Language.Name && p["_latestversion"] == "1" && p.TemplateId == templateId);

        int totalCount = query.Count();

        var products = query.Page(page, pageSize).Select(searchItem => searchItem.GetItem()).ToList();

        return new PagedList<Item>(products, page, pageSize, totalCount);
      }
    }

    /// <summary>
    /// Get category ietem by external id
    /// </summary>
    /// <param name="categoryExternalId">The category external id</param>
    /// <returns>Category item</returns>
    public Item GetCategory([NotNull] string categoryExternalId)
    {
      using (var context = CreateSearchContext())
      {
        var searchItem = context.GetQueryable<SearchResultItem>()
          .FirstOrDefault(item => item.TemplateId == this._productClassificationTemplateId && item["externalid"] == categoryExternalId);

        if (searchItem != null)
        {
          return searchItem.GetItem();
        }
      }

      return null;
    }

    /// <summary>
    /// Get related categories IDs
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <returns>Category IDs</returns>
    public List<Item> GetRelatedCategories([NotNull] ID categoryId)
    {
      using (var context = CreateSearchContext())
      {
        var categories =
          context.GetQueryable<SearchResultItem>()
            .Where(item => item.Paths.Contains(categoryId))
            .Select(i => i.GetItem())
            .ToList() //required ToList() for mapping from index
            .Where(item => item != null && item.ID != categoryId);

        return new List<Item>(categories);
      }
    }

    /// <summary>
    /// Creates the search context.
    /// </summary>
    /// <returns></returns>
    private static IProviderSearchContext CreateSearchContext()
    {
      return ContentSearchManager.GetIndex("commerce_products_web_index").CreateSearchContext();
    }

    /// <summary>
    /// Get Resources items by product id
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [CanBeNull]
    public List<Item> GetResources([NotNull]string productId)
    {
      var productItem = this.ReadProduct(productId);
      if (productItem == null)
      {
        return null;
      }

      return this.GetResources(productItem);
    }

    /// <summary>
    /// Get Resources items by product item
    /// </summary>
    /// <param name="productItem"></param>
    /// <returns></returns>
    [CanBeNull]
    public List<Item> GetResources([NotNull]Item productItem)
    {
      var children = productItem.Children;
      if (children == null)
      {
        return null;
      }

      var resourceFolderItem = children.FirstOrDefault(c => c.Name == "Resources");
      if (resourceFolderItem == null)
      {
        return null;
      }

      return resourceFolderItem.Children.ToList();
    }

    /// <summary>
    /// Get Image by product item
    /// </summary>
    /// <param name="productItem"></param>
    /// <returns></returns>
    [NotNull]
    public string GetImage([NotNull]Item productItem)
    {
      var resources = this.GetResources(productItem);
      if (resources == null)
      {
        return string.Empty;
      }

      var resourceItem = resources.FirstOrDefault(c => c["Type"] == ResourceTypes.Image);
      if (resourceItem == null)
      {
        return string.Empty;
      }

      return GetImageUrl(resourceItem);

    }

    /// <summary>
    /// Get Images by product item
    /// </summary>
    /// <param name="productItem"></param>
    /// <returns></returns>
    [NotNull]
    public List<string> GetImages([NotNull]Item productItem)
    {
      var resources = this.GetResources(productItem);
      if (resources == null)
      {
        return new List<string>(0);
      }

      var resourceItems = resources.Where(c => c["Type"] == ResourceTypes.Image || c["Type"] == "Alt image");
      var list = new List<string>(0);
      foreach (var resourceItem in resourceItems)
      {
        list.Add(this.GetImageUrl(resourceItem));
      }

      return list;
    }

    /// <summary>
    /// Get image by source item
    /// </summary>
    /// <param name="resourceItem"></param>
    /// <returns></returns>
    protected virtual string GetImageUrl(Item resourceItem)
    {
      if (!string.IsNullOrEmpty(resourceItem["URI"]))
      {
        return resourceItem["URI"];
      }

      if (resourceItem.Fields["Resource"] == null)
      {
        return string.Empty;
      }

      ImageField imageField = resourceItem.Fields["Resource"];
      var mediaOptions = new MediaUrlOptions { AbsolutePath = true, UseItemPath = false };
      var mediaUrl = MediaManager.GetMediaUrl(imageField.MediaItem, mediaOptions);
      return mediaUrl;
    }
  }
}