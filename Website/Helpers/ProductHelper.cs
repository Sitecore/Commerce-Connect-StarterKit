// ---------------------------------------------------------------------
// <copyright file="ProductHelper.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the ProductHelper type.
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
namespace Sitecore.Commerce.StarterKit.Helpers
{
  using System.Web;
  using System.Web.Routing;
  using Sitecore;

  /// <summary>
  /// Defines the ProductHelper type.
  /// </summary>
  public class ProductHelper
  {
    /// <summary>
    /// The default product route name.
    /// </summary>
    private const string DefaultProductRouteName = "Products";

    /// <summary>
    /// The HTTP context.
    /// </summary>
    private HttpContextBase httpContext;

    /// <summary>
    /// The route collection.
    /// </summary>
    private RouteCollection routeCollection;

    /// <summary>
    /// The route name.
    /// </summary>
    private string routeName;

    /// <summary>
    /// Gets or sets the HTTP context.
    /// </summary>
    /// <value>
    /// The HTTP context.
    /// </value>
    [CanBeNull]
    public HttpContextBase HttpContext
    {
      get
      {
        if (this.httpContext != null)
        {
          return this.httpContext;
        }

        if (System.Web.HttpContext.Current != null)
        {
          return new HttpContextWrapper(System.Web.HttpContext.Current);
        }

        return null;
      }

      set
      {
        this.httpContext = value;
      }
    }

    /// <summary>
    /// Gets or sets the route collection.
    /// </summary>
    /// <value>The route collection.</value>
    [NotNull]
    public RouteCollection RouteCollection
    {
      get { return this.routeCollection ?? (this.routeCollection = RouteTable.Routes); }
      set { this.routeCollection = value; }
    }

    /// <summary>
    /// Gets or sets the name of the route.
    /// </summary>
    /// <value>The name of the route.</value>
    [NotNull]
    public string RouteName
    {
      get { return this.routeName ?? DefaultProductRouteName; }
      set { this.routeName = value; }
    }

    /// <summary>
    /// Gets the product id from incoming request.
    /// </summary>
    /// <returns>Product id.</returns>
    [CanBeNull]
    public virtual string GetProductIdFromIncomingRequest()
    {
      RouteBase route = this.RouteCollection[this.RouteName];

      if (route == null)
      {
        return null;
      }

      RouteData routeData = route.GetRouteData(this.HttpContext);

      if (routeData == null)
      {
        return null;
      }

      return (string)routeData.Values["id"];
    }
  }
}