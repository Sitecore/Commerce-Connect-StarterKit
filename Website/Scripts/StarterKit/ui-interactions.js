$(document).ready(function() {
  $(".product-image-container .thumbnails li a").on('click', function (e) {
    e.preventDefault();
    var activeThumb = $(this);

    activeThumb.closest("ul").find(".selected-thumb").removeClass("selected-thumb");
    activeThumb.closest("li").toggleClass("selected-thumb");

    $('#prod-large-view').attr('src', $(this).attr('href'));
  });
});


var parseQueryString = function (queryString) {
  var params = {}, queries, temp, i, l;

  // Split into key/value pairs
  queries = decodeURIComponent(queryString).split("&");

  // Convert the array of strings into an object
  for (i = 0, l = queries.length; i < l; i++) {
    temp = queries[i].split('=');
    params[temp[0]] = temp[1];
  }

  return params;
};

//PAGING & SORTING
$(document).ready(function () {
  $("div[id$='ProductListHeader'] select.changePageSize").on('change', function (e) {
    e.preventDefault();

    var query = parseQueryString(document.location.search.slice(1));
    query['page'] = 0;
    query['pageSize'] = this.value;

    document.location = document.location.origin + document.location.pathname + '?' + $.param(query);
  });

  $("div[id$='ProductListHeader'] select.sortDropdown").on('change', function (e) {
    e.preventDefault();

    var query = parseQueryString(document.location.search.slice(1));
    query['sort'] = this.value;

    document.location = document.location.origin + document.location.pathname + '?' + $.param(query);
  });

  $("div[id$='Pagination'] div.pagination-container ul.pagination li.disabled").on('click', function (e) {
    e.preventDefault();
  });
});

//CHECKOUT - BILLING SHIPPING ADDRESS
$(document).ready(function () {
  $("#checkoutContainer").on("change", ".billing-address-control, .shipping-address-control", function () {
    var value = this.value;
    if (value === "new_address") {
      $(".form-address .form-control[name='PartyId']").val("");
      $(".form-address .form-control[name='FirstName']").val("");
      $(".form-address .form-control[name='LastName']").val("");
      $(".form-address .form-control[name='Email']").val("");
      $(".form-address .form-control[name='Company']").val("");
      $(".form-address .form-control[name='State']").val("");
      $(".form-address .form-control[name='City']").val("");
      $(".form-address .form-control[name='Address1']").val("");
      $(".form-address .form-control[name='Address2']").val("");
      $(".form-address .form-control[name='ZipPostalCode']").val("");
      $(".form-address .form-control[name='PhoneNumber']").val("");
      $(".form-address .form-control[name='Country'] option").first().attr("selected", "selected");
    }
    $.getJSON("/samples/Checkout/GetBillingAddress/" + value, function (data) {
      $(".form-address .form-control[name='PartyId']").val(data.PartyId);
      $(".form-address .form-control[name='FirstName']").val(data.FirstName);
      $(".form-address .form-control[name='LastName']").val(data.LastName);
      $(".form-address .form-control[name='Email']").val(data.Email);
      $(".form-address .form-control[name='Company']").val(data.Company);
      $(".form-address .form-control[name='State']").val(data.State);
      $(".form-address .form-control[name='City']").val(data.City);
      $(".form-address .form-control[name='Address1']").val(data.Address1);
      $(".form-address .form-control[name='Address2']").val(data.Address2);
      $(".form-address .form-control[name='ZipPostalCode']").val(data.ZipPostalCode);
      $(".form-address .form-control[name='PhoneNumber']").val(data.PhoneNumber);
      $(".form-address .form-control[name='Country'] option[value=" + data.Country + "]").attr("selected", "selected");
    });
  });
});

//CHECKOUT - SHIPPING METHODS
$(document).ready(function () {
  function updateMethods(id) {
    $.get('/samples/Checkout/ShippingMethodList/' + id, function (data) {
      $('#shippingMethods').html(data);
    });
  }
  $('#checkoutContainer').on('change', 'select.shippingoption-control', function () {
    updateMethods(this.value);
  });
});


//CHECKOUT - PAYMENT METHODS
$(document).ready(function () {
  function updatePaymentMethods(id) {
    $.get("/samples/Checkout/PaymentMethodList/" + id, function (data) {
      $("#paymentMethods").html(data);
    });
  }
  $("#checkoutContainer").on("change", "select.paymentoption-control", function () {
    updatePaymentMethods(this.value);
  });
});


//CHECKOUT - CONFIRM ORDER

var ConfirmApi = function () {

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
  };

  var ShoppingCartViewModel = function (cartLines, totalSum) {
    self.totalSum = ko.observable(totalSum);
    self.cartLines = ko.observableArray(cartLines);
  };

  var showProducts = function () {
    $.getJSON("/data/cart/cart", function (data) {
      var mappedLines = $.map(data.CartLines, function (item) {
        return new CartLine(item);
      });
      ko.applyBindings(new ShoppingCartViewModel(mappedLines, data.TotalSum), document.getElementById('itemsContainer'));
    });
  };

  return {
    showProducts: showProducts
  }

};

$(document).ready(function () {
  $("#checkoutContainer").on("click", "button[name='changeStep']", function () {
    $(this).button('loading');
  });

});

$(function() {
  var createAutocomplete = function () {
    var $input = $(this);

    var options = {
      source: $input.attr("data-otf-autocomplete"),
      minLength: 3,
      select: function(event, ui) {
        var newpath = window.location.protocol + '//' + window.location.hostname + "/" + "products/" + ui.item.id;
        window.location = newpath;
      },
      success: function(data) {
        return {
          label: data.label,
          value: data.value,
          id: data.id
        }
      }

    };

    $input.autocomplete(options);
  };

  $("input[data-otf-autocomplete]").each(createAutocomplete);
});

