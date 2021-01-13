var stars = 0;

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

function OnHover(id) {
    var len = parseInt(id);
    stars = parseInt(id);

    for (var i = 1; i <= 5; i++) {
        $(`#star${i}`).removeClass("checked");
    }
    for (var i = 1; i <= len; i++) {
        $(`#star${i}`).addClass("fa fa-star checked");
    }
}

function SetRating(id) {
    stars = parseInt(id);
}

$(document).ready(() => {
    var url = new URL(location.href);
    var id = url.searchParams.get("id");

    $.ajax({
        url: '/Home/GetGenre/',
        type: 'get',
        data: {
            'id': parseInt(id)
        },
        success: data => {
            for (var i = 0; i < data.length; i++) {
                var img = `<img src='/Home/Show/?path=${data[i].preview}' height='250' width='200' style='margin-top: 30px;margin-left:30px;margin-right:20px;' />`;
                var name = `<input type='submit' name='submit' class='btn btn-outline-info' style='display:block;width:200px;padding-bottom:15px;margin-bottom:30px;margin-left:30px;margin-right:20px;' value='${data[i].name}'/>`;
                var noob = `<div class='noob'>${name}</div>`;

                var col = `<div class='col'>${img}${noob}</div>`
                var form = `<form action='movie.php' method='post'>${col}</form>`;
                $("#genres").append(form);
            }
        },
        async: true
    });

    $(document).on("submit", "#rating", (e) => {
        var token = GetCookie('token');
        var buffer = {
            'id': parseInt(id),
            'count': stars,
            'comment': $("#data").val()
        };

        $.ajax({
            url: '/Home/Rate',
            type: 'post',
            data: buffer,
            headers: {
                "Authorization": `Bearer ${token}`
            },
            success: (res) => {
                setTimeout(() => {
                    location.reload(true);
                }, 1000);
            },
            async: true
        });
    });
});