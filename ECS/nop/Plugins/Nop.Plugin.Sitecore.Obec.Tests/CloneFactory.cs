// -----------------------------------------------------------------
// <copyright file="CloneFactory.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The CloneFactory class.
// </summary>
// -----------------------------------------------------------------
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
namespace Nop.Plugin.Sitecore.Commerce.Tests
{
  using System;
  using System.Collections.Generic;

  public static class CloneFactory
  {
    private const CloningFlags _defaultFlags
        = CloningFlags.Fields | CloningFlags.Properties | CloningFlags.CollectionItems;

    private static HashSet<Type> _knownImmutableTypes = new HashSet<Type>() {
            typeof(String), typeof(DateTime), typeof(TimeSpan)
        };

    private static IDictionary<Type, Func<object, object>> _customInitializers = new Dictionary<Type, Func<object, object>>();

    public static CloningFlags DefaultFlags
    {
      get { return _defaultFlags; }
    }

    public static IEnumerable<Type> KnownImmutableTypes
    {
      get { return _knownImmutableTypes; }
    }

    public static IDictionary<Type, Func<object, object>> CustomInitializers
    {
      get { return _customInitializers; }
    }

    public static T GetClone<T>(this T source)
    {
      return GetClone(source, _defaultFlags);
    }

    public static T GetClone<T>(this T source, CloningFlags flags)
    {
      return GetClone(source, flags, CustomInitializers);
    }

    public static T GetClone<T>(this T source, IDictionary<Type, Func<object, object>> initializers)
    {
      return GetClone(source, _defaultFlags, initializers);
    }

    public static T GetClone<T>(this T source, CloningFlags flags, IDictionary<Type, Func<object, object>> initializers)
    {
      if (initializers == null)
        throw new ArgumentNullException();

      return CloneManager<T>.Clone(source, flags, initializers);
    }
  }
}
