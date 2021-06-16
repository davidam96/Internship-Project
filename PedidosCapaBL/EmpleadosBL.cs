using ModeloDatos;
using PedidosCapaDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosCapaBL
{
    public class EmpleadosBL
    {
        public static Empleado ValidarEmpleado(Dictionary<string, string> info)
        {
            //Comprobamos que 'info' contiene las claves necesarias
            if (!info.ContainsKey("txtNombreEmpleado"))
                throw new Exception("Falta información del nombre de empleado");
            if (!info.ContainsKey("txtPassword"))
                throw new Exception("Falta información del password de empleado");

            //Obtenemos el empleado de la BBDD
            Empleado empleado = EmpleadosDAL.ObtenerEmpleado(info["txtNombreEmpleado"]);

            //Si la password no es correcta...
            if (empleado != null && empleado.Password != info["txtPassword"])
                empleado = null;

            return empleado;
        }


    }
}
