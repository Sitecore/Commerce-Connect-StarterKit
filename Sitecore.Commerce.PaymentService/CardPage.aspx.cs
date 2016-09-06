// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CardPage.aspx.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>
//   Defines the CardPage class.
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Newtonsoft.Json;
    using Resources;
    using Sitecore.Commerce.PaymentService.Connector;

    /// <summary>
    /// The card payment page.
    /// </summary>
    public partial class CardPage : System.Web.UI.Page
    {
        private string entryId;
        private CardPaymentEntry entry;
        private bool isSwipe;

        private string track1;
        private string track2;
        private string cardType;
        private string cardNumber;
        private int cardExpirationMonth;
        private int cardExpirationYear;
        private string cardSecurityCode;
        private string voiceAuthorizationCode;
        private string cardHolderName;
        private string cardStreet1;
        private string cardCity;
        private string cardStateOrProvince;
        private string cardPostalCode;
        private string cardCountryOrRegion;
        private decimal paymentAmount;

        /// <summary>
        /// Gets the custom styles.
        /// </summary>
        public CustomStyles CustomStyles { get; private set; }

        /// <summary>
        /// Gets the text direction.
        /// </summary>
        public string TextDirection { get; private set; }

        /// <summary>
        /// Gets the error message for invalid track data.
        /// </summary>
        public string InvalidCardTrackDataMessage
        {
            get { return WebResources.CardPage_InvalidCardTrackData; }
        }

        /// <summary>
        /// Gets the error message for communication error.
        /// </summary>
        public string CommunicationErrorMessage
        {
            get { return WebResources.CardPage_CommunicationError; }
        }

        /// <summary>
        /// Initializes culture (language) of the page.
        /// </summary>
        protected override void InitializeCulture()
        {
            this.entryId = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(this.entryId))
            {
                // Find card payment entry by entry ID.
                var dataManager = new DataManager();
                this.entry = dataManager.GetCardPaymentEntryByEntryId(this.entryId);

                if (this.entry != null && !this.entry.Used)
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(this.entry.EntryLocale);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(this.entry.EntryLocale);
                }
            }

            base.InitializeCulture();
        }

        /// <summary>
        /// Initializes the page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event data.</param>
        protected void Page_Init(object sender, EventArgs e)
        {
            if (this.entry != null)
            {
                this.ViewStateUserKey = this.entry.EntryId;
            }
        }

        /// <summary>
        /// Loads the content of the page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)", Justification = "By Design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design.")]
        protected void Page_Load(object sender, EventArgs e)
        {
            // Enable Right-to-left for some cultures
            this.TextDirection = Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? "rtl" : "ltr";

            // Load custom styles
            this.LoadCustomStyles();

            // Check payment entry ID
            if (string.IsNullOrEmpty(this.entryId))
            {
                this.ShowErrorMessage(WebResources.CardPage_Error_MissingRequestId);
                return;
            }

            // Check payment entry
            if (this.entry == null)
            {
                this.ShowErrorMessage(WebResources.CardPage_Error_InvalidRequestId);
            }
            else if (this.entry.Used)
            {
                this.ShowErrorMessage(WebResources.CardPage_Error_UsedRequest);
            }
            else if (this.entry.IsExpired)
            {
                this.ShowErrorMessage(WebResources.CardPage_Error_RequestTimedOut);
            }
            else
            {
                if (!Page.IsPostBack)
                {
                    bool isRetail = IndustryType.Retail.ToString().Equals(this.entry.IndustryType, StringComparison.OrdinalIgnoreCase);
                    this.CardDetailsHeaderPanel.Visible = !isRetail;

                    // Load card entry modes
                    this.CardEntryModePanel.Visible = this.entry.SupportCardSwipe || this.entry.AllowVoiceAuthorization;
                    if (this.CardEntryModePanel.Visible)
                    {
                        if (this.entry.SupportCardSwipe)
                        {
                            this.CardEntryModeDropDownList.Items.Add(new ListItem(WebResources.CardPage_CardEntryModeDropDownList_Swipe, "swipe"));
                        }

                        this.CardEntryModeDropDownList.Items.Add(new ListItem(WebResources.CardPage_CardEntryModeDropDownList_Manual, "manual"));

                        if (this.entry.AllowVoiceAuthorization)
                        {
                            this.CardEntryModeDropDownList.Items.Add(new ListItem(WebResources.CardPage_CardEntryModeDropDownList_Voice, "voice"));
                        }
                    }

                    this.CardHolderNamePanel.Visible = !isRetail;
                    this.CardTypePanel.Visible = !isRetail;

                    // Load card types
                    if (this.CardTypePanel.Visible)
                    {
                        this.CardTypeDropDownList.Items.Add(new ListItem(WebResources.CardPage_CardTypeDropDownList_PleaseSelect, string.Empty));
                        string[] cardTypes = CardTypes.ToArray(this.entry.CardTypes);
                        foreach (var ct in cardTypes)
                        {
                            if (CardTypes.Amex.Equals(ct, StringComparison.OrdinalIgnoreCase))
                            {
                                this.CardTypeDropDownList.Items.Add(new ListItem(WebResources.CardPage_CardTypeDropDownList_AmericanExpress, CardTypes.Amex));
                            }
                            else if (CardTypes.Discover.Equals(ct, StringComparison.OrdinalIgnoreCase))
                            {
                                this.CardTypeDropDownList.Items.Add(new ListItem(WebResources.CardPage_CardTypeDropDownList_Discover, CardTypes.Discover));
                            }
                            else if (CardTypes.MasterCard.Equals(ct, StringComparison.OrdinalIgnoreCase))
                            {
                                this.CardTypeDropDownList.Items.Add(new ListItem(WebResources.CardPage_CardTypeDropDownList_MasterCard, CardTypes.MasterCard));
                            }
                            else if (CardTypes.Visa.Equals(ct, StringComparison.OrdinalIgnoreCase))
                            {
                                this.CardTypeDropDownList.Items.Add(new ListItem(WebResources.CardPage_CardTypeDropDownList_Visa, CardTypes.Visa));
                            }
                        }
                    }

                    // Load month list
                    this.ExpirationMonthDropDownList.Items.Add(new ListItem(WebResources.CardPage_ExpirationMonthDropDownList_PleaseSelect, "0"));
                    string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
                    for (int i = 1; i <= 12; i++)
                    {
                        this.ExpirationMonthDropDownList.Items.Add(new ListItem(monthNames[i - 1], string.Format(CultureInfo.InvariantCulture, "{0}", i)));
                    }

                    // Load year list
                    this.ExpirationYearDropDownList.Items.Add(new ListItem(WebResources.CardPage_ExpirationYearDropDownList_PleaseSelect));
                    int currentYear = DateTime.UtcNow.Year;
                    for (int i = 0; i < 20; i++)
                    {
                        this.ExpirationYearDropDownList.Items.Add(new ListItem(string.Format(CultureInfo.InvariantCulture, "{0}", currentYear + i)));
                    }

                    // Show/hide security code and voice authorization code
                    this.SecurityCodePanel.Visible = false;
                    this.VoiceAuthorizationCodePanel.Visible = false;
                    TransactionType transactionType = (TransactionType)Enum.Parse(typeof(TransactionType), this.entry.TransactionType, true);
                    if (transactionType == TransactionType.Authorize || transactionType == TransactionType.Capture)
                    {
                        this.SecurityCodePanel.Visible = true;

                        if (this.entry.AllowVoiceAuthorization)
                        {
                            this.VoiceAuthorizationCodePanel.Visible = true;
                        }
                    }

                    this.ZipPanel.Visible = isRetail;
                    this.BillingAddressPanel.Visible = !isRetail;
                    if (this.BillingAddressPanel.Visible)
                    {
                        // Load country list
                        // Note: the value of country/region must be two-letter ISO code.
                        // TO DO: Filter the countries down to the list you support.
                        this.CountryRegionDropDownList.Items.Add(new ListItem(WebResources.CardPage_CountryRegionDropDownList_PleaseSelect, string.Empty));
                        var dataManager = new DataManager();
                        IEnumerable<CountryOrRegion> countries = dataManager.GetCountryRegionListByLocale(this.entry.EntryLocale);
                        countries = countries.OrderBy(c => c.LongName).ToList();
                        foreach (var country in countries)
                        {
                            this.CountryRegionDropDownList.Items.Add(new ListItem(country.LongName, country.TwoLetterCode));
                        }

                        // Populate default values
                        if (this.CardHolderNamePanel.Visible)
                        {
                            this.CardHolderNameTextBox.Text = this.entry.DefaultCardHolderName;
                        }

                        if (this.entry.ShowSameAsShippingAddress)
                        {
                            this.SameAsShippingPanel.Visible = true;
                            this.DefaultStreetHiddenField.Value = this.entry.DefaultStreet1;
                            this.DefaultCityHiddenField.Value = this.entry.DefaultCity;
                            this.DefaultStateProvinceHiddenField.Value = this.entry.DefaultStateOrProvince;
                            this.DefaultZipHiddenField.Value = this.entry.DefaultPostalCode;
                            this.DefaultCountryRegionHiddenField.Value = this.entry.DefaultCountryCode;
                        }
                        else
                        {
                            this.SameAsShippingPanel.Visible = false;
                            this.StreetTextBox.Text = this.entry.DefaultStreet1;
                            this.CityTextBox.Text = this.entry.DefaultCity;
                            this.StateProvinceTextBox.Text = this.entry.DefaultStateOrProvince;
                            this.ZipTextBox1.Text = this.entry.DefaultPostalCode;
                            this.CountryRegionDropDownList.SelectedValue = this.entry.DefaultCountryCode;
                        }
                    }

                    this.HostPageOriginHiddenField.Value = this.entry.HostPageOrigin;
                }
                else
                {
                    bool.TryParse(this.IsSwipeHiddenField.Value, out this.isSwipe);

                    // Validate inputs
                    if (this.ValidateInputs())
                    {
                        if (this.isSwipe)
                        {
                            this.track1 = this.CardTrack1HiddenField.Value;
                            this.track2 = this.CardTrack2HiddenField.Value;
                            this.cardNumber = this.CardNumberHiddenField.Value;
                        }
                        else
                        {
                            this.cardNumber = this.CardNumberTextBox.Text.Trim();
                        }

                        if (this.CardTypePanel.Visible)
                        {
                            this.cardType = this.CardTypeDropDownList.SelectedItem.Value;
                        }
                        else
                        {
                            this.cardType = this.CardTypeHiddenField.Value;
                        }

                        this.cardExpirationMonth = int.Parse(this.ExpirationMonthDropDownList.SelectedItem.Value, CultureInfo.InvariantCulture);
                        this.cardExpirationYear = int.Parse(this.ExpirationYearDropDownList.SelectedItem.Text, CultureInfo.InvariantCulture);
                        this.cardSecurityCode = this.SecurityCodeTextBox.Text.Trim();
                        this.voiceAuthorizationCode = this.VoiceAuthorizationCodeTextBox.Text.Trim();

                        if (this.CardHolderNamePanel.Visible)
                        {
                            this.cardHolderName = this.CardHolderNameTextBox.Text.Trim();
                        }
                        else
                        {
                            this.cardHolderName = this.CardHolderNameHiddenField.Value.Trim();
                        }

                        this.cardStreet1 = this.StreetTextBox.Text.Trim();
                        this.cardCity = this.CityTextBox.Text.Trim();
                        this.cardStateOrProvince = this.StateProvinceTextBox.Text.Trim();

                        if (this.BillingAddressPanel.Visible)
                        {
                            this.cardPostalCode = this.ZipTextBox1.Text.Trim();
                        }
                        else
                        {
                            this.cardPostalCode = this.ZipTextBox2.Text.Trim();
                        }

                        this.cardCountryOrRegion = this.CountryRegionDropDownList.SelectedValue;

                        if (!string.IsNullOrEmpty(this.PaymentAmountHiddenField.Value))
                        {
                            this.paymentAmount = decimal.Parse(this.PaymentAmountHiddenField.Value, NumberStyles.Number, Thread.CurrentThread.CurrentCulture);
                        }

                        // Process payment, e.g. tokenize, authorize, capture.
                        try
                        {
                            CardPaymentResult result = this.ProcessPayment(this.entry);

                            if (result != null)
                            {
                                // Save the payment result
                                var dataManager = new DataManager();
                                dataManager.CreateCardPaymentResult(result);

                                // Set request access code in hidden field
                                this.ResultAccessCodeHiddenField.Value = result.ResultAccessCode;
                            }
                        }
                        catch (CardPaymentException ex)
                        {
                            // Return the errors from UX
                            this.InputErrorsHiddenField.Value = JsonConvert.SerializeObject(ex.PaymentErrors);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads custom styles to change look and feel of the page.
        /// </summary>
        private void LoadCustomStyles()
        {
            // Encode style value to prevent XSS attack
            this.CustomStyles = CustomStyles.Default;

            string pagewidth = Request.QueryString["pagewidth"];
            if (!string.IsNullOrWhiteSpace(pagewidth))
            {
                this.CustomStyles.PageWidth = HttpUtility.HtmlAttributeEncode(pagewidth);
            }

            string fontSize = Request.QueryString["fontsize"];
            if (!string.IsNullOrWhiteSpace(fontSize))
            {
                this.CustomStyles.FontSize = HttpUtility.HtmlAttributeEncode(fontSize);
            }

            string fontFamily = Request.QueryString["fontfamily"];
            if (!string.IsNullOrWhiteSpace(fontFamily))
            {
                fontFamily = fontFamily.Trim();

                // Trim the quotation marks at the beginning or the end.
                if (fontFamily.Length >= 2
                    && ((fontFamily.StartsWith("\"", StringComparison.OrdinalIgnoreCase) && fontFamily.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
                        || (fontFamily.StartsWith("'", StringComparison.OrdinalIgnoreCase) && fontFamily.EndsWith("'", StringComparison.OrdinalIgnoreCase))))
                {
                    fontFamily = fontFamily.Substring(1, fontFamily.Length - 2);
                }

                fontFamily = HttpUtility.HtmlAttributeEncode(fontFamily);

                // Add the default font as backup in case that the asked font is not supported.
                fontFamily = string.Format(CultureInfo.InvariantCulture, "'{0}',{1}", fontFamily, CustomStyles.Default.FontFamily);

                this.CustomStyles.FontFamily = fontFamily;
            }

            string pageBackgroundColor = Request.QueryString["pagebackgroundcolor"];
            if (!string.IsNullOrWhiteSpace(pageBackgroundColor))
            {
                this.CustomStyles.PageBackgroundColor = HttpUtility.HtmlAttributeEncode(pageBackgroundColor);
            }

            string labelColor = Request.QueryString["labelcolor"];
            if (!string.IsNullOrWhiteSpace(labelColor))
            {
                this.CustomStyles.LabelColor = HttpUtility.HtmlAttributeEncode(labelColor);
            }

            string textBackgroundColor = Request.QueryString["textbackgroundcolor"];
            if (!string.IsNullOrWhiteSpace(textBackgroundColor))
            {
                this.CustomStyles.TextBackgroundColor = HttpUtility.HtmlAttributeEncode(textBackgroundColor);
            }

            string textColor = Request.QueryString["textcolor"];
            if (!string.IsNullOrWhiteSpace(textColor))
            {
                this.CustomStyles.TextColor = HttpUtility.HtmlAttributeEncode(textColor);
            }

            string disabledTextBackgroundColor = Request.QueryString["disabledtextbackgroundcolor"];
            if (!string.IsNullOrWhiteSpace(disabledTextBackgroundColor))
            {
                this.CustomStyles.DisabledTextBackgroundColor = HttpUtility.HtmlAttributeEncode(disabledTextBackgroundColor);
            }

            string columnnumber = Request.QueryString["columnnumber"];
            if (!string.IsNullOrWhiteSpace(columnnumber))
            {
                int number;
                if (int.TryParse(columnnumber, out number) && number >= 1 && number <= 2)
                {
                    this.CustomStyles.ColumnNumber = number;
                }
            }

            // Change the page to one column by applying a CSS style
            if (this.CustomStyles.ColumnNumber == 1)
            {
                this.CardPanel.CssClass += " msax-DisableTable";
                this.CardRowPanel.CssClass += " msax-DisableRow";
                this.CardDetailsPanel.CssClass += " msax-DisableCell";
                this.BillingAddressPanel.CssClass += " msax-DisableCell";
            }
        }

        /// <summary>
        /// Shows error message on the web page.
        /// </summary>
        /// <param name="error">The error message.</param>
        private void ShowErrorMessage(string error)
        {
            // Hide other controls
            this.CardPanel.Visible = false;

            // Show error message
            this.ErrorMessageLabel.Text = error;
            this.ErrorPanel.Visible = true;
        }

        /// <summary>
        /// Process the card payment, e.g. Tokenize, Authorize, Capture.
        /// </summary>
        /// <param name="paymentEntry">The card payment entry.</param>
        /// <returns>The payment result.</returns>
        private CardPaymentResult ProcessPayment(CardPaymentEntry paymentEntry)
        {
            // Get payment processor
            var processor = new SampleConnector();

            // Prepare payment request
            var paymentRequest = new Request();
            paymentRequest.Locale = paymentEntry.EntryLocale;

            // Get payment properties from payment entry which contains the merchant information.
            Request entryData = JsonConvert.DeserializeObject<Request>(paymentEntry.EntryData);
            PaymentProperty[] entryPaymentProperties = entryData.Properties;
            var requestProperties = new List<PaymentProperty>();

            // Filter payment card properties (they are default card data, not final card data)
            foreach (var entryPaymentProperty in entryPaymentProperties)
            {
                if (entryPaymentProperty.Namespace != GenericNamespace.PaymentCard)
                {
                    requestProperties.Add(entryPaymentProperty);
                }
            }

            // Add final card data
            PaymentProperty property;
            if (this.isSwipe)
            {
                property = new PaymentProperty(
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardEntryType,
                    CardEntryTypes.MagneticStripeRead.ToString());
                requestProperties.Add(property);

                if (!string.IsNullOrWhiteSpace(this.track1))
                {
                    property = new PaymentProperty(
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track1,
                        this.track1);
                    requestProperties.Add(property);
                }

                if (!string.IsNullOrWhiteSpace(this.track2))
                {
                    property = new PaymentProperty(
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track2,
                        this.track2);
                    requestProperties.Add(property);
                }
            }
            else
            {
                property = new PaymentProperty(
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardEntryType,
                    CardEntryTypes.ManuallyEntered.ToString());
                requestProperties.Add(property);
            }

            property = new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.CardType,
                this.cardType);
            requestProperties.Add(property);

            property = new PaymentProperty(
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardNumber,
                    this.cardNumber);
            requestProperties.Add(property);

            property = new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.ExpirationMonth,
                this.cardExpirationMonth);
            requestProperties.Add(property);

            property = new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.ExpirationYear,
                this.cardExpirationYear);
            requestProperties.Add(property);

            if (!string.IsNullOrWhiteSpace(this.cardSecurityCode))
            {
                property = new PaymentProperty(
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.AdditionalSecurityData,
                    this.cardSecurityCode);
                requestProperties.Add(property);
            }

            property = new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.Name,
                this.cardHolderName);
            requestProperties.Add(property);

            property = new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.StreetAddress,
                this.cardStreet1);
            requestProperties.Add(property);

            property = new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.City,
                this.cardCity);
            requestProperties.Add(property);

            property = new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.State,
                this.cardStateOrProvince);
            requestProperties.Add(property);

            property = new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.PostalCode,
                this.cardPostalCode);
            requestProperties.Add(property);

            property = new PaymentProperty(
                GenericNamespace.PaymentCard,
                PaymentCardProperties.Country,
                this.cardCountryOrRegion);
            requestProperties.Add(property);

            // Tokenize the card if requested
            Response tokenizeResponse = null;
            if (paymentEntry.SupportCardTokenization)
            {
                paymentRequest.Properties = requestProperties.ToArray();
                tokenizeResponse = processor.GenerateCardToken(paymentRequest, null);
                if (tokenizeResponse.Errors != null && tokenizeResponse.Errors.Any())
                {
                    // Tokenization failure, Throw an exception and stop the payment.
                    throw new CardPaymentException("Tokenization failure.", tokenizeResponse.Errors);
                }
            }

            // Authorize and Capture if requested
            // Do not authorize if tokenization failed.
            Response authorizeResponse = null;
            Response captureResponse = null;
            Response voidResponse = null;
            TransactionType transactionType = (TransactionType)Enum.Parse(typeof(TransactionType), paymentEntry.TransactionType, true);
            if (transactionType == TransactionType.Authorize || transactionType == TransactionType.Capture)
            {
                // Add request properties for Authorize and Capture
                if (!string.IsNullOrWhiteSpace(this.voiceAuthorizationCode))
                {
                    property = new PaymentProperty(
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.VoiceAuthorizationCode,
                        this.voiceAuthorizationCode);
                    requestProperties.Add(property);
                }

                property = new PaymentProperty(
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.Amount,
                    this.paymentAmount);
                requestProperties.Add(property);

                // Authorize payment
                paymentRequest.Properties = requestProperties.ToArray();
                authorizeResponse = processor.Authorize(paymentRequest, null);
                if (authorizeResponse.Errors != null && authorizeResponse.Errors.Any())
                {
                    // Authorization failure, Throw an exception and stop the payment.
                    throw new CardPaymentException("Authorization failure.", authorizeResponse.Errors);
                }

                if (transactionType == TransactionType.Capture)
                {
                    // Check authorization result
                    var authorizeResponseProperties = PaymentProperty.ConvertToHashtable(authorizeResponse.Properties);
                    PaymentProperty innerAuthorizeResponseProperty = PaymentProperty.GetPropertyFromHashtable(
                        authorizeResponseProperties,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.Properties);

                    var innerAuthorizeResponseProperties = PaymentProperty.ConvertToHashtable(innerAuthorizeResponseProperty.PropertyList);

                    string authorizationResult = null;
                    PaymentProperty.GetPropertyValue(
                        innerAuthorizeResponseProperties,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.AuthorizationResult,
                        out authorizationResult);

                    // TO DO: In this sample, we only check the authorization results. CVV2 result and AVS result are ignored. 
                    if (AuthorizationResult.Success.ToString().Equals(authorizationResult, StringComparison.OrdinalIgnoreCase))
                    {
                        // Authorize success...
                        // Get authorized amount
                        decimal authorizedAmount = 0m;
                        PaymentProperty.GetPropertyValue(
                            innerAuthorizeResponseProperties,
                            GenericNamespace.AuthorizationResponse,
                            AuthorizationResponseProperties.ApprovedAmount,
                            out authorizedAmount);

                        // Update capture amount for partial authorization
                        if (this.paymentAmount != authorizedAmount)
                        {
                            foreach (var requestProperty in requestProperties)
                            {
                                if (GenericNamespace.TransactionData.Equals(requestProperty.Namespace)
                                    && TransactionDataProperties.Amount.Equals(requestProperty.Name))
                                {
                                    requestProperty.DecimalValue = authorizedAmount;
                                    break;
                                }
                            }
                        }

                        // Capture payment
                        property = new PaymentProperty(
                            GenericNamespace.AuthorizationResponse,
                            AuthorizationResponseProperties.Properties,
                            innerAuthorizeResponseProperty.PropertyList);
                        requestProperties.Add(property);

                        paymentRequest.Properties = requestProperties.ToArray();
                        captureResponse = processor.Capture(paymentRequest);

                        // Check capture result
                        var captureResponseProperties = PaymentProperty.ConvertToHashtable(captureResponse.Properties);
                        PaymentProperty innerCaptureResponseProperty = PaymentProperty.GetPropertyFromHashtable(
                            captureResponseProperties,
                            GenericNamespace.CaptureResponse,
                            CaptureResponseProperties.Properties);

                        var innerCaptureResponseProperties = PaymentProperty.ConvertToHashtable(innerCaptureResponseProperty.PropertyList);

                        string captureResult = null;
                        PaymentProperty.GetPropertyValue(
                            innerCaptureResponseProperties,
                            GenericNamespace.CaptureResponse,
                            CaptureResponseProperties.CaptureResult,
                            out captureResult);

                        if (!CaptureResult.Success.ToString().Equals(captureResult, StringComparison.OrdinalIgnoreCase))
                        {
                            // Capture failure, we have to void authorization and return the payment result.
                            voidResponse = processor.Void(paymentRequest);
                        }
                    }
                    else
                    {
                        // Authorization failure, Throw an exception and stop the payment.
                        var errors = new List<PaymentError>();
                        errors.Add(new PaymentError(ErrorCode.AuthorizationFailure, "Authorization failure."));
                        throw new CardPaymentException("Authorization failure.", errors);
                    }
                }
            }

            // Combine responses into one.
            Response paymentResponse = this.CombineResponses(tokenizeResponse, authorizeResponse, captureResponse, voidResponse);

            // Save payment result
            CardPaymentResult result = null;
            if (paymentResponse != null)
            {
                // Success
                paymentResponse.Properties = PaymentProperty.RemoveDataEncryption(paymentResponse.Properties);

                result = new CardPaymentResult();
                result.EntryId = paymentEntry.EntryId;
                result.ResultAccessCode = CommonUtility.NewGuid().ToString();
                result.ResultData = JsonConvert.SerializeObject(paymentResponse);
                result.Retrieved = false;
                result.ServiceAccountId = paymentEntry.ServiceAccountId;
            }
            else
            {
                this.InputErrorsHiddenField.Value = WebResources.CardPage_Error_InvalidCard;
            }

            return result;
        }

        /// <summary>
        /// Combines various responses into one.
        /// </summary>
        /// <param name="tokenizeResponse">The tokenize response.</param>
        /// <param name="authorizeResponse">The authorize response.</param>
        /// <param name="captureResponse">The capture response.</param>
        /// <param name="voidResponse">The void response.</param>
        /// <returns>The combined response.</returns>
        private Response CombineResponses(Response tokenizeResponse, Response authorizeResponse, Response captureResponse, Response voidResponse)
        {
            Response paymentResponse = new Response();
            var properties = new List<PaymentProperty>();
            var errors = new List<PaymentError>();

            if (tokenizeResponse != null)
            {
                // Start with tokenize response
                paymentResponse.Locale = tokenizeResponse.Locale;

                if (tokenizeResponse.Properties != null)
                {
                    properties.AddRange(tokenizeResponse.Properties);
                }

                if (tokenizeResponse.Errors != null)
                {
                    errors.AddRange(tokenizeResponse.Errors);
                }

                // Merge with authorize response
                if (authorizeResponse != null)
                {
                    if (authorizeResponse.Properties != null)
                    {
                        var authorizeResponseProperties = PaymentProperty.ConvertToHashtable(authorizeResponse.Properties);
                        PaymentProperty innerAuthorizeResponseProperty = PaymentProperty.GetPropertyFromHashtable(
                            authorizeResponseProperties,
                            GenericNamespace.AuthorizationResponse,
                            AuthorizationResponseProperties.Properties);
                        properties.Add(innerAuthorizeResponseProperty);
                    }

                    if (authorizeResponse.Errors != null)
                    {
                        errors.AddRange(authorizeResponse.Errors);
                    }
                }
            }
            else if (authorizeResponse != null)
            {
                // Start with Authorize response
                paymentResponse.Locale = authorizeResponse.Locale;

                if (authorizeResponse.Properties != null)
                {
                    properties.AddRange(authorizeResponse.Properties);
                }

                if (authorizeResponse.Errors != null)
                {
                    errors.AddRange(authorizeResponse.Errors);
                }
            }

            // Merge with authorize response
            if (captureResponse != null)
            {
                if (captureResponse.Properties != null)
                {
                    var captureResponseProperties = PaymentProperty.ConvertToHashtable(captureResponse.Properties);
                    PaymentProperty innerCaptureResponseProperty = PaymentProperty.GetPropertyFromHashtable(
                        captureResponseProperties,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.Properties);
                    properties.Add(innerCaptureResponseProperty);
                }

                if (captureResponse.Errors != null)
                {
                    errors.AddRange(captureResponse.Errors);
                }
            }

            // Merge with void response
            if (voidResponse != null)
            {
                if (voidResponse.Properties != null)
                {
                    var voidResponseProperties = PaymentProperty.ConvertToHashtable(voidResponse.Properties);
                    PaymentProperty innerVoidResponseProperty = PaymentProperty.GetPropertyFromHashtable(
                        voidResponseProperties,
                        GenericNamespace.VoidResponse,
                        VoidResponseProperties.Properties);
                    properties.Add(innerVoidResponseProperty);
                }

                if (voidResponse.Errors != null)
                {
                    errors.AddRange(voidResponse.Errors);
                }
            }

            if (properties.Count > 0)
            {
                paymentResponse.Properties = properties.ToArray();
            }

            if (errors.Count > 0)
            {
                paymentResponse.Errors = errors.ToArray();
            }

            return paymentResponse;
        }

        /// <summary>
        /// Validates the user inputs.
        /// </summary>
        /// <returns>The error message if validation failed, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design.")]
        private bool ValidateInputs()
        {
            var paymentErrors = new List<PaymentError>();

            bool isEcommerce = IndustryType.Ecommerce.ToString().Equals(this.entry.IndustryType, StringComparison.OrdinalIgnoreCase);
            if (isEcommerce && string.IsNullOrWhiteSpace(this.CardHolderNameTextBox.Text))
            {
                paymentErrors.Add(new PaymentError(ErrorCode.InvalidCardholderNameLastNameRequired, WebResources.CardPage_Error_MissingCardHolderLastName));
                paymentErrors.Add(new PaymentError(ErrorCode.InvalidCardholderNameFirstNameRequired, WebResources.CardPage_Error_MissingCardHolderFirstName));
            }

            string paymentCardType = null;
            if (this.CardTypePanel.Visible)
            {
                paymentCardType = this.CardTypeDropDownList.SelectedItem.Value;
            }
            else
            {
                paymentCardType = this.CardTypeHiddenField.Value;
            }

            if (string.IsNullOrWhiteSpace(paymentCardType))
            {
                paymentErrors.Add(new PaymentError(ErrorCode.CardTypeVerificationError, WebResources.CardPage_Error_MissingCardType));
            }

            if (!string.IsNullOrWhiteSpace(this.entry.CardTypes) && !this.entry.CardTypes.ToUpperInvariant().Contains(paymentCardType.ToUpperInvariant()))
            {
                paymentErrors.Add(new PaymentError(ErrorCode.CardTypeVerificationError, WebResources.CardPage_Error_InvalidCardType));
            }

            string cardNumber = null;
            if (this.isSwipe)
            {
                cardNumber = this.CardNumberHiddenField.Value;
            }
            else
            {
                cardNumber = this.CardNumberTextBox.Text.Trim();
            }

            if (!HelperUtilities.ValidateBankCardNumber(cardNumber))
            {
                paymentErrors.Add(new PaymentError(ErrorCode.InvalidCardNumber, WebResources.CardPage_Error_InvalidCardNumber));
            }
            else
            {
                // TO DO: THE CODE IS ONLY FOR TEST PURPOSE. REMOVE IT IN PRODUCTION.
                // To prevent real credit card numbers entering our system, we only allow card numbers from a whitelist.
                var cardNumberWhitelist = new List<string>();
                cardNumberWhitelist.Add("4111111111111111"); // Visa
                cardNumberWhitelist.Add("5555555555554444"); // MC
                cardNumberWhitelist.Add("378282246310005"); // Amex
                cardNumberWhitelist.Add("6011111111111117"); // Discover
                if (cardNumberWhitelist.FindIndex(c => c.Equals(cardNumber)) < 0)
                {
                    paymentErrors.Add(new PaymentError(ErrorCode.InvalidCardNumber, "This card number is not allowed for testing purpose."));
                }
            }

            if (this.ExpirationMonthDropDownList.SelectedIndex == 0)
            {
                paymentErrors.Add(new PaymentError(ErrorCode.InvalidExpirationDate, WebResources.CardPage_Error_MissingExpirationMonth));
            }

            if (this.ExpirationYearDropDownList.SelectedIndex == 0)
            {
                paymentErrors.Add(new PaymentError(ErrorCode.InvalidExpirationDate, WebResources.CardPage_Error_MissingExpirationYear));
            }

            // Validate security code and amount only when authorize or capture.
            TransactionType transactionType = (TransactionType)Enum.Parse(typeof(TransactionType), this.entry.TransactionType, true);
            if (transactionType == TransactionType.Authorize || transactionType == TransactionType.Capture)
            {
                string securityCode = this.SecurityCodeTextBox.Text.Trim();
                if (!string.IsNullOrWhiteSpace(securityCode)
                    && (securityCode.Length < 3 || securityCode.Length > 4))
                {
                    paymentErrors.Add(new PaymentError(ErrorCode.InvalidCVV2, WebResources.CardPage_Error_InvalidSecurityCode));
                }

                string amountStr = this.PaymentAmountHiddenField.Value.Trim();
                decimal paymentAmountDec;
                if (string.IsNullOrEmpty(amountStr)
                   || !decimal.TryParse(amountStr, NumberStyles.Number, Thread.CurrentThread.CurrentCulture, out paymentAmountDec)
                   || paymentAmountDec < 0)
                {
                    paymentErrors.Add(new PaymentError(ErrorCode.InvalidAmount, WebResources.CardPage_Error_InvalidAmount));
                }
            }

            if (isEcommerce
                && (this.CountryRegionDropDownList.SelectedIndex == 0
                    || string.IsNullOrWhiteSpace(this.StreetTextBox.Text)
                    || string.IsNullOrWhiteSpace(this.CityTextBox.Text)
                    || string.IsNullOrWhiteSpace(this.StateProvinceTextBox.Text)
                    || string.IsNullOrWhiteSpace(this.ZipTextBox1.Text)))
            {
                paymentErrors.Add(new PaymentError(ErrorCode.InvalidAddress, WebResources.CardPage_Error_InvalidAddress));
            }

            if (paymentErrors.Count > 0)
            {
                this.InputErrorsHiddenField.Value = JsonConvert.SerializeObject(paymentErrors);
            }
            else
            {
                this.InputErrorsHiddenField.Value = string.Empty;
            }

            return paymentErrors.Count == 0;
        }
    }
}