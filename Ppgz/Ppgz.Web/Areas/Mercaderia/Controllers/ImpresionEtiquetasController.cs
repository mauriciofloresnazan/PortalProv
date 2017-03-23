﻿using System;
using System.Linq;
using System.Web.Mvc;
using Ppgz.Repository;
using Ppgz.Web.Infrastructure;

namespace Ppgz.Web.Areas.Mercaderia.Controllers
{
    [Authorize]
    [TerminosCondiciones]
    public class ImpresionEtiquetasController : Controller
    {
        // GET: /Mercaderia/ImpresionEtiquetas/
        [Authorize(Roles = "MAESTRO-MERCADERIA")]
        public ActionResult Index()
        {
            var db = new Entities();
            ViewBag.Crs = db.crs.ToList();
            return View();
        }
        public FileResult Descargar(int id)
        {
            var db = new Entities();
            var cr = db.crs.Find(id);
            if (cr == null)
            {
                // TODO
                throw new Exception("CR Incorrecto");
            }
             var fileBytes = System.IO.File.ReadAllBytes(cr.ArchivoCR);
            
            var fileName = cr.Codigo +  ".pdf";

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);


        }

    }
}