function GetCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

$(document).ready(() => {
    $(document).on("submit", "#upkey", (e) => {
        e.preventDefault();

        var key = $('#key').val();
        var conf = $('#conf').val();
        var token = GetCookie('token');

        if (key === conf) {
            var buffer = {
                'key': key
            };

            $.ajax({
                url: '/Account/UpdatePassword',
                type: 'put',
                data: buffer,
                headers: {
                    'Authorization': `Bearer ${token}`
                },
                success: () => {
                    setTimeout(() => {
                        location.href = '/Home/Index';
                    }, 500);
                },
                error: () => {
                    setTimeout(() => {
                        location.href = '/Account/Login';
                    }, 500);
                },
                cache: false,
                async: true
            });
        }
        else {
            $('#conf').val('');
            $('#conf').css('border-color', 'red');
            $('#conf').attr("placeholder", "Retype Password!");
            $('#conf').addClass('danger');
        }
    });
});