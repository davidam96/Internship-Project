// 1) Definicion de variables globales
var cookies = {};
var productos;
var lineasDetalle = [];


// 2) Metodo tras carga de la página
$(document).ready(function () {
    //YA ESTÁ TODO EL HTML CARGADO

    //Gestionamos las cookies que vienen del servidor
    gestionarCookies();

    //Rellenamos el combo con los productos mediante Ajax
    cargarCombo();

    //Cuando abrimos la modal con el formulario del login
    onShowModalLogin();
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

// 3) Resto de funciones

function cargarCombo() {

    //Carga del combo pidiendo los productos al Servidor

    var destino = '/Home/CargarProductos';

    $.ajax({
        url: destino,
        method: "GET",
        contentType: 'application/json; charset=utf-8',
        success: function (respuesta) {
            if (respuesta.Error === undefined) {
                productos = respuesta.Productos;
                cmb = $("#cmbProductos");
                //$(cmb).html("");
                var cad = "";
                for (var i = 0; i < productos.length; i++) {
                    var p = productos[i];
                    //<option value="ProdA">Producto A (5€)</option>
                    var opt = "<option value='" + p.Codigo + "'>" + p.Descripcion +
                        " (" + parseFloat(p.PrecioVenta).toFixed(2) + "€)</option>";
                    cad += opt;
                }
                $(cmb).html(cad);
                ///...
            }
            else {
                alert(respuesta.Error);
            }
        },
        error: function (e) {
            var msg = "Error no controlado en llamada a " + destino;
            if (e !== undefined && e !== null && e !== "")
                if (e.statusText !== "")
                    msg += "<br />" + e.statusText;
                else
                    msg += "<br />" + e;
            alert(msg);
        }
    });

    /*
    //$.get directo
    $.get(destino, function (respuesta) {
        alert("Respuesta recibida.");
    });

    //$.get con formato "promise"
    $.get(destino)
        .done(function (respuesta) {
            alert("Respuesta recibida.");
    });

    //$.get con formato "promise" y captura del error (errores NO controlados.)
    $.get(destino)
        .done(function (respuesta) {
            alert("Respuesta recibida.");
        })
        .fail(function (e) {
            alert("Error!!!");
    });
    */
}

function cargaCombo() {
    //Nos inventamos una serie de productos desde el cliente (JS)
    productos = [];
    for (let i = 1; i <= 5; i++) {
        var p = {};
        p.Id = "Prod" + i;
        p.Descripcion = "Producto " + i;
        p.PrecioVenta = ((Math.random() * 1000) + 1).toFixed(2);
        productos.push(p);
    }

    //Metemos los productos inventados en el combo de la página HTML
    cad = "";
    for (let i = 0; i < productos.length; i++) {
        cad += "<option value='" + productos[i].Id + "'>" + productos[i].Descripcion +
            " (" + productos[i].PrecioVenta + "€)</option>";
    }
    //document.getElementById("cmbProductos").innerHTML = cad;
    $("#cmbProductos").html(cad);
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

function renderLogout(esCliente) {
    $.ajax({
        type: "GET",
        url: "/Home/RenderLogout",
        success: function (info) {
            $('div[class*="navbar-collapse"]').html(info);
            if (esCliente) {
                $('#nombreUsuario').html("<b>Cliente:</b> <i>" + cookies.NombreUsuario + "</i>");
            } else {
                $('#nombreUsuario').html("<b>Empleado:</b> <i>" + cookies.NombreUsuario + "</i>");
            }
        }
    });
}

function renderLogin() {
    $('#renderBody').html("");

    $.ajax({
        type: "GET",
        url: "/Home/RenderLogin",
        success: function (info) {
            $('div[class*="navbar-collapse"]').html(info);
        }
    });
}

function mensaje() {
    //alert('Esta es la vista de los clientes');
}

function ContenidoCombo() {

    var cad;
    var cmb;

    //Sacar productos en un alert sin JQuery
    cad = "JavaScript puro\n";
    cmb = document.getElementById("cmbProductos");
    for (var i = 0; i < cmb.childElementCount; i++) {
        cad += " - " + cmb.children[i].innerText + " (" + cmb.children[i].value + ")\n";
    }
    alert(cad);

    //Sacar productos en un alert con JQuery
    cad = "jQuery para acceder al combo\n";
    cmb = $("#cmbProductos");
    for (var i = 0; i < $(cmb).children().length; i++) {
        var elemento = $(cmb).children()[i];
        cad += " - " + $(elemento).text() + " (" + elemento.val() + ")\n";
    }
    alert(cad);

    cad = "jQuery para iterar por los elementos\n";
    cmb = $("#cmbProductos");
    $(cmb).children().each(function () {
        var elemento = $(this);
        cad += " - " + $(elemento).text() + " (" + $(elemento).val() + ")\n";
    });
    alert(cad);

    cad = "jQuery para iterar por los elementos con index\n";
    cmb = $("#cmbProductos");
    $(cmb).children().each(function (index) {
        var elemento = $(cmb).children()[index];
        cad += " - " + $(elemento).text() + " (" + $(elemento).val() + ")\n";
    });
    alert(cad);

    cad = "jQuery para iterar por los elementos con el elemento\n";
    cmb = $("#cmbProductos");
    $(cmb).children().each(function (index, elemento) {
        cad += index + " - " + $(elemento).text() + " (" + $(elemento).val() + ")\n";
    });
    alert(cad);
}

function miPrueba() {
    var array = new Array();

    for (var n = 0; n < 10; n++) {
        array[n] = "Hola" + n;
    }

    array["colores"] = "rojo y amarillo";
    array["pais"] = "españa";
    array["bebida"] = "cerveza";

    for (i in array) {
        console.log(array[i]);
    }

    //Resultado de la prueba ==> Un bucle 'for...in' devuelve
    //tanto los elementos del array (lo 1º) como las propiedades 
    //añadidas al objeto 'array' (lo 2º)

    //Esto se debe a que todos los objetos (incluidas las colecciones)
    //en JavaScript se comportan como mapas clave-valor, donde la clave
    //es el nombre de la propiedad o el índice del elemento en formato
    //'String', y el valor es su valor.

    //En el caso de un array, las claves de nombre numérico (0...K) se 
    //reservan como elementos del array, y las claves de tipo cadena de
    //texto se reservan como propiedades añadidas al objeto de tipo array.

    //En conclusión, un bucle 'for...in' devuelve todos los pares clave-valor
    //del mapa que conforma a las propiedades y elementos de un objeto.
}
