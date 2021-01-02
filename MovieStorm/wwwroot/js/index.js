$(document).ready(function () {
    var query = location.href;
    var url = new URL(query);
    var filter = url.searchParams.get("filter");
    
    $.ajax({
        url: '/Home/GetLatest/',
        type: 'get',
        cache: false,
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                var img = `<img src='/Home/Show/?path=${data[i].preview}' height='250' width='200' style='margin-top: 30px;margin-left:30px;margin-right:20px;' />`;
                var name = `<input type='submit' name='submit' class='btn btn-outline-info' style='display:block;width:200px;padding-bottom:15px;margin-bottom:30px;margin-left:30px;margin-right:20px;' value='${data[i].name}'/>`;
                var noob = `<div class='noob'>${name}</div>`;

                var col = `<div class='col'>${img}${noob}</div>`
                var form = `<form action='movie.php' method='post'>${col}</form>`;
                $("#news").append(form);
            }
        },
        async: true
    });

    $.ajax({
        url: '/Home/GetMovies/',
        type: 'get',
        cache: false,
        data: {
            'filter': filter
        },
        success: function (data) {
            var str = "<div class='row'>";
            var j = 1;

            for (var i = 0; i < data.length; i++) {
                var img = `<img src='/Home/Show/?path=${data[i].preview}' height='250' width='200' style='margin-top: 30px;margin-left:30px;margin-right:20px;' />`;
                var name = `<input type='submit' name='submit' class='btn btn-outline-info' style='display:block;width:200px;padding-bottom:15px;margin-bottom:30px;margin-left:30px;margin-right:20px;' value='${data[i].name}'/>`;
                var noob = `<div class='noob'>${name}</div>`;

                var col = `<div class='col'>${img}${noob}</div>`
                var form = `<form action='movie.php' method='post'>${col}</form>`;
                str = str.concat(form);

                if (j % 4 == 0 && i < data.length) {
                    str = str.concat('</div>');
                    $("#all").append(str);
                }
                else if (i == data.length - 1) {
                    str = str.concat('</div>');
                    $("#all").append(str);
                }
            }
        },
        async: true
    });
});