// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductsService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The implementation of Product Information Service.
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
// -----------------------------------------------------------------
namespace Nop.Plugin.Sitecore.Commerce.Products
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel.Activation;
    using System.Web.Services;
    using Nop.Core.Data;
    using Nop.Core.Domain.Catalog;
    using Nop.Core.Domain.Media;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Sitecore.Commerce.Products.Models;
    using Nop.Services.Catalog;
    using Nop.Services.Localization;
    using Nop.Services.Media;
    using Nop.Services.Seo;
    using Nop.Services.Stores;

    /// <summary>
    ///   The implementation of Product Information Service.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ProductsService : IProductsService
    {
        /// <summary>
        ///   The category service
        /// </summary>
        private readonly ICategoryService categoryService;

        /// <summary>
        ///   The cross sell product repository
        /// </summary>
        private readonly IRepository<CrossSellProduct> crossSellProductRepository;

        /// <summary>
        ///   The download repository.
        /// </summary>
        private readonly IRepository<Download> downloadRepository;

        /// <summary>
        ///   The download service
        /// </summary>
        private readonly IDownloadService downloadService;

        /// <summary>
        ///   The language service.
        /// </summary>
        private readonly ILanguageService languageService;

        /// <summary>
        ///   The localized entity service.
        /// </summary>
        private readonly ILocalizedEntityService localizedEntityService;

        /// <summary>
        ///   The manufacturer service
        /// </summary>
        private readonly IManufacturerService manufacturerService;

        /// <summary>
        ///   The picture service
        /// </summary>
        private readonly IPictureService pictureService;

        /// <summary>
        ///   The product repository
        /// </summary>
        private readonly IRepository<Product> productRepository;

        /// <summary>
        ///   The product service.
        /// </summary>
        private readonly IProductService productService;

        /// <summary>
        ///   The related product repository
        /// </summary>
        private readonly IRepository<RelatedProduct> relatedProductRepository;

        /// <summary>
        ///   The specification attribute service
        /// </summary>
        private readonly ISpecificationAttributeService specificationAttributeService;

        /// <summary>
        ///   The store mapping service
        /// </summary>
        private readonly IStoreMappingService storeMappingService;

        /// <summary>
        ///   The store service
        /// </summary>
        private readonly IStoreService storeService;

        private readonly IRepository<ProductTemplate> templateRepository;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ProductsService" /> class.
        /// </summary>
        public ProductsService()
        {
            this.productService = EngineContext.Current.Resolve<IProductService>();
            this.manufacturerService = EngineContext.Current.Resolve<IManufacturerService>();
            this.categoryService = EngineContext.Current.Resolve<ICategoryService>();
            this.specificationAttributeService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            this.productRepository = EngineContext.Current.Resolve<IRepository<Product>>();
            this.relatedProductRepository = EngineContext.Current.Resolve<IRepository<RelatedProduct>>();
            this.crossSellProductRepository = EngineContext.Current.Resolve<IRepository<CrossSellProduct>>();

            this.templateRepository = EngineContext.Current.Resolve<IRepository<ProductTemplate>>();

            this.downloadRepository = EngineContext.Current.Resolve<IRepository<Download>>();

            this.storeService = EngineContext.Current.Resolve<IStoreService>();
            this.storeMappingService = EngineContext.Current.Resolve<IStoreMappingService>();

            this.pictureService = EngineContext.Current.Resolve<IPictureService>();

            this.downloadService = EngineContext.Current.Resolve<IDownloadService>();
            this.languageService = EngineContext.Current.Resolve<ILanguageService>();

            this.localizedEntityService = EngineContext.Current.Resolve<ILocalizedEntityService>();
        }

        /// <summary>
        ///   Gets the product by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns>
        ///   Instance of <see cref="ProductModel" />
        /// </returns>
        [WebMethod(EnableSession = false)]
        public virtual ProductModel GetProduct(string externalProductId, string languageCode)
        {
            int productId;

            if (int.TryParse(externalProductId, out productId))
            {
                var product = this.productService.GetProductById(productId);

                if (product == null)
                {
                    return new ProductModel();
                }

                var languages = this.languageService.GetAllLanguages();
                var language = languages.FirstOrDefault(l => l.UniqueSeoCode == languageCode);
                string name;
                string shortDescription;
                string fullDescription;
                if (language != null)
                {
                    name = product.GetLocalized(x => x.Name, language.Id);
                    shortDescription = product.GetLocalized(x => x.ShortDescription, language.Id);
                    fullDescription = product.GetLocalized(x => x.FullDescription, language.Id);
                }
                else
                {
                    name = product.Name;
                    shortDescription = product.ShortDescription;
                    fullDescription = product.FullDescription;
                }

                return new ProductModel
                {
                    ProductId = product.Id.ToString(CultureInfo.InvariantCulture),
                    Name = name,
                    ShortDescription = shortDescription,
                    FullDescription = fullDescription,
                    AdminComment = product.AdminComment,
                    ShowOnHomePage = product.ShowOnHomePage,
                    MetaKeywords = product.MetaKeywords,
                    MetaDescription = product.MetaDescription,
                    MetaTitle = product.MetaTitle,
                    SeName = product.GetSeName(0),
                    ProductVariantId = product.Id.ToString(CultureInfo.InvariantCulture),
                    ProductVariantName = product.Name,
                    Sku = product.Sku,
                    Description = product.MetaDescription,
                    ManufacturerPartNumber = product.ManufacturerPartNumber,
                    Gtin = product.Gtin,
                    Weight = product.Weight,
                    Length = product.Length,
                    Width = product.Width,
                    Height = product.Height,
                    Deleted = product.Deleted,
                    Published = product.Published,
                    CreatedOnUtc = product.CreatedOnUtc,
                    UpdatedOnUtc = product.UpdatedOnUtc,
                    ManufacturerIds = product.ProductManufacturers.Select(x => x.ManufacturerId.ToString(CultureInfo.InvariantCulture)).ToList(),
                    CategoryIds = product.ProductCategories.Select(x => x.CategoryId.ToString(CultureInfo.InvariantCulture)).ToList(),
                    ProductGlobalSpecifications = product.ProductSpecificationAttributes.Select(
                      x =>
                        new ProductGlobalSpecificationModel
                        {
                            SpecificationLookupId = x.SpecificationAttributeOption.SpecificationAttributeId.ToString(CultureInfo.InvariantCulture),
                            LookupValueId = x.SpecificationAttributeOptionId.ToString(CultureInfo.InvariantCulture),
                            CustomValue = x.CustomValue,
                            SpecificationLookupName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                            LookupValueName = x.SpecificationAttributeOption.Name
                        }).ToList()
                };
            }

            return new ProductModel();
        }

        /// <summary>
        ///   Gets all product ids.
        /// </summary>
        /// <returns>The list of product identifiers.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IList<string> GetAllProductsIds()
        {
            var products = this.GetAllProducts(true);

            return products.Select(product => product.Id.ToString(CultureInfo.InvariantCulture)).ToList();
        }

        /// <summary>
        ///   Gets the manufacturer by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The list of manufacturers.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IQueryable<ManufacturerModel> GetManufacturer(string externalProductId)
        {
            int productId;

            if (!int.TryParse(externalProductId, out productId))
            {
                return new List<ManufacturerModel>().AsQueryable();
            }

            var manufacturer = this.manufacturerService.GetProductManufacturersByProductId(this.productService.GetProductById(productId).Id, true);

            return manufacturer.Select(x => new ManufacturerModel
            {
                Id = x.Manufacturer.Id.ToString(CultureInfo.InvariantCulture),
                Name = x.Manufacturer.Name,
                Description = x.Manufacturer.Description,
                Published = x.Manufacturer.Published,
                CreatedOnUtc = x.Manufacturer.CreatedOnUtc,
                UpdatedOnUtc = x.Manufacturer.UpdatedOnUtc
            }).AsQueryable();
        }

        /// <summary>
        ///   Gets all manufacturers.
        /// </summary>
        /// <returns>The list of manufacturers.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IQueryable<ManufacturerModel> GetAllManufacturers()
        {
            return this.manufacturerService.GetAllManufacturers(showHidden: true).Select(x => new ManufacturerModel
            {
                Id = x.Id.ToString(CultureInfo.InvariantCulture),
                Name = x.Name,
                Description = x.Description,
                Published = x.Published,
                CreatedOnUtc = x.CreatedOnUtc,
                UpdatedOnUtc = x.UpdatedOnUtc
            }).AsQueryable();
        }

        /// <summary>
        ///   Gets all product types.
        /// </summary>
        /// <returns>The list of product types.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IQueryable<ProductTypeModel> GetAllProductTypes()
        {
            return this.categoryService.GetAllCategories(showHidden: true).Select(x => new ProductTypeModel
            {
                Id = x.Id.ToString(CultureInfo.InvariantCulture),
                Name = x.Name,
                Description = x.Description,
                MetaKeywords = x.MetaKeywords,
                MetaDescription = x.MetaDescription,
                MetaTitle = x.MetaTitle,
                ParentProductTypeId = x.ParentCategoryId.ToString(CultureInfo.InvariantCulture),
                Published = x.Published,
                CreatedOnUtc = x.CreatedOnUtc,
                UpdatedOnUtc = x.UpdatedOnUtc
            }).AsQueryable();
        }

        /// <summary>
        ///   Gets all categories.
        /// </summary>
        /// <returns>The list of categories.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IQueryable<CategoryModel> GetAllCategories()
        {
            return this.categoryService.GetAllCategories(showHidden: true).Select(x => new CategoryModel
            {
                Id = x.Id.ToString(CultureInfo.InvariantCulture),
                Name = x.Name,
                Description = x.Description,
                MetaKeywords = x.MetaKeywords,
                MetaDescription = x.MetaDescription,
                MetaTitle = x.MetaTitle,
                ParentCategoryId = x.ParentCategoryId.ToString(CultureInfo.InvariantCulture),
                Published = x.Published,
                CreatedOnUtc = x.CreatedOnUtc,
                UpdatedOnUtc = x.UpdatedOnUtc
            }).AsQueryable();
        }

        /// <summary>
        ///   Gets the product types by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The list of product types by external product id.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IQueryable<ProductTypeModel> GetProductTypes(string externalProductId)
        {
            int productId;

            if (int.TryParse(externalProductId, out productId))
            {
                return this.categoryService.GetProductCategoriesByProductId(this.productService.GetProductById(productId).Id, true).Select(x => new ProductTypeModel
                {
                    Id = x.Category.Id.ToString(CultureInfo.InvariantCulture),
                    Name = x.Category.Name,
                    Description = x.Category.Description,
                    MetaKeywords = x.Category.MetaKeywords,
                    MetaDescription = x.Category.MetaDescription,
                    MetaTitle = x.Category.MetaTitle,
                    ParentProductTypeId = x.Category.ParentCategoryId.ToString(CultureInfo.InvariantCulture),
                    Published = x.Category.Published,
                    CreatedOnUtc = x.Category.CreatedOnUtc,
                    UpdatedOnUtc = x.Category.UpdatedOnUtc
                }).AsQueryable();
            }

            return new List<ProductTypeModel>().AsQueryable();
        }

        /// <summary>
        ///   Gets the categories by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The list of categories by product id.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IQueryable<CategoryModel> GetCategories(string externalProductId)
        {
            int productId;

            if (int.TryParse(externalProductId, out productId))
            {
                return this.categoryService.GetProductCategoriesByProductId(this.productService.GetProductById(productId).Id, true).Select(x => new CategoryModel
                {
                    Id = x.Category.Id.ToString(CultureInfo.InvariantCulture),
                    Name = x.Category.Name,
                    Description = x.Category.Description,
                    MetaKeywords = x.Category.MetaKeywords,
                    MetaDescription = x.Category.MetaDescription,
                    MetaTitle = x.Category.MetaTitle,
                    ParentCategoryId = x.Category.ParentCategoryId.ToString(CultureInfo.InvariantCulture),
                    Published = x.Category.Published,
                    CreatedOnUtc = x.Category.CreatedOnUtc,
                    UpdatedOnUtc = x.Category.UpdatedOnUtc
                }).AsQueryable();
            }

            return new List<CategoryModel>().AsQueryable();
        }

        /// <summary>
        ///   Gets all global specifications.
        /// </summary>
        /// <returns>The list of global specifications.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IQueryable<SpecificationLookupModel> GetAllGlobalSpecifications()
        {
            return this.specificationAttributeService.GetSpecificationAttributes().Select(x => new SpecificationLookupModel
            {
                Id = x.Id.ToString(CultureInfo.InvariantCulture),
                Name = x.Name,
                SpecificationOptions = x.SpecificationAttributeOptions.Select(y => new LookupValueModel
                {
                    Id = y.Id.ToString(CultureInfo.InvariantCulture),
                    ParentId = x.Id.ToString(CultureInfo.InvariantCulture),
                    Name = y.Name
                }).ToList()
            }).AsQueryable();
        }

        /// <summary>
        ///   Gets the product global specifications by external product unique identifier.
        /// </summary>
        /// <param name="externalProductId">The external product unique identifier.</param>
        /// <returns>The list of product global specification by external product id.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IList<ProductGlobalSpecificationModel> GetProductGlobalSpecifications(string externalProductId)
        {
            int productId;
            var returnValue = new List<ProductGlobalSpecificationModel>();
            if (!int.TryParse(externalProductId, out productId))
            {
                return returnValue;
            }

            var product = this.productService.GetProductById(productId);

            if (product == null)
            {
                return returnValue;
            }

            return
              product.ProductSpecificationAttributes.Select(
                x =>
                  new ProductGlobalSpecificationModel
                  {
                      SpecificationLookupId =
                        x.SpecificationAttributeOption.SpecificationAttributeId.ToString(
                          CultureInfo.InvariantCulture),
                      LookupValueId =
                        x.SpecificationAttributeOptionId.ToString(
                          CultureInfo.InvariantCulture),
                      CustomValue = x.CustomValue,
                      SpecificationLookupName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                      LookupValueName = x.SpecificationAttributeOption.Name
                  }).ToList();
        }

        /// <summary>
        ///   Gets the related product ids by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The list of related product identifiers.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IList<string> GetRelatedProductsIds(string externalProductId)
        {
            int productId;
            var returnValues = new List<string>();
            if (!int.TryParse(externalProductId, out productId))
            {
                return returnValues;
            }

            var product = this.productService.GetProductById(productId);

            if (product == null)
            {
                return returnValues;
            }

            var relatedProductPairs = this.GetRelatedProductsByProductId(product.Id);

            if (relatedProductPairs.Any())
            {
                returnValues.AddRange(relatedProductPairs.Select(relatedProductPair => relatedProductPair.ProductId2.ToString(CultureInfo.InvariantCulture)));
            }

            return returnValues;
        }

        /// <summary>
        ///   Gets the cross sell product ids by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The cross-sell product identifiers.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IList<string> GetCrossSellProductsIds(string externalProductId)
        {
            int productId;
            var returnValues = new List<string>();
            if (!int.TryParse(externalProductId, out productId))
            {
                return returnValues;
            }

            var product = this.productService.GetProductById(productId);

            if (product == null)
            {
                return returnValues;
            }

            var crossSellProductPairs = this.GetCrossSellProductsByProductId(product.Id);

            if (crossSellProductPairs.Any())
            {
                returnValues.AddRange(crossSellProductPairs.Select(crossSellProductPair => crossSellProductPair.ProductId2.ToString(CultureInfo.InvariantCulture)));
            }

            return returnValues;
        }

        /// <summary>
        ///   Gets the variant product ids by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>
        ///   The list of product variant attributes.
        /// </returns>
        [WebMethod(EnableSession = false)]
        public virtual IList<string> GetVariantProductsIds(string externalProductId)
        {
            int productId;
            var returnValue = new List<string>();
            if (!int.TryParse(externalProductId, out productId))
            {
                return returnValue;
            }

            var product = this.productService.GetProductById(productId);

            if (product == null)
            {
                return returnValue;
            }

            if (product.ProductVariantAttributes.Any(p => p.Id != productId))
            {
                returnValue.AddRange(
                  product.ProductVariantAttributes.Where(p => p.Id != productId)
                    .Select(variantProduct => variantProduct.Id.ToString(CultureInfo.InvariantCulture)));
            }

            return returnValue;
        }

        /// <summary>
        ///   Gets all divisions.
        /// </summary>
        /// <returns>The list of all divisions.</returns>
        [WebMethod(EnableSession = false)]
        public virtual IList<DivisionModel> GetAllDivisions()
        {
            return this.storeService.GetAllStores().Select(x => new DivisionModel
            {
                Id = x.Id.ToString(CultureInfo.InvariantCulture),
                Name = x.Name
            }).ToList();
        }

        /// <summary>
        ///   Gets the related divisions by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The list of related divisions.</returns>
        [WebMethod(EnableSession = false)]
        public virtual ProductDivisionsModel GetRelatedDivisions(string externalProductId)
        {
            int productId;
            if (!int.TryParse(externalProductId, out productId))
            {
                return new ProductDivisionsModel
                {
                    ProductId = externalProductId,
                    Divisions = new List<DivisionModel>()
                };
            }

            var product = this.productService.GetProductById(productId);

            if (product == null)
            {
                return new ProductDivisionsModel
                {
                    ProductId = externalProductId,
                    Divisions = new List<DivisionModel>()
                };
            }

            if (product.LimitedToStores)
            {
                return new ProductDivisionsModel
                {
                    ProductId = externalProductId,
                    Divisions = this.storeMappingService.GetStoreMappings(product).Select(x => new DivisionModel
                    {
                        Id = x.StoreId.ToString(CultureInfo.InvariantCulture)
                    }).ToList()
                };
            }

            return new ProductDivisionsModel
            {
                ProductId = externalProductId,
                Divisions = this.GetAllDivisions()
            };
        }

        /// <summary>
        ///   Gets the product main image by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The main image of the product.</returns>
        [WebMethod(EnableSession = false)]
        public virtual ProductResourceModel GetProductMainImage(string externalProductId)
        {
            int productId;
            if (!int.TryParse(externalProductId, out productId))
            {
                return new ProductResourceModel();
            }

            var product = this.productService.GetProductById(productId);

            if (product == null)
            {
                return new ProductResourceModel();
            }

            var productResourceModel = new ProductResourceModel
            {
                ProductId = externalProductId,
                ProductResourceType = "Image",
                Resources = new List<ResourceModel>()
            };

            var mainProductPictureId = GetMainImageIdByProduct(product);
            if (mainProductPictureId == 0 || mainProductPictureId == int.MaxValue || mainProductPictureId == int.MinValue)
            {
                return productResourceModel;
            }
            
            var picture = this.pictureService.GetPictureById(mainProductPictureId);
            productResourceModel.Resources.Add(new ResourceModel
            {
                Url = this.pictureService.GetPictureUrl(picture.Id),
                Name = picture.SeoFilename,
                ResourceType = "Image",
                Id = picture.Id.ToString(CultureInfo.InvariantCulture)
            });

            return productResourceModel;
        }

        /// <summary>
        ///   Gets the product alternate images by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The list of product alternate images.</returns>
        [WebMethod(EnableSession = false)]
        public virtual ProductResourceModel GetProductAlternateImages(string externalProductId)
        {
            int productId;
            if (!int.TryParse(externalProductId, out productId))
            {
                return new ProductResourceModel();
            }

            var product = this.productService.GetProductById(productId);

            if (product == null)
            {
                return new ProductResourceModel();
            }

            var productResourceModel = new ProductResourceModel
            {
                ProductId = externalProductId,
                ProductResourceType = "Alt image",
                Resources = new List<ResourceModel>()
            };
            var mainProductPictureId = GetMainImageIdByProduct(product);

            if (mainProductPictureId == 0 || mainProductPictureId >= int.MaxValue || mainProductPictureId <= int.MinValue)
            {
                return productResourceModel;
            }

            var pictureIds = GetAlternateImagesByProductVariantAndMainImageId(product, mainProductPictureId);
            foreach (var picture in pictureIds.Select(pictureId => this.pictureService.GetPictureById(pictureId)))
            {
                productResourceModel.Resources.Add(new ResourceModel
                {
                    Url = this.pictureService.GetPictureUrl(picture.Id),
                    Name = picture.SeoFilename,
                    ResourceType = "Alt image",
                    Id = picture.Id.ToString(CultureInfo.InvariantCulture)
                });
            }

            return productResourceModel;
        }

        /// <summary>
        ///   Gets the product downloads by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The list of product downloads.</returns>
        [WebMethod(EnableSession = false)]
        public virtual ProductResourceModel GetProductDownloads(string externalProductId)
        {
            int productId;
            if (!int.TryParse(externalProductId, out productId))
            {
                return new ProductResourceModel();
            }

            var productVariant = this.productService.GetProductById(productId);

            if (productVariant == null || !productVariant.IsDownload)
            {
                return new ProductResourceModel();
            }

            var download = this.downloadService.GetDownloadById(productVariant.DownloadId);
            var productResourceModel = new ProductResourceModel
            {
                ProductId = externalProductId,
                ProductResourceType = "Download",
                Resources = new List<ResourceModel>()
            };
            var resourceModel = new ResourceModel
            {
                Url = download.DownloadUrl,
                ResourceType = "Download",
                Id = download.Id.ToString(CultureInfo.InvariantCulture),
                Name = download.UseDownloadUrl ? download.DownloadGuid.ToString() : download.Filename
            };

            productResourceModel.Resources.Add(resourceModel);
            return productResourceModel;
        }

        /// <summary>
        ///   Gets the product manufacturers by external product id.
        /// </summary>
        /// <param name="externalProductId">The external product id.</param>
        /// <returns>The list of product manufacturers.</returns>
        [WebMethod(EnableSession = false)]
        public virtual ProductManufacturersModel GetProductManufacturers(string externalProductId)
        {
            int productId;
            if (!int.TryParse(externalProductId, out productId))
            {
                return new ProductManufacturersModel();
            }

            var product = this.productService.GetProductById(productId);

            if (product == null)
            {
                return new ProductManufacturersModel();
            }

            return new ProductManufacturersModel
            {
                ProductId = externalProductId,
                Manufacturers = product.ProductManufacturers.Select(x => new ManufacturerModel
                {
                    Id = x.Manufacturer.Id.ToString(CultureInfo.InvariantCulture),
                    Name = x.Manufacturer.Name,
                    Description = x.Manufacturer.Description,
                    Published = x.Manufacturer.Published,
                    CreatedOnUtc = x.Manufacturer.CreatedOnUtc,
                    UpdatedOnUtc = x.Manufacturer.UpdatedOnUtc
                }).ToList()
            };
        }

        /// <summary>
        ///   Gets all resources.
        /// </summary>
        /// <returns>
        ///   The collection of resources.
        /// </returns>
        [WebMethod(EnableSession = false)]
        public virtual IQueryable<ResourceModel> GetAllResources()
        {
            var pictures = this.pictureService
              .GetPictures(0, int.MaxValue)
              .Select(p => new ResourceModel
              {
                  Id = p.Id.ToString(CultureInfo.InvariantCulture),
                  Name = p.SeoFilename,
                  MimeType = p.MimeType,
                  BinaryData = p.PictureBinary,
                  ResourceType = "Image"
              });

            var downloads = this.downloadRepository
              .Table
              .ToList()
              .Select(d => new ResourceModel
              {
                  Id = d.Id.ToString(CultureInfo.InvariantCulture),
                  Name = d.Filename,
                  MimeType = d.ContentType,
                  BinaryData = d.DownloadBinary,
                  ResourceType = "Download"
              });

            return pictures.Concat(downloads).AsQueryable();
        }

        /// <summary>
        ///   Adds the product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="languageCode">The language code.</param>
        public void UpdateProduct(ProductModel product, string languageCode)
        {
            if (string.IsNullOrEmpty(product.ProductId) || string.IsNullOrEmpty(languageCode))
            {
                return;
            }

            var languages = this.languageService.GetAllLanguages();
            var language = languages.FirstOrDefault(l => l.UniqueSeoCode == languageCode);
            if (language == null)
            {
                return;
            }

            int productId;
            if (!int.TryParse(product.ProductId, out productId))
            {
                return;
            }

            var productToCreateOrUpdate = this.productService.GetProductById(productId);

            if (productToCreateOrUpdate == null)
            {
                return;
            }

            this.localizedEntityService.SaveLocalizedValue(productToCreateOrUpdate, p => p.Name, product.Name, language.Id);
            this.localizedEntityService.SaveLocalizedValue(productToCreateOrUpdate, p => p.ShortDescription, product.ShortDescription, language.Id);
            this.localizedEntityService.SaveLocalizedValue(productToCreateOrUpdate, p => p.FullDescription, product.FullDescription, language.Id);
            if (product.MetaDescription != null)
            {
                productToCreateOrUpdate.MetaDescription = product.MetaDescription;
            }

            productToCreateOrUpdate.Sku = product.Sku;

            this.productService.UpdateProduct(productToCreateOrUpdate);
        }

        /// <summary>
        ///   Gets the main image id by product variant.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>
        ///   The id of main image.
        /// </returns>
        private static int GetMainImageIdByProduct(Product product)
        {
            return product.ProductPictures.Any() ? product.ProductPictures.OrderBy(c => c.DisplayOrder).First().PictureId : 0;
        }

        /// <summary>
        ///   Gets the alternate images by product variant and main image id.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="mainImageId">The main image id.</param>
        /// <returns>
        ///   The list of alternate image identifiers.
        /// </returns>
        private static IEnumerable<int> GetAlternateImagesByProductVariantAndMainImageId(Product product, int mainImageId)
        {
            IList<int> returnValue = new List<int>();

            if (!product.ProductPictures.Select(c => c.PictureId != mainImageId).Any())
            {
                return returnValue;
            }

            foreach (var picture in product.ProductPictures.Where(c => c.PictureId != mainImageId))
            {
                returnValue.Add(picture.PictureId);
            }

            return returnValue;
        }

        /// <summary>
        ///   Gets all products.
        /// </summary>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns>The list of products.</returns>
        private IEnumerable<Product> GetAllProducts(bool showHidden = false)
        {
            var simpleTemplate = this.templateRepository.Table.FirstOrDefault(p => p.Name.ToLower().Trim().Equals("simple product"))
                                 ?? new ProductTemplate
                                 {
                                     Id = 1
                                 };

            var query = from p in this.productRepository.Table
                        orderby p.Name
                        where (showHidden || p.Published) && !p.Deleted &&
                              !p.ProductVariantAttributes.Any() && // MKL: only products without variants are supported
                              p.ProductTemplateId == simpleTemplate.Id // MKL: only simple products are supported.
                        select p;
            var products = query.ToList();
            return products;
        }

        /// <summary>
        ///   Gets the related products by product id.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns>The list of related products.</returns>
        private IList<RelatedProduct> GetRelatedProductsByProductId(int productId)
        {
            var query = from rp in this.relatedProductRepository.Table
                        join p in this.productRepository.Table on rp.ProductId2 equals p.Id
                        where rp.ProductId1 == productId
                        orderby rp.DisplayOrder
                        select rp;
            var relatedProducts = query.ToList();

            return relatedProducts;
        }

        /// <summary>
        ///   Gets the cross sell products by product id.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns>The list of cross-sell products.</returns>
        private IList<CrossSellProduct> GetCrossSellProductsByProductId(int productId)
        {
            var query = from rp in this.crossSellProductRepository.Table
                        join p in this.productRepository.Table on rp.ProductId2 equals p.Id
                        where rp.ProductId1 == productId
                        select rp;
            var crossSellProducts = query.ToList();

            return crossSellProducts;
        }
    }
}