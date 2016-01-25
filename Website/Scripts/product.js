var product = (function () {
  "use strict";

  var signUpForBackInStockNotification = function (id) {
    $.post("/data/product/signupforbackinstocknotification/" + id);
    return false;
  };

  var addProductToCart = function (id) {
    var quantity = document.getElementById('Quantity');
    cartWidget.addToCart(id, quantity.value);
    quantity.value = 1;
    return false;
  };

  var addProductToWishlist = function (id) {
    var quantity = document.getElementById('Quantity');
    cartWidget.addToWishlist(id, quantity.value);
    quantity.value = 1;
    return false;
  };

  return {
    signUpForBackInStockNotification: signUpForBackInStockNotification,
    addProductToCart: addProductToCart,
    addProductToWishlist: addProductToWishlist
  }
}());