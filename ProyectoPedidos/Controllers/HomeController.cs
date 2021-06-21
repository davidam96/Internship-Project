using ModeloDatos;
using ProyectoPedidos.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            //pues de lo contrario el login no es válido.
            if (cliente == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                return View("LoginConLayout");
            }

            //Creamos varias cookies en el servidor que enviaremos al cliente (navegador)
            Response.Cookies.Add(new HttpCookie("CLI_CodigoCliente", cliente.Codigo.ToString()));
            Response.Cookies.Add(new HttpCookie("CLI_NombreCliente", cliente.NombreCliente + " " + cliente.ApellidosCliente));

            #region Hay que ponerlo en TODOS los métodos que devuelvan una vista del cliente:

            ViewBag.TipoLogout = "LogoutCliente";
            var c = new HttpCookie("Tipo", "Cliente");
            Response.Cookies.Add(c);

            #endregion

            return View("ListaPedidosCliente");
        }

        public ActionResult LogoutCliente()
        {
            //Para eliminar cookies del navegador, ESTO NO FUNCIONA:
            //Response.Cookies.Remove("CLI_CodigoCliente");

            HttpCookie c;

            foreach (string key in Request.Cookies.AllKeys)
            {
                if (key.StartsWith("CLI"))
                {
                    //Esta es la única manera de hacer que una cookie del
                    //lado del cliente (el navegador) se pueda eliminar. 

                    c = Request.Cookies[key]; // --> Opcion 1
                    //var cookie = new HttpCookie(key); // --> Opcion 2
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }
            }

            //Cookie temporal para el control correcto del logout
            c = new HttpCookie("Tipo", "Logout");
            Response.Cookies.Add(c);

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
                int codigoCliente = int.Parse(Request.Cookies["CLI_CodigoCliente"].Value);

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

        public ActionResult ObtenerPedidos(int? codigoCliente, DateTime? fechaDesde, DateTime? fechaHasta, bool? incluirEmpleados)
        {
            try
            {
                Dictionary<string, string> datos = new Dictionary<string, string>();

                if (codigoCliente.HasValue)
                    datos.Add("CodigoCliente", codigoCliente.ToString());
                if (fechaDesde.HasValue)
                    datos.Add("FechaDesde", fechaDesde.ToString());
                if (fechaHasta.HasValue)
                    datos.Add("FechaHasta", fechaHasta.ToString());
                if (incluirEmpleados.HasValue)
                    datos.Add("IncluirEmpleados", incluirEmpleados.ToString());

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
            #region Hay que ponerlo en TODOS los métodos que devuelvan una vista del cliente:

            ViewBag.TipoLogout = "LogoutCliente";
            var c = new HttpCookie("Tipo", "Cliente");
            Response.Cookies.Add(c);

            #endregion

            return View();
        }

        public ActionResult ListaPedidosCliente()
        {
            if (Request.Cookies.AllKeys.Contains("CLI_CodigoCliente"))
            {
                #region Hay que ponerlo en TODOS los métodos que devuelvan una vista del cliente:

                ViewBag.TipoLogout = "LogoutCliente";
                var c = new HttpCookie("Tipo", "Cliente");
                Response.Cookies.Add(c);

                #endregion

                return View();
            }

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

        public ActionResult ValidarEmpleado(string txtNombreEmpleado, string txtPassword)
        {
            var empleado = ConectorAPI.ValidarEmpleado(txtNombreEmpleado, txtPassword);

            //Comprobamos que el empleado no sea nulo,
            //pues de lo contrario el login no es válido.
            if (empleado == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                return View("LoginConLayout");
            }

            //Creamos varias cookies en el servidor que enviaremos al cliente (navegador)
            Response.Cookies.Add(new HttpCookie("EMP_CodigoEmpleado", empleado.Codigo.ToString()));
            Response.Cookies.Add(new HttpCookie("EMP_NombreEmpleado", empleado.Nombre + " " + empleado.Apellidos));
            Response.Cookies.Add(new HttpCookie("EMP_PuedePrepararPedidos",  empleado.PuedePrepararPedidos.ToString()));
            Response.Cookies.Add(new HttpCookie("EMP_PuedeEnviarPedidos",  empleado.PuedeEnviarPedidos.ToString()));

            #region Hay que ponerlo en TODOS los métodos que devuelvan una vista del empleado:

            ViewBag.TipoLogout = "LogoutEmpleado";
            var c = new HttpCookie("Tipo", "Empleado");
            Response.Cookies.Add(c);

            #endregion

            return View("ListaPedidosEmpleado");

        }

        public ActionResult LogoutEmpleado()
        {
            //Para eliminar cookies del navegador, ESTO NO FUNCIONA:
            //Response.Cookies.Remove("EMP_CodigoEmpleado");

            HttpCookie c;

            foreach (string key in Request.Cookies.AllKeys)
            {
                if (key.StartsWith("EMP"))
                {
                    //Esta es la única manera de hacer que una cookie del
                    //lado del cliente (el navegador) se pueda eliminar. 

                    c = Request.Cookies[key]; // --> Opcion 1
                    //var cookie = new HttpCookie(key); // --> Opcion 2
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }
            }

            //Cookie temporal para el control correcto del logout
            c = new HttpCookie("Tipo", "Logout");
            Response.Cookies.Add(c);

            return View("LoginConLayout");
        }

        public ActionResult PrepararPedido(int codigoPedido, int codigoEmpleado)
        {
            try
            {
                Dictionary<string, string> datos = new Dictionary<string, string>();
                datos.Add("CodigoPedido", codigoPedido.ToString());
                datos.Add("CodigoEmpleado", codigoEmpleado.ToString());
                datos.Add("accion", "Preparar");

                Pedido pedido = ConectorAPI.ModificarPedido(datos);

                if (pedido != null)
                    return Json(new { Fecha = pedido.FechaPreparacionCadena }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Error = "No se puede preparar el pedido, actualice los datos." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EnviarPedido(int codigoPedido, int codigoEmpleado)
        {
            try
            {
                Dictionary<string, string> datos = new Dictionary<string, string>();
                datos.Add("CodigoPedido", codigoPedido.ToString());
                datos.Add("CodigoEmpleado", codigoEmpleado.ToString());
                datos.Add("accion", "Enviar");

                Pedido pedido = ConectorAPI.ModificarPedido(datos);

                if (pedido != null)
                    return Json(new { Fecha = pedido.FechaEnvioCadena }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Error = "No se puede enviar el pedido, actualice los datos." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CancelarPedido(int codigoPedido)
        {
            try
            {
                Dictionary<string, string> datos = new Dictionary<string, string>();
                datos.Add("CodigoPedido", codigoPedido.ToString());
                datos.Add("accion", "Cancelar");

                Thread.Sleep(1000); //BORRA ESTA LINEA

                Pedido pedido = ConectorAPI.ModificarPedido(datos);

                return Json(new { Fecha = pedido.FechaCancelacionCadena }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}