function registerEventListener() {
    window.addEventListener("message", receiveMessage, false);
}

function submitCard() {
    var cardPageOrigin = document.getElementById("CardPageOrigin").value;
    if (cardPageOrigin != "") {
        var iframe = document.getElementById("CardPageFrame");

        var messageObject = new Object();
        messageObject.type = "msax-cc-amount";
        messageObject.value = document.getElementById("PaymentAmount").value;
        iframe.contentWindow.postMessage(JSON.stringify(messageObject), cardPageOrigin);

        messageObject.type = "msax-cc-submit";
        messageObject.value = "true";
        iframe.contentWindow.postMessage(JSON.stringify(messageObject), cardPageOrigin);
    }
}

function receiveMessage(event) {
    var cardPageOrigin = document.getElementById("CardPageOrigin").value;
    if (event.origin != cardPageOrigin)
        return;

    var message = event.data;
    if (typeof message == "string" && message.length > 0) {
        var messageObject = JSON.parse(message);
        switch (messageObject.type) {
            case "msax-cc-width":
                // Set width of the iframe
                document.getElementById("CardPageFrame").width = messageObject.value;
                break;
            case "msax-cc-height":
                // Set height of the iframe
                document.getElementById("CardPageFrame").height = messageObject.value;
                // Set focus on iframe
                var iframe = document.getElementById("CardPageFrame");
                iframe.contentWindow.focus();
                break;
            case "msax-cc-cardtype":
                // Use the card type
                // In real product, the card type may used for transaction validation.
                document.getElementById("CreditCardType").value = messageObject.value;
                break;
            case "msax-cc-cardprefix":
                // Use the card prefix
                // In real product, the prefix may used for transaction validation.
                document.getElementById("CreditCardPrefix").value = messageObject.value;
                break;
            case "msax-cc-error":
                // Show input errors
                var paymentErrors = messageObject.value;
                var errorLabel = document.getElementById("ErrorMessageLabel");
                errorLabel.innerText = "";
                for (var i = 0; i < paymentErrors.length; i++) {
                    errorLabel.innerText += paymentErrors[i].Message + " ";
                }
                break;
            case "msax-cc-result":
                document.getElementById("ResultAccessCode").value = messageObject.value;
                document.getElementById("NextButton").click();
                break;
            default:
                // Unexpected message, just ignore it.
        }
    }
}
