// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericErrorPage.aspx.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the GenericErrorPage class.
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

namespace Sitecore.Commerce.PaymentService
{
    using System;

    /// <summary>
    /// Defines a generic error page.
    /// </summary>
    public partial class GenericErrorPage : System.Web.UI.Page
    {
        /// <summary>
        /// Initializes the page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event data.</param>
        protected void Page_Init(object sender, EventArgs e)
        {
            this.ViewStateUserKey = Session.SessionID;
        }
    }
}