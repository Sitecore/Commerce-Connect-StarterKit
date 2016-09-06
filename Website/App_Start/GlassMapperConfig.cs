// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlassMapperConfig.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GlassMapperConfig type.
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
using Sitecore.Commerce.StarterKit.App_Start;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(GlassMapperConfig), "ConfigMapper")]

namespace Sitecore.Commerce.StarterKit.App_Start
{
  /// <summary>
  /// Defines the GlassMapperConfig type.
  /// </summary>
  public static class GlassMapperConfig
  {
    /// <summary>
    /// Configures the mapper.
    /// </summary>
    public static void ConfigMapper()
    {
      var loader = new Glass.Sitecore.Mapper.Configuration.Attributes.AttributeConfigurationLoader("Sitecore.Commerce.StarterKit.Models, Sitecore.Commerce.StarterKit");
      var context = new Glass.Sitecore.Mapper.Context(loader);
    }
  }
}