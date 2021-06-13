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
    public class ProductosController : ApiController
    {

        [HttpGet]
        public HttpResponseMessage ObtenerProductos()
        {
            HttpResponseMessage response;

            try
            {
                List<Producto> Productos = ProductosBL.ObtenerProductosBL();

                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(Productos), Encoding.UTF8, "application/json");
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict);
                response.ReasonPhrase = ex.Message;
            }
            return response;
        }


    }
}
