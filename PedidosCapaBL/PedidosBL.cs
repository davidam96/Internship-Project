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
            var pedidos = PedidosDAL.ObtenerPedidos(datos);

            if (datos.ContainsKey("IncluirEmpleados") && bool.Parse(datos["IncluirEmpleados"]))
            {
                var empleados = EmpleadosDAL.ObtenerEmpleados();

                //Creando a mano un diccionario con todos los empleados
                Dictionary<int, Empleado> empleadosDIC = new Dictionary<int, Empleado>();
                foreach (var empleado in empleados)
                    empleadosDIC.Add(empleado.Codigo, empleado);

                //Creando el diccionario con metodos de extension de la clase 'List<>'
                empleadosDIC = empleados.ToDictionary(emp => emp.Codigo);

                //Si quisieramos un diccionario con los nombres de empleado en vez de los empleados...
                var nombresEmpleadoDIC = empleados.ToDictionary(emp => emp.Codigo, emp => emp.NombreCompleto);

                foreach (var pedido in pedidos)
                {
                    if (pedido.CodigoEmpleadoPrep.HasValue)
                        pedido.NombreEmpleadoPrep = empleadosDIC[pedido.CodigoEmpleadoPrep.Value].NombreCompleto;

                    if (pedido.CodigoEmpleadoEnv.HasValue)
                        pedido.NombreEmpleadoEnv = empleadosDIC[pedido.CodigoEmpleadoEnv.Value].NombreCompleto;
                }
            }

            return pedidos;
        }

        public static LineaDetalle[] ObtenerLineasDetalle(Dictionary<string, string> datos)
        {
            return PedidosDAL.ObtenerLineasDetalle(datos);
        }

        public static Pedido ModificarPedido(Dictionary<string, string> datos)
        {
            return PedidosDAL.ModificarPedido(datos);
        }

    }
}
