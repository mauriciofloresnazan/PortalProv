using System;
using System.Linq;
using System.Web.Mvc;
using Ppgz.Services;
using Ppgz.Web.Areas.Nazan.Models;
using Ppgz.Web.Infrastructure;

namespace Ppgz.Web.Areas.Nazan.Controllers
{
    [Authorize]
    public class AdministrarPerfilesNazanController : Controller
    {
        private readonly PerfilManager _perfilManager = new PerfilManager();

        //
        // GET: /Nazan/AdministrarPerfilesNazan/
        [Authorize(Roles = "MAESTRO-NAZAN,NAZAN-ADMINISTRARPERFILESNAZAN-LISTAR,NAZAN-ADMINISTRARPERFILESNAZAN-MODIFICAR")]
        public ActionResult Index()
        {
            ViewBag.Perfiles = _perfilManager.FindPerfilesNazan();

            return View();
        }
        [Authorize(Roles = "MAESTRO-NAZAN,NAZAN-ADMINISTRARPERFILESNAZAN-MODIFICAR")]
        public ActionResult Crear()
        {
            return View(new PefilNazanViewModel());
        }

        [Authorize(Roles = "MAESTRO-NAZAN,NAZAN-ADMINISTRARPERFILESNAZAN-MODIFICAR")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Crear(PefilNazanViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                _perfilManager.CrearNazan(model.Nombre, model.RolesIds);

                TempData["FlashSuccess"] = MensajesResource.INFO_PerfilNazan_CreadoCorrectamente;
                return RedirectToAction("Index");
            }
            catch (BusinessException businessEx)
            {
                ModelState.AddModelError(string.Empty, businessEx.Message);

                return View(model);
            }
            catch (Exception e)
            {
                var log = CommonManager.BuildMessageLog(
                    TipoMensaje.Error,
                    ControllerContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString(),
                    ControllerContext.Controller.ValueProvider.GetValue("action").RawValue.ToString(),
                    e.ToString(), Request);

                CommonManager.WriteAppLog(log, TipoMensaje.Error);

                return View(model);
            }
        }

        [Authorize(Roles = "MAESTRO-NAZAN,NAZAN-ADMINISTRARPERFILESNAZAN-MODIFICAR")]
        public ActionResult Editar(int id)
        {
            var perfil = _perfilManager.Find(id);

            if (perfil == null)
            {
                TempData["FlashError"] = MensajesResource.ERROR_PerfilNazan_PefilIdIncorrecto;
                return RedirectToAction("Index");

            }
            
            var model = new PefilNazanViewModel
            {
                Nombre = perfil.Nombre,
                RolesIds = perfil.AspNetRoles.Select(p=> p.Id).ToArray()

            };

            return View(model);
        }


        [Authorize(Roles = "MAESTRO-NAZAN,NAZAN-ADMINISTRARPERFILESNAZAN-MODIFICAR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, PefilNazanViewModel model)
        {
            ActionResult actionResult;
            try
            {
                _perfilManager.ActualizarNazan(
                    id,
                    model.Nombre,
                    model.RolesIds);

                TempData["FlashSuccess"] = MensajesResource.INFO_PerfilNazan_ActualizadoCorrectamente;
                return RedirectToAction("Index");
            }
            catch (BusinessException businessEx)
            {
                ModelState.AddModelError(string.Empty, businessEx.Message);

                actionResult = View(model);
            }
            catch (Exception e)
            {
                var log = CommonManager.BuildMessageLog(
                    TipoMensaje.Error,
                    ControllerContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString(),
                    ControllerContext.Controller.ValueProvider.GetValue("action").RawValue.ToString(),
                    e.ToString(), Request);

                CommonManager.WriteAppLog(log, TipoMensaje.Error);

                TempData["FlashError"] = MensajesResource.ERROR_General;
                return RedirectToAction("Index");
            }

            return actionResult;
        }

        [Authorize(Roles = "MAESTRO-NAZAN,NAZAN-ADMINISTRARPERFILESNAZAN-MODIFICAR")]
        public ActionResult Eliminar(int id)
        {
            try
            {
                _perfilManager.Eliminar(id);

                TempData["FlashSuccess"] = MensajesResource.INFO_PerfilNazan_EliminadoCorrectamente;
                return RedirectToAction("Index");
            }
            catch (BusinessException businessEx)
            {
                TempData["FlashError"] = businessEx.Message;
                return RedirectToAction("Index");
   
            }
            catch (Exception e)
            {
                var log = CommonManager.BuildMessageLog(
                    TipoMensaje.Error,
                    ControllerContext.Controller.ValueProvider.GetValue("controller").RawValue.ToString(),
                    ControllerContext.Controller.ValueProvider.GetValue("action").RawValue.ToString(),
                    e.ToString(), Request);

                CommonManager.WriteAppLog(log, TipoMensaje.Error);

                TempData["FlashError"] = MensajesResource.ERROR_General;
                return RedirectToAction("Index");
            }

        }
    }
}