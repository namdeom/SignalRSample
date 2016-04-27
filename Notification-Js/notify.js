
$('#user-notification').ready(function () {
    if (loggedInUser) {
        var userNotificationPopup = $("#user-notification").notificationPopup();
        $.connection.hub.url = notificationServerUrl + 'signalr';
        var chat = $.connection.notificationHub;
        $.connection.hub.qs = "UserId=" + loggedInUser;
        chat.client.sendNotificationDetails = function (notification) {
            userNotificationPopup.addNotification(notification);
        };
        $.connection.hub.start();

        userNotificationPopup.click(function () {
            if (!userNotificationPopup.isVisible()) {
                userNotificationPopup.openPopup();
            } else {
                userNotificationPopup.closePopup();
            }
        });
    }
});