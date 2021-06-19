'use strict';

$(document).ready(function () {
    //YA ESTÁ TODO EL HTML CARGADO

    CargarPedidos();

});

function FiltrarFechaPedidos() {
    let fDesde = $("#fechaDesde").val();
    let fHasta = $("#fechaHasta").val();

    //IMPORTANTE: Las propiedades del objeto 'datos' que vas a enviar
    //mediante Ajax a un método del HomeController se deben llamar
    //EXACTAMENTE IGUAL que los parámetros de entrada de dicho método,
    //exceptuando la 1ª letra que puede estar en mayúsculas o minúsculas.
    let datos = {};
  
    if (fDesde !== undefined && fDesde !== "")
        datos.FechaDesde = fDesde;
    if (fHasta !== undefined && fHasta !== "")
        datos.FechaHasta = fHasta;

    CargarPedidos(datos);
}

function QuitarFiltros() {
    $("#fechaDesde").val("");
    $("#fechaHasta").val("");
}

function CargarPedidos(datos) {

    var destino = '/Home/ObtenerPedidos';

    if (datos === undefined)
        var datos = {};

    datos.CodigoCliente = cookies.CLI_CodigoCliente;

    $.ajax({
        url: destino,
        method: "POST",
        data: JSON.stringify(datos), // --> datos que enviamos al servidor
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (respuesta) {
            if (respuesta.Error === undefined) {
                CargarTablaPedidos(respuesta.Pedidos);
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

function CargarTablaPedidos(pedidos) {
    var cad = "<tr><th>Acciones</th><th>Código</th><th>Fecha</th><th>Importe</th><th>Envío</th></tr>";
    pedidos.forEach((pedido) => {
        cad += '<tr ' + (pedido.FechaCancelacionCadena !== null ? 'class="pedidoCancelado"' : "") + '>';

        //Boton Ver
        cad += '<td><button class="btn btn-primary border-dark rounded"' +
            'onclick="VerDetalle(' + pedido.Codigo + ')" ';
        cad += 'data-toggle="modal" data-target="#modalDetalle"';
        cad += '> Ver</button ></td > ';

        cad += "<td><span>" + pedido.Codigo + "</span></td>";
        cad += "<td><span>" + pedido.FechaPedidoCadena + "</span></td>";
        cad += "<td><span>" + pedido.ImporteTotal.toFixed(2) + "€ </span></td>";

        //Fecha Envio
        cad += "<td>";
        if (pedido.FechaEnvioCadena !== null)
            cad += pedido.FechaEnvioCadena;
        else if (pedido.FechaCancelacionCadena !== null) {
            cad += '<span class="text-danger" title="Pedido cancelado en fecha ' + pedido.FechaCancelacionCadena;
            cad += '"><b>Cancelado</b></span>';
        }
        else {
            cad += '<button class="btn btn-danger border-dark rounded" ';
            cad += 'onclick="CancelarPedido(' + pedido.Codigo + ')" ';
            cad += 'title="Cancelar pedido pendiente \n de preparar o enviar"';
            cad += '>Cancelar</button>';
        }
        cad += "</td>";

        cad += "</tr>";
    });

    $("#tablaPedidos").html(cad);
}

function VerDetalle(codigoPedido) {

    var destino = '/Home/ObtenerLineasDetalle';

    var datos = {};
    datos.CodigoPedido = codigoPedido;

    $.ajax({
        url: destino,
        method: "POST",
        data: JSON.stringify(datos), // --> datos que enviamos al servidor
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (respuesta) {
            if (respuesta.Error === undefined) {
                let fila;
                $("#tablaPedidos").children().each((index, row) => {
                    var filaAux = $(row).children().eq(1).text();
                    if (filaAux === codigoPedido.toString())
                        fila = $(row);
                });

                if (fila !== undefined) {
                    //$("#lblCodigoPedido").html(codigoPedido);
                    //$("#lblFechaPedido").text(fila.children().eq(2).text());
                    //$("#lblImportePedido").text(fila.children().eq(3).text());

                    CargarTablaLineasDetalle(respuesta.LineasDetalle);

                    //$("#divDetalle").css("display", "");
                    $("#divDetalle").removeClass("d-none");
                }
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

function CargarTablaLineasDetalle(lineasDetalle) {

    var total = 0;
    var cad = "";

    //Cabecera tabla
    cad += "<thead>";
    cad += '<tr class="table-secondary">';
    cad += "<th>Codigo</th><th>Descripcion</th><th>Unidades</th><th>Precio unidad</th><th>Precio total</th>";
    cad += "</tr></thead>";

    //Cuerpo tabla
    cad += "<tbody>";
    lineasDetalle.forEach((lineaDetalle) => {
        cad += "<tr>";
        cad += "<td>" + lineaDetalle.CodigoProducto + "</td>";
        cad += "<td>" + lineaDetalle.Descripcion + "</td>";
        cad += "<td>" + lineaDetalle.Unidades + "</td>";
        cad += "<td>" + lineaDetalle.PrecioVenta + "€ </td>";
        let subtotal = (parseInt(lineaDetalle.Unidades) * parseFloat(lineaDetalle.PrecioVenta)).toFixed(2);
        cad += "<td>" + subtotal + "€ </td>";
        total += parseFloat(subtotal);
        cad += "</tr>";
    });

    //Importe total
    cad += '<tr><td colspan="5"><span style="font-size: 28px;">';
    cad += '<b>Total: </b>' + total + "€ </span></td></tr>";

    cad += "</tbody>";
    $("#tablaDetalle").html(cad);
}

function CancelarPedido(codigoPedido) {

    let ok = confirm("¿Está seguro de que desea cancelar este pedido?");
    if (!ok)
        return; 

    //Han aceptado la cancelación y seguimos con el proceso.
    //Bloqueamos el botón para evitar que lo vuelvan a pulsar.

    let tabla = $("#tablaPedidos")

    let btnCancelar;
    tabla.children().each((index, row) => {
        let fila = $(row);
        if (fila.children().eq(1).text() === codigoPedido.toString()) {
            btnCancelar = fila.children().eq(4).children().eq(0);
            btnCancelar.attr("disabled", "disabled");
            return;
        }    
    });


    var destino = '/Home/CancelarPedido';

    var datos = {};
    datos.CodigoPedido = codigoPedido;

    $.ajax({
        url: destino,
        method: "POST",
        //async: false, // --> bloquea el navegador hasta terminar y retornar la peticion
        data: JSON.stringify(datos), // --> datos que enviamos al servidor
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (respuesta) {
            if (respuesta.Error === undefined) {
                //Todo ok.
                tabla.children().each((index, row) => {
                    let fila = $(row);
                    if (fila.children().eq(1).text() === codigoPedido.toString())
                        fila.children().eq(4).html('<b class="text-danger">Cancelado</b>');
                });

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