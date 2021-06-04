using ModeloDatos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace WebApiPedidos.Controllers
{
    public class GeneralController : ApiController
    {
        //URI: api/General/ValidarCliente
        public HttpResponseMessage ValidarCliente(object datos)
        {
            Dictionary<string, string> info;

            try
            {
                //Deserializamos el parametro 'datos'
                info = JsonConvert.DeserializeObject<Dictionary<string, string>>(datos.ToString());

                //Comprobamos que 'info' contiene las claves necesarias
                if (!info.ContainsKey("txtMail"))
                {
                    throw new Exception("Falta información del mail de usuario");
                }
                if (!info.ContainsKey("txtPassword"))
                {
                    throw new Exception("Falta información del password de usuario");
                }

                var txtMail = info["txtMail"];
                var txtPassword = info["txtPassword"];

                //Realizar validacion
                Cliente cliente = null;
                if (txtMail.IndexOf("@") != -1 && txtPassword.Length >= 4)
                {
                    cliente = new Cliente();
                    cliente.NombreCliente = "David";
                    cliente.ApellidosCliente = "Arroyo Moreno";
                    cliente.MailCliente = txtMail;
                }

                //HttpResponseMessage response = new HttpResponseMessage();
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
                return response;
            } 
            catch (Exception ex)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Conflict);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }



    }
}
