/******************************** - = [ Old Cart ] = - ********************************/
var cartWidget = (function ($) {
  "use strict";

  var refresh = function () {
    AJAXGet("/data/cart/Cart", null, function (data, success, sender) {
      miniCartItemListViewModel.reload(data);
    });

  },
    reload = function () {
      refresh();
    },

    addToCart = function (id, quantity) {
      $.post("/data/cart/addproduct/" + id + "/" + quantity)
        .done(function (data) {
          refresh();
        });
      return false;
    },

    addToWishlist = function (id, quantity) {
      $.post("/data/cart/addproducttowishlist/" + id + "/" + quantity)
        .done(function (data) {
          //refresh();
          // set wishlit quantity
          $(".btn-wishlist span").text(data);
        });
      return false;
    },

    reloadWishlistQuantity = function () {
      $.get("/data/cart/getwishlistquantity").done(function(data) {
        $(".btn-wishlist span").text(data);
      });
    };

  $(document).ready(function () {
    $.ajaxSetup({ cache: false });
    cartWidget.reloadWishlistQuantity();
    cartWidget.reload();
  });

  return {
    addToCart: addToCart,
    addToWishlist: addToWishlist,
    refresh: refresh,
    reload: reload,
    reloadWishlistQuantity: reloadWishlistQuantity
  };

}(window.jQuery));

//
//Jquery Actions and Functions
//
function manageMiniCartActions() {

  $(document).ready(function () {

    if ($("#cartContainer").length == 0) {
      $('.toggle-cart').hover(function() {
        $('.minicart').slideDown(500);
        return false;
      });

      $('.minicart').mouseleave(function() {
        $(this).slideUp(500);
        return false;
      });
    } else {
      $('.toggle-cart span.caret').remove();
    }


    $('.minicart-content').on('click', ".minicart-delete", function (event) {

      $(event.currentTarget).find(".glyphicon").removeClass("glyphicon-remove-circle");
      $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh");
      $(event.currentTarget).find(".glyphicon").addClass("glyphicon-refresh-animate");
      var lineItem = $(event.currentTarget).parent();
      var lineItemId = lineItem.attr("data-ajax-lineitemid");

      $.ajax({
        url: "/data/cart/removefromcart/" + lineItemId,
        type: 'DELETE',
        success: function (data) {
          cartWidget.refresh(data);
        }
      });
      return false;
    });
  });

}

/******************************** - = [ New cart ] = - ********************************/

function AJAXCall(callType, url, data, responseFunction, sender) {
  $.ajax({
    type: callType,
    url: url,
    data: data,
    cache: false,
    contentType: "application/json; charset=utf-8",
    success: function (data) {
      if (responseFunction != null) {
        responseFunction(data, true, sender);
      }
    },
    error: function (data) {
      if (responseFunction != null) {
        responseFunction(data, false, sender);
      }
    }
  });
}


function AJAXGet(url, data, responseFunction, sender) {
  AJAXCall("GET", url, data, responseFunction, sender);
}

// Global Vars
var miniCartItemListViewModel = null;
var miniCartUpdateViewModel = null;


function initMiniShoppingCart(sectionId) {
  AJAXGet("/data/cart/Cart", null, function (data, success, sender) {
    miniCartItemListViewModel = new MiniCartItemListViewModel(data);
    ko.applyBindings(miniCartItemListViewModel, document.getElementById(sectionId));
    manageMiniCartActions();
  });
}


function MiniCartItemViewModel(image, displayName, quantity, linePrice, unitPrice, productId, externalCartlineId) {

  var self = this;

  self.image = image;
  self.displayName = displayName;
  self.quantity = quantity;
  self.linePrice = linePrice;
  self.unitPrice = unitPrice;
  self.productUrl = ko.computed(function () {
    return "/products/" + productId;
  });
  self.externalCartlineId = externalCartlineId;
}

function MiniCartItemListViewModel(data) {

  if (data != null) {
    var self = this;

    self.miniCartItems = ko.observableArray();

    $(data.CartLines).each(function () {
      self.miniCartItems.push(new MiniCartItemViewModel(this.Image, this.ProductName, this.Quantity, this.TotalPrice, this.UnitPrice, this.ProductId, this.Id));
    });

    self.totalQuantity = ko.computed(function () {
      var total = 0;
      for (var i = 0; i < self.miniCartItems().length; i++)
        total += self.miniCartItems()[i].quantity;
      return total;
    });

    self.lineitemcount = ko.observable(data.CartLines.length);

    self.total = ko.observable(data.TotalSum);

    self.reload = function (data) {

      self.miniCartItems.removeAll();

      $(data.CartLines).each(function () {
        self.miniCartItems.push(new MiniCartItemViewModel(this.Image, this.ProductName, this.Quantity, this.TotalPrice, this.UnitPrice, this.ProductId, this.Id));
      });

      self.total(data.TotalSum);
      self.lineitemcount(data.CartLines.length);
      manageMiniCartActions();

    }
  }
}


