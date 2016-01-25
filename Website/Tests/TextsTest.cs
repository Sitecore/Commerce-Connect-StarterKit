// ----------------------------------------------------------------------------------------------
// <copyright file="TextsTest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   The texts test.
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
namespace Sitecore.Commerce.StarterKit.Tests
{
  using System.Reflection;
  using FluentAssertions;
  using Sitecore.Commerce.StarterKit;
  using Xunit;

  /// <summary>
  /// The texts test.
  /// </summary>
  public class TextsTest
  {
    /// <summary>
    /// Should contain only string constants.
    /// </summary>
    [Fact]
    public void ShouldContainOnlyStringConstants()
    {
      // arrange
      var type = typeof(Texts);

      // Act
      FieldInfo[] fieldInfos = type.GetFields();

      // Assert
      foreach (FieldInfo fi in fieldInfos)
      {
        (fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string)).Should().BeTrue(string.Format("{0} is not string constant", fi.Name));
      }
    }
  }
}