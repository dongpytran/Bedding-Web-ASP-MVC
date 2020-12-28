$(document).ready(function () {
    loadData();
});

function loadData() {
    $.ajax({
        url: "/AdminMenu/listUser",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<tr>';
                html += '<td>' + item.USERNAME + '</td>';
                html += '<td>' + item.MATKHAU + '</td>';
                html += '<td>' + item.HOTEN + '</td>';
                if (item.TRANGTHAI == 1) {
                    html += '<td><button type="button" id="' +item.USERNAME +'" onclick="changeStatus('+(this.USERNAME)+')" class="btn-active btn btn-success">Lock</button></td>';
                } if(item.TRANGTHAI ==0){
                    html += '<td><button type="button" id="' + item.USERNAME + '" onclick="changeStatus(' + (this.USERNAME) + ')" class="btn-active btn btn-danger">Unlock</button></td>';
                }
                html += '</tr>';
            });
            $('.ttbody').html(html);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
function changeStatus(USERNAME) {
        $.ajax({
            url: "/AdminMenu/changeStatus/?USERNAME=" + String($.trim(USERNAME)),
            type: "POST",
            contentType: "application/json;charset:utf-8",
            dataType: "json",
            success: function (result) {
                alert('Success!');
                loadData();
                USERNAME = null;
            },
            error: function (errorMess) {
                alert(errorMess.responseText);
            }
        });
}
function Add() {
    var obj = {
        USERNAME: $('#username').val(),
        MATKHAU: $('#pass').val(),
        type: $('#fullname').val(),
    };
    $.ajax({
        url: "/AdminMenu/Add",
        data: JSON.stringify(obj),
        type: "POST",
        contentType: "application/json;charset:utf-8",
        dataType: "json",
        success: function (result) {
            loadData();
            $('#addModal').trigger('click');
        },
        error: function (errorMess) {
            alert(errorMess.responseText);
        }
    });
}

function getById(FoodId) {
    $.ajax({
        url: "Tuan12/getById?id=" + FoodId,
        type: "GET",
        contentType: "application/json;charset:utf-8",
        dataType: "json",
        success: function (result) {
            $('#id').val(result.id);
            $('#name').val(result.name);
            $('#type').val(result.type);
            $('#price').val(result.price);
            $('#addModal').modal('show');
            $('#btnUpdate').show();
            $('#btnAdd').hide();
        },
        error: function (errorMess) {
            alert(errorMess.responseText);
        }
    });
}

function clearTextBox() {
    $('#id').val("");
    $('#name').val("");
    $('#type').val("");
    $('#price').val("");
    $('#btnUpdate').hide();
    $('#btnAdd').show();
}

function Update() {
    var obj = {
        id: $('#id').val(),
        name: $('#name').val(),
        type: $('#type').val(),
        price: $('#price').val(),
    };
    $.ajax({
        url: "/Tuan12/Update",
        data: JSON.stringify(obj),
        type: "POST",
        contentType: "application/json;charset:utf-8",
        dataType: "json",
        success: function (result) {
            loadData();
            $('#addModal').trigger('click');
            clearTextBox();
        },
        error: function (errorMess) {
            alert(errorMess.responseText);
        }
    });
}

function Delete(id) {
    var conf = confirm("Delete this record?");
    if (conf) {
        $.ajax({
            url: "/Tuan12/Delete/" + id,
            type: "POST",
            contentType: "application/json;charset:utf-8",
            dataType: "json",
            success: function (result) {
                loadData();
            },
            error: function (errorMess) {
                alert(errorMess.responseText);
            }
        });
    }
}
