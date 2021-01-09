﻿$(document).ready(() => {
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
});