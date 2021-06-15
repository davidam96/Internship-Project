using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos
{
    public class LineaDetalle
    {
        public int? Codigo { get; set; }
        public int? CodigoPedido { get; set; }
        public string CodigoProducto { get; set; }
        public int Unidades { get; set; }
        public float PrecioVenta { get; set; }
        public string Descripcion { get; set; }
    }
}
