// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomStyles.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CustomStyles class.
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
    /// <summary>
    /// The custom styles.
    /// </summary>
    public class CustomStyles
    {
        /// <summary>
        /// Gets the default styles.
        /// </summary>
        public static CustomStyles Default
        {
            get
            {
                return new CustomStyles()
                {
                    PageWidth = "76.66em",
                    FontSize = "12px",
                    FontFamily = "'Segoe UI'",
                    PageBackgroundColor = "white",
                    LabelColor = "black",
                    TextBackgroundColor = "white",
                    TextColor = "black",
                    DisabledTextBackgroundColor = "#E4E4E4", // light grey,
                    ColumnNumber = 2,
                };
            }
        }

        /// <summary>
        /// Gets or sets the page width, e.g. <c>920px</c> or <c>76.66em</c>.
        /// </summary>
        public string PageWidth { get; set; }

        /// <summary>
        /// Gets or sets the font size, e.g. <c>12px</c>.
        /// </summary>
        public string FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font family, e.g. "Segoe UI".
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the background color of the page, e.g. "white".
        /// </summary>
        public string PageBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the label, e.g. "black".
        /// </summary>
        public string LabelColor { get; set; }

        /// <summary>
        /// Gets or sets the background color of the textbox or the dropdown box, e.g. "white".
        /// </summary>
        public string TextBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the background color of the disabled textbox or the disabled dropdown box, e.g. "#E4E4E4" (light grey).
        /// </summary>
        public string DisabledTextBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the text in textbox or the dropdown box, e.g. "black".
        /// </summary>
        public string TextColor { get; set; }

        /// <summary>
        /// Gets or sets the number of columns, e.g. 1 or 2.
        /// </summary>
        public int ColumnNumber { get; set; }
    }
}