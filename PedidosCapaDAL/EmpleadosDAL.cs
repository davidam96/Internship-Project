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
    public class EmpleadosDAL
    {
        public static Empleado ObtenerEmpleado(string nombreEmpleado)
        {
            Empleado empleado = null;

            //El using sirve para gestionar manualmente el 'Garbage Collection' de un objeto concreto,
            //de tal forma que en vez de ponerlo en cola para su destrucción (ticks de reloj ~ x segundos),
            //lo destruye de manera casi inmediata (ticks de reloj ~ x milisegundos).
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Auxiliar.CadenaConexion;

                //SqlCommand cmd = new SqlCommand(); 
                //cmd.Connection = cn;
                var cmd = cn.CreateCommand(); // --> Las dos líneas de arriba, en una sola.

                cmd.CommandText = "SELECT * FROM Empleados WHERE NombreEmpleado = @nombreEmpleado";
                cmd.Parameters.AddWithValue("@nombreEmpleado", nombreEmpleado);

                cn.Open();

                SqlDataReader datos = cmd.ExecuteReader();
                while (datos.Read())
                {
                    empleado = new Empleado();
                    empleado.Codigo = datos.GetInt32(0); 
                    empleado.NombreEmpleado = datos["NombreEmpleado"].ToString();
                    empleado.Password = datos["Password"].ToString();
                    empleado.Nombre = datos["Nombre"].ToString();
                    empleado.Apellidos = datos["Apellidos"].ToString();

                    if (datos["PuedePrepararPedidos"] != DBNull.Value)
                        empleado.PuedePrepararPedidos = Convert.ToBoolean(datos["PuedePrepararPedidos"]);
                    if (datos["PuedeEnviarPedidos"] != DBNull.Value)
                        empleado.PuedeEnviarPedidos = Convert.ToBoolean(datos["PuedeEnviarPedidos"]);
                }

                cn.Close();
            }

            return empleado;
        }


    }
}
