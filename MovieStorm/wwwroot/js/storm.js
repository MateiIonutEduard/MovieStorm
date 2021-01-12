$(document).ready(() => {
    $(document).on('submit', '#results', function (e) {
        e.preventDefault();
        var name = $('#name').val();
        location.href = `/Home/Results/?name=${name}`;
    });

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

    var token = GetCookie('token');

    setInterval(() => {
        $.ajax({
            url: '/Account/RefreshToken/',
            type: 'post',
            data: {
                'token': GetCookie('token')
            },
            headers: {
                "Authorization": `Bearer ${token}`
            },
            success: data => {
                document.cookie = `token=${data.token};path=/`;
            },
            async: true
        });
    }, 119000);
});