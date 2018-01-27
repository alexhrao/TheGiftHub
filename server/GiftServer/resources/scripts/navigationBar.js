$(document).ready(function () {
    $('.dropdown-menu').on('click tap', function () {
        $(this).toggleClass("open");
    });
    $('.dropdown-submenu').on('mouseenter mouseleave', function () {
        $(this).toggleClass("open");
    });
});