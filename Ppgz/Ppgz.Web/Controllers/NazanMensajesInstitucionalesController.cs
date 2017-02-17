﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ppgz.Repository;
using Ppgz.Web.Models;

namespace Ppgz.Web.Controllers
{
    [Authorize]
    public class NazanMensajesInstitucionalesController : Controller
    {

        private readonly PpgzEntities _db = new PpgzEntities();
        //
        // GET: /NazanMensajesInstitucionales/
        public ActionResult Index()
        {
            var mensajes = _db.mensajes.ToList();

            ViewBag.mensajes = mensajes;
            return View();
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(MensajeViewModel model, FormCollection collection)
        {



            if (ModelState.IsValid)
            {



                var mensaje = new mensaje()
                {
                    fecha_publicacion = DateTime.ParseExact(model.FechaPublicacion, "d/M/yyyy", CultureInfo.InvariantCulture),
                    fecha_caducidad = DateTime.ParseExact(model.FechaCaducidad, "d/M/yyyy", CultureInfo.InvariantCulture),
                    titulo = model.Titulo

                };

                if (!String.IsNullOrEmpty(Request["pdf-mensaje"]))
                {
                    if (Request.Files.Count == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Debe agregar un contenido textual para el mensaje o un Archivo PDF.");
                        return View(model);
                    }

                    var file = Request.Files[0];

                    if (file != null && file.ContentType != "application/pdf")
                    {
                        ModelState.AddModelError(string.Empty, "El archivo debe ser de tipo PDF.");
                        return View(model);
                    }


                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);

                        if (fileName != null)
                        {
                            var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                            file.SaveAs(path);

                            mensaje.archivo = "~/Uploads/"+ fileName;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(model.Contenido))
                    {
                        ModelState.AddModelError(string.Empty, "Debe agregar un contenido textual para el mensaje o un Archivo PDF.");
                        return View(model);
                    }

                    mensaje.contenido = model.Contenido;
                }


                mensaje.enviado_a = model.TipoProveedor;
                _db.mensajes.Add(mensaje);


                _db.SaveChanges();


                return RedirectToAction("Index");
            }

            return View(model);

        }


        public ActionResult Editar(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var mensaje = _db.mensajes.Single(i => i.id == id);


            if (mensaje == null)
            {
                return HttpNotFound();
            }
            

            var mensajeModel = new MensajeViewModel()
            {
                Contenido = mensaje.contenido,
                FechaCaducidad = ((DateTime)mensaje.fecha_caducidad).ToString("dd/MM/yyyy"),
                FechaPublicacion = ((DateTime)mensaje.fecha_publicacion).ToString("dd/MM/yyyy"),
                TipoProveedor = mensaje.enviado_a,
                Titulo=  mensaje.titulo
                
            };

            return View(mensajeModel);
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPost(int? id, FormCollection collection)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mensajeParaActualizar = _db.mensajes.Single(i => i.id == id);
            var oldMensajeModel = new MensajeViewModel()
            {
                Contenido = mensajeParaActualizar.contenido,
                FechaCaducidad = ((DateTime)mensajeParaActualizar.fecha_caducidad).ToString("dd/MM/yyyy"),
                FechaPublicacion = ((DateTime)mensajeParaActualizar.fecha_publicacion).ToString("dd/MM/yyyy"),
                TipoProveedor = mensajeParaActualizar.enviado_a,
                Titulo = mensajeParaActualizar.titulo

            };

            mensajeParaActualizar.archivo = null;

            if (!String.IsNullOrEmpty(Request["pdf-mensaje"]))
            {
                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "Debe agregar un contenido textual para el mensaje o un Archivo PDF.");
                    return View(oldMensajeModel);
                }

                var file = Request.Files[0];

                if (file != null && file.ContentType != "application/pdf")
                {
                    ModelState.AddModelError(string.Empty, "El archivo debe ser de tipo PDF.");
                    return View(oldMensajeModel);
                }


                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    if (fileName != null)
                    {
                        var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                        file.SaveAs(path);

                        mensajeParaActualizar.archivo = "~/Uploads/" + fileName;
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(collection["Contenido"]))
                {
                    ModelState.AddModelError(string.Empty, "Debe agregar un contenido textual para el mensaje o un Archivo PDF.");
                    return View(oldMensajeModel);
                }

                mensajeParaActualizar.contenido = collection["Contenido"];
            }

            mensajeParaActualizar.titulo = collection["Titulo"];

            mensajeParaActualizar.enviado_a = collection["TipoProveedor"];
            
            _db.Entry(mensajeParaActualizar).State = EntityState.Modified;
            
            _db.SaveChanges();



            return RedirectToAction("Index");
        }

        public ActionResult Eliminar(int id)
        {
            
            var mensaje = _db.mensajes.Single(i => i.id == id);


            if (mensaje == null)
            {
                return HttpNotFound();
            }
            

            try
            {
                _db.mensajes.Remove(mensaje);
                _db.SaveChanges();
            }
            catch (RetryLimitExceededException)
            {

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}