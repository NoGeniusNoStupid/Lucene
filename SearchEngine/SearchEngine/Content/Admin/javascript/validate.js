function check() {

    var controls = document.getElementsByTagName("input");
    for (var i = 0; i < controls.length; i++) {
        if (controls[i].type == "file") {
            continue;
        }

        if (controls[i].value == "") {
            alert("请输入" + controls[i].getAttribute("placeholder"));
            return false;
        }
        if (controls[i].type == "tel") {
            var date = controls[i].value;
            if (!validatemobile(date)) {
                return false;
            }
        }
        if (controls[i].type == "password") {
            var date = controls[i].value;
            if (date.length < 6) {
                alert(controls[i].getAttribute("placeholder") + "长度不能小于6");
            }
        }
    }
    $('#add').submit();
}

//手机校验
function validatemobile(mobile) {
    if (mobile.length == 0) {
        alert('手机号码不能为空！');
        return false;
    }
    if (mobile.length != 11) {
        alert('请输入有效的手机号码，需是11位！');
        return false;
    }
    var myreg = /^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
    if (!myreg.test(mobile)) {
        alert('请输入有效的手机号码！');
        return false;
    }
    return true;
}