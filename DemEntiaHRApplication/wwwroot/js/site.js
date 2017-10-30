// Write your JavaScript code.
$(document).ready(function () {

    $(".editUserBtn").click(function () {
        $(".shadow").removeClass("hidden");
        $(".editUserPage").removeClass("hidden");
    });

    $(".closeEditUserPage").click(function () {
        $(".shadow").addClass("hidden");
        $(".editUserPage").addClass("hidden");
    });
});