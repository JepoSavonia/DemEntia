// Write your JavaScript code.
$(document).ready(function () {

    $(".editUserBtn").click(function () {
        $(".shadow").removeClass("hidden");
        $(".editUserPage").removeClass("hidden");
        $(".editUserPage").addClass("visiblePopUp");
    });

    $(".adminAddUserBtn").click(function () {
        $(".shadow").removeClass("hidden");
        $(".addUserPage").removeClass("hidden");
        $(".addUserPage").addClass("visiblePopUp");
    });

    $(".resetPassBtn").click(function () {
        $(".shadow").removeClass("hidden");
        $(".resetPassPage").removeClass("hidden");
        $(".resetPassPage").addClass("visiblePopUp");
    });

    $(".closePopUp").click(function () {
        $(".shadow").addClass("hidden");
        $(".visiblePopUp").addClass("hidden");
        $(".visiblePopUp").removeClass("visiblePopUp");
    });

    $(".shadow").click(function () {
        $(".shadow").addClass("hidden");
        $(".visiblePopUp").addClass("hidden");
        $(".visiblePopUp").removeClass("visiblePopUp");
    });

    
});