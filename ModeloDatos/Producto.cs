using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos
{
    public class Producto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int CodigoProveedor { get; set; }
        public string ReferenciaProveedor { get; set; }
        public float PrecioCompra { get; set; }
        public float PrecioVenta { get; set; }
        public int Stock { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaBaja { get; set; }
        public DateTime FechaHoraUltMod { get; set; }
    }
}
