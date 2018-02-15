var numReserved;
var totalGifts;
$(document).ready(function ($) {
    $(".clickable-row").click(function () {
        window.location = $(this).data("href");
    });
    $('.star-rating').rating({
        displayOnly: true,
        step: 0.5,
        size: 'xs',
        containerClass: 'minimized'
    });
    $('#viewGiftRating').rating({
        step: 0.5,
        size: 'xs',
        containerClass: 'minimized'
    });

    $('.gift-row').click(function () {
        // Get ID of this
        var id = $(this).attr('data-gift-id');
        $.post(".", {
            action: "Fetch",
            type: "Gift",
            itemId: id
        },
            function (data, status, xhr) {
                if (status == "success") {
                    var data = xhr.responseText;
                    var dom = $.parseXML(data);
                    $('#viewGiftImage').attr("src", dom.getElementsByTagName("image")[0].innerHTML);
                    $('#viewGiftRating').rating("update", parseFloat(dom.getElementsByTagName("rating")[0].innerHTML));
                    $('#viewGiftId').val(dom.getElementsByTagName("giftId")[0].innerHTML);
                    $("#viewGiftName").text(dom.getElementsByTagName("name")[0].innerHTML);
                    $('#viewGiftDescription').text(dom.getElementsByTagName("description")[0].innerHTML);
                    $('#viewGiftUrl').text(dom.getElementsByTagName("url")[0].innerHTML);
                    $('#viewGiftUrl').attr('href', (dom.getElementsByTagName("url")[0].innerHTML));
                    $('#viewGiftStores').text(dom.getElementsByTagName("stores")[0].innerHTML);
                    $('#viewGiftCost').text(dom.getElementsByTagName("cost")[0].innerHTML);
                    numReserved = dom.getElementsByTagName("reservations")[0].children.length;
                    var reservedByMe = 0;
                    for (var i = 0; i < dom.getElementsByTagName("reservations")[0].children.length; i++) {
                        if (dom.getElementsByTagName("reservations")[0].children[i].getElementsByTagName("userId")[0].innerHTML == $('#thisUserId').attr('data-user-id')) {
                            reservedByMe++;
                        }
                    }
                    totalGifts = parseInt(dom.getElementsByTagName("quantity")[0].innerHTML)
                    if (totalGifts == numReserved) {
                        $('#viewGiftReserve').addClass("hidden");
                    } else {
                        $('#viewGiftReserve').removeClass("hidden");
                    }
                    $('#viewGiftQuantity').text((totalGifts - numReserved) + " / " + dom.getElementsByTagName("quantity")[0].innerHTML + " available (" + reservedByMe + " reserved by me)");
                    $('#reserveGiftAmount').attr('placeholder', (totalGifts - numReserved) + ' available');
                    $('#viewGiftColor').css("background-color", dom.getElementsByTagName("color")[0].innerHTML);
                    $('#viewGiftColorText').text(dom.getElementsByTagName("colorText")[0].innerHTML);
                    $('#viewGiftSize').text(dom.getElementsByTagName("size")[0].innerHTML);
                    $('#viewGiftCategory').text(dom.getElementsByTagName("category")[0].innerHTML);
                    $('#viewGiftItem').modal();
                }
            });
    });
});

$(document).ready(function () {
    $('#viewGiftReserve').click(function () {
        $('#viewGiftItem').modal('hide');
        $('#reserveGift').modal();
    });
    $('#reserveGiftAmount').keyup(function () {
        if (!$(this).val() || $(this).val() <= 0 || $(this).val() > (totalGifts - numReserved)) {
            $('#reserveGiftSubmit').addClass("hidden");
        } else {
            $('#reserveGiftSubmit').removeClass("hidden");
        }
    });
    $('#reserveGiftSubmit').click(function () {
        $.post(".", {
            action: "Change",
            type: "Gift",
            item: "reserve",
            itemId: $('#viewGiftId').val(),
            numReserve: $('#reserveGiftAmount').val()
        }, function (data, status) {
            if (data == "0") {
                // Do the right thing
            }
            location.reload(true);
        });
    });
    $('#reserveGiftCancel').click(function () {
        $('#reserveGiftAmount').val("");
        $('#reserveGift').modal("hide");
        $('#viewGiftItem').modal();
    });
});

$(document).ready(function () {
    $('#viewGiftRelease').click(function () {
        $('#viewGiftItem').modal('hide');
        $('#releaseGift').modal();
    });
    $('#releaseGiftAmount').keyup(function () {
        if (!$(this).val() || $(this).val() <= 0 || $(this).val() > numReserved) {
            $('#releaseGiftSubmit').addClass("hidden");
        } else {
            $('#releaseGiftSubmit').removeClass("hidden");
        }
    });
    $('#releaseGiftSubmit').click(function () {
        $.post(".", {
            action: "Change",
            type: "Gift",
            item: "release",
            itemId: $('#viewGiftId').val(),
            numRelease: $('#releaseGiftAmount').val()
        }, function (data, status) {
            location.reload(true);
        });
    });
    $('#releaseGiftCancel').click(function () {
        $('#releaseGiftAmount').val("");
        $('#releaseGift').modal('hide');
        $('#viewGiftItem').modal();
    });
});
function rgb2hex(rgb) {
    rgb = rgb.match(/^rgba?[\s+]?\([\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?/i);
    return (rgb && rgb.length === 4) ? "#" +
        ("0" + parseInt(rgb[1], 10).toString(16)).slice(-2) +
        ("0" + parseInt(rgb[2], 10).toString(16)).slice(-2) +
        ("0" + parseInt(rgb[3], 10).toString(16)).slice(-2) : '';
}