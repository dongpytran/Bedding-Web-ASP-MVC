var form;
var form2;
form = document.getElementById('createProForm');
form2 = document.getElementById('editForm');
const masp = document.getElementById('MASANPHAM');
const tensp = document.getElementById('TENSANPHAM');
const giagoc = document.getElementById('GIAGOC');
const anh = document.getElementById('fileupload');
const sl = document.getElementById('SLTON');
if (form != null) {
    //add event cho form create new sp
    form.addEventListener('submit', function (e) {
        let message = []
        if (masp.value === '') {
            message.push("Ma san pham khong duoc trong");
        }
        if (masp.value.length > 5) {
            message.push()
        }
        if (tensp.value === '') {
            message.push("Ten san pham khong duoc trong");
        }
        if (giagoc.value === '') {
            message.push("Gia san pham khong duoc trong");
        }
        if (anh.value === '') {
            message.push("Anh san pham khong duoc trong");
        }
        if (sl.value === '') {
            message.push('Nhap So Luong Ton');
        }
        if (message.length > 0) {
            e.preventDefault()
            alert(message.join(' , '));
        }
    })
}
