(function () {
    
    jQuery(function ($) {

        $("#fetch-user-form").submit(function (event) {
            var region = $("#region-field").val();
            makeApiRequest({ "region": region || null });
            event.preventDefault();
        });

    });

    function makeApiRequest(query) {
        var jqxhr = jQuery.ajax({
            url: "http://higimiddlewaredemo.azurewebsites.net/api/User/FetchRandomUser",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(query)
        }).done(function (a, b, c) {
            console.log("Fetch request successful.");
        }).fail(function (a, b, c) {
            console.log("Fetch request failed");
        }).always(function () {
            console.log("Fetch request completed");
        });
    }

}());
