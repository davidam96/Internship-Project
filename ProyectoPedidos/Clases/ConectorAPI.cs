using ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ProyectoPedidos.Clases
{
    public static class ConectorAPI
    {

        static string baseUrlAPI;
        enum tipoLlamada
        {
            GET,
            POST
        }

        /// <summary>
        /// Constructor estático para inicializar lo necesario
        /// </summary>
        static ConectorAPI()
        {
            baseUrlAPI = "https://localhost:44351/"; //Poner cada uno la de su Api
        }

        #region Métodos internos

        /// <summary>
        /// Método interno común para todas las llamadas de tipo GET.
        /// </summary>
        /// <param name="uri">Cadena con la uri relativa de llamada a la API</param>
        /// <returns></returns>
        static HttpResponseMessage RespuestaGET(string uri)
        {
            return RespuestaGET(uri, -1);
        }

        /// <summary>
        /// Método interno común para todas las llamadas de tipo GET.
        /// </summary>
        /// <param name="uri">Cadena con la uri relativa de llamada a la API</param>
        /// <param name="segundosTimeout">Entero para modificar el tiempo de timeout.</param>
        /// <returns></returns>
        static HttpResponseMessage RespuestaGET(string uri, int segundosTimeout)
        {
            return RespuestaGETPOST(tipoLlamada.GET, uri, null, segundosTimeout);
        }

        /// <summary>
        /// Método interno común para todas las llamadas de tipo POST.
        /// </summary>
        /// <param name="uri">Cadena con la uri relativa de llamada a la API</param>
        /// <param name="o">Objeto a pasar para su alta</param>
        /// <returns></returns>
        static HttpResponseMessage RespuestaPOST(string uri, object o)
        {
            return RespuestaPOST(uri, o, -1);
        }

        /// <summary>
        /// Método interno común para todas las llamadas de tipo POST.
        /// </summary>
        /// <param name="uri">Cadena con la uri relativa de llamada a la API</param>
        /// <param name="o">Objeto a pasar para su alta</param>
        /// <param name="segundosTimeout">Entero para modificar el tiempo de timeout.</param>
        /// <returns></returns>
        static HttpResponseMessage RespuestaPOST(string uri, object o, int segundosTimeout)
        {
            return RespuestaGETPOST(tipoLlamada.POST, uri, o, segundosTimeout);
        }

        /// <summary>
        /// Método interno común para todas las llamadas de tipo GET y POST.
        /// </summary>
        /// <param name="tipo">Tipo de llamada a realizar.</param>
        /// <param name="uri">Cadena con la uri relativa de llamada a la API.</param>
        /// <param name="o">Objeto a pasar para su alta.</param>
        /// <param name="segundosTimeout">Entero para modificar el tiempo de timeout.</param>
        /// <returns></returns>
        static HttpResponseMessage RespuestaGETPOST(tipoLlamada tipo, string uri, object o, int segundosTimeout)
        {
            //Hay que chequear limitaciones de tamaño en el objeto.
            //De momento probado hasta 2Mb y funciona, pero con archivos más grandes no...
            string url = baseUrlAPI + uri;
            using (HttpClient clienteAPI = new HttpClient())
            {
                HttpResponseMessage response;
                try
                {
                    if (segundosTimeout != -1) //Por si hiciera falta reducirlo o aumentarlo, por defecto es 100 segundos.
                        clienteAPI.Timeout = TimeSpan.FromSeconds(segundosTimeout);

                    if (tipo == tipoLlamada.GET)
                        response = clienteAPI.GetAsync(url).Result;
                    else
                        response = clienteAPI.PostAsJsonAsync(url, o).Result;
                }
                catch (Exception ex)
                {
                    response = new HttpResponseMessage();
                    response.StatusCode = System.Net.HttpStatusCode.Conflict; //Lo pongo para que salvo en timeout, todos vayan con este.

                    string msgError = "Excepción de tipo " + ex.GetType().Name;

                    if (ex is AggregateException)
                    {
                        if (((AggregateException)ex).InnerExceptions.Count == 1)
                        {
                            if (ex.InnerException != null)
                            {
                                if (ex.InnerException is TaskCanceledException)
                                    response.StatusCode = System.Net.HttpStatusCode.RequestTimeout;

                                msgError += " - " + ex.InnerException.Message;
                                if (ex.InnerException.InnerException != null)
                                {
                                    msgError += " -> " + ex.InnerException.InnerException.Message;
                                }
                            }
                        }
                        else
                        {
                            foreach (var exc in ((AggregateException)ex).InnerExceptions)
                            {
                                msgError += "\n - " + exc.Message;
                                if (exc.InnerException != null)
                                {
                                    msgError += " (" + exc.InnerException.Message;
                                    if (exc.InnerException.InnerException != null)
                                    {
                                        msgError += " -> " + exc.InnerException.InnerException.Message;
                                    }
                                    msgError += ")";
                                }
                            }
                        }
                    }
                    else //No es AggregateException
                    {
                        msgError += " - " + ex.Message;

                        if (ex.InnerException != null)
                        {
                            msgError += " (" + ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                msgError += " -> " + ex.InnerException.InnerException.Message;
                            }
                            msgError += ")";
                        }
                    }

                    if (response.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
                        response.ReasonPhrase = "Tiempo de espera agotado.";
                    else
                        response.ReasonPhrase = ex.Message;
                }

                return response;
            }
        }

        #endregion

        #region Métodos públicos

        //public static Dictionary<string, string> ValidarLoginUsuario(Dictionary<string, string> infoLogin)
        //{
        //    infoLogin["Login"] = bool.TrueString;
        //    Dictionary<string, string> retorno;
        //    string uri = "api/Usuarios/ValidarLoginUsuario";
        //    HttpResponseMessage response = RespuestaPOST(uri, infoLogin);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        retorno = response.Content.ReadAsAsync<Dictionary<string, string>>().Result;
        //        if (retorno == null)
        //        {
        //            retorno = new Dictionary<string, string>();
        //            retorno.Add("INFOACCESO vacío", "Error en llamada a " + uri + " - Motivo: " + response.ReasonPhrase);
        //        }
        //        return retorno;
        //    }
        //    else
        //    {
        //        throw new HttpRequestException(response.ReasonPhrase);
        //    }
        //}

        public static Cliente ValidarLogin(string txtMail, string txtPassword)
        {
            
            Dictionary<string, string> infoConexion = new Dictionary<string, string>();
            infoConexion.Add("txtMail", txtMail);
            infoConexion.Add("txtPassword", txtPassword);

            Cliente cliente = null;

            string uri = "api/General/ValidarCliente";
            HttpResponseMessage response = RespuestaPOST(uri, infoConexion);
            if (response.IsSuccessStatusCode)
            {
                cliente = response.Content.ReadAsAsync<Cliente>().Result;
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            return cliente;
        }

        public static List<Producto> ObtenerProductos()
        {

            List<Producto> Productos = new List<Producto>();
            var rnd = new Random();
            Producto p;
            for (int i = 1; i <= 5; i++)
            {
                p = new Producto();
                p.Codigo = "Prod" + i;
                p.Descripcion = "Producto " + i;
                p.PrecioVenta = (float)Math.Round(rnd.NextDouble() * 1000, 2);
                Productos.Add(p);
            }
            return Productos;
        }

        #endregion
    }
}