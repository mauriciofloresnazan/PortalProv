﻿using System;
using System.Linq;
using System.Web.Mvc;
using Ppgz.Repository;
using Ppgz.Services;
using Ppgz.Web.Models;

namespace Ppgz.Web.Controllers
{
    [Authorize]
    public class NazanGestionProveedoresController : Controller
    {
        readonly CuentaManager _cuentaManager = new CuentaManager();
        readonly TipoProveedorManager _tipoProveedorManager = new TipoProveedorManager();
        readonly UsuarioManager _usuarioManager = new UsuarioManager();
        readonly TipoUsuarioManager _tipoUsuarioManager = new TipoUsuarioManager();
        readonly CommonManager _commonManager = new CommonManager();
        //
        // GET: /NazanGestionProveedores/
        public ActionResult Index()
        {
            var proveedores = _cuentaManager.FinAll();

            var proveedorMercaderia = _tipoProveedorManager.GetByCodigo("MERCADERIA");
            var proveedorServicio = _tipoProveedorManager.GetByCodigo("SERVICIO");

            ViewBag.proveedorMercaderia = proveedorMercaderia;
            ViewBag.proveedorServicio = proveedorServicio;
            ViewBag.proveedores = proveedores;

            return View();
        }

        // GET: /NazanGestionProveedores/
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(CuentaViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (_usuarioManager.FindUsuarioByUserName(model.UserName) != null)
                {
                    ModelState.AddModelError(string.Empty, "Este nombre de usuario ya fue utilizado. Por favor intente con otro");
                    return View(model);
                }

                _tipoUsuarioManager.GetNazan();

                var tipoProveedor = _tipoProveedorManager.GetByCodigo(model.TipoProveedor);

                var usuario = new usuario()
                {
                    userName = model.UserName,
                    nombre = model.ResponsableNombre,
                    apellido = model.ResponsableApellido,
                    cargo = model.ResponsableCargo,
                    email = model.ResponsableEmail,
                    telefono = model.ResponsableTelefono,
                    PasswordHash = model.ResponsablePassword,
                    SecurityStamp = model.ResponsablePassword,
                    tipo_usuario_id = _tipoUsuarioManager.GetNazan().id,
                };

                _usuarioManager.Add(usuario);


                var cuenta = new cuenta
                {
                    nombre_proveedor = model.ProveedorNombre,

                    codigo_proveedor = DateTime.Now.ToString("yyyyMMddHHmmssf"),
                    reponsable_usuario_id = usuario.Id,
                    tipo_proveedor_id = tipoProveedor.id,
                    activo = true

                };

                _cuentaManager.Add(cuenta);

                var xref = new usuarios_cuentas_xref
                {
                    usuario_id = usuario.Id,
                    cuenta_id = cuenta.id
                };


                _commonManager.UsuarioCuentaXrefAdd(xref);
                return RedirectToAction("Index");
            }

            return View(model);

        }
    }
}