using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Ppgz.Services;
using Ppgz.Web.Infrastructure;

namespace Ppgz.Web.Areas.Servicio.Controllers
{
    [Authorize]
    [TerminosCondiciones]
    public class FacturasController : Controller
    {
        private readonly CommonManager _commonManager = new CommonManager();
        private readonly ProveedorManager _proveedorManager = new ProveedorManager();
        private readonly FacturaManager _facturaManager = new FacturaManager();


        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-FACTURAS")]
        public ActionResult Index()
        {

            var cuenta = _commonManager.GetCuentaUsuarioAutenticado();

            ViewBag.proveedores = _proveedorManager.FindByCuentaId(cuenta.Id);

            return View();
        }

        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-FACTURAS")]
        public ActionResult Facturas(int proveedorId)
        {
            var cuenta = _commonManager.GetCuentaUsuarioAutenticado();

            var proveedor = _proveedorManager.Find(proveedorId, cuenta.Id);

            if (proveedor == null)
            {
                // TODO
                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }

            ViewBag.Facturas = _facturaManager.GetFacturas(proveedorId);

            ViewBag.Proveedor = proveedor;

            return View();

        }

        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-FACTURAS")]
        public ActionResult CargarFactura(int proveedorId)
        {
            var cuenta = _commonManager.GetCuentaUsuarioAutenticado();

            var proveedor = _proveedorManager.Find(proveedorId, cuenta.Id);

            if (proveedor == null)
            {
                // TODO
                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }


            ViewBag.Proveedor = proveedor;

            return View();

        }


        internal void ValidarPdf(HttpPostedFileBase file)
        {
            if (file != null && file.ContentType != "application/pdf")
            {
                throw new BusinessException("Pdf Invalido");
            }

            if (file == null)
            {
                throw new BusinessException("Pdf Invalido");
            }

        }
        internal void ValidarXml(HttpPostedFileBase file)
        {
            if (file != null && file.ContentType != "text/xml")
            {
                throw new BusinessException("El archivo no puede ser procesado");
            }

            if (file == null)
            {
                throw new BusinessException("El archivo no puede ser procesado");
            }

            _facturaManager.ValidarFactura(file.InputStream);


        }



        internal void ValidarPdfVersion4(HttpPostedFileBase file)
        {
            if (file != null && file.ContentType != "application/pdf")
            {
                throw new BusinessException("Pdf Invalido");
            }

            if (file == null)
            {
                throw new BusinessException("Pdf Invalido");
            }
            /*
            var fileName = Path.GetFileName(file.FileName);

            if (fileName == null)
            {
                throw new BusinessException(MensajesResource.ERROR_MensajesInstitucionales_PdfInvalido);
            }

            var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);

            file.SaveAs(path);

            return "~/Uploads/" + fileName;*/
        }
        internal void ValidarXmlVersion4(HttpPostedFileBase file)
        {
            if (file != null && file.ContentType != "text/xml")
            {
                throw new BusinessException("El archivo no puede ser procesado");
            }

            if (file == null)
            {
                throw new BusinessException("El archivo no puede ser procesado");
            }




            _facturaManager.ValidarFacturaVersion4(file.InputStream);

            /*
            var fileName = Path.GetFileName(file.FileName);

            if (fileName == null)
            {
                throw new BusinessException(MensajesResource.ERROR_MensajesInstitucionales_PdfInvalido);
            }

            var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);

            file.SaveAs(path);

            return "~/Uploads/" + fileName;*/
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-FACTURAS")]
        public ActionResult CargarFactura(FacturaViewModel model)
        {
            var cuenta = _commonManager.GetCuentaUsuarioAutenticado();

            var proveedor = _proveedorManager.Find(model.ProveedorId, cuenta.Id);

            if (proveedor == null)
            {

                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }

            ViewBag.Proveedor = proveedor;


            if (!ModelState.IsValid) return View(model);

            var tempXmlPath = Path.Combine(Server.MapPath("~/Uploads/"), "temp-" + Guid.NewGuid() + ".xml");
            var tempPdfPath = Path.Combine(Server.MapPath("~/Uploads/"), "temp-" + Guid.NewGuid() + ".pdf");
            try
            {

                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "Archivos incorrectos");
                    return View(model);
                }

                if (Request.Files["xml"] == null)
                {
                    ModelState.AddModelError(string.Empty, "Xml Incorrecto");

                    return View(model);

                }
                if (Request.Files["pdf"] == null)
                {
                    ModelState.AddModelError(string.Empty, "Pdf incorrecto.");

                    return View(model);

                }


                var xmlFiles = Request.Files["xml"];
                xmlFiles.SaveAs(tempXmlPath);
                XmlDocument doc = new XmlDocument();
                doc.Load(tempXmlPath);//Leer el XML

                //agregamos un Namespace, que usaremos para buscar que el nodo no exista:
                XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
                nsm.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                XmlNode nodeComprobante = doc.SelectSingleNode("//cfdi:Comprobante", nsm);

                if (nodeComprobante == null)
                {
                    var xmlFile = Request.Files["xml"];
                    ValidarXmlVersion4(xmlFile);
                    xmlFile.SaveAs(tempXmlPath);

                    var pdfFile = Request.Files["pdf"];
                    ValidarPdfVersion4(pdfFile);
                    pdfFile.SaveAs(tempPdfPath);
                    _facturaManager.CargarFacturaServicioV4(proveedor.Id, cuenta.Id, tempXmlPath, tempPdfPath);

                }
                else
                {
                    var xmlFile = Request.Files["xml"];
                    ValidarXml(xmlFile);
                    xmlFile.SaveAs(tempXmlPath);

                    var pdfFile = Request.Files["pdf"];
                    ValidarPdf(pdfFile);
                    pdfFile.SaveAs(tempPdfPath);

                    _facturaManager.CargarFacturaServicio(proveedor.Id, cuenta.Id, tempXmlPath, tempPdfPath);
                }







                TempData["FlashSuccess"] = "Factura registrada satisfactoriamente";
                return RedirectToAction("Facturas", new { proveedorId = proveedor.Id });
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

                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
            finally
            {

                if (System.IO.File.Exists(tempPdfPath))
                    System.IO.File.Delete(tempPdfPath);

                if (System.IO.File.Exists(tempXmlPath))
                    System.IO.File.Delete(tempXmlPath);
            }

        }


        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-FACTURAS")]
        public FileResult DescargarXml(int facturaId, int proveedorId)
        {
            var cuenta = _commonManager.GetCuentaUsuarioAutenticado();
            var proveedor = _proveedorManager.Find(proveedorId, cuenta.Id);
            if (proveedor == null)
            {
                throw new Exception("Proveedor incorrecto");
            }

            var factura = _facturaManager.Find(facturaId, proveedorId);

            if (factura == null)
            {
                throw new Exception("Factura incorrecta");
            }

            var fileBytes = System.IO.File.ReadAllBytes(factura.XmlRuta);

            var fileName = factura.Uuid + ".xml";

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Text.Xml, fileName);


        }

        [Authorize(Roles = "MAESTRO-SERVICIO,SERVICIO-FACTURAS")]
        public FileResult DescargarPdf(int facturaId, int proveedorId)
        {
            var cuenta = _commonManager.GetCuentaUsuarioAutenticado();
            var proveedor = _proveedorManager.Find(proveedorId, cuenta.Id);
            if (proveedor == null)
            {
                throw new Exception("Proveedor incorrecto");
            }

            var factura = _facturaManager.Find(facturaId, proveedorId);

            if (factura == null)
            {
                throw new Exception("Factura incorrecta");
            }

            var fileBytes = System.IO.File.ReadAllBytes(factura.PdfRuta);

            var fileName = factura.Uuid + ".pdf";

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);


        }
    }
}