$(document).ready(function () {
    // by default alert pop-up dismisses upon click. stop that behaviour.
    $(".notifications-menu .dropdown-menu").click(function (ev) {
        ev.stopPropagation();
    });

    // removing alerts via ajax when "x" button clicked
    $(".alert-dismiss").click(function (ev) {
        var alertItem = $(ev.target).closest("li");
        var alertId = alertItem.data("alertid");

        $.ajax("/alert/" + alertId, {
            method: "delete"
        }).done(function () {
            alertItem.remove();

            var alertCounters = $(".alert-count");
            var oldAlertCount = parseFloat(alertCounters[0].innerText);
            alertCounters.text(oldAlertCount - 1);

            if (oldAlertCount === 1) {
                $(alertCounters[0]).remove();  // remove the badge if there no alerts left
            }

        }).fail(function () {
            alert("error: could not remove alert");
        });

    });

    $(".mailbox-link").click(function (ev) {
        var sender = $(ev.target);
        var message = sender.data("message");
        var messageId = sender.data("messageid");

        var unreadMessages = $("#unreadmessagecount");
        var unreadMessagesCount = parseInt(unreadMessages.text());


        var currentUser = $("#currentUser").text();

        var mailMsgBox = $("#message-box");
        var mailMsgBoxBody = $("#mailmsg-body");
        var mailMsgBoxTitle = $("#mailmsg-title");
        var mailMsgBoxInfo = $("#mailmsg-info");
        var messageTableBody = $("#message-tablebody");
      
        $.getJSON("/message/" + messageId)
            .done(function (data) {
                mailMsgBox.show();
                mailMsgBoxBody.text(data.body);
                mailMsgBoxTitle.text(data.subject)
                if (currentUser === data.receiverId) {
                    mailMsgBoxInfo.text(data.sender + " (" + data.created + ")")
                } else {
                    mailMsgBoxInfo.text(data.receiver + " (" + data.created + ")")
                }
                

                // set the default value of new message recipient to match sender of current message
                $("#receiverId").find("option").removeAttr("selected");
                $("#receiverId").find("option[value=" + data.senderId + "]").prop("selected", "selected");

                // highlight the chosen row
                messageTableBody.find("tr").removeClass("mailbox-chosen");
                sender.closest("tr").addClass("mailbox-chosen");

                // mark as read on server if not already
                if (!data.isRead && currentUser === data.receiverId ) {
                    $.ajax({
                        url: "/message/read/" + messageId,
                        method: "put"
                    }).done(function () {
                        sender.closest("tr").removeClass("mailbox-unread").addClass("mailbox-read");

                        // update the unread message count of the left bar
                        if (unreadMessagesCount == 1) {
                            unreadMessages.hide();
                        } else {
                            unreadMessages.text(unreadMessagesCount - 1);
                        }
                        
                    }).fail(function () {
                        console.error("couldn't mark as read");
                    });
                }
            })
            .fail(function () {
                console.error("couldn't receive message")
            });
    });

    // unhighlight all messages if the message box is closed
    $("#close-message").click(function () {
        $("#message-tablebody").find("tr").removeClass("mailbox-chosen");
    });

    // clear the message title if new message was clicked
    $("#newmessagebutton").click(function () {
        $("#subject").val("");
    });

    // set the message title to reply to the currently viewed one if "reply" is clicked
    $("#replymessagebutton").click(function () {
        var replyTitle = "RE: " + $("#mailmsg-title").text();
        $("#subject").val(replyTitle);
    });
});