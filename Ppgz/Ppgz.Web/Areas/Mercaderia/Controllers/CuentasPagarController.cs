﻿/*using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using Ppgz.Repository;
using Ppgz.Services;
using Ppgz.Web.Infrastructure;

namespace Ppgz.Web.Areas.Mercaderia.Controllers
{
    [Authorize]
    [TerminosCondiciones]
    public class CuentasPagarController : Controller
    {

        readonly CommonManager _commonManager = new CommonManager();
        readonly CuentasPorPagarManager _cuentasPorPagarManager = new CuentasPorPagarManager();
        readonly ProveedorManager _proveedorManager = new ProveedorManager();


        // GET: /Mercaderia/CuentasPagar/
        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult Index()
        {
            var cuenta = _commonManager.GetCuentaUsuarioAutenticado();

            ViewBag.proveedores = _proveedorManager.FindByCuentaId(cuenta.Id);

            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult Devoluciones(int proveedorId = 0)
        {
            int id = proveedorId > 0 ? proveedorId : Convert.ToInt32(System.Web.HttpContext.Current.Session["proveedorId"]);

            if (id == 0)
                return RedirectToAction("Index");
            var commonManager = new CommonManager();
            MySqlParameter[] parametes = {
					new MySqlParameter("id", id)
				};
            const string sql = @"
				SELECT * 
				FROM   devoluciones d
				JOIN   proveedores p ON p.Id = d.ProveedorId
				WHERE  p.Id = @id";
            //ViewBag.devoluciones = commonManager.QueryToTable(sql, parametes);
            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult DevolucionesDetalle(int id)
        {
            var commonManager = new CommonManager();
            MySqlParameter[] parametes = {
					new MySqlParameter("id", id)
				};
            const string sql = @"
				SELECT * 
				FROM   devoluciones d
				WHERE  d.Id = @id";
            ViewBag.devolucion = commonManager.QueryToTable(sql, parametes).Rows[0];

            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public JsonResult PagoDetalle(string numeroCompensacion)
        {
            var transacciones = _cuentasPorPagarManager.FindPagoDetalleByNumeroCompensacion(numeroCompensacion).ToList();

            var jsonData = new
            {
                data = from transaccion in transacciones select transaccion
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult Pagos(int proveedorId = 0)
        {
            int id = proveedorId > 0 ? proveedorId : Convert.ToInt32(System.Web.HttpContext.Current.Session["proveedorId"]);

            if (id == 0)
                return RedirectToAction("Index");

            ViewBag.pagos = _cuentasPorPagarManager.FindPagosByProveedorId(id);
      
            ViewBag.proveedor = _proveedorManager.Find(id);
            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult PagosDetalle(string numeroCompensacion)
        {
            var data = _cuentasPorPagarManager.FindPagoDetalleByNumeroCompensacion(numeroCompensacion).ToList();

            ViewBag.pagos = data;

            var id = Convert.ToInt32(System.Web.HttpContext.Current.Session["proveedorId"]);

            ViewBag.proveedor = _proveedorManager.Find(id);

            return View();
        }


        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult PagosPendientes(int proveedorId = 0)
        {
            int id = proveedorId > 0 ? proveedorId : Convert.ToInt32(System.Web.HttpContext.Current.Session["proveedorId"]);

            if (id == 0)
                return RedirectToAction("Index");
            Entities db = new Entities();
            string[] tiposMovimientos = new string[] { "10", "21", "4" };

            ViewBag.proveedor = _proveedorManager.Find(id);
            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult PagosPendientesDetalle(int proveedorId)
        {
            Entities db = new Entities();

            string[] tiposMovimientos = new string[] { "10", "21", "4" };



            ViewBag.proveedor = _proveedorManager.Find(proveedorId);
            return View();
        }

        public string PersitedProviderId(string proveedorId)
        {
            int result = 0;
            if (int.TryParse(proveedorId, out result))
                System.Web.HttpContext.Current.Session["proveedorId"] = result;

            System.Web.HttpContext.Current.Session["razonsocial"] = _proveedorManager.Find(result).Rfc;

            return System.Web.HttpContext.Current.Session["proveedorId"].ToString();
        }

        public void DevolucionesDetalleDescargar(int id)
        {
            var commonManager = new CommonManager();
            MySqlParameter[] parametes = {
                    new MySqlParameter("id", id)
                };
            const string sql = @"
				SELECT * 
				FROM   devoluciones d
				WHERE  d.Id = @id";
            var data = commonManager.QueryToTable(sql, parametes).Rows[0];

            var cantidad = int.Parse(data["cantidad"].ToString());

            var dt = new DataTable();

            dt.Columns.Add(new DataColumn("Articulo"));
            dt.Columns.Add(new DataColumn("Descripcion"));
            dt.Columns.Add(new DataColumn("Total"));
            dt.Columns.Add(new DataColumn("Cantidad"));

            while (cantidad > 0)
            {

                dt.Rows.Add(new[] { data["Material"], data["Descripcion"], 230.5, 1 });


                cantidad--;
            }
            ExportExcel(dt, id.ToString());
        }

        public void ExportExcel(DataTable dt, string nombreXls)
        {

            var grid = new GridView();
            grid.DataSource = dt;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + nombreXls + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return;

        }

        public CuentasPagarController()
        {
            if (!(Convert.ToInt32(System.Web.HttpContext.Current.Session["proveedorId"]) > 0))
                System.Web.HttpContext.Current.Session["proveedorId"] = 0;
        }

        public void Descargar(int id)
        {
            var commonManager = new CommonManager();

            MySqlParameter[] parametes = {
                    new MySqlParameter("id", id)
                };

            const string sql = @"
			SELECT * 
			FROM   cuentasxpagar 
			WHERE TipoMovimiento in(10,21,4) AND ProveedoresId = @id;";

            var dt = commonManager.QueryToTable(sql, parametes);

            ExportExcel(dt, id.ToString());

        }

    }
}*/

using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ppgz.Repository;
using Ppgz.Services;
using Ppgz.Web.Infrastructure;

namespace Ppgz.Web.Areas.Mercaderia.Controllers
{
    [Authorize]
    [TerminosCondiciones]
    public class CuentasPagarController : Controller
    {

        readonly CommonManager _commonManager = new CommonManager();
        readonly ProveedorManager _proveedorManager = new ProveedorManager();

        internal proveedore ProveedorCxp
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["proveedorcxp"] != null)
                {
                    return (proveedore)System.Web.HttpContext.Current.Session["proveedorcxp"];
                }
                return null;
            }
            set
            {
                System.Web.HttpContext.Current.Session["proveedorcxp"] = value;
            }
        }

        // GET: /Mercaderia/CuentasPagar/
        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult Index()
        {
            var cuenta = _commonManager.GetCuentaUsuarioAutenticado();

            ViewBag.proveedores = _proveedorManager.FindByCuentaId(cuenta.Id);

            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult SeleccionarProveedor(int proveedorId)
        {

            var cuenta = _commonManager.GetCuentaUsuarioAutenticado();

            var proveedor = _proveedorManager.Find(proveedorId, cuenta.Id);

            if (proveedor == null)
            {
                // TODO pasar a recurso
                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }

            ProveedorCxp = proveedor;
            return RedirectToAction("Pagos");
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult Pagos()
        {
            if (ProveedorCxp == null)
            {
                // TODO pasar a recurso
                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }

            var partidasManager = new PartidasManager();

            var dsPagos = partidasManager.GetPagos(ProveedorCxp.NumeroProveedor);

            ViewBag.Pagos = dsPagos.Tables["T_LISTA_PAGOS"];

            ViewBag.Proveedor = ProveedorCxp;

            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult PagosDetalle(string numeroDocumento)
        {
            if (ProveedorCxp == null)
            {
                // TODO pasar a recurso
                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }

            var partidasManager = new PartidasManager();

            var dsPagos = partidasManager.GetPagos(ProveedorCxp.NumeroProveedor);

            ViewBag.Pago = dsPagos.Tables["T_LISTA_PAGOS"]
                .Select(string.Format("BELNR = '{0}'", numeroDocumento))[0];

            ViewBag.PagoDetalles = dsPagos.Tables["T_PAGOS"]
                .Select(string.Format("BELNR_PAGO = '{0}'", numeroDocumento));

            ViewBag.Proveedor = ProveedorCxp;

            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public void PagoDetallesDescargar(string numeroDocumento)
        {
            if (ProveedorCxp == null)
            {
                return;
            }

            var partidasManager = new PartidasManager();

            var dsPagos = partidasManager.GetPagos(ProveedorCxp.NumeroProveedor);

            var dt = dsPagos.Tables["T_PAGOS"]
                .Select(string.Format("BELNR_PAGO = '{0}'", numeroDocumento)).CopyToDataTable();

            var columnsNames = new[]
            {        
                "BELNR_COMPEN","DMBTR_COMPEN","WAERS_COMPEN",  "BLART_COMPEN"
            };

            foreach (DataRow dr in dt.Rows)
            {




                var tipo = "";
                switch (dr["BLART_COMPEN"].ToString())
                {
                    case "4":
                        tipo = "Cargo a proveedor";
                        break;
                    case "10":
                        tipo = "Factura a proveedor";
                        break;
                    case "21":
                        tipo = "Factura a proveedor";
                        break;
                    case "23":
                        tipo = "Nota de Cargo";
                        break;
                    case "AB":
                        tipo = "Cargo a proveedor";
                        break;
                    case "DA":
                        tipo = "Cargo a proveedor";
                        break;
                    case "DG":
                        tipo = "Cargo a proveedor";
                        break;
                    case "DZ":
                        tipo = "Pago de proveedor";
                        break;
                    case "KA":
                        tipo = "Cargo a proveedor";
                        break;
                    case "KG":
                        tipo = "Cargo a proveedor";
                        break;
                    case "KR":
                        tipo = "Devolución";
                        break;
                    case "":
                        tipo = "Cargo a proveedor";
                        break;
                    case "RE":
                        tipo = "Factura a proveedor";
                        break;

                    case "RV":
                        tipo = "Cargo a proveedor";
                        break;
                    case "SA":
                        tipo = "Cargo a proveedor";
                        break;
                    case "ZN":
                        tipo = "Anulación de documento";
                        break;
                    case "ZP":
                        tipo = "Pago";
                        break;


                }


                dr["BLART_COMPEN"] = tipo;
            }

            for (var i = dt.Columns.Count - 1; i >= 0; i--)
            {
                if (!columnsNames.Contains(dt.Columns[i].ColumnName))
                {
                    dt.Columns.RemoveAt(i);
                }
            }

            dt.Columns["BELNR_COMPEN"].ColumnName = "Referencia";
            dt.Columns["DMBTR_COMPEN"].ColumnName = "Importe";
            dt.Columns["WAERS_COMPEN"].ColumnName = "Ml";
            dt.Columns["BLART_COMPEN"].ColumnName = "Tipo de Movimiento";

            FileManager.ExportExcel(dt,"RefdPago" + numeroDocumento, HttpContext);
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult Devoluciones()
        {
            if (ProveedorCxp == null)
            {
                // TODO pasar a recurso
                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }

            var partidasManager = new PartidasManager();

            var dsDevoluciones = partidasManager.GetDevoluciones(ProveedorCxp.NumeroProveedor);

            ViewBag.Devoluciones = dsDevoluciones.Tables["T_DEVOLUCIONES"];

            ViewBag.Proveedor = ProveedorCxp;

            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult DevolucionesDetalle(string numeroDocumento)
        {
            if (ProveedorCxp == null)
            {
                // TODO pasar a recurso
                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }

            var partidasManager = new PartidasManager();

            var dsDevoluciones = partidasManager.GetDevoluciones(ProveedorCxp.NumeroProveedor);

            ViewBag.Devolucion = dsDevoluciones.Tables["T_DEVOLUCIONES"]
                .Select(string.Format("BELNR = '{0}'", numeroDocumento))[0];

            ViewBag.DevolucionDetalles = dsDevoluciones.Tables["T_MAT_DEV"]
                .Select(string.Format("BELNR = '{0}' AND  BUZID = 'W'", numeroDocumento));

            var drsImpuesto = dsDevoluciones.Tables["T_MAT_DEV"]
                .Select(string.Format("BELNR = '{0}' AND  BUZID = 'T'", numeroDocumento));

            Decimal impuesto = 0;
            if (drsImpuesto.Any())
            {
                impuesto = drsImpuesto.Aggregate(
                    impuesto, (current, dr) => current + decimal.Parse(dr["DMBTR"].ToString()));
            }
            ViewBag.Impuesto = impuesto;

            ViewBag.Proveedor = ProveedorCxp;

            return View();
        }


        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public void DevolucionesDetalleDescargar(string numeroDocumento)
        {
            if (ProveedorCxp == null)
            {
                return;
            }

            var partidasManager = new PartidasManager();

            var dsDevoluciones = partidasManager.GetDevoluciones(ProveedorCxp.NumeroProveedor);

            var dt = dsDevoluciones.Tables["T_MAT_DEV"]
                .Select(string.Format("BELNR = '{0}'", numeroDocumento)).CopyToDataTable();

            var columnsNames = new[]
            {        
                "EBELN","MATNR","MAKTX","DMBTR","MENGE"
            };

            for (var i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i]["BUZID"].ToString() == "T")
                {
                    dt.Rows.RemoveAt(i);
                }
            }

            for (var i = dt.Columns.Count - 1; i >= 0; i--)
            {
                if (!columnsNames.Contains(dt.Columns[i].ColumnName))
                {
                    dt.Columns.RemoveAt(i);
                }
            }



            dt.Columns["EBELN"].ColumnName = "Documento";
            dt.Columns["MATNR"].ColumnName = "Artículo";
            dt.Columns["MAKTX"].ColumnName = "Descripción";
            dt.Columns["DMBTR"].ColumnName = "Total";
            dt.Columns["MENGE"].ColumnName = "Cantidad";

            FileManager.ExportExcel(dt, numeroDocumento, HttpContext);
        }







        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult PagosPendientes()
        {
            if (ProveedorCxp == null)
            {
                // TODO pasar a recurso
                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }

            var partidasManager = new PartidasManager();

            var dsPagosPendientes = partidasManager.GetPartidasAbiertas(ProveedorCxp.NumeroProveedor);

            ViewBag.PagosPendientes = dsPagosPendientes.Tables["T_PARTIDAS_ABIERTAS"];

            ViewBag.Proveedor = ProveedorCxp;

            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public ActionResult PagosPendientesDetalle(int proveedorId)
        {
            if (ProveedorCxp == null)
            {
                // TODO pasar a recurso
                TempData["FlashError"] = "Proveedor incorrecto";
                return RedirectToAction("Index");
            }

            var partidasManager = new PartidasManager();

            var dsPagosPendientes = partidasManager.GetPartidasAbiertas(ProveedorCxp.NumeroProveedor);

            ViewBag.PagosPendientes = dsPagosPendientes.Tables["T_PARTIDAS_ABIERTAS"];

            ViewBag.Proveedor = ProveedorCxp;

            return View();
        }


        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-CUENTASPAGAR")]
        public void PagosPendientesDescargar()
        {
            if (ProveedorCxp == null)
            {
                return;
            }

            var partidasManager = new PartidasManager();

            var dsPagosPendientes = partidasManager.GetPartidasAbiertas(ProveedorCxp.NumeroProveedor);

            var dt = dsPagosPendientes.Tables["T_PARTIDAS_ABIERTAS"];

            var columnsNames = new[]
            {        
                "BELNR","DMBTR","WAERS","BLART","FECHA_PAGO"
            };

            foreach (DataRow dr in dt.Rows)
            {
                dr["FECHA_PAGO"] = DateTime.ParseExact(dr["FECHA_PAGO"].ToString(), "yyyyMMdd",
                CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");

                var tipo = "";
                switch (dr["BLART"].ToString())
                {
                    case "4":
                        tipo = "Cargo a proveedor";
                        break;
                    case "10":
                        tipo = "Factura a proveedor";
                        break;
                    case "21":
                        tipo = "Factura a proveedor";
                        break;
                    case "23":
                        tipo = "Nota de Cargo";
                        break;
                    case "AB":
                        tipo = "Cargo a proveedor";
                        break;
                    case "DA":
                        tipo = "Cargo a proveedor";
                        break;
                    case "DG":
                        tipo = "Cargo a proveedor";
                        break;
                    case "DZ":
                        tipo = "Pago de proveedor";
                        break;
                    case "KA":
                        tipo = "Cargo a proveedor";
                        break;
                    case "KG":
                        tipo = "Cargo a proveedor";
                        break;
                    case "KR":
                        tipo = "Devolución";
                        break;
                    case "":
                        tipo = "Cargo a proveedor";
                        break;
                    case "RE":
                        tipo = "Factura a proveedor";
                        break;

                    case "RV":
                        tipo = "Cargo a proveedor";
                        break;
                    case "SA":
                        tipo = "Cargo a proveedor";
                        break;
                    case "ZN":
                        tipo = "Anulación de documento";
                        break;
                    case "ZP":
                        tipo = "Pago";
                        break;


                }

                dr["BLART"] = tipo;
            }

            for (var i = dt.Columns.Count - 1; i >= 0; i--)
            {
                if (!columnsNames.Contains(dt.Columns[i].ColumnName))
                {
                    dt.Columns.RemoveAt(i);
                }
            }

            dt.Columns["BELNR"].ColumnName = "Referencia";
            dt.Columns["DMBTR"].ColumnName = "Importe";
            dt.Columns["WAERS"].ColumnName = "Ml";
            dt.Columns["BLART"].ColumnName = "Tipo de Movimiento";

           FileManager.ExportExcel(dt, "NroSAP" + ProveedorCxp.NumeroProveedor, HttpContext);
        }


    }
}