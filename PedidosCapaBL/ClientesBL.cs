using ModeloDatos;
using PedidosCapaDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedidosCapaBL
{
    public class ClientesBL
    {
        public static Cliente ValidarCliente(Dictionary<string, string> info)
        {
            //Comprobamos que 'info' contiene las claves necesarias
            if (!info.ContainsKey("txtMail"))
                throw new Exception("Falta información del mail de cliente");
            if (!info.ContainsKey("txtPassword"))
                throw new Exception("Falta información del password de cliente");

            var txtMail = info["txtMail"];
            var txtPassword = info["txtPassword"];

            //Obtenemos el cliente de la BBDD
            Cliente cliente = ClientesDAL.ObtenerCliente(txtMail);

            //Si la password no es correcta...
            if (cliente != null && cliente.PasswordCliente != txtPassword)
                cliente = null;

            return cliente;
        }


    }
}
