using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Ppgz.Repository;
using SapWrapper;

namespace Ppgz.Services
{
    public class ReporteProveedorManager
    {
        private readonly Entities _db = new Entities();
        public class ReporteProveedor
        {
            public string NumeroProveedor { get; set; }
            public string NombreProveedor { get; set; }
            public string Material { get; set; }
            public string NombreMaterial { get; set; }
            public DateTime FechaProceso { get; set; }
            public string UnidadMedida { get; set; }
            public string CantidadVentas2 { get; set; }
            public string CantidadVentas1 { get; set; }
            public string CantidadVentas { get; set; }
            public string CantidadTotal { get; set; }
            public string CalculoTotal { get; set; }
            public string InvTienda { get; set; }
            public string InvTransito { get; set; }
            public string InvCedis { get; set; }
            public string PedidosPendiente { get; set; }
            public string EstadoMaterial { get; set; }

        }


        private string QuitarDecimales( string valor)
        {
            string[] sindecimal = valor.Split(".".ToCharArray());
            return sindecimal[0];
        }


        public List<ReporteProveedor> FindReporteProveedor(string codigoProveedor)
        {

            var bwreporteproveedorManager = new BwReporteProveedorManager();
            var result = bwreporteproveedorManager.GetReporteProveedor(codigoProveedor);

            var reporteProveedor = new List<ReporteProveedor>();

            if (result.Rows.Count > 0)
            {
                var statusValidos = new [] {"S1", "S2", "S3", "S6"};

                

                reporteProveedor = (from DataRow dr in result.Rows
                    where statusValidos.Contains(dr["ZMMSTA"].ToString().ToUpper())
                    select new ReporteProveedor
                    {
                        NumeroProveedor = dr["VENDOR"].ToString(),
                        NombreProveedor = dr["VENDOR_TXT"].ToString(),
                        Material = dr["MATERIAL"].ToString(),
                        NombreMaterial = dr["MATERIAL_TXT"].ToString(),
                        FechaProceso = DateTime.ParseExact(dr["CALDAY"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture),
                        UnidadMedida = dr["BASE_UOM"].ToString(),
                        CantidadVentas2 = QuitarDecimales(dr["MESACT2"].ToString()),
                        CantidadVentas1 = QuitarDecimales(dr["MESACT1"].ToString()),
                        CantidadVentas = QuitarDecimales(dr["MESACT"].ToString()),
                        CantidadTotal = QuitarDecimales(dr["TOTAL"].ToString()),
                        CalculoTotal = dr["SELLTHRU"] + " %",
                        InvTienda = QuitarDecimales(dr["INVENTDA"].ToString()),
                        InvTransito = QuitarDecimales(dr["TRANSITO"].ToString()),
                        InvCedis = QuitarDecimales(dr["INVENCEDIS"].ToString()),
                        PedidosPendiente = QuitarDecimales(dr["PEDIDOSPEN"].ToString()),
                        EstadoMaterial = dr["ZMMSTA"].ToString()
                    }).ToList();
                
                /*reporteProveedor.AddRange(from DataRow dr in result.Rows
                    select new ReporteProveedor
                    {
                        NumeroProveedor = dr["VENDOR"].ToString(),
                        NombreProveedor = dr["VENDOR_TXT"].ToString(),
                        Material = dr["MATERIAL"].ToString(),
                        NombreMaterial = dr["MATERIAL_TXT"].ToString(),
                        FechaProceso = DateTime.ParseExact(dr["CALDAY"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture),
                        UnidadMedida = dr["BASE_UOM"].ToString(),
                        CantidadVentas2 = dr["MESACT2"].ToString(),
                        CantidadVentas1 = dr["MESACT1"].ToString(),
                        CantidadVentas = dr["MESACT"].ToString(),
                        CantidadTotal = dr["TOTAL"].ToString(),
                        CalculoTotal = dr["SELLTHRU"] + " %",
                        InvTienda = dr["INVENTDA"].ToString(),
                        InvTransito = dr["TRANSITO"].ToString(),
                        InvCedis = dr["INVENCEDIS"].ToString(),
                        PedidosPendiente = dr["PEDIDOSPEN"].ToString(),
                        EstadoMaterial = dr["ZMMSTA"].ToString()
                    });*/
            }
            return reporteProveedor;

        }

        public void CrearNivelServicio(string numeroProveedor, decimal ultimoMes, decimal temporadaActual,
           decimal acumuladoAnual, decimal pedidoAtrasado, decimal pedidoEntiempo, decimal pedidoTotal)
        {
            const char pad = '0';
            var proveedorManager = new ProveedorManager();
            var proveedor = proveedorManager.FindByNumeroProveedor(numeroProveedor.PadLeft(10, pad));

            if (proveedor == null)
            {
                throw new BusinessException(CommonMensajesResource.ERROR_Proveedor_Id);
            }

            var nivelServicio = _db.niveleseervicios.FirstOrDefault(n => n.ProveedorId == proveedor.Id);

            if (nivelServicio == null)
            {
                nivelServicio = new niveleseervicio
                {
                    ProveedorId = proveedor.Id,
                    UltimoMes = ultimoMes,
                    TemporadaActual = temporadaActual,
                    AcumuladoAnual = acumuladoAnual,
                    PedidoAtrasado = pedidoAtrasado,
                    PedidoEnTiempo = pedidoEntiempo,
                    PedidoTotal = pedidoTotal,
                    Fecha = DateTime.Now

                };

                _db.niveleseervicios.Add(nivelServicio);
            }
            else
            {
                nivelServicio.ProveedorId = proveedor.Id;
                nivelServicio.UltimoMes = ultimoMes;
                nivelServicio.TemporadaActual = temporadaActual;
                nivelServicio.AcumuladoAnual = acumuladoAnual;
                nivelServicio.PedidoAtrasado = pedidoAtrasado;
                nivelServicio.PedidoEnTiempo = pedidoEntiempo;
                nivelServicio.PedidoTotal = pedidoTotal;
                nivelServicio.Fecha = DateTime.Now;
                _db.Entry(nivelServicio).State =  EntityState.Modified;
            }
            _db.SaveChanges();

        }

        public niveleseervicio FindNivelSerNiveleseervicio(int proveedorId)
        {
            var proveedorManager = new ProveedorManager();
            var proveedor = proveedorManager.Find(proveedorId);

            if (proveedor == null)
            {
                throw new BusinessException(CommonMensajesResource.ERROR_Proveedor_Id);
            }

           return _db.niveleseervicios.FirstOrDefault(n => n.ProveedorId == proveedor.Id);
            
        }

        public niveleseervicio LastNivleServicio()
        {
            return _db.niveleseervicios.OrderBy(n=> n.Fecha).FirstOrDefault();
        }

    }
}
