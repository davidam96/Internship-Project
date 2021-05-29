
$(document).ready(function () {
    //YA ESTÁ TODO EL HTML CARGADO

    //Gestionamos las cookies que vienen del servidor
    gestionarCookies();
});

function gestionarCookies() {

    var cAux = document.cookie.split("; ");

    cAux.forEach(function (cookie) {
        let info = cookie.split("=");
        cookies[info[0]] = info[1];
    });

    var esLoginValido = Number.parseInt(cookies.EsLoginValido);
    if (esLoginValido) {
        let esCliente = true;
        switch (cookies.EsCliente) {
            case "0":
                renderLogout(!esCliente);
                break;
            case "1":
                renderLogout(esCliente);
                break;
        }
    } else {
        renderLogin();
    }
}