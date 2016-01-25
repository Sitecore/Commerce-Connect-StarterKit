(function ($, ko, cartWidget) {
  "use strict";

  var WishlistLineModel = function (data) {
    var self = this;
    self.productId = ko.observable(data.ProductId);
    self.productName = ko.observable(data.ProductName);
    self.id = ko.observable(data.Id);
    self.unitPrice = ko.observable(data.UnitPriceLocal);
    self.quantity = ko.observable(data.Quantity)
                      .extend({ quantityTracker: self });
    self.image = ko.observable(data.Image !== "" ? data.Image + "?w=100&h=100" : data.Image);
    self.totalLineSum = ko.observable(data.TotalPrice);
    self.productUrl = ko.computed(function () {
      return "/products/" + self.productId();
    });
  },

    WishlistModel = function (cartLines) {
      self.cartLines = ko.observableArray(cartLines);

      self.removeLine = function (line, event) {
        $(event.target).removeClass('glyphicon-remove-circle');
        $(event.target).addClass("glyphicon-refresh");
        $(event.target).addClass("glyphicon-refresh-animate");
        $.ajax({
          url: "/data/cart/removelinefromewishlist/" + line.id(),
          type: 'DELETE',
          success: function (data) {
            self.cartLines.remove(line);
            cartWidget.reloadWishlistQuantity();
          }
        });
      };

      self.addLineToCart = function(line, event) {
        //Add from wishlist to cart
        cartWidget.addToCart(line.productId(), line.quantity());
        //Remove from wishlist
        self.removeLine(line, event);
        cartWidget.reloadWishlistQuantity();
      };

      self.hideLineElement = function (elem) {
        $(elem).fadeOut("slow");
      };

      self.slideLineDown = function (elem) {
        $(elem).fadeIn("slow");
      };
    };

  ko.extenders.quantityTracker = function (target, cartLine) {
    //create a writeable computed observable 
    //to intercept writes to our observable
    var result = ko.computed({
      read: target,  //always return the original observables value
      write: function (newValue) {
        var current = target(),
          valueToWrite = isNaN(newValue) ? current : parseInt(+newValue, 10);
        //only write if it changed
        if (valueToWrite !== current && valueToWrite > 0) {
          $.ajax({
            url: "/data/cart/changewishlistlinequantity/" + cartLine.productId() + "/" + valueToWrite,
            type: "PUT",
            success: function (data) {
              target(valueToWrite);
              cartLine.totalLineSum(data.TotalLineSum);
              cartWidget.reloadWishlistQuantity();
            }
          });
        } else {
          //if the rounded value is the same, 
          //but a different value was written, 
          //force a notification for the current field
          if (newValue !== current) {
            target.notifySubscribers(valueToWrite);
          }
        }
      }
    });

    //initialize with current value to make sure it is rounded appropriately
    result(target());

    //return the new computed observable
    return result;
  };

  $(document).ready(function () {
    $.getJSON("/data/cart/wishlist", function (data) {
      var mappedLines = $.map(data.Lines, function (item) {
        return new WishlistLineModel(item);
      });
      ko.applyBindings(new WishlistModel(mappedLines), document.getElementById('wishlistContainer'));
    });
  });
}(window.jQuery, window.ko, window.cartWidget));