$(document).ready(function () {
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            // Validate:
            if ($('#secondResetPass').val() && $('#secondResetPass').val() === $('#firstResetPass').val()) {
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