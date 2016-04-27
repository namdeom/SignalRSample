(function( $ ) {

    var _notificationType = {
        Ro: { text: "RO", color: "#0099cc" },
        Parts: { text: "PART", color: "#00A000" },
        Text: { text: "TEXT", color: "#FF7300" },
        Info: { text: "INFO", color: "#987A00" }
    };
    var _notifyConstants = {
        notificationURL: NotificationApiURL + "notification?UserId=" + loggedInUser,
        readReceiptURL: NotificationApiURL + "notification/"
    };

    var _unreadMessageCount = 0;
    var _lastMessageId;

  $.fn.notificationPopup = function() {
    var _notificationBody;
    var _popup;

    var _fetchUserNotification = function () {
        var url = _notifyConstants.notificationURL;
        if(_lastMessageId){
          url += "&NotificationId="+_lastMessageId;
        }
        $.get(url, function (resp) {
            _updateMessageCount(resp.Count);

            if(resp.NotificationDetailList){
              resp.NotificationDetailList.forEach(function (element, index, array) {
                _lastMessageId = element.NotificationId;
                _notificationBody.append(_createNotificationElement(element));
              });
            }
            if(resp.NotificationDetailList && resp.NotificationDetailList.length >= 10){
              if(_notificationBody.find(".loadMore").length == 0){
                _notificationBody.append($("<div class='loadMore'>Load More</div>").click(function(){
                  _fetchUserNotification();
                }));
              }
            }else{
                _notificationBody.remove(".loadMore");

                if($(".notification-item").length > 0){
                  _notificationBody.remove(".noMoreMessages");
                  _notificationBody.append("<div class='noMoreMessages'>No more notifications</div>");
                }else{
                  _notificationBody.append("<div class='noMoreMessages'>No notifications for you :)</div>");
                }
            }
        });
    };
    var _sendReadReceipt = function (notificationId, location, callback) {
          var data = {
              "NotificationId": notificationId,
              "RecipientId": loggedInUser,
              "IsRead": true
          };
          $.ajax({
              type: "POST",
              url: _notifyConstants.readReceiptURL,
              dataType: 'json',
              async: false,
              data: data,
              success: function () {
                  callback();
              }
          });

      };
    var _createNotificationElement = function (notification) {
        var type = _notificationType[notification.NotificationType];
        if (!type) {
            type = _notificationType.Info;
        }

        var location = window.location.protocol + "//" + window.location.host + window.location.pathname + "?RONumber=" + notification.NotificationNumber;
        return $("<div class='notification-item " + (notification.IsRead? '' : 'unread') + "'>" +
                   "<div class='notification-type' style='background-color:" + type.color + "'>" + type.text + "</div>" +
                   "<div class='notification-message'>" + notification.DisplayMessage + "</div>" +
                 "</div>").click(function () {
                     var that = this;
                   _sendReadReceipt(notification.NotificationId, location, function() {
                       if ($(that).hasClass("unread")) {
                           _updateMessageCount(--_unreadMessageCount);
                           $(that).removeClass("unread");
                       }
                       window.location = location;
                       
                   });
               });
    };

    var _updateMessageCount = function (count) {
        if (count == 0) {
            _popup.find(".notification-count").addClass("no-nitification");
        } else {
            _popup.find(".notification-count").removeClass("no-nitification");
        }
        if (count >= 100) {
            count = "99+";
        }
        _unreadMessageCount = count;
        _popup.find(".notification-count").html(count);
    };
    this.each(function() {
        _popup = $( this );
        _popup.append("<span class='notification-count no-nitification'>0</span>");
        _popup.append("<div class='notification-body'></div>");

        _notificationBody = $(this).find(".notification-body").hide();
        _fetchUserNotification();
    });

    $.fn.openPopup = function() {
        _notificationBody.show(100);
        if ($(".notification-mask").length == 0) {
            $("body").append("<div class='notification-mask'></div>");
            $(".notification-mask").click(function () {
                _popup.closePopup();
            }).hide();
        }
        $(".notification-mask").show();
        $(".notification-mask").css("height", $("body").height()+"px");
    };

    $.fn.closePopup = function() {
        _notificationBody.hide(100);
        $(".notification-mask").hide();
    };

    $.fn.isVisible = function() {
        return _notificationBody.is(":visible");
    };

    $.fn.addNotification = function(notification) {
        _notificationBody.prepend(_createNotificationElement(notification));
        _updateMessageCount(++_unreadMessageCount);
    };

    return this;
  };
}( jQuery ));
