<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardPage.aspx.cs" Inherits="Sitecore.Commerce.PaymentService.CardPage" %>

<!--
// Copyright 2016 Sitecore Corporation A/S 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at 
//       http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License. 
-->

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" dir="<%= this.TextDirection %>">
<head runat="server">
    <script type="text/javascript">
        // When the page body is loaded (execute once)
        function bodyOnload() {

            // Do nothing if the whole card panel is hidden
            if (document.getElementById("CardPanel") == null) {
                return;
            }

            // Update card fields by card entry mode
            updateFieldsByCardEntryMode();

            // Remove top padding of the page
            removeTopPadding();

            // Register key press handler to hanle card swipe.
            document.onkeypress = handleDocumentKeyPress;

            // Register handler for request errors
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(handleEndRequest);

            // Notify the host page about the page size
            sendDimensions();

            // Listen to change event on "Same as shipping address"
            var sameAsShippingCheckBox = document.getElementById("SameAsShippingCheckBox");
            if (sameAsShippingCheckBox != null) {
                sameAsShippingCheckBox.addEventListener("change", changeSameAsShipping);
            }
        }

        // When postback is finished (after every postback)
        function pageLoad() {

            // Do nothing if the origin of the request page (i.e. host page) is unavailable
            var hostPageOrigin = document.getElementById("HostPageOriginHiddenField").value;
            if (hostPageOrigin != "") {
                var messageObject = new Object();

                // Cross domain message: send input errors to the request page to display.
                var errorField = document.getElementById("InputErrorsHiddenField");
                if (errorField.value != "") {
                    messageObject.type = "msax-cc-error";
                    messageObject.value = JSON.parse(errorField.value);
                    parent.postMessage(JSON.stringify(messageObject), hostPageOrigin);
                }

                // Cross domain message: send token access code to the request page.
                var resultField = document.getElementById("ResultAccessCodeHiddenField");
                if (resultField.value != "") {
                    // Disable inputs
                    disableInputs();

                    // Remove message listener
                    window.removeEventListener("message", receiveMessage, false);

                    messageObject.type = "msax-cc-result";
                    messageObject.value = resultField.value;
                    parent.postMessage(JSON.stringify(messageObject), hostPageOrigin);
                }
            }
        }

        // When a message is received from the request page.
        function receiveMessage(event) {
            var hostPageOrigin = document.getElementById("HostPageOriginHiddenField").value;

            // Validate origin
            if (event.origin != hostPageOrigin)
                return;

            // Parse message
            var message = event.data;
            if (typeof message == "string" && message.length > 0) {

                // Handle various messages from the host page
                var messageObject = JSON.parse(message);
                switch (messageObject.type) {
                    case "msax-cc-amount":
                        // Set payment amount
                        document.getElementById("PaymentAmountHiddenField").value = messageObject.value;
                        break;
                    case "msax-cc-submit":
                        // Submit page using ASP.NET AJAX
                        // DO NOT USE form.submit(). IT WILL TRIGGER THE PARENT PAGE TO RELOAD IN SOME SCENARIOS.
                        document.getElementById("SubmitButton").click();
                        break;
                    default:
                        // Unexpected message, just ignore it.
                }
            }
        }

        // Sends the page width and the page height to the host page.
        function sendDimensions() {
            // Do nothing if the origin of the request page (i.e. host page) is unavailable
            var hostPageOrigin = document.getElementById("HostPageOriginHiddenField").value;
            if (hostPageOrigin != "") {

                // Register event to listen message from the request page.
                window.addEventListener("message", receiveMessage, false);

                // Cross domain message: send iframe width and height to the request page for auto-sizing.
                var messageObject = new Object();
                messageObject.type = "msax-cc-width";
                messageObject.value = pageWidth();
                parent.postMessage(JSON.stringify(messageObject), hostPageOrigin);

                messageObject.type = "msax-cc-height";
                messageObject.value = pageHeight() + 1; // Add 1px to avoid scroll bars
                parent.postMessage(JSON.stringify(messageObject), hostPageOrigin);
            }
        }

        // Get the width of HTML page
        function pageWidth() {
            return Math.max(
                document.body.scrollWidth,
                document.body.offsetWidth,
                document.documentElement.scrollWidth,
                document.documentElement.offsetWidth,
                document.documentElement.clientWidth);
        }

        // Get the height of HTML page
        function pageHeight() {
            return Math.max(
                document.body.scrollHeight,
                document.body.offsetHeight,
                document.documentElement.scrollHeight,
                document.documentElement.offsetHeight,
                document.documentElement.clientHeight);
        }

        // Update fields by card entry mode
        function updateFieldsByCardEntryMode() {
            var cardEntryModeDropDownList = document.getElementById("CardEntryModeDropDownList");
            if (cardEntryModeDropDownList == null) {
                return;
            }

            var cardEntryMode = cardEntryModeDropDownList.options[cardEntryModeDropDownList.selectedIndex].value;
            var isSwipe = false;
            var isVoiceAuthorization = false;
            if (cardEntryMode == "swipe") {
                isSwipe = true;
            }
            else if (cardEntryMode == "voice") {
                isVoiceAuthorization = true;
            }

            document.getElementById("IsSwipeHiddenField").value = isSwipe.toString();

            var cardHolderNameTextBox = document.getElementById("CardHolderNameTextBox");
            if (cardHolderNameTextBox != null) {
                cardHolderNameTextBox.value = "";
                cardHolderNameTextBox.disabled = isSwipe;
            }
            document.getElementById("CardHolderNameHiddenField").value = "";

            var cardTypeDropDownList = document.getElementById("CardTypeDropDownList");
            if (cardTypeDropDownList != null) {
                cardTypeDropDownList.selectedIndex = 0;
                cardTypeDropDownList.disabled = isSwipe;
            }
            document.getElementById("CardTypeHiddenField").value = "";

            document.getElementById("CardNumberTextBox").value = "";
            document.getElementById("CardNumberTextBox").disabled = isSwipe;
            document.getElementById("CardNumberHiddenField").value = "";
            document.getElementById("ExpirationMonthDropDownList").selectedIndex = 0;
            document.getElementById("ExpirationMonthDropDownList").disabled = isSwipe;
            document.getElementById("ExpirationYearDropDownList").selectedIndex = 0;
            document.getElementById("ExpirationYearDropDownList").disabled = isSwipe;

            var securityCodeTextBox = document.getElementById("SecurityCodeTextBox");
            if (securityCodeTextBox != null) {
                securityCodeTextBox.value = "";
            }

            var voiceAuthorizationCodePanel = document.getElementById("VoiceAuthorizationCodePanel");
            if (voiceAuthorizationCodePanel != null) {
                if (isVoiceAuthorization) {
                    voiceAuthorizationCodePanel.style.display = "inline";
                }
                else {
                    voiceAuthorizationCodePanel.style.display = "none";
                }

                document.getElementById("VoiceAuthorizationCodeTextBox").value = "";
            }

            var zipPanel = document.getElementById("ZipPanel");
            if (zipPanel != null) {
                if (isSwipe) {
                    zipPanel.style.display = "none";
                }
                else {
                    zipPanel.style.display = "inline";
                }

                document.getElementById("ZipTextBox2").value = "";
            }

            sendDimensions();
            sendCardType();
            sendCardNumberPrefix();
        }

        // When the card type is changed, sends a message for card type.
        var oldCardType = "";
        function sendCardType() {

            // Do nothing if the origin of the request page (i.e. host page) is unavailable
            var hostPageOrigin = document.getElementById("HostPageOriginHiddenField").value;
            if (hostPageOrigin != "") {

                // Get card type
                var newCardType = "";
                var cardTypeDropDownList = document.getElementById("CardTypeDropDownList");
                if (cardTypeDropDownList != null) {
                    newCardType = cardTypeDropDownList.options[cardTypeDropDownList.selectedIndex].value;
                }
                else {
                    newCardType = document.getElementById("CardTypeHiddenField").value;
                }

                if (oldCardType != newCardType) {

                    // Cross domain message: the selected credit card type
                    var messageObject = new Object();
                    messageObject.type = "msax-cc-cardtype";
                    messageObject.value = newCardType;
                    parent.postMessage(JSON.stringify(messageObject), hostPageOrigin);

                    oldCardType = newCardType;
                }
            }
        }

        // When the card number is changed, sends a message for the prefix of the card number (first 6 digits).
        var oldPrefix = "";
        function sendCardNumberPrefix() {

            // Do nothing if the origin of the request page (i.e. host page) is unavailable
            var hostPageOrigin = document.getElementById("HostPageOriginHiddenField").value;
            if (hostPageOrigin != "") {

                // Get card number
                
                var cardNumber = "";
                var cardNumberTextBox = document.getElementById("CardNumberTextBox");
                if (cardNumberTextBox.disabled) {
                    cardNumber = document.getElementById("CardNumberHiddenField").value;
                }
                else {
                    cardNumber = cardNumberTextBox.value;
                }

                // Remove spaces
                cardNumber = cardNumber.replace(" ", "");

                // Get new prefix (first 6 digits)
                var newPrefix = "";
                if (cardNumber.length >= 13)
                {
                    var newPrefix = cardNumber.substring(0, 6);
                }

                // Compare with old before send
                if (oldPrefix != newPrefix) {

                    // Cross domain message: the prefix of the card number
                    var messageObject = new Object();
                    messageObject.type = "msax-cc-cardprefix";
                    messageObject.value = newPrefix;
                    parent.postMessage(JSON.stringify(messageObject), hostPageOrigin);

                    oldPrefix = newPrefix;
                }
            }
        }

        // Update card type based on new card number
        function cardNumberOnChange() {
            var cardNumberTextBox = document.getElementById("CardNumberTextBox");
            if (!cardNumberTextBox.disabled) {
                var cardNumber = cardNumberTextBox.value;
                var newCardType = convertCardNumberToCardType(cardNumber);
                if (oldCardType != newCardType) {
                    selectCardType(cardNumber);
                }

                sendCardType();
                sendCardNumberPrefix();
            }
        }

        // When the "Same as shipping address" checkbox is changed.
        function changeSameAsShipping(event) {
            var sameAsShippingCheckBox = document.getElementById("SameAsShippingCheckBox");

            if (sameAsShippingCheckBox != null && sameAsShippingCheckBox.checked) {
                // Show default values and disable fields
                document.getElementById("StreetTextBox").value = document.getElementById("DefaultStreetHiddenField").value;
                document.getElementById("StreetTextBox").disabled = true;
                document.getElementById("CityTextBox").value = document.getElementById("DefaultCityHiddenField").value;
                document.getElementById("CityTextBox").disabled = true;
                document.getElementById("StateProvinceTextBox").value = document.getElementById("DefaultStateProvinceHiddenField").value;
                document.getElementById("StateProvinceTextBox").disabled = true;
                document.getElementById("ZipTextBox1").value = document.getElementById("DefaultZipHiddenField").value;
                document.getElementById("ZipTextBox1").disabled = true;
                document.getElementById("CountryRegionDropDownList").value = document.getElementById("DefaultCountryRegionHiddenField").value;
                document.getElementById("CountryRegionDropDownList").disabled = true;
            }
            else {
                // Clear values and enable fields
                document.getElementById("StreetTextBox").value = "";
                document.getElementById("StreetTextBox").disabled = false;
                document.getElementById("CityTextBox").value = "";
                document.getElementById("CityTextBox").disabled = false;
                document.getElementById("StateProvinceTextBox").value = "";
                document.getElementById("StateProvinceTextBox").disabled = false;
                document.getElementById("ZipTextBox1").value = "";
                document.getElementById("ZipTextBox1").disabled = false;
                document.getElementById("CountryRegionDropDownList").selectedIndex = 0;
                document.getElementById("CountryRegionDropDownList").disabled = false;
            }
        }

        // Disable inputs after result is recieved.
        function disableInputs() {
            var cardEntryModeDropDownList = document.getElementById("CardEntryModeDropDownList");
            if (cardEntryModeDropDownList != null) {
                cardEntryModeDropDownList.disabled = true;
            }

            var cardHolderNameTextBox = document.getElementById("CardHolderNameTextBox");
            if (cardHolderNameTextBox != null) {
                cardHolderNameTextBox.disabled = true;
            }

            var cardTypeDropDownList = document.getElementById("CardTypeDropDownList");
            if (cardTypeDropDownList != null) {
                cardTypeDropDownList.disabled = true;
            }

            var cardNumberTextBox = document.getElementById("CardNumberTextBox");
            cardNumberTextBox.value = maskCardNumber(cardNumberTextBox.value);
            cardNumberTextBox.disabled = true;

            document.getElementById("ExpirationMonthDropDownList").disabled = true;
            document.getElementById("ExpirationYearDropDownList").disabled = true;

            var securityCodeTextBox = document.getElementById("SecurityCodeTextBox");
            if (securityCodeTextBox != null) {
                securityCodeTextBox.value = "****";
                securityCodeTextBox.disabled = true;
            }

            var voiceAuthorizationCodeTextBox = document.getElementById("VoiceAuthorizationCodeTextBox");
            if (voiceAuthorizationCodeTextBox != null) {
                voiceAuthorizationCodeTextBox.disabled = true;
            }

            var zipTextBox2 = document.getElementById("ZipTextBox2");
            if (zipTextBox2 != null) {
                zipTextBox2.disabled = true;
            }

            var countryRegionDropDownList = document.getElementById("CountryRegionDropDownList");
            if (countryRegionDropDownList != null) {
                countryRegionDropDownList.disabled = true;
            }

            var streetTextBox = document.getElementById("StreetTextBox");
            if (streetTextBox != null) {
                streetTextBox.disabled = true;
            }

            var cityTextBox = document.getElementById("CityTextBox");
            if (cityTextBox != null) {
                cityTextBox.disabled = true;
            }

            var stateProvinceTextBox = document.getElementById("StateProvinceTextBox");
            if (stateProvinceTextBox != null) {
                stateProvinceTextBox.disabled = true;
            }

            var zipTextBox1 = document.getElementById("ZipTextBox1");
            if (zipTextBox1 != null) {
                zipTextBox1.disabled = true;
            }

            var sameAsShippingCheckBox = document.getElementById("SameAsShippingCheckBox");
            if (sameAsShippingCheckBox != null) {
                sameAsShippingCheckBox.disabled = true;
            }
        }

        // Remove the top padding of the page
        function removeTopPadding() {
            // When headers are displayed, nothing to do because hearders already have zero margintop and zero padding top.
            var cardDetailsHeaderPanel = document.getElementById("CardDetailsHeaderPanel");
            if (cardDetailsHeaderPanel == null) {
                // Find the top field, one by one, card number is already displayed
                var topLabel = document.getElementById("CardEntryModeLabel");
                if (topLabel == null) {
                    topLabel = document.getElementById("CardHolderNameLabel");
                    if (topLabel == null) {
                        topLabel = document.getElementById("CardTypeLabel");
                        if (topLabel == null) {
                            topLabel = document.getElementById("CardNumberLabel");
                        }
                    }
                }

                // Remove the top padding (margintop is already zero).
                topLabel.style.paddingTop = "0px"
            }
        }

        // Handles request errors
        function handleEndRequest(sender, args) {
            if (args.get_error() != undefined) {
                // Send error message
                var hostPageOrigin = document.getElementById("HostPageOriginHiddenField").value;
                if (hostPageOrigin != "") {

                    var errorArray = [];
                    var error = new Object();
                    error.Code = "22001"; // ErrorCode.CommunicationError
                    error.Message = "<%= this.CommunicationErrorMessage %>";
                    errorArray.push(error);

                    var messageObject = new Object();
                    messageObject.type = "msax-cc-error";
                    messageObject.value = errorArray;

                    // Cross domain message: send input errors to the request page to display.
                    parent.postMessage(JSON.stringify(messageObject), hostPageOrigin);

                    args.set_errorHandled(true);
                }
            }
        }

        // Card swipe variables
        var keyPressedSourceElement; // the source element (e.g. an input) when key press happens
        var isEatingFastKeyStrokesEnabled = false; // the flag to enable eating some fast key strokes e.g. track 3 that we don't use.
        var eatenFastKeyStrokeChars = ""; // The fast key strokes that are eaten, e.g. track 3.
        var isLoggingSwipeEnabled = false; // The flag to enable logging of the card swipe including track 1 and track 2.
        var loggedSwipeChars = ""; // The characters that are logged as the card swipe.
        var detectCardSwipeTimerId = -1; // The timer Id of detectCardSwipe()
        var stopLoggingCardSwipeTimerId = -1; // Time Id of stopLoggingCardSwipe();
        var startChar = ""; // The first character of track data, e.g. '%', ';'

        // Handles a key press
        function handleDocumentKeyPress(e) {
            // Ignore "Enter" key
            if (e.keyCode == 13) {
                return false;
            }

            // Convert key code to a character
            var pressedChar = String.fromCharCode(e.keyCode);

            // Eat the character if the flag is enable, e.g. track 3 that we don't use.
            if (isEatingFastKeyStrokesEnabled) {
                eatenFastKeyStrokeChars += pressedChar;
                return false; // hide the character
            }

            // Log the character as part of card swipe if the flag is enabled.
            if (isLoggingSwipeEnabled) {
                logCharactor(pressedChar);
                return false;	// hide the character
            }
            else if (pressedChar == '%') { // track data begins from track1
                keyPressedSourceElement = e.srcElement;
                startChar = '%';
                startLogging(pressedChar);
                return false;	// hide the character
            }
            else if (pressedChar == ';') { // track data begins from track2
                keyPressedSourceElement = e.srcElement;
                startChar = ';';
                startLogging(pressedChar);
                return false;	// hide the character
            }
            else {
                return true;	// pass through char
            }
        }

        // Logs a character as part of card swipe.
        function logCharactor(pressedChar) {

            // Append the charracter
            loggedSwipeChars = loggedSwipeChars + pressedChar;

            // Recieved a '?'. It could be the end of track 1 or track 2.
            if (pressedChar == '?') {

                // This is the end of the track.
                // Eat the rest of the fast key strokes, e.g. track 3 that we don't use.
                isEatingFastKeyStrokesEnabled = true;
                window.setTimeout('eatFastKeyStrokes();', 250);

                // Disable logging for card swipe
                isLoggingSwipeEnabled = false;

                // Clear any open timers
                clearTimers();

                // Parse track data when entry mode is swipe. Otherwise, just eat the swipe.
                var cardEntryModeDropDownList = document.getElementById("CardEntryModeDropDownList");
                if (cardEntryModeDropDownList != null && cardEntryModeDropDownList.selectedIndex == 0) {
                    if (!ParseSwipe()) {
                        // Reset fields if swipe is invalid.
                        updateFieldsByCardEntryMode();

                        // Send error message
                        var hostPageOrigin = document.getElementById("HostPageOriginHiddenField").value;
                        if (hostPageOrigin != "") {

                            var errorArray = [];

                            var error = new Object();
                            error.Code = "22150"; // ErrorCode.InvalidCardTrackData
                            error.Message = "<%= this.InvalidCardTrackDataMessage %>";
                            errorArray.push(error);

                            var messageObject = new Object();
                            messageObject.type = "msax-cc-error";
                            messageObject.value = errorArray;

                            // Cross domain message: send input errors to the request page to display.
                            parent.postMessage(JSON.stringify(messageObject), hostPageOrigin);
                        }
                    }
                }

                //clear track data
                loggedSwipeChars = '';
            }
        }

        // Starts the logging of card swipe.
        function startLogging(pressedChar, sourceElement) {

            // Clear any open timers.
            clearTimers();

            // Enable logging of card swipe
            isLoggingSwipeEnabled = true;
            loggedSwipeChars = pressedChar;

            // Set a timer to detect if this is really a swipe.
            detectCardSwipeTimerId = window.setTimeout('detectCardSwipe();', 250);

            // Set a timer to stop logging the card swipe.
            // The log can only stay open for 7.5 seconds. After that all card read input is lost.
            // This handles the scenario where the card swipe was busted.
            stopLoggingCardSwipeTimerId = window.setTimeout('stopLoggingCardSwipe();', 7500);
        }

        // Detects whether the read data is really from a card swipe.
        // The IDTech reads 22 chars in 1/4 second.  Human typing is max 3 chars in 1/4 second.
        // The key press must get at least 5 chars in 1/4 second to be considered a swipe.
        function detectCardSwipe() {

            // Notify the main execution that the detectCardSwipe() has executed. No need to clear timer.
            detectCardSwipeTimerId = -1;

            // Check the second character.
            // If it is also the start character ('%', ';'), the fast key strokes could be the result of long press '%' or ';' key.
            // Then it is not a card swipe.
            var secondChar = loggedSwipeChars.substring(1, 2);
            if (loggedSwipeChars.length < 5 || secondChar == startChar) {

                // This is not a swipe. Instead, the user manually entered a swipe key character.
                // isLoggingSwipeEnabled = false will tell the calling code to allow the characters to pass through.
                isLoggingSwipeEnabled = false;

                // Append the logged characters back into the source element, e.g. textbox
                if (keyPressedSourceElement.tagName.toLowerCase() == "input"
                    && keyPressedSourceElement.type.toLowerCase() == "text") {

                    // NOTE: all textfield attributes have the .maxLength property with a default value
                    // of 2^31 (or, 0 if it was set to an improper value) if the value was not programatically set...
                    // Check maxlength property of this field before appending the additional chars
                    var maxLength = keyPressedSourceElement.maxLength;
                    if (maxLength > 0) {
                        keyPressedSourceElement.value = (keyPressedSourceElement.value + loggedSwipeChars).substring(0, maxLength);
                    }
                    else {
                        keyPressedSourceElement.value += loggedSwipeChars;
                    }
                }

                loggedSwipeChars = '';
            }
        }

        // Stops the logging of the card swipe.
        function stopLoggingCardSwipe() {

            // Notify the main execution that the stopLoggingCardSwipe() has executed. No need to clear timer.
            stopLoggingCardSwipeTimerId = -1;

            if (isLoggingSwipeEnabled) {

                // We are going to clear the keyboard log resulting in a loss of all logged chars.
                isLoggingSwipeEnabled = false;
                loggedSwipeChars = '';
            }
        }

        // Clear the timers
        function clearTimers() {
            if (detectCardSwipeTimerId != -1) {
                window.clearTimeout(detectCardSwipeTimerId);
                detectCardSwipeTimerId = -1;
            }

            if (stopLoggingCardSwipeTimerId != -1) {
                window.clearTimeout(stopLoggingCardSwipeTimerId);
                stopLoggingCardSwipeTimerId = -1;
            }
        }

        // Eats the fast key strokes, used for track 3.
        function eatFastKeyStrokes() {

            // Stop eating when the key stroke is not fast. We must be at the end of the line
            if (eatenFastKeyStrokeChars.length < 5) {

                isEatingFastKeyStrokesEnabled = false;
                eatenFastKeyStrokeChars = "";
                return;
            }

            // Execution returns from this instance of eatFastKeyStrokes()
            // We schedule another instance of eatFastKeyStrokes() to run again in 1/4 second.
            eatenFastKeyStrokeChars = "";
            window.setTimeout('eatFastKeyStrokes();', 250);
        }

        // Parses the track data
        function ParseSwipe() {

            var swipeCopy = loggedSwipeChars;
            if (swipeCopy == null || swipeCopy.length == 0) {
                return false;
            }

            // Flatten the raw data into one line for simpler parsing... 
            // Some card readers do not use the \r\n when delimiting the track data.
            swipeCopy = swipeCopy.replace("\n", "").replace("\r", "");

            // Determin the presentce of track1 and track by checking their field separators.
            var hasTrack1 = swipeCopy.indexOf("^") > 0;
            var hasTrack2 = swipeCopy.indexOf("=") > 0;
            var trackEndIndex = swipeCopy.indexOf("?");

            var track1, track2;
            if (hasTrack1 && hasTrack2)
            {
                track1 = swipeCopy.substring(0, trackEndIndex + 1);
                track2 = swipeCopy.substring(trackEndIndex + 1);
            }
            else if (hasTrack1) {
                track1 = swipeCopy;
            }
            else if (hasTrack2) {
                track2 = swipeCopy;
            }
            else {
                return false;
            }

            // Parse tracks for card data
            var cardNumber, names;
            var expMonth, expYear;  //unsigned ints
            if (hasTrack1) {
                // Track1 e.g. %B4111111111111111^DOE/JOHN^25120123456789?
                var track1parts = track1.split("^");

                if (track1parts.length >= 3) {
                    if (track1parts[0].length >= 3) {
                        // Extract the card number without the first two characters "%B"
                        cardNumber = track1parts[0].substring(2);
                    }
                    else {
                        return false;
                    }

                    names = track1parts[1];

                    if (track1parts[2].length >= 4) {
                        // The first four-digit is yymm
                        expMonth = track1parts[2].substring(2, 4);
                        expYear = track1parts[2].substring(0, 2);
                    }
                }
                else {
                    return false;
                }
            }
            else if (hasTrack2) {
                // Track2 e.g. ;4111111111111111=25120123456789?
                var track2parts = track2.split("=");

                if (track2parts.length >= 2) {
                    if (track2parts[0].length >= 2) {
                        // Extract the card number without the first characters ";"
                        cardNumber = track2parts[0].substring(1);
                    }
                    else {
                        return false;
                    }

                    if (track2parts[1].length >= 4) {
                        // The first four-digit is yymm
                        expMonth = track2parts[1].substring(2, 4);
                        expYear = track2parts[1].substring(0, 2);
                    }
                }
                else {
                    return false;
                }
            }

            // Validate and display card number
            cardNumber = cardNumber.replace(" ", "");
            var reDigits = /^\d+$/;  // Only 0 - 9
            if (!reDigits.test(cardNumber)) {
                return false;
            }

            document.getElementById("CardNumberTextBox").value = maskCardNumber(cardNumber);
            document.getElementById("CardNumberHiddenField").value = cardNumber;

            if (!selectCardType(cardNumber)) {
                // Error selecting card type
                return false;
            }
            sendCardType();
            sendCardNumberPrefix();

            // Parse and display names
            var firstName, lastName;
            if (names != null && names.length > 0) {
                var nameSeparatorIndex = names.indexOf("/");
                if (nameSeparatorIndex != -1) {
                    lastName = names.substring(0, nameSeparatorIndex);
                    firstName = names.substring(nameSeparatorIndex + 1);
                }
                else {
                    lastName = "";
                    firstName = names;
                }

                var trimmedFirstName = firstName.replace(/^\s+/, "").replace(/\s+$/, "");
                var trimmedLastName = lastName.replace(/^\s+/, "").replace(/\s+$/, "");
                var displayName;
                if (trimmedFirstName.length > 0 && trimmedLastName.length > 0) {
                    displayName = trimmedFirstName + " " + trimmedLastName;
                }
                else if (trimmedFirstName.length > 0) {
                    displayName = trimmedFirstName;
                }
                else if (trimmedLastName.length > 0) {
                    displayName = trimmedLastName;
                }

                var cardHolderNameTextBox = document.getElementById("CardHolderNameTextBox");
                if (cardHolderNameTextBox != null) {
                    cardHolderNameTextBox.value = displayName;
                }
                else {
                    document.getElementById("CardHolderNameHiddenField").value = displayName;
                }
            }

            // Validate and display expiration month and year
            if (!reDigits.test(expMonth)) {
                return false;
            }

            if (expMonth < 1 || expMonth > 12) {
                return false;
            }

            if (!reDigits.test(expYear)) {
                return false;
            }

            if (!selectExpirationMonth(parseInt(expMonth))) {
                // Error selecting month
                return false;
            }

            if (!selectExpirationYear(parseInt(expYear) + 2000)) {
                // Error selecting year
                return false;
            }

            // Set track data into hidden fields
            document.getElementById("CardTrack1HiddenField").value = track1;
            document.getElementById("CardTrack2HiddenField").value = track2;

            return true;
        }

        // Select card type in dropdown based on the card number
        function selectCardType(cardNumber) {

            var success = true;
            var cardType = convertCardNumberToCardType(cardNumber);
            var cardTypeDropDown = document.getElementById("CardTypeDropDownList");
            if (cardTypeDropDown != null) {
                success = selectDropDown(cardTypeDropDown, cardType);
            }
            else {
                document.getElementById("CardTypeHiddenField").value = cardType;
            }

            if (success) {
                success = cardType != "";
            }

            return success;
        }

        function convertCardNumberToCardType(cardNumber) {
            var cardType = "";
            if (cardNumber.length == 0) {
                return cardType;
            }

            var firstDigit = cardNumber.substring(0, 1);
            switch (firstDigit) {
                case "4":
                    cardType = "Visa";
                    break;
                case "5":
                    cardType = "MasterCard";
                    break;
                case "3":
                    cardType = "Amex";
                    break;
                case "6":
                    cardType = "Discover";
                    break;
                default:
                    break;
            }

            return cardType;
        }


        // Masks card number leaving only the last four digit
        function maskCardNumber(cardNumber) {

            if (cardNumber != null && cardNumber.length > 4) {
                return "************" + cardNumber.substr(cardNumber.length - 4, 4);
            }
            else {
                return cardNumber;
            }
        }

        function selectExpirationMonth(month) {
            var monthDropDown = document.getElementById("ExpirationMonthDropDownList");
            return selectDropDown(monthDropDown, month.toString());
        }

        function selectExpirationYear(year) {
            var yearDropDown = document.getElementById("ExpirationYearDropDownList");
            return selectDropDown(yearDropDown, year.toString());
        }

        function selectDropDown(dropdownControl, selectedValue) {

            for (var i = 0; i < dropdownControl.options.length; i++) {
                if (dropdownControl.options[i].value == selectedValue) {
                    dropdownControl.options[i].selected = true;
                    return true;
                }
            }

            return false;
        }
    </script>
    <link href="CSS/Ecommerce.css" rel="stylesheet" type="text/css" />
    <!-- Override styles -->
    <style type="text/css">
        html {
            background-color: <%=this.CustomStyles.PageBackgroundColor%> !important;
        }

        body {
            font-size: <%=this.CustomStyles.FontSize%> !important;
            width: <%=this.CustomStyles.PageWidth%> !important;
        }

        form {
            background-color: <%=this.CustomStyles.PageBackgroundColor%> !important;
        }

        .msax-Control {
            font-size: <%=this.CustomStyles.FontSize%> !important;
            font-family: <%=this.CustomStyles.FontFamily%> !important;
            color: <%=this.CustomStyles.LabelColor%> !important;
        }
        
        .msax-Control input,
        .msax-Control select {
            background-color: <%=this.CustomStyles.TextBackgroundColor%> !important;
            font-family: <%=this.CustomStyles.FontFamily%> !important;
            color: <%=this.CustomStyles.TextColor%> !important;
        }

        .msax-Control input:disabled,
        .msax-Control select:disabled {
            background-color: <%=this.CustomStyles.DisabledTextBackgroundColor%> !important;
            color: #707070 !important; /* grey */
        }

        /* If RTL is enabled, turn left padding to right padding, and vice versa. */
        <% if("rtl".Equals(this.TextDirection)) { %>
        .msax-Control .msax-PaddingLeft10Right0 {
            padding-left: 0px;
            padding-right: 0.83em;
        }

        .msax-Control .msax-PaddingLeft0Right10 {
            padding-left: 0.83em;
            padding-right: 0px;
            
        }
        <% } %>
    </style>
    <title></title>
</head>
<body onload="bodyOnload();">
    <form id="CardForm" autocomplete="off" runat="server">
    <div class="msax-Control">
        <asp:Panel ID="CardPanel" CssClass="msax-Table" runat="server">
            <asp:Panel ID="CardRowPanel" CssClass="msax-Row" runat="server">
                <!--Card details-->
                <asp:Panel ID="CardDetailsPanel" CssClass="msax-WidthRatio50 msax-PaddingLeft0Right10"  runat="server">
                    <asp:Panel ID="CardDetailsHeaderPanel" runat="server">
                        <h4 class="msax-MarginTop0"><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_CardDetailsLabel %>"/></h4>
                    </asp:Panel>
                    <asp:Panel CssClass="msax-FieldPanel" ID="CardEntryModePanel" runat="server" Visible="false">
                        <label id="CardEntryModeLabel"><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_CardEntryModeLabel %>"/></label>
                        <asp:DropDownList ID="CardEntryModeDropDownList" runat="server" onchange="updateFieldsByCardEntryMode();"/>
                    </asp:Panel>
                    <asp:Panel CssClass="msax-FieldPanel" ID="CardHolderNamePanel" runat="server">
                        <label id="CardHolderNameLabel"><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_CardHolderNameLabel %>"/></label>
                        <asp:TextBox ID="CardHolderNameTextBox" MaxLength="255" runat="server"></asp:TextBox>
                    </asp:Panel>
                    <asp:Panel CssClass="msax-FieldPanel" ID="CardTypePanel" runat="server">
                        <label id="CardTypeLabel"><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_CardTypeLabel %>"/></label>
                        <asp:DropDownList ID="CardTypeDropDownList" runat="server" onchange="sendCardType();"/>
                    </asp:Panel>
                    <div class="msax-FieldPanel">
                            <label id="CardNumberLabel"><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_CardNumberLabel %>"/></label>
                            <asp:TextBox ID="CardNumberTextBox" MaxLength="20" runat="server" onchange="cardNumberOnChange();"></asp:TextBox>
                        </div>
                    <div class="msax-FieldPanel msax-Table">
                        <div class="msax-Row">
                            <div class="msax-FieldPanel msax-WidthRatio50 msax-PaddingLeft0Right10">
                                <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_ExpirationMonthLabel %>"/></label>
                                <asp:DropDownList ID="ExpirationMonthDropDownList" runat="server"/>
                            </div>
                            <div class="msax-FieldPanel msax-WidthRatio50 msax-PaddingLeft10Right0">
                                <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_ExpirationYearLabel %>"/></label>
                                <asp:DropDownList ID="ExpirationYearDropDownList" runat="server"/>
                            </div>
                        </div>
                    </div>
                    <asp:Panel CssClass="msax-FieldPanel" ID="SecurityCodePanel" runat="server">
                            <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_SecurityCodeLabel %>"/></label>
                            <asp:TextBox ID="SecurityCodeTextBox" MaxLength="4" runat="server"></asp:TextBox>
                        </asp:Panel>
                    <asp:Panel CssClass="msax-FieldPanel" ID="VoiceAuthorizationCodePanel" runat="server">
                        <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_VoiceAuthorizationCodeLabel %>"/></label>
                        <asp:TextBox ID="VoiceAuthorizationCodeTextBox" MaxLength="50" runat="server"></asp:TextBox>
                    </asp:Panel>
                    <asp:Panel CssClass="msax-FieldPanel" ID="ZipPanel" Visible="false"  runat="server">
                        <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_PostalCodeLabel %>"/></label>
                        <asp:TextBox ID="ZipTextBox2" MaxLength="20" runat="server"></asp:TextBox>
                    </asp:Panel>
                </asp:Panel>

                <!--Billing address-->
                <asp:Panel ID="BillingAddressPanel" CssClass="msax-WidthRatio50 msax-PaddingLeft10Right0"  runat="server">
                    <h4 class="msax-MarginTop0"><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_ShippingAddressLabel %>"/></h4>
                    <asp:Panel CssClass="msax-FieldPanel msax-CheckBoxUnderH4Top" ID="SameAsShippingPanel" runat="server">
                        <asp:CheckBox ID="SameAsShippingCheckBox" Text="<%$ Resources:WebResources, CardPage_SameAsShippingAddressCheckbox %>" runat="server" />
                    </asp:Panel>
                    <div class="msax-FieldPanel">
                        <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_CountryRegionLabel %>"/></label>
                        <asp:DropDownList ID="CountryRegionDropDownList" runat="server"></asp:DropDownList>
                    </div>
                    <div class="msax-FieldPanel">
                        <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_StreetLabel %>"/></label>
                        <asp:TextBox ID="StreetTextBox" MaxLength="255" runat="server"></asp:TextBox>
                    </div>
                    <div class="msax-FieldPanel msax-Table">
                        <div class="msax-Row">
                            <div class="msax-FieldPanel msax-WidthRatio45 msax-PaddingLeft0Right10">
                                <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_CityLabel %>"/></label>
                                <asp:TextBox ID="CityTextBox" MaxLength="255" runat="server"></asp:TextBox>
                            </div>
                            <div class="msax-FieldPanel msax-WidthRatio30 msax-PaddingLeft10Right10">
                                <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_StateProvinceLabel %>"/></label>
                                <asp:TextBox ID="StateProvinceTextBox" MaxLength="255" runat="server"></asp:TextBox>
                            </div>
                            <div class="msax-FieldPanel msax-WidthRatio25 msax-PaddingLeft10Right0">
                                <label><asp:Literal runat="server" Text="<%$ Resources:WebResources, CardPage_PostalCodeLabel %>"/></label>
                                <asp:TextBox ID="ZipTextBox1" MaxLength="20" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>

        <!--Error message e.g. invalid request ID.-->
        <asp:Panel CssClass="msax-Row" ID="ErrorPanel" runat="server" Visible="false">
            <asp:Label CssClass="msax-ErrorLabel" ID="ErrorMessageLabel" runat="server"/>
        </asp:Panel>

        <!--Hidden fields-->
        <div><asp:HiddenField ID="IsSwipeHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="CardHolderNameHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="CardNumberHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="CardTypeHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="DefaultStreetHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="DefaultCityHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="DefaultStateProvinceHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="DefaultZipHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="DefaultCountryRegionHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="HostPageOriginHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="PaymentAmountHiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="CardTrack1HiddenField" runat="server" /></div>
        <div><asp:HiddenField ID="CardTrack2HiddenField" runat="server" /></div>

        <!-- ASP.NET AJAX enabled fields -->
        <asp:ScriptManager ID="MainScriptManager" runat="server" />
        <asp:UpdatePanel ID="ResultUpdatePannel" runat="server">
            <ContentTemplate>
                <asp:HiddenField ID="InputErrorsHiddenField" runat="server" />
                <asp:HiddenField ID="ResultAccessCodeHiddenField" runat="server" />
                <asp:Button ID="SubmitButton" style="visibility: hidden; display: none;" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </div>
    </form>
</body>
</html>