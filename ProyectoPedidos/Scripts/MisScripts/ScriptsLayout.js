'use strict;'

// 1) Definicion de variables globales
var cookies = {};


// 2) Metodo tras carga de la página
$(document).ready(function () {
    //YA ESTÁ TODO EL HTML CARGADO

    //Gestionamos las cookies que vienen del servidor
    GestionarCookies();

    //Cuando abrimos la modal con el formulario del login
    OnShowModalLogin();
});


// 3) Resto de funciones

//TEN CUIDADO: No llames a una funcion de la misma manera en dos
//scripts diferentes, pues la última sobreescribe a la primera. 
//¿Porqué ? --> Lo que hace un script(un fichero de extensión .js)
//es guardar variables y funciones en el WINDOW OBJECT DEL NAVEGADOR
//como propiedades suyas, y como no puede haber 2 propiedades distintas
//con el mismo nombre, la última añadida sobreescribe a la primera.
function GestionarCookies() {

    let cAux = document.cookie.split("; ");

    cAux.forEach(function (cookie) {
        let info = cookie.split("=");
        cookies[info[0]] = info[1];
    });

    if (cookies.CodigoCliente || cookies.CodigoEmpleado)
        displayLogout();
    else 
        displayLogin();
}

function OnShowModalLogin() {
    $('#modalLogin').on("show.bs.modal", (event) => {
        var button = $(event.relatedTarget);
        var esCliente = button.data('esCliente');
        if (esCliente) {
            $('#modalLogin .modal-title').text("Login Cliente");
            $('#modalLogin .modal-header').removeClass("bg-success");
            $('#modalLogin .modal-header').addClass("bg-primary");
            $('#formLogin').attr("action", "/Home/ValidarCliente");
            $('#username').attr("name", "txtMail");
        } else {
            $('#modalLogin .modal-title').text("Login Empleado");
            $('#modalLogin .modal-header').removeClass("bg-primary");
            $('#modalLogin .modal-header').addClass("bg-success");
            $('#formLogin').attr("action", "/Home/ValidarEmpleado");
            $('#username').attr("name", "txtNombreEmpleado");
        }
    });
}

function desaparece() {
    setTimeout(() => {
        $('#txtLoginIncorrecto').html("");
    }, 2000);
}

function displayLogin() {
    $("#login").css("display", "");
    $("#logout").css("display", "none");
    $('#nombreUsuario').html("");
}

function displayLogout() {
    $("#login").css("display", "none");
    $("#logout").css("display", "");

    if(cookies.CodigoCliente)
        $('#nombreUsuario').html("<b>Cliente:</b> <i>" + cookies.NombreCliente + "</i>");
    else if (cookies.CodigoEmpleado)
        $('#nombreUsuario').html("<b>Empleado:</b> <i>" + cookies.NombreEmpleado + "</i>");
}