$(document).ready(() => {
    $(document).on("submit", "#signup", (e) => {
        e.preventDefault();
        
        var buffer = {
            'username': $("#name").val(),
            'password': $("#pass").val(),
            'address': $("#address").val(),
            'logo': $('#logo').get(0).files[0],
            'admin': $('#admin').val()
        };

        $.ajax({
            url: '/Account/Signup',
            type: 'post',
            data: buffer,
            cache: false,
            contentType: false,
            processData: false,
            success: data => {
                setTimeout(() => {
                    location.href = '/Home/Index';
                }, 1000);
            },
            async: true
        });
    });
});