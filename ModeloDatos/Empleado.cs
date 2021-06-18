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
        public string NombreEmpleado { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public bool PuedePrepararPedidos { get; set; }
        public bool PuedeEnviarPedidos { get; set; }
        public string NombreCompleto
        {
            get { return (Nombre + " " + Apellidos); }
        }
    }
}
