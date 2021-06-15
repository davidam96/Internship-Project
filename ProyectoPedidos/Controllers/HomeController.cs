using ModeloDatos;
using ProyectoPedidos.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoPedidos.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {   
            ViewBag.Titulo = "Login";

            return View("LoginConLayout");
        }

        public ActionResult About()
        {
            ViewBag.Title = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Your contact page.";

            return View();
        }

        //En vez de poner Request.getParameter() para recuperar datos de un formulario,
        //puedes hacerlo añadiendo parametros string al metodo y se te rellenan
        //automáticamente al enviar el formulario desde el cliente al servidor.
        public ActionResult ValidarCliente(string txtMail, string txtPassword)
        {

            var cliente = ConectorAPI.ValidarCliente(txtMail, txtPassword);

            //Comprobamos que el cliente no sea nulo,
            //pues de lo contrario el loguin no es válido.
            if (cliente == null)
            {
                ViewBag.TextoError = "Usuario o contraseña incorrectos.";
                return View("LoginConLayout");
            }

            //Creamos varias cookies en el servidor que enviaremos al cliente
            Response.Cookies.Add(new HttpCookie("CodigoCliente", cliente.Codigo.ToString()));
            Response.Cookies.Add(new HttpCookie("NombreCliente", cliente.NombreCliente + " " + cliente.ApellidosCliente));

            //return View("HacerPedido");
            return View("ListaPedidosCliente");
        }

        public ActionResult LogoutCliente()
        {
            //Para eliminar cookies del cliente, ESTO NO FUNCIONA:
            //Response.Cookies.Remove("CodigoCliente");

            //Esta es la única manera de hacer que una cookie
            //del cliente (el navegador) se pueda eliminar.
            var c1 = Request.Cookies["CodigoCliente"];
            c1.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c1);

            var c2 = new HttpCookie("NombreCliente");
            c1.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c2);

            return View("LoginConLayout");
        }

        [OutputCache(Duration = 10)]
        public ActionResult CargarProductos()
        {
            try
            {
                var Productos = ConectorAPI.ObtenerProductos();

                //return Json(Porductos, JsonRequestBehavior.AllowGet);
                return Json(new { Productos }, JsonRequestBehavior.AllowGet);
            } 
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Estas llamando a este metodo mediante una peticion ajax con datos;
        //estos datos van a entrar en el método a traves de sus parámetros.
        //Si haces que el tipo del parámetro cuadre con el tipo de los datos
        //originales (en vez de poner 'object' como tipo genérico), el sistema
        //forzará la conversión y deserialización automática de dichos datos.
        public ActionResult CrearPedido(LineaDetalle[] lineasDetalle)
        {
            //Diferencia entre 'LineaDetalle[]'  y  'List<LineaDetalle>'
            //LineaDetalle[]: es inmutable, es decir, tiene un tamaño fijo.
            //List<LineaDetalle>: es mutable, es decir, puede variar su tamaño.

            try
            {
                //El codigo de cliente es necesario para identificar al pedido
                int codigoCliente = int.Parse(Request.Cookies["CodigoCliente"].Value);

                //Creamos un diccionario para almacenar varios datos diferentes bajo una
                //misma estructura de datos, dado que en el método 'HacerPedido()' de la
                //clase ConectorAPI, solo se te permite serializar un único objeto a la
                //hora de llamar a 'RespuestaPOST()'.
                Dictionary<string, object> datos = new Dictionary<string, object>();
                datos.Add("CodigoCliente", codigoCliente);
                datos.Add("LineasDetalle", lineasDetalle);

                var p = ConectorAPI.HacerPedido(datos);

                return Json(new { Pedido = p }, JsonRequestBehavior.AllowGet);

            } catch (Exception ex)
            {
                return Json(new { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ObtenerPedidos(int codigoCliente, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                Dictionary<string, string> datos = new Dictionary<string, string>();

                datos.Add("CodigoCliente", codigoCliente.ToString());

                if (fechaDesde.HasValue)
                    datos.Add("FechaDesde", fechaDesde.ToString());
                if (fechaDesde.HasValue)
                    datos.Add("FechaHasta", fechaHasta.ToString());

                Pedido[] p = ConectorAPI.ObtenerPedidos(datos);

                return Json(new { Pedidos = p }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult HacerPedido()
        {
            return View();
        }

        public ActionResult ListaPedidosCliente()
        {
            if (Request.Cookies.AllKeys.Contains("CodigoCliente"))
            return View();

            //Aqui no escribimos 'return View("...")' porque
            //hacemos algo ANTES de devolver la vista:
            return RedirectToAction("Index");
        }

        public ActionResult ObtenerLineasDetalle(int codigoPedido)
        {
            try
            {
                Dictionary<string, string> datos = new Dictionary<string, string>();
                datos.Add("CodigoPedido", codigoPedido.ToString());

                LineaDetalle[] ld = ConectorAPI.ObtenerLineasDetalle(datos);

                return Json(new { LineasDetalle = ld }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}