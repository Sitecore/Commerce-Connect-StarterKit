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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Sitecore.Analytics.Model.Entities;

    public class FakeContact : IContact
    {
        public FakeContact()
        {
            this.Identifiers = new FakeContactIdentifiers();
            this.System = new FakeContactSystemInfo();
        }

        public IContactExtensions Extensions { get { return null; } }

        public IContactBehaviorProfiles BehaviorProfiles { get; set; }
        public IContactIdentifiers Identifiers { get; set; }
        public Analytics.Model.LeaseData Lease { get; set; }
        public IContactSystemInfo System { get; set; }
        public IContactTags Tags { get { return null; } }
        public Analytics.Model.Framework.IFacet AddFacet(string name, Type type) { return null; }
        public TFacet AddFacet<TFacet>(string name) where TFacet : class, Analytics.Model.Framework.IFacet { return null; }
        public IReadOnlyDictionary<string, Analytics.Model.Framework.IFacet> Facets { get { return null; } }
        public TFacet GetFacet<TFacet>(string name) where TFacet : class, Analytics.Model.Framework.IFacet { return null; }
        public Sitecore.Data.ID Id { get; set; }
        public void Traverse(Analytics.Model.Framework.IModelMemberVisitor visitor) { }
        public void Validate() { }
    }
}
