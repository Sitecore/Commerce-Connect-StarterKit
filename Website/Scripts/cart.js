(function ($, ko, cartWidget) {
  "use strict";

  var CartLine = function (data) {
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

    ShoppingCartViewModel = function (cartLines, totalSum) {
      self.totalSum = ko.observable(totalSum);
      self.cartLines = ko.observableArray(cartLines);

      self.removeLine = function (line, event) {
        $(event.target).removeClass('glyphicon-remove-circle');
        $(event.target).addClass("glyphicon-refresh");
        $(event.target).addClass("glyphicon-refresh-animate");
        $.ajax({
          url: "/data/cart/removefromcart/" + line.id(),
          type: 'DELETE',
          success: function (data) {
            self.cartLines.remove(line);
            self.totalSum(data.TotalSum);
            cartWidget.reload();
          }
        });
      };

      self.hideLineElement = function(elem) {
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
            url: "/data/cart/changelinequantity/" + cartLine.productId() + "/" + valueToWrite,
            type: "PUT",
            success: function (data) {
              target(valueToWrite);
              cartLine.totalLineSum(data.TotalLineSum);
              self.totalSum(data.TotalSum);
              cartWidget.reload();
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
    $.getJSON("/data/cart/cart", function (data) {
      var mappedLines = $.map(data.CartLines, function (item) {
        return new CartLine(item);
      });
      ko.applyBindings(new ShoppingCartViewModel(mappedLines, data.TotalSum), document.getElementById('cartContainer'));
    });
  });
}(window.jQuery, window.ko, window.cartWidget));