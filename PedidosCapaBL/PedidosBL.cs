using ModeloDatos;
using PedidosCapaDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosCapaBL
{
    public class PedidosBL
    {

        public static Pedido CrearPedido(int codigoCliente, LineaDetalle[] lineasDetalle)
        {
            Pedido pedido = null;

            pedido = PedidosDAL.CrearPedido(codigoCliente);

            if (pedido != null) 
            {
                float importeTotal = PedidosDAL.AltaLineasDetalle(pedido.Codigo, lineasDetalle);
                PedidosDAL.ActualizarImportePedido(pedido.Codigo, importeTotal);
                pedido.ImporteTotal = importeTotal;
            }

            return pedido;
        }

        public static Pedido[] ObtenerPedidos(Dictionary<string, string> datos)
        {
            return PedidosDAL.ObtenerPedidos(datos);
        }

        public static LineaDetalle[] ObtenerLineasDetalle(Dictionary<string, string> datos)
        {
            return PedidosDAL.ObtenerLineasDetalle(datos);
        }

    }
}
