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
    public class ClientesDAL
    {

        public static Cliente ObtenerCliente(string mail)
        {
            Cliente cliente = null;

            //El using sirve para gestionar manualmente el 'Garbage Collection' de un objeto concreto,
            //de tal forma que en vez de ponerlo en cola para su destrucción (ticks de reloj ~ x segundos),
            //lo destruye de manera casi inmediata (ticks de reloj ~ x milisegundos).
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = Auxiliar.CadenaConexion;

                //SqlCommand cmd = new SqlCommand(); 
                //cmd.Connection = cn;
                var cmd = cn.CreateCommand(); // --> Las dos líneas de arriba, en una sola.

                //cmd.CommandType = CommandType.Text;  // --> opción por defecto
                //cmd.CommandText = "SELECT * FROM CLIENTES WHERE Mail = '" + mail + "'";

                //Obtenemos el cliente de la BBDD a traves de un 'Stored procedure' en vez de
                //una simple consulta, por seguridad, para evitar inyecciones de codigo SQL.
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ObtenerClientePorMail";
                cmd.Parameters.AddWithValue("@mail", mail);

                cn.Open();

                SqlDataReader datos = cmd.ExecuteReader();
                while (datos.Read())
                {
                    cliente = new Cliente();
                    //cliente.Codigo = Convert.ToInt32(datos["Codigo"]); // esta operacion conlleva '+++' ticks de reloj
                    //cliente.Codigo = Convert.ToInt32(datos[0]); // esta operacion conlleva '++' ticks de reloj
                    cliente.Codigo = datos.GetInt32(0); // esta operacion conlleva '+' ticks de reloj
                    cliente.MailCliente = datos["Mail"].ToString();
                    cliente.PasswordCliente = datos["Password"].ToString();
                    cliente.NombreCliente = datos["Nombre"].ToString();
                    cliente.ApellidosCliente = datos["Apellidos"].ToString();
                    cliente.TelefonoCliente = datos["Telefono"].ToString();
                }

                cn.Close();
            }

            return cliente;
        }

    }
}
