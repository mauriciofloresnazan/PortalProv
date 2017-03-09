﻿using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Ppgz.Web.Areas.Servicio.Models;
using Ppgz.Web.Infrastructure;
using Ppgz.Web.Infrastructure.Nazan;

namespace Ppgz.Web.Areas.Servicio.Controllers
{
    [Authorize]
    [TerminosCondiciones]
    public class AdministrarPerfilesController : Controller
    {
        private readonly PerfilProveedorManager _perfilProveedorManager = new PerfilProveedorManager();
        private readonly CommonManager _commonManager = new CommonManager();

        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-ADMINISTRARPERFILES-LISTAR,SERVICIO-ADMINISTRARPERFILES-MODIFICAR")]
        public ActionResult Index()
        {
            var perfiles = _perfilProveedorManager
                .FindByCuentaId(_commonManager.GetCuentaUsuarioAutenticado().Id);

            perfiles.Add(_perfilProveedorManager.GetMaestroByUsuarioTipo(CuentaManager.Tipo.Servicio));

            ViewBag.Perfiles = perfiles;
            return View();
        }

        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-ADMINISTRARPERFILES-MODIFICAR")]
        public ActionResult Crear()
        {
            return View(new PefilProveedorViewModel());
        }

        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-ADMINISTRARPERFILES-MODIFICAR")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Crear(PefilProveedorViewModel model)
        {

            if (!ModelState.IsValid) return View(model);

            if (_perfilProveedorManager.FindByNombre(model.Nombre.Trim()) != null)
            {
                ModelState.AddModelError(string.Empty, Nazan.MensajesResource.ERROR_PerfilNazan_NombreExistente);
                return View(model);
            }

            _perfilProveedorManager
                .Create(
                    model.Nombre, model.RolesIds,
                    _commonManager.GetCuentaUsuarioAutenticado().Id);



            TempData["FlashSuccess"] = "Perfil creado con éxito.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-ADMINISTRARPERFILES-MODIFICAR")]
        public ActionResult Editar(int id)
        {
            var perfil = _perfilProveedorManager.Find(id);

            if (perfil == null)
            {
                //TODO ACTUALIZAR MENSAJE AL RESOURCE
                TempData["FlashError"] = "Perfil incorrecto.";
                return RedirectToAction("Index");

            }
            
            var model = new PefilProveedorViewModel
            {
                Nombre = perfil.Nombre,
                RolesIds = perfil.aspnetroles.Select(p => p.Id).ToArray()

            };

            return View(model);
        }


        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-ADMINISTRARPERFILES-MODIFICAR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, PefilProveedorViewModel model)
        {

            var perfil = _perfilProveedorManager.Find(id);

            if (perfil == null)
            {
                //TODO ACTUALIZAR MENSAJE AL RESOURCE
                TempData["FlashError"] = "Perfil incorrecto.";
                return RedirectToAction("Index");

            }

            try
            {
                _perfilProveedorManager.Update(
                    id,
                    model.Nombre,
                    model.RolesIds);

                TempData["FlashSuccess"] = "Perfil actualizado con éxito.";
                return RedirectToAction("Index");
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", Mensajes.ERROR_General);
            }
            catch (Exception exception)
            {
                //TODO ACTUALIZAR MENSAJE AL RESOURCE
                TempData["FlashError"] = exception.Message;
                return RedirectToAction("Index");
            }


            return View(model);
        }

        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-ADMINISTRARPERFILES-MODIFICAR")]
        public ActionResult Eliminar(int id)
        {

            var perfil = _perfilProveedorManager.Find(id);

            if (perfil == null)
            {
                //TODO ACTUALIZAR MENSAJE AL RESOURCE
                TempData["FlashError"] = "Perfil incorrecto.";
                return RedirectToAction("Index");
            }

            try
            {

                _perfilProveedorManager.Remove(id);
            }
            catch (RetryLimitExceededException)
            {
                //TODO ACTUALIZAR MENSAJE AL RESOURCE
                TempData["FlashError"] = "INTENTOS EXEDIDOS";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                //TODO ACTUALIZAR MENSAJE AL RESOURCE
                TempData["FlashError"] = exception.Message;
                return RedirectToAction("Index");
            }

            TempData["FlashSuccess"] = "Perfil eliminado con éxito.";
            return RedirectToAction("Index");
        }
    }
}