﻿using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Ppgz.Web.Infrastructure;

namespace Ppgz.Web.Controllers
{
	[Authorize]
	public class ProveedorMensajesInstitucionalesController : Controller
	{


		private readonly CommonManager _commonManager = new CommonManager();

        private readonly TipoProveedorManager _tipoProveedorManager  = new TipoProveedorManager();

        private readonly MensajesManager _mensajesManager = new MensajesManager();

		//
		// GET: /ProveedorMensajesInstitucionales/
		public ActionResult Index()
		{

			//CUENTAS
		    var cuenta = _commonManager.GetCuentaUsuarioAutenticado();

		    var tipoProveedor = _tipoProveedorManager.Find(cuenta.tipo_proveedor_id);

            var mensajes = _mensajesManager.GetByEnviadoA(tipoProveedor.codigo);

            var mensajesUsuario = _mensajesManager.GetByUsuarioId(User.Identity.GetUserId());

			ViewBag.mensajes = mensajes;

			ViewBag.mensajesUsuario = Json(mensajesUsuario);

			return View();
		}

		[HttpPost]
		public ActionResult Visualizar(int id)
		{
			var mensaje = _mensajesManager.FindById(id);
            
            
            var mensajes = _mensajesManager.GetByUsuarioId(User.Identity.GetUserId());

            if (mensajes.Any(i => i.mensaje_id == id))
			{
				return Content("Actualizado"); ;
			}

		    _mensajesManager.Visualizar(User.Identity.GetUserId(), id);

			return Content("Actualizado");
		}

        /*
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Leido(int id)
		{

			var mensaje = _db.mensajes.Single(i => i.id == id);
				
		
			if(_db.usuario_mensajes.Any(i => i.mensaje_id == id && i.usuario_id ==int.Parse(User.Identity.GetUserId())))
			{
			   return Content("Leido");;
			}

			var usuarioMensaje = new usuario_mensajes
			{
				mensaje_id = mensaje.id,
				usuario_id = int.Parse(User.Identity.GetUserId())
			};

			_db.usuario_mensajes.Add(usuarioMensaje);

			_db.SaveChanges();

			return Content("Leido");
		}*/
	}
}