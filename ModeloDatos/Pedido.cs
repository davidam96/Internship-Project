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
        public DateTime FechaHoraPedido { get; set; }
        public int CodigoEmplPrep { get; set; }
        public DateTime FechaHoraEnvio { get; set; }
        public int CodigoEmplRep { get; set; }
        public DateTime FechaHoraUltMod { get; set; }
    }
}
