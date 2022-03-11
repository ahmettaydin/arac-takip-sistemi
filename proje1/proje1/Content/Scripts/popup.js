var popup = document.getElementById('popup');

var btn1 = document.getElementById("btn1");
var btn2 = document.getElementById("btn2");

btn1.onclick = function () {
    popup.style.display = "block";
}

btn2.onclick = function () {
    popup.style.display = "block";
}

window.onclick = function (event) {
    if (event.target == popup) {
        popup.style.display = "none";
    }
}