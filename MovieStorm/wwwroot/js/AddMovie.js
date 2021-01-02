$(document).ready(function () {
    $.ajax({
        url: '/Home/GetLangs/',
        type: 'get',
        cache: false,
        success: function (data) {
            for (var i = 0; i < data.length; i++)
                $("#lang").append(`<option value='${data[i].code}'>${data[i].countryName}</option>`);
        },
        async: true
    });

    $(document).on('submit', '#upform', function (e) {
        e.preventDefault();
        var buffer = new FormData(this);
        
        $.ajax({
            url: '/Home/AddMovie/',
            type: 'post',
            contentType: false,
            cache: false,
            processData: false,
            data: buffer,
            xhr: function () {
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener('progress', function (e) {
                    if (e.lengthComputable) {
                        var percent = parseInt((e.loaded / e.total) * 100);
                        $('.progress-bar').width(`${percent}%`);
                    }
                }, false);

                return xhr;
            },
            error: function (xhr) {
                console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
            },
            beforeSend: function () {
                $('.container-sm').css('display', 'block');
                $('.progress-bar').width("0%");
            },
            success: function (data) {
                $('.progress-bar').width('0%');
                $('.container-sm').css('display', 'none');
                console.log('all done!');
                location.href = "/Home/Index";
            }
        });
    });
});