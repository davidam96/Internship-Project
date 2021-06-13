// 1) Definicion de variables globales
// ....


// 2) Metodo tras carga de la página
$(document).ready(function () {
    //YA ESTÁ TODO EL HTML CARGADO

    //Gestionamos las cookies que vienen del servidor
    gestionarCookies();

    //Cuando abrimos la modal con el formulario del login
    onShowModalLogin();
});


// 3) Resto de funciones
function gestionarCookies() {

    var cAux = document.cookie.split("; ");
    var cookies = {};

    cAux.forEach(function (cookie) {
        let info = cookie.split("=");
        cookies[info[0]] = info[1];
    });

    if (cookies.CodigoUsuario === undefined) {
        displayLogin();
    }
}

function onShowModalLogin() {
    $('#modalLogin').on("show.bs.modal", (event) => {
        var button = $(event.relatedTarget);
        var esCliente = button.data('esCliente');
        if (esCliente) {
            $('#modalLogin .modal-title').text("Login Cliente");
            $('#modalLogin .modal-header').removeClass("bg-success");
            $('#modalLogin .modal-header').addClass("bg-primary");
            $('#esCliente').attr("value", "1");
        } else {
            $('#modalLogin .modal-title').text("Login Empleado");
            $('#modalLogin .modal-header').removeClass("bg-primary");
            $('#modalLogin .modal-header').addClass("bg-success");
            $('#esCliente').attr("value", "0");
        }
    });
}

function displayLogin() {
    $("#login").css("display", "");
    $("#logout").css("display", "none");
    $('#nombreUsuario').html("");
}