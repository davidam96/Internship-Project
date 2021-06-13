using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace PedidosCapaDAL
{
    //Esta clase es un singleton creado para poder recoger la cadena de conexión
    //tirando del archivo 'Web.config' del proyecto 'WebApiPedidos' una única vez,
    //de tal manera que nuestra aplicacion web no se ralentice segun escale el numero
    //de peticiones al servidor.
    //Según palabras de Javier: "El cuello de botella de (el rendimiento de) cualquier
    //aplicacion es acceder a ficheros fisicos".
    static class Auxiliar
    {
        static string cadenaConexion = null;

        //Un constructor estático se inicializa automaticamente nada mas referenciar
        //(clase estatica) o instanciar (clase normal) a la clase por primera vez.
        //El constructor estático es el mecanismo fundamental del patrón singleton.
        static Auxiliar()
        {
            //Para escribir esta línea hemos necesitado añadir dos ensamblados en
            //las referencias de PedidosCapaDAL: 'System.Web' y 'System.Config'.
            cadenaConexion = WebConfigurationManager.ConnectionStrings["BBDD_Pedidos"].ConnectionString;
        }

        // ¿Como se documenta una propiedad o un método? Así:
        //(Solo vale dentro de Visual Studio)
        /// <summary>
        /// Contiene la cadena de conexión del archivo Web.config
        /// </summary>
        internal static string CadenaConexion
        {
            get { return cadenaConexion; }
        }

    }
}
