// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrototypeFactory.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the class which creates object clones from prototype.
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
namespace Sitecore.Commerce.Nop.Tests
{
  using System;
  using System.Reflection;

  /// <summary>
  /// Defines the class which creates object clones from prototype.
  /// </summary>
  public static class PrototypeFactory
  {
    /// <summary>
    /// Clones the specified object to clone.
    /// </summary>
    /// <typeparam name="T">Type of the cloned object.</typeparam>
    /// <param name="objectToClone">The object to clone.</param>
    /// <returns>The clone of the object.</returns>
    public static T Clone<T>(T objectToClone) where T : new()
    {
      return (T)CloneInternal(typeof(T), objectToClone);
    }

    /// <summary>
    /// Clones the internal.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="objectToClone">The object to clone.</param>
    /// <returns>The clone of the object.</returns>
    /// <exception cref="System.InvalidOperationException">Thrown when it is impossible to instantiate object of the specified type.</exception>
    private static object CloneInternal(Type type, object objectToClone)
    {
      if (type.GetConstructor(new Type[0]) == null)
      {
        throw new InvalidOperationException(string.Format("Cannot create an instance of {0} type", type.FullName));
      }

      if (objectToClone == null)
      {
        return null;
      }

      var result = Activator.CreateInstance(type);

      foreach (var propertyInfo in type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public))
      {
          if (!propertyInfo.CanRead || !propertyInfo.CanWrite) continue;
          var value = propertyInfo.GetValue(objectToClone);

          if (value != null)
          {
            if ((propertyInfo.PropertyType != value.GetType()) && (!(propertyInfo.PropertyType.IsInterface || propertyInfo.PropertyType.IsAbstract || propertyInfo.PropertyType.IsValueType)))
              {
                  value = CloneInternal(propertyInfo.PropertyType, value);
              }
          }

          propertyInfo.SetValue(result, value);

          if (propertyInfo.PropertyType.IsEnum && (!Enum.IsDefined(propertyInfo.PropertyType, propertyInfo.GetValue(result))))
          {
              propertyInfo.SetValue(result, Enum.GetValues(propertyInfo.PropertyType).GetValue(0));
          }
      }

      return result;
    }
  }
}