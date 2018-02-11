var imgData;
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var loadTimer;
            var imgObject = new Image();
            imgObject.src = reader.result;
            imgObject.onLoad = onImgLoaded();
            function onImgLoaded() {
                if (!loadTimer != null) clearTimeout(loadTimer);
                if (!imgObject.complete) {
                    loadTimer = setTimeout(function () {
                        onImgLoaded();
                    }, 3);
                } else {
                    onPreloadComplete();
                }
            }
            function onPreloadComplete() {
                var newImg = getImagePortion(imgObject);
                $('#previewNewImage').attr('src', newImg);
                $('#previewNewImage').removeClass("hidden");
                $('#acceptNewImg').removeClass("hidden");
                $('#previewImage').attr('src', newImg);
                $('#previewImage').removeClass("hidden");
                $('#submitImg').removeClass("hidden");
                imgData = newImg;
            }
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function rgb2hex(rgb) {
    rgb = rgb.match(/^rgba?[\s+]?\([\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?,[\s+]?(\d+)[\s+]?/i);
    return (rgb && rgb.length === 4) ? "" +
        ("0" + parseInt(rgb[1], 10).toString(16)).slice(-2) +
        ("0" + parseInt(rgb[2], 10).toString(16)).slice(-2) +
        ("0" + parseInt(rgb[3], 10).toString(16)).slice(-2) : '';
}
$(document).ready(function ($) {
    $(".clickable-row").click(function () {
        window.location = $(this).data("href");
    });
    $("#image").change(function () {
        readURL(this);
    });
    $('.star-rating').rating({
        displayOnly: true,
        step: 0.5,
        size: 'xs',
        containerClass: 'minimized'
    });
    $('#editGiftRating').rating({
        step: 0.5,
        size: 'xs',
        containerClass: 'minimized'
    });
    $('#newGiftRating').rating({
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
                    $('#editGiftImage').attr("src", dom.getElementsByTagName("image")[0].innerHTML);
                    $('#uploadImageId').val(dom.getElementsByTagName("giftId")[0].innerHTML);
                    $('#editGiftRating').rating("update", parseFloat(dom.getElementsByTagName("rating")[0].innerHTML));
                    $('#editGiftId').val(dom.getElementsByTagName("giftId")[0].innerHTML);
                    $("#editGiftName").val(dom.getElementsByTagName("name")[0].innerHTML);
                    $('#editGiftDescription').val(dom.getElementsByTagName("description")[0].innerHTML);
                    $('#editGiftUrl').val(dom.getElementsByTagName("url")[0].innerHTML);
                    $('#editGiftStores').val(dom.getElementsByTagName("stores")[0].innerHTML);
                    $('#editGiftCost').val(dom.getElementsByTagName("cost")[0].innerHTML);
                    $('#editGiftQuantity').val(dom.getElementsByTagName("quantity")[0].innerHTML);
                    $('#editGiftColorPicker').colorpicker({
                        "useAlpha": false,
                        "color": dom.getElementsByTagName("color")[0].innerHTML
                    });
                    $('#editGiftColor').css("background-color", dom.getElementsByTagName("color")[0].innerHTML);
                    $('#editGiftColorText').val(dom.getElementsByTagName("colorText")[0].innerHTML);
                    $('#editGiftSize').val(dom.getElementsByTagName("size")[0].innerHTML);
                    $('#editGiftCategory').val(dom.getElementsByTagName("category")[0].innerHTML);
                    for (var i = 0; i < dom.getElementsByTagName("groups")[0].children.length; i++) {
                        $('#editGiftItem #editSharedGroups input[data-group-id=' + dom.getElementsByTagName("groups")[0].children[i].innerHTML + ']')[0].checked = true;
                    }
                    // if received is null, then show I have Received, otherwise opposite
                    if (dom.getElementsByTagName("dateReceived")[0].innerHTML == "") {
                        $('#receivedGiftLabel').removeClass("hidden");
                        $('#notReceivedGiftLabel').addClass("hidden");
                    } else {
                        $('#receivedGiftLabel').addClass("hidden");
                        $('#notReceivedGiftLabel').removeClass("hidden");
                    }
                    $('#editGiftItem').modal();
                }
            });
    });

    $('#editGiftSubmit').click(function () {
        $.post(".", {
            action: "Change",
            type: "Gift",
            item: "update",
            itemId: $('#editGiftId').val(),
            name: $('#editGiftName').val(),
            description: $('#editGiftDescription').val(),
            url: $('#editGiftUrl').val(),
            size: $('#editGiftSize').val(),
            stores: $('#editGiftStores').val(),
            category: $('#editGiftCategory').val(),
            cost: $('#editGiftCost').val(),
            quantity: $('#editGiftQuantity').val(),
            rating: $('#editGiftRating').val(),
            color: $('#editGiftColor').val(),
            colorText: $('#editGiftColorText').val()
        }, function (data, status) {
            if (data == 200) {
                // Add gifts to the group
                $('#editSharedGroups input').each(function () {
                    // post each of these
                    if ($(this)[0].checked) {
                        $.post(".",
                            {
                                action: "Change",
                                type: "Group",
                                item: "addGift",
                                itemId: $(this).attr('data-group-id'),
                                giftId: $('#editGiftId').val()
                            });
                    } else {
                        $.post(".",
                            {
                                action: "Change",
                                type: "Group",
                                item: "removeGift",
                                itemId: $(this).attr('data-group-id'),
                                giftId: $('#editGiftId').val()
                            });
                    }

                });
                window.location.reload(true);
            } else {
                $('#editGiftItem').modal('hide');
            }
        });
    });

    $('#editGiftDelete').click(function () {
        $('#editGiftItem').modal('hide');
        $('#deleteGiftWarning').modal();
    });

    $('#editGiftReceived').click(function () {
        $.post(".", {
            action: "Change",
            type: "Gift",
            item: "receive",
            itemId: $('#editGiftId').val()
        }, function (data, status, xhr) {
            location.reload(true);
        });
    });
});
$(document).ready(function () {
    $('#deleteGiftConfirm').click(function () {
        $.post(".", {
            action: "Change",
            type: "Gift",
            item: "delete",
            itemId: $('#editGiftId').val(),

        }, function (data, status) {
            if (data == 200) {
                location.reload(true);
            }
        });
    });
    $('#deleteGiftCancel').click(function () {
        $('#deleteGiftWarning').modal('hide');
    });
});
$(document).ready(function () {
    $('#newGiftColorPicker').colorpicker({
        "useAlpha": false,
        "color": "#000000"
    });
    $('#newGiftSubmit').click(function () {
        $.post(".", {
            action: "Create",
            type: "Gift",
            name: $('#newGiftName').val(),
            description: $('#newGiftDescription').val(),
            url: $('#newGiftUrl').val(),
            size: $('#newGiftSize').val(),
            stores: $('#newGiftStores').val(),
            category: $('#newGiftCategory').val(),
            cost: $('#newGiftCost').val(),
            quantity: $('#newGiftQuantity').val(),
            rating: $('#newGiftRating').val(),
            color: $('#newGiftColor').val(),
            colorText: $('#newGiftColorText').val()
        }, function (data, status) {
            if (data !== "0" && $('#newImageAdded').val() == 1) {
                $('#newImageId').val(data);
                $.post(".", {
                    action: "Image",
                    type: "gift",
                    itemId: data,
                    image: imgData.split(",")[1]
                }, function () {
                    location.reload(true);
                });
            } else {
                location.reload(true);
            }
        });
    });

    $('#newImage').change(function () {
        readURL(this);

        if (this.files && this.files[0]) {
            $('#previewImage').addClass("hidden");
            $('#submitImg').addClass("hidden");
        }
    });
    $('#acceptNewImg').click(function () {
        $('#newGiftImage').attr('src', $('#previewNewImage').attr('src'));
        $('#newImageAdded').val("1");
        $('#uploadNewImg').modal('hide');
    });
    $('#submitImg').click(function () {
        $.post(".", {
            action: "Image",
            type: "gift",
            itemId: $('#uploadImageId').val(),
            image: imgData.split(",")[1]
        }, function () {
            location.reload(true);
        })
    })
});
function getImagePortion(imgObj) {
    var bufferCanvas = document.createElement('canvas');
    var bufferContext = bufferCanvas.getContext('2d');
    bufferCanvas.width = imgObj.width;
    bufferCanvas.height = imgObj.height;
    bufferContext.drawImage(imgObj, 0, 0);
    return bufferCanvas.toDataURL();
}