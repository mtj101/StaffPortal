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
            }).done(function(){
                alertItem.remove();

                var alertCounters = $(".alert-count");
                var oldAlertCount = parseFloat(alertCounters[0].innerText);
                alertCounters.text(oldAlertCount - 1);

                if (oldAlertCount === 1) {
                    $(alertCounters[0]).remove();  // remove the badge if there no alerts left
                }
                
            }).fail(function(){
                alert("error: could not remove alert");
            });

    });
});