using ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoPedidos.Clases
{
    public class ConectorAPI
    {

        public static Cliente ValidarLogin(string txtMail, string txtPassword)
        {
            Cliente cliente = null;

            //Realizar validacion
            if (txtMail.IndexOf("@") != -1 && txtPassword.Length >= 4)
            {
                cliente = new Cliente();
                cliente.NombreCliente = "David";
                cliente.ApellidosCliente = "Arroyo Moreno";
                cliente.MailCliente = txtMail;
            }

            return cliente;
        }

        public static List<Producto> ObtenerProductos() {

            List<Producto> Productos = new List<Producto>();
            var rnd = new Random();
            Producto p;
            for (int i = 1; i <= 5; i++)
            {
                p = new Producto();
                p.Codigo = "Prod" + i;
                p.Descripcion = "Producto " + i;
                p.PrecioVenta = (float) Math.Round(rnd.NextDouble() * 1000, 2);
                Productos.Add(p);
            }
            return Productos;
        }


    }
}