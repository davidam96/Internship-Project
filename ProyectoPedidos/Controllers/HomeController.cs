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

            //Mantenemos en la sesión el login, por si
            //el usuario se había logueado previamente.
            //if (Session["HayLogin"] != null && Session["HayLogin"].ToString().Equals("1"))
            //{
            //    Response.Cookies.Add(new HttpCookie("EsLoginValido", "1"));
            //} else
            //{
            //    Response.Cookies.Add(new HttpCookie("EsLoginValido", "0"));
            //}

            return View("LoginConLayout");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //MIS METODOS

        //En vez de poner Request.getParameter() para recuperar datos de un formulario,
        //puedes hacerlo añadiendo parametros string al metodo y se te rellenan
        //automáticamente al enviar el formulario desde el cliente al servidor.
        public ActionResult ValidarLogin(string txtMail, string txtPassword, string esCliente)
        {

            var cliente = ConectorAPI.ValidarLogin(txtMail, txtPassword);

            //Comprobamos que el cliente no sea nulo,
            //pues de lo contrario el loguin no es válido.
            if (cliente == null)
            {
                ViewBag.TextoError = "Usuario o contraseña incorrectos.";

                Response.Cookies.Add(new HttpCookie("EsLoginValido", "0"));

                return View("LoginConLayout");
            }

            //Creamos varias cookies en el servidor que enviaremos al cliente
            Response.Cookies.Add(new HttpCookie("CodigoUsuario", cliente.Codigo.ToString()));
            Response.Cookies.Add(new HttpCookie("NombreUsuario", cliente.NombreCliente + " " + cliente.ApellidosCliente));
            Response.Cookies.Add(new HttpCookie("EsLoginValido", "1"));

            //Distinguimos entre si se loguea un cliente o un empleado
            if (esCliente.Equals("1"))
            {
                Response.Cookies.Add(new HttpCookie("EsCliente", "1"));
            } else
            {
                Response.Cookies.Add(new HttpCookie("EsCliente", "0"));
            }

            return View("VistaCliente");
        }

        public ActionResult RenderLogin()
        {
            //El problema es que estás llamando a este método desde un
            //hilo secundario con Ajax, por eso la sesión no se guarda.
            //Session["HayLogin"] = "0";

            return PartialView("Login");
        }

        public ActionResult RenderLogout()
        {
            //El problema es que estás llamando a este método desde un
            //hilo secundario con Ajax, por eso la sesión no se guarda.
            //Session["HayLogin"] = "1";

            return PartialView("Logout");
        }

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