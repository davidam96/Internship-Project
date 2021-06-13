// 1) Definicion de variables globales
var productos = [];
var lineasDetalle = [];


// 2) Metodo tras carga de la página
$(document).ready(function () {
    //YA ESTÁ TODO EL HTML CARGADO

    //Gestionamos las cookies que vienen del servidor
    gestionarCookies();

    //Rellenamos el combo con los productos mediante Ajax
    cargarCombo();
});


// 3) Resto de funciones
function gestionarCookies() {
    
    var cAux = document.cookie.split("; ");
    var cookies = {};

    cAux.forEach(function (cookie) {
        let info = cookie.split("=");
        cookies[info[0]] = info[1];
    });

    if (cookies.CodigoUsuario != -1)
        displayLogout(cookies.NombreUsuario);
}

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

function displayLogout(nombreUsuario) {
    $("#login").css("display", "none");
    $("#logout").css("display", "");
    $('#nombreUsuario').html("<b>Cliente:</b> <i>" + nombreUsuario + "</i>");
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

function nuevoProducto() {

    //Recogemos la cantidad escrita en el input "txtUnidades"
    var cantidad = $("#txtUnidades").val();
    if (cantidad === undefined || cantidad.trim() === "") {
        alert("Introduzca unidades");
        return;
    }

    //Comprobamos que la cantidad introducida sea válida
    var unidades = parseInt(cantidad);
    if (Number.isNaN(unidades)) {
        alert("Unidades incorrectas");
    }

    //Creamos una línea de detalle con el producto escogido
    //y la cantidad escrita
    var codigoProducto = $("#cmbProductos").val();
    productos.some((producto) => {
        if (producto.Codigo === codigoProducto) {
            let lineaDetalle = {};
            lineaDetalle.codigoProducto = producto.Codigo;
            lineaDetalle.descripcion = producto.Descripcion;
            lineaDetalle.unidades = unidades;
            lineaDetalle.precioVenta = producto.PrecioVenta;
            lineasDetalle.push(lineaDetalle);
            return true;
        }
        return false;
    });

    //Actualizamos la info de todo el pedido
    var cad = "<tr><td>Codigo</td><td>Descripcion</td><td>Unidades</td><td>Precio unidad</td><td>Precio total</td></tr>";
    var total = 0;
    lineasDetalle.forEach((lineaDetalle) => {
        cad += "<tr>";
        cad += "<td>" + lineaDetalle.codigoProducto + "</td>";
        cad += "<td>" + lineaDetalle.descripcion + "</td>";
        cad += "<td>" + lineaDetalle.unidades + "</td>";
        cad += "<td>" + lineaDetalle.precioVenta + "</td>";
        let subtotal = Math.trunc(parseInt(lineaDetalle.unidades) * parseFloat(lineaDetalle.precioVenta), 2);
        cad += "<td>" + subtotal + "</td>";
        total += subtotal;
        cad += "</tr>";
    });
    cad += '<tr><td colspan="5">Total... ' + total + "</td></tr>";
    $("#tablaDetalle").html(cad);
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
