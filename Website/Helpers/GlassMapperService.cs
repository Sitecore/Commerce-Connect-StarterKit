// ----------------------------------------------------------------------------------------------
// <copyright file="GlassMapperService.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines GlassMapperService class.
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
namespace Sitecore.Commerce.StarterKit.Helpers
{
  using Glass.Sitecore.Mapper;
  using Sitecore;

  /// <summary>
  /// Defines GlassMapperService class.
  /// </summary>
  public static class GlassMapperService
  {
    /// <summary>
    /// Instance of ISitecoreService.
    /// </summary>
    private static ISitecoreService sitecoreService;

    /// <summary>
    /// Gets or sets ISitecoreService instance to use.
    /// </summary>
    /// <value>ISitecoreService instance to use.</value>
    [NotNull]
    public static ISitecoreService Current
    {
      get { return sitecoreService ?? (sitecoreService = new SitecoreService(Sitecore.Context.Database)); }
      set { sitecoreService = value; }
    }
  }
}