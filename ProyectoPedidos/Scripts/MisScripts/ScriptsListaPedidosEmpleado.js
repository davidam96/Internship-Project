'use strict';

$(document).ready(function () {
    //YA ESTÁ TODO EL HTML CARGADO

    CargarPedidos();

});

function FiltrarPedidos() {
    let codigoCliente = $("#codigoCliente").val();
    let fDesde = $("#fechaDesde").val();
    let fHasta = $("#fechaHasta").val();

    //IMPORTANTE: Las propiedades del objeto 'datos' que vas a enviar
    //mediante Ajax a un método del HomeController se deben llamar
    //EXACTAMENTE IGUAL que los parámetros de entrada de dicho método,
    //exceptuando la 1ª letra que puede estar en mayúsculas o minúsculas.
    let datos = {};

    if (codigoCliente !== undefined && codigoCliente !== "")
        datos.CodigoCliente = codigoCliente;
    if (fDesde !== undefined && fDesde !== "")
        datos.FechaDesde = fDesde;
    if (fHasta !== undefined && fHasta !== "")
        datos.FechaHasta = fHasta;

    CargarPedidos(datos);
}

function QuitarFiltros() {
    $("#codigoCliente").val("");
    $("#fechaDesde").val("");
    $("#fechaHasta").val("");
}

function CargarPedidos(datos) {

    var destino = '/Home/ObtenerPedidos';

    if (datos === undefined)
        var datos = {};

    datos.IncluirEmpleados = true;

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
    var cad = "<tr><th>Acciones</th><th>Código</th><th>Cliente</th><th>Fecha</th><th>Importe</th><th>Preparación</th><th>Envío</th></tr>";
    pedidos.forEach((pedido) => {
        cad += "<tr>";

        cad += '<td><button class="btn btn-primary border-dark rounded"' +
            'onclick="VerDetalle(' + pedido.Codigo + ')">Ver</button></td>';
        cad += "<td>" + pedido.Codigo + "</td>";
        cad += "<td>" + pedido.CodigoCliente + "</td>";
        cad += "<td>" + pedido.FechaPedidoCadena + "</td>";
        cad += "<td>" + pedido.ImporteTotal.toFixed(2) + "€ </td>";

        //Fecha Preparacion
        cad += "<td>";
        if (pedido.FechaPreparacionCadena != null) {
            let texto = "Código empleado: " + pedido.CodigoEmpleadoPrep + "\n";
            texto += "Nombre empleado: " + pedido.NombreEmpleadoPrep;
            cad += '<span title="' + texto + '">' + pedido.FechaPreparacionCadena + "</span>";
        }
        else if (pedido.FechaCancelacionCadena != null) {
            cad += '<span class="text-danger" title="Pedido cancelado en fecha ' + pedido.FechaCancelacionCadena;
            cad += '"><b>Cancelado</b></span>';
        }
        else if (cookies.EMP_PuedePrepararPedidos) {
            cad += '<button class="btn btn-success border-dark rounded"' +
                'onclick="PrepararPedido(' + pedido.Codigo + ')">Preparar</button>';
        }
        cad += "</td>";

        //Fecha Envio
        cad += "<td>";
        if (pedido.FechaEnvioCadena != null) {
            let texto = "Código empleado: " + pedido.CodigoEmpleadoEnv + "\n";
            texto += "Nombre empleado: " + pedido.NombreEmpleadoEnv;
            cad += '<span title="' + texto + '">' + pedido.FechaEnvioCadena + "</span>";
        }
        else if (pedido.FechaCancelacionCadena != null) {
            cad += '<span class="text-danger" title="Pedido cancelado en fecha ' + pedido.FechaCancelacionCadena;
            cad += '"><b>Cancelado</b></span>';
        }
        else if (cookies.EMP_PuedeEnviarPedidos) {
            cad += '<button class="btn btn-warning border-dark rounded" ';
            cad += 'onclick="EnviarPedido(' + pedido.Codigo + ')" ';
            cad += (pedido.FechaPreparacionCadena === null ? 'disabled="disabled "' : "");
            cad +=  '>Enviar</button>';
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
                    $("#lblCodigoPedido").html(codigoPedido);
                    $("#lblFechaPedido").text(fila.children().eq(3).text());
                    $("#lblImportePedido").text(fila.children().eq(4).text());

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
    var cad = "<tr><th>Codigo</th><th>Descripcion</th><th>Unidades</th><th>Precio unidad</th><th>Precio total</th></tr>";
    var total = 0;
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
    cad += '<tr><td colspan="5">Total... ' + total + "€ </td></tr>";
    $("#tablaDetalle").html(cad);
}

function PrepararPedido(codigoPedido) {

    var destino = '/Home/PrepararPedido';

    var datos = {};
    datos.CodigoPedido = codigoPedido;
    datos.CodigoEmpleado = cookies.EMP_CodigoEmpleado;

    $.ajax({
        url: destino,
        method: "POST",
        data: JSON.stringify(datos), // --> datos que enviamos al servidor
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (respuesta) {
            if (respuesta.Error === undefined) {
                //Todo ok.
                let tabla = $("#tablaPedidos")
                tabla.children().each((index, row) => {
                    let fila = $(row);
                    if (fila.children().eq(1).text() === codigoPedido.toString()) {

                        fila.children().eq(5).html("<i>" + respuesta.Fecha + "</i>");

                        //Caso de que el boton "Enviar" este deshabilitado
                        let btnEnviar = fila.children().eq(6).children().eq(0);
                        btnEnviar.removeAttr("disabled");
                    }
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

function EnviarPedido(codigoPedido) {

    var destino = '/Home/EnviarPedido';

    var datos = {};
    datos.CodigoPedido = codigoPedido;
    datos.CodigoEmpleado = cookies.EMP_CodigoEmpleado;

    $.ajax({
        url: destino,
        method: "POST",
        data: JSON.stringify(datos), // --> datos que enviamos al servidor
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (respuesta) {
            if (respuesta.Error === undefined) {
                //Todo ok.
                let tabla = $("#tablaPedidos")
                tabla.children().each((index, row) => {
                    let fila = $(row);
                    if (fila.children().eq(1).text() === codigoPedido.toString())
                        fila.children().eq(6).html("<i>" + respuesta.Fecha + "</i>");
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