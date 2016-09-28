// -----------------------------------------------------------------
// <copyright file="CloneManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The CloneManager class.
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

  internal static class CloneManager<T>
  {
    private static Func<T, CloningFlags, IDictionary<Type, Func<object, object>>, T> _clone;

    private static readonly IDictionary<Type, Func<object, object>> _emptyCustomInitializersDictionary = new Dictionary<Type, Func<object, object>>();

    static CloneManager()
    {
      var factory = new ExpressionFactory<T>();
      _clone = factory.GetCloneFunc();
    }

    public static T Clone(T source, CloningFlags flags)
    {
      return _clone(source, flags, _emptyCustomInitializersDictionary);
    }

    public static T Clone(T source, CloningFlags flags, IDictionary<Type, Func<object, object>> initializers)
    {
      if (initializers == null)
        throw new ArgumentNullException();

      return _clone(source, flags, initializers);
    }
  }
}
