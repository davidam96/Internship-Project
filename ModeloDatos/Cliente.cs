using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos
{
    public class Cliente
    {
        public int Codigo { get; set; }
        public string MailCliente { get; set; }
        public string PasswordCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidosCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaBaja { get; set; }
        public DateTime FechaHoraUltMod { get; set; }
    }
}
