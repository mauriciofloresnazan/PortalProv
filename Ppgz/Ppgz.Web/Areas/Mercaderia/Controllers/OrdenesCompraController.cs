﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using MySql.Data.MySqlClient;
using Ppgz.Repository;
using Ppgz.Web.Infrastructure;
using Ppgz.Web.Infrastructure.Nazan;
using Ppgz.Web.Infrastructure.Proveedor;

namespace Ppgz.Web.Areas.Mercaderia.Controllers
{
    [Authorize]
    [TerminosCondiciones]
    public class OrdenesCompraController : Controller
    {
        private readonly OrdenCompraManager  _ordenCompraManager = new OrdenCompraManager();


        //
        // GET: /Mercaderia/OrdenesCompra/
        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-ORDENESCOMPRA-LISTAR,MERCADERIA-ORDENESCOMPRA-MODIFICAR")]
        //
        // GET: /Servicio/OrdenesCompra/
        public ActionResult Index()
        {
            CommonManager commonManager =  new CommonManager();

            var cuenta = commonManager.GetCuentaUsuarioAutenticado();




           ViewBag.data= _ordenCompraManager.FindByCuentaId(cuenta.Id);
            //var perfiles = _perfilProveedorManager
            //    .FindByCuentaId(_commonManager.GetCuentaUsuarioAutenticado().Id);

            //perfiles.Add(_perfilProveedorManager.GetMaestroByUsuarioTipo(CuentaManager.Tipo.MERCADERIA));

            //ViewBag.Perfiles = perfiles;

            
            /*DataTable dt = crearDt();

            exportExcel(dt, "T_PROV_001");*/
            
            return View();
        }

        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-ORDENESCOMPRA-LISTAR,MERCADERIA-ORDENESCOMPRA-MODIFICAR")]
        //
        // GET: /Servicio/OrdenesCompra/
        public void Descargar(int id)
        {            //todo pasar a un manejador

            Entities db = new Entities();
            var orden = db.ordencompras.Find(id);
            orden.FechaVisualizado = DateTime.Today;
            db.Entry(orden).State = EntityState.Modified;
            db.SaveChanges();



            var commonManager = new CommonManager();


            MySqlParameter[] parametes = {
                    new MySqlParameter("id", id)
                };


            const string sql = @"
            SELECT * 
            FROM   detalleordencompra
            WHERE  OrdenComprasId = @id;";

            var dt = commonManager.QueryToTable(sql, parametes);

            ExportExcel(dt, id.ToString());

            
        }


        [Authorize(Roles = "MAESTRO-MERCADERIA,MERCADERIA-ORDENESCOMPRA-LISTAR,MERCADERIA-ORDENESCOMPRA-MODIFICAR")]
        [HttpPost]
        public ActionResult Visualizar(int id)
        {




            //var mensajes = _mensajesInstitucionalesManager.FindUsuarioMensajes(User.Identity.GetUserId());

            //if (mensajes.Any(i => i.MensajeId == id))
            //{
            //    return Content("Actualizado"); ;
            //}
            //_mensajesInstitucionalesManager.Visualizar(User.Identity.GetUserId(), id);
            return Content("Actualizado");
        }

        public DataTable crearDt()
        {
            DataTable dt = new DataTable();

            //agrego las columnas
            dt.Columns.Add("NUmeroLinea");
            dt.Columns.Add("Almacen");
            dt.Columns.Add("Item");
            dt.Columns.Add("Descripcion");
            dt.Columns.Add("CantidadTotal");
            dt.Columns.Add("UnidadMedida");
            dt.Columns.Add("Color");
            dt.Columns.Add("Estilo");
            dt.Columns.Add("USER_DEF6");
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Categoria");
            dt.Columns.Add("Categoria2");
            dt.Columns.Add("OrdenCompra");
            dt.Columns.Add("warehouse");
            dt.Columns.Add("COMPANY");
            dt.Columns.Add("FehchaOrden");

            //agrego algo de data
            DataRow dr = dt.NewRow();
            dr["NUmeroLinea"] = "1";
            dr["Almacen"] = "1";
            dr["Item"] = "1";
            dr["Descripcion"] = "1";
            dr["CantidadTotal"] = "1";
            dr["UnidadMedida"] = "1";
            dr["Color"] = "1";
            dr["Estilo"] = "1";
            dr["USER_DEF6"] = "1";
            dr["Fecha"] = "1";
            dr["Categoria"] = "1";
            dr["Categoria2"] = "1";
            dr["OrdenCompra"] = "1";
            dr["warehouse"] = "1";
            dr["COMPANY"] = "1";
            dr["FehchaOrden"] = "1";
            dt.Rows.Add(dr);

            return dt;
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

            StreamWriter wr = new StreamWriter(@"c:\\temp\" + nombreXls + ".xls");
            try
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    wr.Write(dt.Columns[i].ToString().ToUpper() + "\t");
                }
                wr.WriteLine();

                //write rows to excel file
                for (int i = 0; i < (dt.Rows.Count); i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i][j] != null)
                        {
                            wr.Write(Convert.ToString(dt.Rows[i][j]) + "\t");
                        }
                        else
                        {
                            wr.Write("\t");
                        }
                    }
                    //go to next line
                    wr.WriteLine();
                }
                //close file
                wr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}