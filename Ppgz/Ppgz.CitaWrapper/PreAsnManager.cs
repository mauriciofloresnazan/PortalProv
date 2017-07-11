﻿using System;
using System.Collections.Generic;
using System.Linq;
using SapWrapper;

namespace Ppgz.CitaWrapper
{
    public class PreAsnManager
    {

        public List<PreAsn> GetOrdenesActivasConDetalle(int proveedorId)
        {
            var db = new Repository.Entities();
            var proveedor = db.proveedores.Single(p => p.Id == proveedorId);

            var organizacionCompras = db.configuraciones.Single(c => c.Clave == "rfc.common.function.param.ekorg.mercaderia").Valor;

            var sapOrdenCompraManager = new SapOrdenCompraManager();
            var ordenesSap = sapOrdenCompraManager.GetActivasConDetalle(proveedor.NumeroProveedor, organizacionCompras);

            // TODO MEJORAR Limpieza de las ordenes de acuerdo a su fecha de entrega
            var ordenes = ordenesSap.Where(o => o.FechaEntrega > DateTime.Today.AddDays(-22)).ToList();

            // TODO POSIBLE CARGA DE DATOS INCORRECTA A CAUSA DE LA SINCORNIZACIÓN DE LA RECEPCIÓN CON SAP
            var asnFuturos = db.asns
                .Where(asn => asn.cita.ProveedorId == proveedor.Id && asn.cita.FechaCita > DateTime.Today).ToList();

            var result = new List<PreAsn>();

            foreach (var orden in ordenes)
            {
                var preAsn = new PreAsn
                {
                    FechaEntrega = orden.FechaEntrega,
                    NumeroDocumento = orden.NumeroDocumento,
                    NumeroProveedor = orden.NumeroProveedor,
                    FechasPermitidas = RulesManager.GetFechasPermitidas(orden.FechaEntrega, proveedor.cuenta.EsEspecial),

                    EsCrossDock = orden.CrossD.ToUpper() == "X",
                    Tienda = orden.TiDest,
                    TiendaOrigen = orden.TiOrig,
                    InOut = orden.InOut,
                    NumeroOrdenSurtido = orden.NumOs,
                    Centro = orden.Centro
                   
                };

                var detalles = orden.Detalles.Select(detalle => new PreAsnDetalle
                {
                    NumeroPosicion = detalle.NumeroPosicion, 
                    Centro = detalle.Centro, 
                    Almacen = detalle.Almacen, 
                    CantidadCitasFuturas = asnFuturos
                        .Where(asn => asn.OrdenNumeroDocumento == detalle.NumeroDocumento 
                               && asn.NumeroPosicion == detalle.NumeroPosicion 
                               && asn.NumeroMaterial == detalle.NumeroMaterial)
                        .Sum(asn => asn.Cantidad), 
                    CantidadPermitidaSap = detalle.CantidadPorEntregar, 
                    CantidadPedido = detalle.CantidadPedido, 
                    DescripcionMaterial = detalle.Descripcion, 
                    NumeroMaterial = detalle.NumeroMaterial,
                    NumeroMaterial2 = detalle.NumeroMaterial2,
                    Precio = detalle.ValorNeto,
                    UnidadMedida = detalle.UnidadMedidaPedido,

                    Cantidad = detalle.CantidadPorEntregar - asnFuturos
                        .Where(asn => asn.OrdenNumeroDocumento == detalle.NumeroDocumento
                               && asn.NumeroPosicion == detalle.NumeroPosicion
                               && asn.NumeroMaterial == detalle.NumeroMaterial)
                        .Sum(asn => asn.Cantidad)}).ToList();

                preAsn.Detalles = detalles;
                result.Add(preAsn);
            }


            return result.Any() ? result.Where(o => o.FechasPermitidas.Count > 0).ToList() : result;
        }

    }
}
