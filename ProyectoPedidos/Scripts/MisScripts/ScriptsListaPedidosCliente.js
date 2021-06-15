'use strict';

$(document).ready(function () {
    //YA ESTÁ TODO EL HTML CARGADO

    CargarPedidos();

});

function CargarPedidos() {

    var destino = '/Home/ObtenerPedidos';

    var datos = {};
    datos.codigoCliente = cookies.CodigoCliente;

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

    var cad = "<tr><td>Código</td><td>Fecha</td><td>Importe</td></tr>";
    pedidos.forEach((pedido) => {
        cad += "<tr>";
        cad += "<td>" + pedido.Codigo + "</td>";
        cad += "<td>" + pedido.FechaPedidoCadena + "</td>";
        cad += "<td>" + pedido.ImporteTotal.toFixed(2) + "€ </td>";
        cad += "</tr>";
    });
    $("#tablaPedidos").html(cad);
}