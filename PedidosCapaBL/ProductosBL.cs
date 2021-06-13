using ModeloDatos;
using PedidosCapaDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosCapaBL
{
    public class ProductosBL
    {

        public static List<Producto> ObtenerProductosBL()
        {
            return ProductosDAL.ObtenerProductosDAL();
        }

    }
}
