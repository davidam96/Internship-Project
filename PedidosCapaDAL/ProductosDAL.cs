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
    public class ProductosDAL
    {

        public static List<Producto> ObtenerProductosDAL()
        {
            List<Producto> Productos = new List<Producto>();

            //var rnd = new Random();
            //Producto p;
            //for (int i = 1; i <= 5; i++)
            //{
            //    p = new Producto();
            //    p.Codigo = "Prod" + i;
            //    p.Descripcion = "Producto " + i;
            //    p.PrecioVenta = (float)Math.Round(rnd.NextDouble() * 1000, 2);
            //    Productos.Add(p);
            //}

            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = "data source=DESKTOP-4RNMN7I; initial catalog=Beca2021; User ID=UsrBeca; Password=1234";
            //cn.ConnectionString = "data source=(local); initial catalog=Beca2021; User ID=UsrBeca; Password=1234";
            //cn.ConnectionString = "data source=.; initial catalog=Beca2021; User ID=UsrBeca; Password=1234";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM PRODUCTOS";

            cn.Open();

            SqlDataReader datos = cmd.ExecuteReader();
            while (datos.Read())
            {
                Producto p = new Producto();
                p.Codigo = datos["Codigo"].ToString();
                p.Descripcion = datos["Descripcion"].ToString();
                p.PrecioVenta = Convert.ToSingle(datos["PrecioVenta"]);
                Productos.Add(p);
            }

            cn.Close();

            return Productos;
        }

    }
}
