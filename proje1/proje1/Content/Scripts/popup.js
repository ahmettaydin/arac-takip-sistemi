var popup = document.getElementById('popup');

var btn1 = document.getElementById("btn1");
var btn2 = document.getElementById("btn2");
var vId = document.querySelector('input[name="vId"]');

btn1.onclick = function () {
    vId.value = "1";
    popup.style.display = "block";
}

btn2.onclick = function () {
    vId.value = "2";
    popup.style.display = "block";
}

window.onclick = function (event) {
    if (event.target == popup) {
        popup.style.display = "none";
    }
}