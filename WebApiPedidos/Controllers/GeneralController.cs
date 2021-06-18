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

        [Route("api/General/ValidarCliente")]
        [HttpPost]
        public HttpResponseMessage ValidarCliente(object oDatos)
        {
            Dictionary<string, string> datos;

            try
            {
                //Deserializamos el parametro 'oDatos'
                datos = JsonConvert.DeserializeObject<Dictionary<string, string>>(oDatos.ToString());

                //Implementamos la logica de negocio sobre 'datos'
                Cliente cliente = ClientesBL.ValidarCliente(datos);

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

        [Route("api/General/CrearPedido")]
        [HttpPost]
        public HttpResponseMessage CrearPedido(object oDatos)
        {
            Dictionary<string, object> datos;

            try
            {
                datos = JsonConvert.DeserializeObject<Dictionary<string, object>>(oDatos.ToString());

                int codigoCliente = Convert.ToInt32(datos["CodigoCliente"]);
                LineaDetalle[] lineasDetalle = JsonConvert.DeserializeObject<LineaDetalle[]>(datos["LineasDetalle"].ToString());

                Pedido pedido = PedidosBL.CrearPedido(codigoCliente, lineasDetalle);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(pedido), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Conflict);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [Route("api/General/ObtenerPedidos")]
        [HttpPost]
        public HttpResponseMessage ObtenerPedidos(object oDatos)
        {
            Dictionary<string, string> datos;

            try
            {
                datos = JsonConvert.DeserializeObject<Dictionary<string, string>>(oDatos.ToString());

                Pedido[] pedidos = PedidosBL.ObtenerPedidos(datos);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(pedidos), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Conflict);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [Route("api/General/ObtenerLineasDetalle")]
        [HttpPost]
        public HttpResponseMessage ObtenerLineasDetalle(object oDatos)
        {
            Dictionary<string, string> datos;

            try
            {
                datos = JsonConvert.DeserializeObject<Dictionary<string, string>>(oDatos.ToString());

                LineaDetalle[] detalles = PedidosBL.ObtenerLineasDetalle(datos);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(detalles), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Conflict);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [Route("api/General/ValidarEmpleado")]
        [HttpPost]
        public HttpResponseMessage ValidarEmpleado(object oDatos)
        {
            Dictionary<string, string> datos;

            try
            {
                //Deserializamos el parametro 'oDatos'
                datos = JsonConvert.DeserializeObject<Dictionary<string, string>>(oDatos.ToString());

                //Implementamos la logica de negocio sobre 'datos'
                Empleado empleado = EmpleadosBL.ValidarEmpleado(datos);

                //HttpResponseMessage response = new HttpResponseMessage();
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(empleado), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Conflict);
                response.ReasonPhrase = ex.Message;
                return response;
            }
        }

        [Route("api/General/ModificarPedido")]
        [HttpPost]
        public HttpResponseMessage ModificarPedido(object oDatos)
        {
            Dictionary<string, string> datos;

            try
            {
                datos = JsonConvert.DeserializeObject<Dictionary<string, string>>(oDatos.ToString());

                Pedido pedido = PedidosBL.ModificarPedido(datos);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(pedido), Encoding.UTF8, "application/json");
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
