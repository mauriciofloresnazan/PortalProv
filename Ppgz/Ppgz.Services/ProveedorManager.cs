﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ppgz.Repository;
using SapWrapper;

namespace Ppgz.Services
{
    public class ProveedorManager
    {
        private readonly Entities _db = new Entities();

        public List<proveedore> FindByCuentaId(int cuentaId)
        {
            return _db.proveedores.Where(p => p.CuentaId == cuentaId).ToList();
        }

        public proveedore Find(int id)
        {
            return _db.proveedores.Find(id);
        }

        public proveedore FindByNumeroProveedor(string numeroProveedor)
        {
            return _db.proveedores.FirstOrDefault(p => p.NumeroProveedor == numeroProveedor);
        }

        public proveedore FindProveedorEnSap(string numeroProveedor)
        {

            var sapproveedorManager = new SapProveedorManager();
            var result = sapproveedorManager.GetProveedor(numeroProveedor);
            if (result == null)
            {
                throw new BusinessException(CommonMensajesResource.ERRO_Sap_ProveedorCodigo);
            }

            try
            {
                var proveedor = new proveedore
                {
                    NumeroProveedor = result["LIFNR"].ToString(),
                    ClavePais = result["LAND1"].ToString(),
                    Nombre1 = result["NAME1"].ToString(),
                    Nombre2 = result["NAME2"].ToString(),
                    Nombre3 = result["NAME3"].ToString(),
                    Nombre4 = result["NAME4"].ToString(),
                    Poblacion = result["ORT01"].ToString(),
                    Distrito = result["ORT02"].ToString(),
                    Apartado = result["PFACH"].ToString(),
                    CodigoApartado = result["PSTL2"].ToString(),
                    CodigoPostal = result["PSTLZ"].ToString(),
                    Region = result["REGIO"].ToString(),
                    Calle = result["STRAS"].ToString(),
                    Direccion = result["ADRNR"].ToString(),
                    Sociedad = result["BUKRS"].ToString(),
                    OrganizacionCompra = result["EKORG"].ToString(),
                    ClaveMoned = result["WAERS"].ToString(),
                    VendedorResponsable = result["VERKF"].ToString(),
                    NumeroTelefono = result["TELF1"].ToString(),
                    CondicionPago = result["ZTERM"].ToString(),
                    IncoTerminos1 = result["INCO1"].ToString(),
                    IncoTerminos2 = result["INCO2"].ToString(),
                    GrupoCompras = result["EKGRP"].ToString(),
                    DenominacionGrupo = result["EKNAM"].ToString(),
                    TelefonoGrupoCompra = result["EKTEL"].ToString(),
                    TelefonoPrefijo = result["TEL_NUMER"].ToString(),
                    TelefonoExtension = result["TEL_EXTENS"].ToString(),
                    Correo = result["SMTP_ADDR"].ToString()
                };

                return proveedor;
            }
            catch (NullReferenceException nullReferenceException)
            {
                // TODO
                throw;
            }


        }

        public void Eliminar(int id)
        {
            var proveedor = Find(id);

            if (proveedor == null)
            {
                throw new BusinessException(CommonMensajesResource.ERROR_Proveedor_Id);
            }

            //TODO VALIDR LAS CITAS


            _db.proveedores.Remove(proveedor);
            _db.SaveChanges();
        }

    }
}