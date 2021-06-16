using ModeloDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosCapaDAL
{
    public class PedidosDAL
    {

        public static Pedido CrearPedido(int codigoCliente)
        {
            Pedido pedido = null;

            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Auxiliar.CadenaConexion;

                var cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CrearPedido";
                cmd.Parameters.AddWithValue("@codigoCliente", codigoCliente);

                cn.Open();

                var datos = cmd.ExecuteReader();
                if (datos.Read())
                {
                    pedido = new Pedido();
                    pedido.Codigo = Convert.ToInt32(datos["Codigo"]);
                    pedido.CodigoCliente = Convert.ToInt32(datos["CodigoCliente"]);
                    pedido.ImporteTotal = Convert.ToSingle(datos["ImporteTotal"]);
                    pedido.FechaPedido = Convert.ToDateTime(datos["FechaPedido"]);
                }

                //No hace falta cerrar la conexion, el bloque using
                //se encarga de ello al terminar de ejecutarse.
            }

            return pedido;
        }

        public static float AltaLineasDetalle(int codigoPedido, LineaDetalle[] lineasDetalle)
        {
            float importeTotal = 0.0f;

            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Auxiliar.CadenaConexion;

                var cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AltaLineaDetalle";

                cmd.Parameters.AddWithValue("@codigoPedido", codigoPedido);
                cmd.Parameters.Add("@codigoProducto", SqlDbType.NVarChar, 25);
                cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar, 50);
                cmd.Parameters.Add("@unidades", SqlDbType.Int);
                cmd.Parameters.Add("@precioVenta", SqlDbType.Float);

                cn.Open();

                foreach(var lineaDetalle in lineasDetalle)
                {
                    cmd.Parameters["@codigoProducto"].Value = lineaDetalle.CodigoProducto;
                    cmd.Parameters["@descripcion"].Value = lineaDetalle.Descripcion;
                    cmd.Parameters["@unidades"].Value = lineaDetalle.Unidades;
                    cmd.Parameters["@precioVenta"].Value = lineaDetalle.PrecioVenta;
                    cmd.ExecuteNonQuery();

                    importeTotal += lineaDetalle.Unidades * lineaDetalle.PrecioVenta;
                }
            }

            return importeTotal;
        }

        public static void ActualizarImportePedido(int codigoPedido, float importeTotal)
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Auxiliar.CadenaConexion;

                var cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "UPDATE Pedidos SET ImporteTotal = @importe WHERE Codigo = @codigoPedido";
                cmd.Parameters.AddWithValue("@codigoPedido", codigoPedido);
                cmd.Parameters.AddWithValue("@importe", importeTotal);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static Pedido[] ObtenerPedidos(Dictionary<string, string> datos)
        {
            List<Pedido> pedidos = new List<Pedido>();

            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Auxiliar.CadenaConexion;

                var cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "SELECT * FROM Pedidos";
                string filtro = "";

                if (datos.ContainsKey("CodigoCliente"))
                {
                    if (filtro == "")
                        filtro += " WHERE ";

                    filtro += "CodigoCliente = @codigoCliente";
                    cmd.Parameters.AddWithValue("@codigoCliente", Convert.ToInt32(datos["CodigoCliente"]));

                }

                if (datos.ContainsKey("FechaDesde"))
                {
                    if (filtro == "")
                        filtro += " WHERE ";
                    else
                        filtro += " AND ";

                    filtro += "FechaPedido >= @fechaDesde";
                    string fechaDesdeTxt = Convert.ToDateTime(datos["FechaDesde"]).ToString("yyyy-MM-dd 23:59:59");
                    cmd.Parameters.AddWithValue("@fechaDesde", Convert.ToDateTime(fechaDesdeTxt));

                }

                if (datos.ContainsKey("FechaHasta"))
                {
                    if (filtro == "")
                        filtro += " WHERE ";
                    else
                        filtro += " AND ";

                    filtro += "FechaPedido <= @fechaHasta";
                    string fechaHastaTxt = Convert.ToDateTime(datos["FechaHasta"]).ToString("yyyy-MM-dd 23:59:59");
                    cmd.Parameters.AddWithValue("@fechaHasta", Convert.ToDateTime(fechaHastaTxt));

                }

                cmd.CommandText += filtro;

                cn.Open();

                var pedidosReader = cmd.ExecuteReader();
                while (pedidosReader.Read())
                {
                    Pedido pedido = new Pedido();
                    pedido.Codigo = Convert.ToInt32(pedidosReader["Codigo"]);
                    pedido.CodigoCliente = Convert.ToInt32(pedidosReader["CodigoCliente"]);
                    pedido.ImporteTotal = Convert.ToSingle(pedidosReader["ImporteTotal"]);
                    pedido.FechaPedido = Convert.ToDateTime(pedidosReader["FechaPedido"]);
                    pedido.FechaPedidoCadena = pedido.FechaPedido.ToString();
                    pedidos.Add(pedido);
                }
            }

            return pedidos.ToArray();
        }

        public static LineaDetalle[] ObtenerLineasDetalle(Dictionary<string, string> datos)
        {
            List<LineaDetalle> detalles = new List<LineaDetalle>();

            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Auxiliar.CadenaConexion;

                var cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "SELECT * FROM LineasDetalle WHERE CodigoPedido = @codigoPedido";
                cmd.Parameters.AddWithValue("@codigoPedido", Convert.ToInt32(datos["CodigoPedido"]));

                cn.Open();

                var detallesReader = cmd.ExecuteReader();
                while (detallesReader.Read())
                {
                    LineaDetalle detalle = new LineaDetalle();
                    detalle.Codigo = Convert.ToInt32(detallesReader["Codigo"]);
                    detalle.CodigoPedido = Convert.ToInt32(detallesReader["CodigoPedido"]);
                    detalle.CodigoProducto = detallesReader["CodigoProducto"].ToString();
                    detalle.Descripcion = detallesReader["Descripcion"].ToString();
                    detalle.Unidades = Convert.ToInt32(detallesReader["Unidades"]);
                    detalle.PrecioVenta = Convert.ToSingle(detallesReader["PrecioVenta"]);
                    detalles.Add(detalle);
                }
            }

            return detalles.ToArray();
        }


    }
}
