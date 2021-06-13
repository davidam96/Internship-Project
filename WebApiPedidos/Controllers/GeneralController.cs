using ModeloDatos;
using Newtonsoft.Json;
using PedidosCapaBL;
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

                //Implementamos la logica de negocio sobre 'info'
                Cliente cliente = ClientesBL.ValidarCliente(info);

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
