(function ($, ko) {
  "use strict";

  var path = "/api/carts/get",
    CartListViewModel = function (carts) {
      var self = this,
        getSubArray = function (arr, field) {
          return $.unique($.map(arr, function (n) { return n[field]; }));
        },
        updateCarts = function (params) {
          var search = $.param(params);
          $.getJSON(path, search, function (data) {
            self.carts.removeAll();
            var i;
            for (i = 0; i < data.length; ++i) {
              self.carts.push(data[i]);
            }
          });
        },
        parameters = {};

      self.carts = ko.observableArray(carts);
      self.userIds = ko.observableArray(getSubArray(carts, "UserId"));
      self.customerIds = ko.observableArray(getSubArray(carts, "CustomerId"));
      self.cartNames = ko.observableArray(getSubArray(carts, "CartName"));
      self.cartStatuses = ko.observableArray(getSubArray(carts, "CartStatus"));

      self.selectedUserId = ko.observable(undefined);
      self.selectedCustomerId = ko.observable(undefined);
      self.selectedCartName = ko.observable(undefined);
      self.selectedCartStatus = ko.observable(undefined);

      self.applyFilter = function () {
        var tempParams = {};
        if (self.selectedUserId() !== undefined) {
          tempParams.userId = self.selectedUserId();
        }
        if (self.selectedCustomerId() !== undefined) {
          tempParams.customerId = self.selectedCustomerId();
        }
        if (self.selectedCartName() !== undefined) {
          tempParams.cartName = self.selectedCartName();
        }
        if (self.selectedCartStatus() !== undefined) {
          tempParams.cartStatus = self.selectedCartStatus();
        }
        parameters = $.extend(parameters, tempParams);
        updateCarts(parameters);
      };
      self.clearAll = function () {
        self.selectedUserId(null);
        self.selectedCustomerId(null);
        self.selectedCartName(null);
        self.selectedCartStatus(null);
        parameters = {};
        updateCarts(parameters);
      };
    };

  // main entry point
  $(document).ready(function () {
    $.getJSON(path, function (data) {
      ko.applyBindings(new CartListViewModel(data));
    });
  });
}(window.jQuery, window.ko));