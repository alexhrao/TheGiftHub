$(document).ready(function () {
    $('.event-header').click(function () {
        $(this).find('span').toggleClass('fa-angle-right').toggleClass('fa-angle-down');
    });
});

$(document).ready(function () {
    $('.event-expander').click(function () {
        if ($(this).hasClass("fa-angle-down")) {
            $(this).parent().children(".hidden").removeClass("hidden").addClass("event-to-hide");
            $(this).removeClass("fa-angle-down").addClass("fa-angle-up");
        } else {
            $(this).parent().children(".event-to-hide").removeClass(".event-to-hide").addClass("hidden");
            $(this).removeClass("fa-angle-up").addClass("fa-angle-down");
        }
    });
});