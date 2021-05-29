using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloDatos
{
    public class Empleado
    {
        public int Codigo { get; set; }
        public string UsuarioApp { get; set; }
        public string PasswordUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidosUsuario { get; set; }
        public string TelefonoUsuario { get; set; }
        public string MailUsuario { get; set; }
        public int TipoEmpleado { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaBaja { get; set; }
        public DateTime FechaHoraUltMod { get; set; }
    }
}
