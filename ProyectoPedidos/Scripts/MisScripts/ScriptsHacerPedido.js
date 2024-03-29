﻿'use strict';

// 1) Definicion de variables globales
var productos = [];
var lineasDetalle = [];


// 2) Metodo tras carga de la página
$(document).ready(function () {
    //YA ESTÁ TODO EL HTML CARGADO

    //Rellenamos el combo con los productos mediante Ajax
    cargarCombo();
});

// 3) Resto de funciones

/** Carga los productos desde la BBDD al combo. */
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
                let cmb = $("#cmbProductos");
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

/** Da de alta un producto en las lineas de detalle. */
function NuevoProducto(elem) {

    //Recogemos la cantidad escrita en el input "txtUnidades"
    var cantidad = $("#txtUnidades").val();
    if (cantidad === undefined || cantidad.trim() === "") {
        alert("Introduzca unidades");
        $("#txtUnidades").val("");
        return;
    }

    //Comprobamos que la cantidad introducida sea válida
    var unidades = parseInt(cantidad);
    if (Number.isNaN(unidades) || unidades <= 0) {
        alert("Unidades incorrectas");
        $("#txtUnidades").val("");
        return;
    }

    var codigoProducto = $("#cmbProductos").val();

    //Primero comprobamos si ya teniamos el producto
    let lineaDetalle = undefined;
    lineasDetalle.forEach((ld) => {
        if (ld.CodigoProducto === codigoProducto) {
            lineaDetalle = ld;
            if ($(elem).attr("id") === "btnAnadir")
                lineaDetalle.Unidades += unidades; //Añadimos las unidades
            else
                lineaDetalle.Unidades = unidades; //Modificamos las unidades
            return;
        }
    });

    //Creamos una línea de detalle con el
    //producto escogido y la cantidad escrita
    if (lineaDetalle === undefined) {
        productos.some((producto) => {
            lineaDetalle = {};
            if (producto.Codigo === codigoProducto) {
                lineaDetalle.CodigoProducto = producto.Codigo;
                lineaDetalle.Descripcion = producto.Descripcion;
                lineaDetalle.Unidades = unidades;
                lineaDetalle.PrecioVenta = producto.PrecioVenta;
                lineasDetalle.push(lineaDetalle);
                return true;
            }
            return false;
        });
    }

    //Actualizamos la info de todo el pedido
    CargarTablaLineasDetalle(lineasDetalle);

    //Activamos el boton de hacer pedido
    $("#btnHacerPedido").removeAttr("disabled");

    //Borramos las unidades del pedido anterior
    $("#txtUnidades").val("");
}

function ModificarProducto(elem) {
    NuevoProducto(elem);
}

function EliminarProducto(codigoProducto) {
    //Eliminamos la linea de detalle del array 'lineasDetalle',
    //haciendo uso de la funcion Array.filter()
    var ldAux = lineasDetalle.filter((lineaDetalle) => {
        if (lineaDetalle.CodigoProducto !== codigoProducto)
            return true;
        else
            return false;
    });
    lineasDetalle = ldAux;

    //Actualizamos la tabla con el producto ya eliminado
    CargarTablaLineasDetalle(lineasDetalle);
}

function CargarTablaLineasDetalle(lineasDet) {

    if (lineasDet.length === 0)
        //Si hemos borrado todos los productos del pedido,
        //eliminamos el html restante de la tabla.
        $("#tablaLineasDetalle").html("");
    else {
        var total = 0;
        var cad = "<tr><th>Codigo</th><th>Descripcion</th><th>Unidades</th><th>Precio unidad</th><th>Precio total</th><th>Acciones</th></tr>";
        lineasDet.forEach((lineaDetalle) => {
            cad += "<tr>";
            cad += "<td>" + lineaDetalle.CodigoProducto + "</td>";
            cad += "<td>" + lineaDetalle.Descripcion + "</td>";
            cad += "<td>" + lineaDetalle.Unidades + "</td>";
            cad += "<td>" + lineaDetalle.PrecioVenta + "€ </td>";
            let subtotal = (parseInt(lineaDetalle.Unidades) * parseFloat(lineaDetalle.PrecioVenta)).toFixed(2);
            cad += "<td>" + subtotal + "€ </td>";
            total += parseFloat(subtotal);

            //Boton Eliminar Producto
            cad += "<td>"
            cad += '<button class="btn btn-danger border-dark rounded" ';
            cad += 'onclick="EliminarProducto(' + "'" + lineaDetalle.CodigoProducto + "'" + ')" ';
            cad += '>Eliminar</button>';
            cad += "</td>"

            cad += "</tr>";
        });
        cad += '<tr><td colspan="6"><span style="font-size: 28px;">';
        cad += '<b>Total: </b>' + total.toFixed(2) + "€ </span></td></tr>";
        $("#tablaLineasDetalle").html(cad);
    }
}

/** Da de alta el pedido en la BBDD */
function CrearPedido() {

    var destino = '/Home/CrearPedido';
    var datos = JSON.stringify(lineasDetalle);

    $.ajax({
        url: destino,
        method: "POST",
        data: datos, // --> datos que enviamos al servidor
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (respuesta) {
            if (respuesta.Error === undefined) {
                var p = respuesta.Pedido;
                alert("Pedido " + p.Codigo + " creado correctamente");

                //Desactivamos el boton de hacer pedido
                $("#btnHacerPedido").attr("disabled", "disabled");

                //Eliminamos las lineas de detalle creadas del pedido anterior
                //(en caso de que hagamos mas de 2 pedidos antes de salir)
                $("#tablaLineasDetalle").html("");
            }
            else {
                alert(respuesta.Error);
            }
        },
        error: function (e) {
            var msg = "Error no controlado en llamada a " + destino;
            if (e !== undefined && e !== null && e !== "")
                if (e.statusText !== "")
                    msg += "\n" + e.statusText;
                else
                    msg += "\n" + e;
            alert(msg);
        }
    });
}

function VolverAListaPedidos() {
    window.location = '/Home/ListaPedidosCliente';
}

/** Tutorial DOM y JQuery.  */
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

/** Pruebo el funcionamiento de un array asociativo. */
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
