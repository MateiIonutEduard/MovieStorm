$(document).ready(() => {
    $(document).on('submit', '#results', function (e) {
        e.preventDefault();
        var name = $('#name').val();
        location.href = `/Home/Results/?name=${name}`;
    });
});