
$(document).ready(function () {
    $("#image").change(function () {
        readURL(this);
    });
});
$(document).ready(function () {
    $('#events .event-closer').click(function () {
        eNumber = $(this).attr('data-event-id');
        $.post(".",
            {
                action: "Change",
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
        gNumber = $(this).attr('data-group-id');
        $.post(".",
            {
                action: "Change",
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
                        action: "Change",
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
                        action: "Change",
                        type: "User",
                        item: "email",
                        email: $('#userEmailChange').val()
                    });
            })
            .then(function () {
                if ($('#userBirthdayDayChange').val() == null || $('#userBirthdayDayChange').val() == 0) {
                    return Promise.resolve();
                } else {
                    $.post(".",
                        {
                            action: "Change",
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
                        action: "Change",
                        type: "User",
                        item: "bio",
                        bio: $('#userBioChange').val()
                    });
            })
            .then(function () {
                return $.post(".",
                    {
                        action: "Change",
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
                action: "Fetch",
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
                action: "Create",
                type: "Group",
                name: $('#newGroupName').val()
            }, function (data, status) {
                if (data != 0) {
                    // Success! loop over elems, sending email as additions:
                    $('#newGroupMembers li').each(function (index) {
                        // Post this group:
                        $.post(".", {
                            action: "Change",
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
            action: "Fetch",
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
                action: "Fetch",
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
                        action: "Change",
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
                        action: "Change",
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
                    action: "Change",
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
// New Event
$(document).ready(function () {
    // State Variables
    var newEventSteps = [0];
    var newEventStartDate = "";
    var newEventEndDate = "";
    var newEventName = null;
    var newEventRecurs = null;
    var newEventRecurType = null;
    var newEventInterval = null;
    var newEventSkip = null;
    var newEventDayOfWeek = null;
    var newEventPosn = null;
    var newEventGroupIds = [];
    var newEventGroupNames = [];
    var newEventBlackouts = [];

    $('#newEventPrevious').click(function () {
        if (newEventSteps[newEventSteps.length - 1] == 9) {
            $('#newEventSubmit').addClass("hidden");
            $('#newEventNext').removeClass("hidden");
        }
        $('#newEvent' + newEventSteps.pop(newEventSteps.length - 1)).fadeOut(500, function () {
            $('#newEvent' + newEventSteps[newEventSteps.length - 1]).fadeIn(500);
        });
        // re enable forward
        $('#newEventNext').removeAttr('disabled');
    });
    $('#newEventNext').click(function () {
        // Switch the step
        switch (newEventSteps[newEventSteps.length - 1]) {
            case 0:
                newEventName = $('#newEventName').val();
                // Fill in all spans
                $('.event-name-placeholder').text(newEventName);
                dispatch(1);
                $('#newEventPrevious').removeClass("hidden");
                $('#newEventNext').prop('disabled', true);
                break;
            case 1:
                newEventStartDate = $('#newEventStartDate').val();
                dispatch(2);
                $('#newEventNext').attr('disabled', true);
                break;
            case 2:
                if (newEventRecurs) {
                    // Go to 3
                    dispatch(3);
                    $('#newEventNext').attr('disabled', true);
                } else {
                    dispatch(8);
                }
                break;
            case 3:
                if (newEventRecurType == "exact") {
                    // Go to next
                    dispatch(4);
                } else {
                    dispatch(5);
                }
                break;
            case 4:
                newEventInterval = $('#newExactInterval').val();
                newEventSkip = $('#newExactSkip').val();
                if (newEventSkip == "n") {
                    newEventSkip = $('#newExactSkipEvery').val();
                }
                dispatch(6);
                break;
            case 5:
                newEventInterval = $('#newRelativeInterval').val();
                newEventSkip = $('#newRelativeSkip').val();
                if (newEventSkip == 'n') {
                    newEventSkip = $('#newRelativeSkipEvery').val();
                }
                newEventDayOfWeek = $('#newRelativeDayOfWeek').val();
                newEventPosn = $('#newRelativePosn').val();
                dispatch(6);
                break;
            case 6:
                newEventEndDate = $('#newEventEndDate').val();
                dispatch(7);
                break;
            case 7:
                $('.event-blackout').each(function (index) {
                    newEventBlackouts.push($(this).val());
                });
                dispatch(8);
                break;
            case 8:
                $('.event-group').each(function (index) {
                    newEventGroupIds.push($(this).attr('data-group-id'));
                    newEventGroupNames.push($(this).attr('data-group-name'));
                });
                dispatch(9);
                $('#newEventNext').addClass('hidden');
                // unhide button
                fillDescription();
                $('#newEventSubmit').removeClass("hidden");
                break;
        }
    });
    function dispatch(next) {
        newEventSteps.push(next);
        $('#newEvent' + newEventSteps[length - 2]).fadeOut(500, function () {
            $('#newEvent' + next).fadeIn(500);
        });
    }
    $('#newEventName').keypress(validateName);
    $('#newEventStartDate').change(validateStartDate);
    $('#newEventRecurYes').click(function () {
        $('#newEventRecurYes').removeClass('btn-info btn-danger').addClass('btn-success').html("Yes <i class=\"fa fa-check\"></i>");
        $('#newEventRecurNo').removeClass('btn-info btn-success').addClass('btn-info').html("No");
        $('#newEventNext').removeAttr('disabled');
        newEventRecurs = true;
    });
    $('#newEventRecurNo').click(function () {
        $('#newEventRecurNo').removeClass('btn-info btn-danger').addClass('btn-success').html("No <i class=\"fa fa-check\"></i>");
        $('#newEventRecurYes').removeClass('btn-info btn-success').addClass('btn-info').html("Yes");
        $('#newEventNext').removeAttr('disabled');
        newEventRecurs = false;
    });
    $('#newEventExact').click(function () {
        $('#newEventExact').removeClass('btn-info btn-danger').addClass('btn-success').html("Exact <i class=\"fa fa-check\"></i>");
        $('#newEventRelative').removeClass('btn-info btn-success').addClass('btn-info').html("Relative");
        $('#newEventNext').removeAttr('disabled');
        newEventRecurType = "exact";
    });
    $('#newEventRelative').click(function () {
        $('#newEventRelative').removeClass('btn-info btn-danger').addClass('btn-success').html("Relative <i class=\"fa fa-check\"></i>");
        $('#newEventExact').removeClass('btn-info btn-success').addClass('btn-info').html("Exact");
        $('#newEventNext').removeAttr('disabled');
        newEventRecurType = "relative";
    });
    $('#newExactSkip').change(function () {
        if ($('#newExactSkip').val() == 'n') {
            $('#newExactSkipEvery').removeClass("hidden");
            $('#newEventNext').attr('disabled', true);
        } else {
            $('#newExactSkipEvery').addClass("hidden");
            $('#newEventNext').removeAttr('disabled');
        }
    });
    $('#newExactSkipEvery').keypress(function () {
        if ($('#newExactSkipEvery').val() && $('#newExactSkipEvery').val() > 0) {
            $('#newEventNext').removeAttr('disabled');
        } else {
            $('#newEventNext').attr('disabled', true);
        }
    });
    $('#newRelativeSkip').change(function () {
        if ($('#newRelativeSkip').val() == 'n') {
            $('#newRelativeSkipEvery').removeClass("hidden");
            $('#newEventNext').attr('disabled', true);
        } else {
            $('#newRelativeSkipEvery').addClass("hidden");
            $('#newEventNext').removeAttr('disabled');
        }
    });
    $('.event-blackout').change(function () {
        // Check value of last one, if not empty, add one:
        var lastElem = $('.event-blackout').last();
        if (lastElem.val()) {
            var newRow = $('<div></div>');
            newRow.addClass("text-center row");
            var newBlack = $('<input />');
            newBlack.addClass('form-control step-control event-blackout');
            newBlack.attr('type', 'date');
            newBlack.attr('placeholder', 'Blackout Date');
            newRow.append(newBlack);
            $('#newEventBlackouts').append(newRow);
        } else {
            // Now check second to last. If valueless, delete last
            if ($('.event-blackout:nth-last-child(2)').val()) {
                // Delete last row
                lastElem.parent.remove();
            }
        }
        // Loop through all BUT the last two, delete if empty
        $('.event-blackout:nth-last-child(n+3)').each(function (index) {
            if (!$(this).val() && !$(this).is(":focus")) {

            }
        });
    });
    function validateName() {
        if ($('#newEventName').val() && $('#newEventName').val().length <= 32 && $('#newEventName').val().trim()) {
            return isCorrect();
        } else {
            $('#newEventNext').attr('disabled', true);
            return false;
        }
    }
    function validateStartDate() {
        if ($('#newEventStartDate').val()) {
            return isCorrect();
        } else {
            $('#newEventNext').attr('disabled', true);
            return false;
        }
    }
    function validateEndDate() {
        if ($('#newEventEndDate').val()) {
            // check if less than start
            if (new Date(newEventStartDate) < new Date(newEventEndDate)) {
                // OK
                return isCorrect();
            } else {
                // show error
                $('#newEventMessage').text("Please choose an end date that is after your starting date").addClass('step-error');
                $('#newEventMessages').removeClass("hidden");
                $('#newEventNext').attr('disabled', true);
                return false;
            }
        } else {
            // null so fine
            return isCorrect();
        }
    }
    function isCorrect() {
        $('#newEventNext').removeAttr('disabled');
        $('#newEventMessages').addClass("hidden");
        $('#newEventMessage').removeClass("step-danger step-info");
        return true;
    }
    function createEvent() {
        // Post to the serve all the date details:
        if (!newEventRecurs) {
            // no recurrence, so just send:
            // Name
            // Date
            // Groups
            $.post(".", {
                action: "Create",
                type: "Event",
                name: newEventName,
                startDate: newEventStartDate
            }, createGroups);
        } else {
            // figure out type of recurrence and engage:
            if (newEventRecurType == "exact") {
                // post new exact event:
                // Name
                // StartDate
                // SkipEvery
                // Interval
                // Then add blackouts, etc?
                $.post(".", {
                    action: "Create",
                    type: "Event",
                    name: newEventName,
                    startDate: newEventStartDate,
                    endDate: newEventEndDate,
                    recurType: newEventRecurType,
                    skipEvery: newEventSkip,
                    interval: newEventInterval
                }, createBlackouts);
            } else {
                $.post("." {
                    action: "Create",
                    type: "Event",
                    name: newEventName,
                    startDate: newEventStartDate,
                    endDate: newEventEndDate,
                    recurType: newEventRecurType,
                    skipEvery: newEventSkip,
                    interval: newEventInterval,
                    posn: newEventPosn,
                    dayOfWeek: newEventDayOfWeek
                }, createBlackouts);
            }
        }
    }
    function createGroups(eid) {
        for (var i = 0; i < newEventGroupIds.length; i++) {
            $.post(".", {
                action: "Change",
                type: "Event",
                itemId: eid,
                item: "addGroup",
                groupId: newEventGroupIds[i]
            });
        }
        var eventHolder = $('#events');

        // Add record of event to event lines
        var row = $('<tr></tr');
        row.attr('data-event-id', eid);
        var closer = $('<span></span>');
        closer.addClass('event-closer');
        closer.attr('data-event-id', eid);
        row.append($('<td></td>').append(closer));
        var eName = $('<h3></h3>');
        eName.text(newEventName);
        row.append($('<td></td>').append(eName));
        eventHolder.append(row);
    }

    function createBlackouts(eid) {
        // Add blackouts
        for (var i = 0; i < newEventBlackouts.length; i++) {
            $.post(".", {
                action: "Change",
                type: "Event",
                itemId: eid,
                item: "addBlackout",
                blackout: newEventBlackouts[i]
            });
        }
        createGroups(eid);
    }
    function fillDescription() {
        // Fill Description div:
        var desc = $('#newEventDescription');
        var str;
        if (newEventRecurs) {
            // see if exact or relative:
            if (newEventRecurType == "exact") {
                str = "<p>" + escapeHtml(newEventName) + " starts on " + escapeHtml(newEventStartDate) + ", and occurs every ";
                if (newEventSkip == 2) {
                    var str = str + "other ";
                } else if (newEventSkip > 2) {
                    var str = str + newEventSkip + " ";
                }
                if (newEventInterval == 'D') {
                    str = str + "Day";
                } else if (newEventInterval == 'W') {
                    str = str + "Week";
                } else if (newEventInterval == 'M') {
                    str = str + "Month";
                } else {
                    str = str + "Year";
                }
            } else {
                // Relative event
                str = "<p>" + escapeHtml(newEventName) + " starts on " + escapeHtml(newEventStartDate) + ", and occurs on the ";
                if (newEventPosn == 1) {
                    str = str + "First ";
                } else if (newEventPosn == 2) {
                    str = str + "Second ";
                } else if (newEventPosn == 3) {
                    str = str + "Third ";
                } else if (newEventPosn == 4) {
                    str = str + "Fourth ";
                } else {
                    str = str + "Last ";
                }
                if (newEventDayOfWeek == "N") {
                    str = str + "Sunday";
                } else if (newEventDayOfWeek == "M") {
                    str = str + "Monday";
                } else if (newEventDayOfWeek == "T") {
                    str = str + "Tuesday";
                } else if (newEventDayOfWeek == "W") {
                    str = str + "Wednesday";
                } else if (newEventDayOfWeek == "R") {
                    str = str + "Thursday";
                } else if (newEventDayOfWeek == "F") {
                    str = str + "Friday";
                } else if (newEventDayOfWeek == "S") {
                    str = str + "Saturday";
                }
                str = str + " of every ";
                if (newEventSkip == 2) {
                    str = str + "other ";
                } else if (newEventSkip > 2) {
                    str = str + newEventSkip;
                }
                if (newEventInterval == "JAN") {
                    str += "January";
                } else if (newEventInterval == "FEB") {
                    str += "February";
                } else if (newEventInterval == "MAR") {
                    str += "March";
                } else if (newEventInterval == "APR") {
                    str += "April";
                } else if (newEventInterval == "MAY") {
                    str += "May";
                } else if (newEventInterval == "JUN") {
                    str += "June";
                } else if (newEventInterval == "JUL") {
                    str += "July";
                } else if (newEventInterval == "AUG") {
                    str += "August";
                } else if (newEventInterval == "SEP") {
                    str += "September";
                } else if (newEventInterval == "OCT") {
                    str += "October";
                } else if (newEventInterval == "NOV") {
                    str += "November";
                } else if (newEventInterval == "DEC") {
                    str += "December";
                } else {
                    str += "Month";
                }
            }
            if (newEventEndDate != null) {
                str = str + ". It stops on " + escapeHtml(newEventEndDate) + ".";
            } else {
                str = str + ".";
            }
            str = str + "</p>";
            str = $(str);
            desc.append(str);

            if (newEventBlackouts.length > 0) {
                var intro = $("<p>Additionally, this event will <em><strong>not</strong></em> occur on any of the following dates:</p>");
                var blackouts = $("<ul></ul>");
                for (var i = 0; i < newEventBlackouts; i++) {
                    blackouts.append($("<li>" + escapeHtml(newEventBlackouts[i]) + "</li>"));
                }
                desc.append(intro);
                desc.append(blackouts);
            }
        } else {
            // Single date:
            str = $("<p>" + escapeHtml(newEventName) + " occurs on " + escapeHtml(newEventStartDate) + ".</p>");
            desc.append(str);
        }
        desc.append($("<br /><p>It's viewable by the following groups:</p>"));
        var groups = $("<ul></ul>");
        for (var i = 0; i < newEventGroupNames; i++) {
            groups.append("<li>" + escapeHtml(newEventGroupNames[i]) + "</li>");
        }
        desc.append(groups);
    }
});
$(document).ready(function () {
    $('#uploadNewImg').click(function () {
        $.post(".", {
            action: "Image",
            type: "user",
            image: imgData.split(",")[1]
        });
    });
});
$(document).ready(function () {
    // Change checks to x on mouse over
    $('#facebookConfirmed').click(function () {
        $.post(".", {
            action: "Change",
            type: "User",
            item: "facebookLogin"
        }, function (data, status, xhr) {
            location.reload(true);
        });
    });
    $('#googleConfirmed').click(function () {
        $.post(".", {
            action: "Change",
            type: "User",
            item: "googleLogin"
        }, function (data, status, xhr) {
            location.reload(true);
        });
    });
});
$(document).ready(function () {
    $('#deleteAccountCancel').click(function () {
        // clear email box
        $('#deleteAccountEmail').val("");
        $('#deleteAccount').modal('hide');
    });
    $('#accountDeleter').click(function () {
        $('#changePreferences').modal('hide');
        $('#deleteAccount').modal();
    });
    $('#deleteAccountEmail').keyup(function () {
        if ($('#deleteAccountEmail').val() == $('#email').text().substr(7)) {
            $('#deleteAccountConfirm').removeClass("hidden");
        } else {
            $('#deleteAccountConfirm').addClass("hidden");
        }
    });
});

var imgData;
function validateReset() {
    return $('#resetEmail').val() !== "";
}
function renderGoogleLogin() {
    $(document).ready(function () {
        gapi.signin2.render('googleLogin', {
            'scope': 'profile email',
            'width': 240,
            'height': 50,
            'longtitle': true,
            'theme': 'dark',
            'onsuccess': onSuccess,
            'onfailure': onFailure
        });
    });
}
function onSuccess(googleUser) {
    $.post(".", {
        action: "Login",
        type: "Google",
        token: googleUser.getAuthResponse().id_token
    }, function (data, status, xhr) {
        // Parse data - if error message, populate red alert; otherwise, reload to dashboard:
        var resp = xhr.responseText;
        if (resp == "success") {
            // Show user we succeeded
            $('#googleLoginStatus *').remove();
            $('#googleLoginStatus').append('<span id=\"googleConfirmed\" class=\"fa fa-close oauth-confirmed\"></span>');
            var auth2 = gapi.auth2.getAuthInstance();
            auth2.signOut();
        } else {
            // Show user not added (add alert?)

        }
    });
}
function onFailure(error) {

}
window.fbAsyncInit = function () {
    FB.init({
        appId: '553216418365065',
        cookie: true,
        xfbml: true,
        version: 'v2.11'
    });

    FB.AppEvents.logPageView();

};

(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.11&appId=553216418365065";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

function fbLoginStatusChange(response) {
    if (response.status === 'connected') {
        // logged into app, send access token
        $.post(".", {
            action: "Login",
            type: "Facebook",
            token: response.authResponse.accessToken
        }, function (data, status, xhr) {
            var resp = xhr.responseText;
            if (resp == "success") {
                $('#facebookLoginStatus *').remove();
                $('#facebookLoginStatus').append('<span id=\"facebookConfirmed\" class=\"fa fa-close oauth-confirmed\"></span>');
            } else {
                // We need to die gracefully

            }
        });
    }
}
function fbCheckLoginState() {
    FB.getLoginStatus(function (response) {
        fbLoginStatusChange(response);
    });
}
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

function escapeHtml(str) {
    var div = document.createElement('div');
    div.appendChild(document.createTextNode(str));
    return div.innerHTML;
}