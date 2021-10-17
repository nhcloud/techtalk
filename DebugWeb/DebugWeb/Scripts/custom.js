$(document).ready(function () {
    console.log("ready!");
    $("#btnGet").click(function () {
        $("#txtData").val("loading...");
        $.ajax({
            url: "/api/data/get?url=" + $("#txtUrl").val()
        }).then(function (data) {

            $("#txtData").val(data);

        });
    });
});