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
            Response.Cookies.Add(new HttpCookie("CodigoUsuario", cliente.Codigo.ToString()));
            Response.Cookies.Add(new HttpCookie("NombreUsuario", cliente.NombreCliente + " " + cliente.ApellidosCliente));

            return View("VistaCliente");
        }

        public ActionResult LogoutCliente()
        {
            //Para eliminar cookies del cliente, ESTO NO FUNCIONA:
            //Response.Cookies.Remove("CodigoUsuario");

            //Esta es la única manera de hacer que una cookie
            //del cliente (el navegador) se pueda eliminar.
            var c1 = Request.Cookies["CodigoUsuario"];
            c1.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c1);

            var c2 = new HttpCookie("NombreUsuario");
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

    }
}