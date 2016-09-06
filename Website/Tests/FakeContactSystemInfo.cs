// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeContact.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the FakeContact class.
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
namespace Sitecore.Commerce.StarterKit.Tests
{
    using Sitecore.Analytics.Model.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FakeContactSystemInfo : IContactSystemInfo
    {
        public Analytics.Model.AuthenticationLevel AuthenticationLevel { get; set; }
        public int Classification { get; set; }
        public Guid IntegrationId { get; set; }
        public string IntegrationLabel { get; set; }
        public int OverrideClassification { get; set; }
        public int Value { get; set; }
        public int VisitCount { get; set; }
        public bool IsEmpty { get { return false; } }
        public Analytics.Model.Framework.IModelMemberCollection Members { get { return null; } }
        public void Reset() { }
        public void Traverse(string path, Analytics.Model.Framework.IModelMemberVisitor visitor) { }
        public void Validate() { }
    }
}
