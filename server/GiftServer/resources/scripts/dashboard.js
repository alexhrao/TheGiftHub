$(document).ready(function () {
    $('.event-header').click(function () {
        $(this).find('span').toggleClass('glyphicon-chevron-right').toggleClass('glyphicon-chevron-down');
    });
});