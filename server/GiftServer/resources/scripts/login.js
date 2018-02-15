$(document).ready(function () {
    $('#loginForm').submit(function (event) {
        if (!validateLogin()) {
            event.preventDefault();
            return false;
        }
    });
    $('#signupForm').submit(function (event) {
        if (!validateSignup()) {
            event.preventDefault();
            return false;
        }
    });
    $('#resetForm').submit(function (event) {
        if (!validateReset()) {
            event.preventDefault();
            return false;
        }
    });
});
$(document).ready(function () {
    $('.loginControl').keyup(validateLogin);
    $('.signupControl').keyup(validateSignup);
});
$(document).ready(function () {
    $('#cultureSelector').change(function () {
        $.post(".", {
            action: "Culture",
            culture: $(this).val()
        },
            function () {
                location.reload(true);
            });
    });
});
function validateLogin() {
    // Check email and password
    var valid = true;
    if ($('#loginEmail').val()) {
        // green email
        $('#loginEmail').parent().removeClass('has-error');
        $('#loginEmail').parent().addClass('has-success');
    } else {
        $('#loginEmail').parent().removeClass('has-success');
        $('#loginEmail').parent().addClass('has-error');
        valid = false;
    }
    if ($('#password').val()) {
        $('#password').parent().removeClass('has-error');
        $('#password').parent().addClass('has-success');
    } else {
        $('#password').parent().removeClass('has-success');
        $('#password').parent().addClass('has-error');
        valid = false;
    }
    return valid;
}
function validateSignup() {
    // Check Name, Email, Passwords
    var valid = true;
    if ($('#userName').val()) {
        // We are good to go!
        $('#userName').parent().removeClass('has-error');
        $('#userName').parent().addClass('has-success');
    } else {
        // Passwords don't match; disable submit and warn
        $('#userName').parent().removeClass('has-success');
        $('#userName').parent().addClass('has-error');
        valid = false;
    }
    if ($('#signupEmail').val()) {
        // We are good to go!
        $('#signupEmail').parent().removeClass('has-error');
        $('#signupEmail').parent().addClass('has-success');
    } else {
        // Passwords don't match; disable submit and warn
        $('#signupEmail').parent().removeClass('has-success');
        $('#signupEmail').parent().addClass('has-error');
        valid = false;
    }
    if ($('#secondPass').val() === $('#firstPass').val() && $('#firstPass').val()) {
        // We are good to go!
        $('#firstPass').parent().removeClass('has-error');
        $('#firstPass').parent().addClass('has-success');
        $('#secondPass').parent().removeClass('has-error');
        $('#secondPass').parent().addClass('has-success');
    } else {
        // Passwords don't match; disable submit and warn
        $('#firstPass').parent().removeClass('has-success');
        $('#firstPass').parent().addClass('has-error');
        $('#secondPass').parent().removeClass('has-success');
        $('#secondPass').parent().addClass('has-error');
        valid = false;
    }
    return valid;
}
function validateReset() {
    return $('#resetEmail').val() !== "";
}
var btnHolder = [];

function loadingLogin(btn) {
    var elem = $(btn).parent();
    btnHolder.push($(btn).detach());
    elem.append('<i class="fas fa-spinner fa-spin fa-3x fa-fw loader signup-loader"></i>');
}
function onSuccess(googleUser) {
    // Show spinner
    // remove google login
    var googleToken = googleUser.getAuthResponse().id_token;
    loadingLogin("#googleLogin");
    loadingLogin("#googleSignup");
    $.post(".", {
        action: "Login",
        type: "Google",
        token: googleToken
    }, function (data, status, xhr) {
        // Parse data - if error message, populate red alert; otherwise, reload to dashboard:
        var resp = xhr.responseText;
        if (resp === "success") {
            var auth2 = gapi.auth2.getAuthInstance();
            auth2.signOut().then(function () {
                location.reload(true);
            });
        } else if (resp === "confirm") {
            // Ask for password, prep to resubmit
            $('#oauthToken').val(googleToken);
            $('#oauthType').val("Google");
            $('#oauthTypeName').text("Google");
            // Show the password prompt
            $('#oauthPassword').modal();
        } else {
            // We need to die gracefully; sign out and onFailure
            auth2.signOut();
            onFailure();
        }
    });
}
function onFailure(error) {
    var elem = $('#loader').parent();
    $('#loader').remove();
    elem.append(btnHolder.pop());
    var msg = $($('meta[name="oauth-fail-alert"').attr('content'));
    $('#loginAlert').empty().prepend(msg).addClass('alert-danger').addClass('in').removeClass('hidden');
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
        gapi.signin2.render('googleSignup', {
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
    loadingLogin(".fb-login-button");
    if (response.status === 'connected') {
        // logged into app, send access token
        var fbToken = response.authResponse.accessToken;
        $.post(".", {
            action: "Login",
            type: "Facebook",
            token: fbToken
        }, function (data, status, xhr) {
            var resp = xhr.responseText;
            if (resp === "success") {
                location.replace(".?dest=dashboard");
            } else if (resp === "confirm") {
                // Ask for password, prep to resubmit
                $('#oauthToken').val(fbToken);
                $('#oauthType').val("Facebook");
                $('#oauthTypeName').text("Facebook");
                // Show the password prompt
                $('#oauthPassword').modal();
            } else {
                onFailure(null);
            }
        });
    }
}

function fbCheckLoginState() {
    FB.getLoginStatus(function (response) {
        fbLoginStatusChange(response);
    });
}