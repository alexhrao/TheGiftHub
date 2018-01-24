
$(document).ready(function () {
    $("#image").change(function () {
        readURL(this);
    });
});
$(document).ready(function () {
    $('#events .event-closer').click(function () {
        var eNumber = this.id;
        eNumber = parseInt(eNumber.substring(11))
        $.post(".",
            {
                submit: "Change",
                type: "Event",
                item: "delete",
                itemId: eNumber
            },
            function (data, status) {
                if (data == 200) {
                    $('#event' + eNumber).remove();
                } else {
                    alert("We're unable to make your changes right now - try again in a few minutes.");
                }
            });
    });
});
$(document).ready(function () {
    $('#groups .group-closer').click(function () {
        var gNumber = this.id;
        gNumber = parseInt(gNumber.substring(11))
        $.post(".",
            {
                submit: "Change",
                type: "Group",
                item: "removeMe",
                itemId: gNumber
            },
            function (data, status) {
                if (data == 200) {
                    $('#group' + gNumber).remove();
                } else {
                    alert("We're unable to make your changes right now - try again in a few minutes.");
                }
            });
    });
});
$(document).ready(function () {
    $('#userBirthdayMonthChange').change(populateDays);
    $('#submitPref').click(function () {
        (Promise.resolve())
            .then(function () {
                if ($('#userNameChange').val() == "") {
                    return Promise.resolve();
                }
                return $.post(".",
                    {
                        submit: "Change",
                        type: "User",
                        item: "name",
                        name: $('#userNameChange').val()
                    });
            })
            .then(function () {
                if ($('#userEmailChange').val() == "") {
                    return Promise.resolve();
                }
                return $.post(".",
                    {
                        submit: "Change",
                        type: "User",
                        item: "email",
                        name: $('#userEmailChange').val()
                    });
            })
            .then(function () {
                if ($('#userBirthdayDayChange').val() == null || $('#userBirthdayDayChange').val() == 0) {
                    return Promise.resolve();
                } else {
                    $.post(".",
                        {
                            submit: "Change",
                            type: "User",
                            item: "birthday",
                            month: $('#userBirthdayMonthChange').val(),
                            day: $('#userBirthdayDayChange').val()
                        });
                }
            })
            .then(function () {
                return $.post(".",
                    {
                        submit: "Change",
                        type: "User",
                        item: "bio",
                        bio: $('#userBioChange').val()
                    });
            })
            .then(function () {
                return $.post(".",
                    {
                        submit: "Change",
                        type: "Preferences",
                        culture: $('#userCulture').val()
                    });
            })
            .then(function () {
                location.reload(true);
            });
    });
});
        $(document).ready(function () {
        $('#searchNewUser').click(function () {
            $.post(".",
                {
                    submit: "Fetch",
                    type: "Email",
                    email: $('#newGroupAddUser').val()
                },
                function (data, status, xhr) {
                    if (status == "success") {
                        var dom = $.parseXML(xhr.responseText);
                        var id = parseInt(dom.getElementsByTagName("userId")[0].innerHTML);
                        // Iterate over lis, make sure not already added
                        var exists = false;
                        $("#newGroupMembers li").each(function (index) {
                            if ($(this).attr('data-user-id') == id) {
                                // found
                                exists = true;
                                return false;
                            }
                        });
                        if (!exists) {
                            $('#userResult').removeClass("hidden");
                            $('#foundUser').text(dom.getElementsByTagName("userName")[0].innerHTML);
                            $('#foundUser').val(id);
                        } else {
                            $('#userResult').addClass("hidden");
                        }
                    } else {
                        $('#userResult').addClass("hidden");
                    }
                });
        });
    $('#groupAddUser').click(function () {
                // Add to list of users (add email as value, name as text)
                // grab name
                var name = $('#foundUser').text();
                var id = $('#foundUser').val();
                $('#newGroupMembers').prepend("<li data-user-id=\"" + id + "\">" + name + " <a class=\"remove-user\"><i class=\"fa fa-times\"></i></a ></li > ");
$('#userResult').addClass("hidden");
$('#newGroupAddUser').val("");
            });
$('#createNewGroupSubmit').click(function () {
    $.post(".",
        {
            submit: "Create",
            type: "Group",
            name: $('#newGroupName').val()
        }, function (data, status) {
            if (data != 0) {
                // Success! loop over elems, sending email as additions:
                $('#newGroupMembers li').each(function (index) {
                    // Post this group:
                    $.post(".", {
                        submit: "Change",
                        type: "Group",
                        item: "addUser",
                        itemId: data,
                        userId: $(this).attr("data-user-id")
                    });
                });
                // close modal
                $('#addGroup').modal('hide');
            }
        });
});
        });
$(document).ready(function () {
    $('.group-members').on('click', '.remove-user', function () {
        $(this.parentElement).remove();
    });
});
$(document).ready(function () {
    $('.group-name').click(function () {
        // get group information:
        // Get ID of this
        var id = $(this).attr('data-group-id');
        id = parseInt(id);
        $.post(".", {
            submit: "Fetch",
            type: "Group",
            itemId: id
        },
            function (data, status, xhr) {
                if (status == "success") {
                    var data = xhr.responseText;
                    var dom = $.parseXML(data);
                    $('#editGroupName').attr('placeholder', dom.getElementsByTagName("name")[0].innerHTML);
                    $('#editGroupId').val(dom.getElementsByTagName("groupId")[0].innerHTML);
                    var members = dom.getElementsByTagName("members")[0].children;
                    $('#editGroupMembers li').each(function () {
                        $(this).remove();
                    });
                    $('#origGroupMembers li').each(function () {
                        $(this).remove();
                    });
                    for (var i = 0; i < members.length; i++) {
                        var id = members[i].children[0].innerHTML;
                        var name = members[i].children[1].innerHTML;
                        $('#editGroupMembers').prepend("<li data-user-id=\"" + id + "\">" + name + " <a class=\"remove-user\"><i class=\"fa fa-times\"></i></a></li>");
                        $('#origGroupMembers').prepend("<li data-user-id=\"" + id + "\"></li>");
                    }
                    $('#editGroup').modal('show');
                }
            });
    });
    $('#editSearchUser').click(function () {
        $.post(".",
            {
                submit: "Fetch",
                type: "Email",
                email: $('#editGroupAddUser').val()
            },
            function (data, status, xhr) {
                if (status == "success") {
                    var dom = $.parseXML(xhr.responseText);
                    var id = parseInt(dom.getElementsByTagName("userId")[0].innerHTML);
                    // Iterate over lis, make sure not already added
                    var exists = false;
                    $("#editGroupMembers li").each(function (index) {
                        if ($(this).attr('data-user-id') == id) {
                            // found
                            exists = true;
                            return false;
                        }
                    });
                    if (!exists) {
                        $('#editUserResult').removeClass("hidden");
                        $('#editFoundUser').text(dom.getElementsByTagName("userName")[0].innerHTML);
                        $('#editFoundUser').val(id);
                    } else {
                        $('#editUserResult').addClass("hidden");
                    }
                } else {
                    $('#editUserResult').addClass("hidden");
                }
            });
    });
    $('#editGroupNewUser').click(function () {
        // Add to list of users (add id as value, name as text)
        // grab name
        var name = $('#editFoundUser').text();
        var id = $('#editFoundUser').val();
        $('#editGroupMembers').prepend("<li data-user-id=\"" + id + "\">" + name + " <a class=\"remove-user\"><i class=\"fa fa-times\"></i></a></li>");
        $('#editUserResult').addClass("hidden");
        $('#editGroupAddUser').val("");
    });
    $('#editGroupSubmit').click(function () {
        // loop through original group members. If any of them aren't found, REMOVE that member!
        $('#origGroupMembers li').each(function () {
            // search in new list
            var id = $(this).attr('data-user-id');
            var isFound = false;
            $('#editGroupMembers li').each(function () {
                if ($(this).attr('data-user-id') === id) {
                    isFound = true;
                    return false;
                }
            });
            if (!isFound) {
                // Remove user
                $.post(".",
                    {
                        submit: "Change",
                        type: "Group",
                        item: "removeUser",
                        itemId: $('#editGroupId').val(),
                        userID: $(this).attr('data-user-id')
                    });
            }
        });
        // loop through edited group members. If any of them aren't found originally, ADD that member!
        $('#editGroupMembers li').each(function () {
            var id = $(this).attr('data-user-id');
            var isFound = false;
            $('#origGroupMembers li').each(function () {
                if ($(this).attr('data-user-id') === id) {
                    isFound = true;
                    return false;
                }
            });
            if (!isFound) {
                $.post(".",
                    {
                        submit: "Change",
                        type: "Group",
                        item: "addUser",
                        itemId: $('#editGroupId').val(),
                        userId: $(this).attr('data-user-id')
                    });
            }
        });
        if ($('#editGroupName').val() !== "") {
            $.post(".",
                {
                    submit: "Change",
                    type: "Group",
                    item: "name",
                    itemId: $('#editGroupId').val(),
                    name: $('#editGroupName').val()
                }, function () {
                    $('.group-name[data-group-id=' + $('#editGroupId').val() + ']').children().text($('#editGroupName').val());
                });
        }
        $('#editGroup').modal('hide');
    });
});
$(document).ready(function () {
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            // Validate:
            if (!$('#resetPassword').hasClass('in') || ($('#secondResetPass').val() && $('#secondResetPass').val() === $('#firstResetPass').val())) {
                return true;
            } else {
                event.preventDefault();
                return false;
            }
        }
    });
    $('#resetSubmit').hide();
    $('#firstResetPass').keyup(function () {
        if ($('#secondResetPass').val() === $('#firstResetPass').val() && $('#secondResetPass').val()) {
            // We are good to go!
            $('#secondResetPass').parent().removeClass('has-error');
            $('#secondResetPass').parent().addClass('has-success');
            $('#firstResetPass').parent().removeClass('has-error');
            $('#firstResetPass').parent().addClass('has-success');
            $('#resetSubmit').show();
        } else {
            // Passwords don't match; disable submit and warn
            $('#secondResetPass').parent().removeClass('has-success');
            $('#secondResetPass').parent().addClass('has-error');
            $('#firstResetPass').parent().removeClass('has-success');
            $('#firstResetPass').parent().addClass('has-error');
            $('#resetSubmit').hide();
        }
    });
    $('#secondResetPass').keyup(function () {
        if ($('#secondResetPass').val() === $('#firstResetPass').val() && $('#secondResetPass').val()) {
            // We are good to go!
            $('#secondResetPass').parent().removeClass('has-error');
            $('#secondResetPass').parent().addClass('has-success');
            $('#firstResetPass').parent().removeClass('has-error');
            $('#firstResetPass').parent().addClass('has-success');
            $('#resetSubmit').show();
        } else {
            // Passwords don't match; disable submit and warn
            $('#secondResetPass').parent().removeClass('has-success');
            $('#secondResetPass').parent().addClass('has-error');
            $('#firstResetPass').parent().removeClass('has-success');
            $('#firstResetPass').parent().addClass('has-error');
            $('#resetSubmit').hide();
        }
    });
});
$(document).ready(function () {
    $('#uploadNewImg').click(function () {
        $.post(".", {
            submit: "Image",
            type: "user",
            image: imgData.split(",")[1]
        });
    });
});

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
                var newImg;
                var h = imgObject.naturalHeight;
                var w = imgObject.naturalWidth;
                if (w > h) {
                    newImg = getImagePortion(imgObject, h, h, (w / 2) - (h / 2), 0, 1);
                } else {
                    newImg = getImagePortion(imgObject, w, w, 0, (h / 2) - (w / 2), 1);
                }
                // var newImg = getImagePortion(imgObject, imgObject.width, imgObject.height, 0, 0, 1);
                $('#previewImage').attr('src', newImg);
                $('#previewImage').removeClass("hidden");
                $('#uploadNewImg').removeClass("hidden");
                imgData = newImg;
            }
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function populateDays() {
    $('.days').remove();
    $("<option class=\"days\" selected value=\"0\">Select a Day</option>").appendTo($('#days'));
    for (var i = 1; i <= daysInMonth($('#userBirthdayMonthChange').val()); i++) {
        // Create a day:
        $("<option class=\"days\" value=\"" + i + "\">" + i + "</option>").appendTo($('#days'));
    }
}
function daysInMonth(month) {
    // Leap day
    if (month == 0) {
        return 0;
    }
    return new Date(2004, month, 0).getDate();
}
function getImagePortion(imgObj, newWidth, newHeight, startX, startY, ratio) {
    /* the parameters: - the image element - the new width - the new height - the x point we start taking pixels - the y point we start taking pixels - the ratio */
    //set up canvas for thumbnail
    var tnCanvas = document.createElement('canvas');
    var tnCanvasContext = tnCanvas.getContext('2d');
    tnCanvas.width = newWidth; tnCanvas.height = newHeight;

    /* use the sourceCanvas to duplicate the entire image. This step was crucial for iOS4 and under devices. Follow the link at the end of this post to see what happens when you don’t do this */
    var bufferCanvas = document.createElement('canvas');
    var bufferContext = bufferCanvas.getContext('2d');
    bufferCanvas.width = imgObj.width;
    bufferCanvas.height = imgObj.height;
    bufferContext.drawImage(imgObj, 0, 0);

    /* now we use the drawImage method to take the pixels from our bufferCanvas and draw them into our thumbnail canvas */
    tnCanvasContext.drawImage(bufferCanvas, startX, startY, newWidth * ratio, newHeight * ratio, 0, 0, newWidth, newHeight);
    return tnCanvas.toDataURL();
}