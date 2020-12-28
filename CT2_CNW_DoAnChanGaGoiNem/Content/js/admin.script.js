var max_chars = 5;
$('#MASANPHAM').keydown(function (e) {
    if ($(this).val().length >= max_chars) {
        $(this).val($(this).val().substr(0, max_chars));
    }
});

$('#MASANPHAM').keyup(function (e) {
    if ($(this).val().length >= max_chars) {
        $(this).val($(this).val().substr(0, max_chars));
    }
});

//View img when upload len
function ShowImagePreview(imageUploader, previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}

//Update gia ban sau khi chon giam gia
$(document).ready(function () {
    var goc;
    var tong;
    $('#GIAGOC').change(function () {
        var giagoc = $(this).val();
        var giam = $('#GIAM').val();
        var tongt = parseInt(giagoc) - (parseFloat(giam) * parseInt(giagoc))
        goc = giagoc;
        $("#GIABAN").val(tongt);
    })
    $("#GIAM").change(function () {
        var tt = $('#GIAGOC').val()
        var giam = $('#GIAM').val()
        if (giam == 0) {
            tong = tt;
        } else {
            var tongt = parseInt(tt) - (parseFloat(giam) * parseInt(tt))
            tong = tongt
        }
        $("#GIABAN").val(tong);
    });
})

//show duoi file upload
$('#fileupload').on('change', function () {
    //get the file name
    var fileName = $(this).val();
    //replace the "Choose a file" label
    $('.custom-file-label').html(fileName);
})
