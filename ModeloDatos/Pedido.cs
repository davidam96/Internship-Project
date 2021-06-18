using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos
{
    public class Pedido
    {
        public int Codigo { get; set; }
        public int CodigoCliente { get; set; }
        public float ImporteTotal { get; set; }
        public DateTime FechaPedido { get; set; }
        public string FechaPedidoCadena { get; set; }
        public string FechaPreparacionCadena { get; set; } = null;
        public int? CodigoEmpleadoPrep { get; set; } = null;
        public string FechaEnvioCadena { get; set; } = null;
        public int? CodigoEmpleadoEnv { get; set; } = null;
        public string FechaCancelacionCadena { get; set; } = null;
    }
}
