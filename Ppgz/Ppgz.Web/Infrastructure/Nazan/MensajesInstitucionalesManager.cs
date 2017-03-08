﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ppgz.Repository;
using Ppgz.Web.Areas.Nazan;

namespace Ppgz.Web.Infrastructure.Nazan
{
    public class MensajesInstitucionalesManager
    {
        private readonly Entities _db = new Entities();

        /// <summary>
        /// Representa a quienes fue enviado el Mensaje
        /// </summary>
        public enum EnviadoA
        {
            Todos,
            Mercaderia,
            Servicio
        }

        internal static string GetEnviadoAString(EnviadoA enviadoA)
        {
            switch (enviadoA)
            {
                case EnviadoA.Mercaderia:
                    return "MERCADERIA";
                case EnviadoA.Servicio:
                    return "SERVICIO";
                case EnviadoA.Todos:
                    return "TODOS";
            }
            return null;
        }

        public static EnviadoA GetEnviadoAByString(string enviadoA)
        {
            switch (enviadoA)
            {
                case "MERCADERIA":
                    return EnviadoA.Mercaderia;
                case "SERVICIO":
                    return EnviadoA.Servicio;
                case "TODOS":
                    return EnviadoA.Todos;
            }

            throw new BusinessException(MensajesResource.ERROR_MensajesInstitucionalesManager_GetEnviadoAByString);

        }

        public List<mensaje> FindAll()
        {
            return _db.mensajes.ToList();
        }

        public mensaje Find(int id)
        {
            return _db.mensajes
                .FirstOrDefault(m => m.Id == id);
        }

        public List<mensaje> FindPublicadosByCuentaId(int cuentaId)
        {
            var cuentaManager = new CuentaManager();

            var cuenta = cuentaManager.Find(cuentaId);

            return _db.mensajes
                .Where(m => m.FechaPublicacion < DateTime.Now && (m.EnviadoA == "TODOS" || m.EnviadoA == cuenta.Tipo))
                .ToList();
        }

        public List<cuentasmensaje> FindCuentaMensajes(int cuentaId)
        {
            return _db.cuentasmensajes.Where(cm => cm.CuentaId == cuentaId).ToList();
        }

        internal void ValidarFechas(DateTime fechaCaducidad, DateTime fechaPublicacion)
        {
            // validacion de las fechas
            if (fechaCaducidad <= fechaPublicacion)
            {
                throw new BusinessException(MensajesResource.ERROR_MensajesInstitucionalesManager_Crear_FechaCadMayor);
            }
            if (fechaPublicacion < DateTime.Today)
            {
                throw new BusinessException(MensajesResource.ERROR_MensajesInstitucionales_FechaPublicacion);
            }
        }

        internal void ValidarMensajeId(int id)
        {
            var mensaje = Find(id);

            if (mensaje == null)
            {
                throw new BusinessException(MensajesResource.ERROR_MensajesInstitucionales_MensajeNoExiste);
            }
        }

        internal void ValidarPdf(string pdf)
        {
            if (string.IsNullOrWhiteSpace(pdf))
            {
                throw new BusinessException(MensajesResource.ERROR_MensajesInstitucionalesManager_PdfIncorrecto);
            }
            
        }
        internal void ValidarContenido(string contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                throw new BusinessException(MensajesResource.ERROR_MensajesInstitucionalesManager_ContenidoIncorrecto);
            }
            //TODO ESTO DEBE SER CONFIGURADO
            if (contenido.Length < 20)
            {
                throw new BusinessException(MensajesResource.ERROR_MensajesInstitucionalesManager_ContenidoIncorrecto);
            }

        }

        /// <summary>
        /// Crear mensaje de tipo texto en el contenido
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="contenido"></param>
        /// <param name="fechaPublicacion"></param>
        /// <param name="fechaCaducidad"></param>
        /// <param name="enviadoA"></param>
        public void CrearTexto(string titulo, string contenido,
            DateTime fechaPublicacion, DateTime fechaCaducidad, EnviadoA enviadoA)
        {
            // Validaciones
            ValidarFechas(fechaCaducidad, fechaPublicacion);
            ValidarContenido(contenido);

            var mensaje = new mensaje()
            {
                Titulo = titulo.Trim(),
                Contenido = contenido.Trim(),
                FechaCaducidad = fechaCaducidad,
                FechaPublicacion = fechaPublicacion,
                EnviadoA = GetEnviadoAString(enviadoA)
            };


            _db.mensajes.Add(mensaje);
            _db.SaveChanges();

        }

        /// <summary>
        /// Crear mensaje de tipo PDF en el contenido
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="pdf"></param>
        /// <param name="fechaPublicacion"></param>
        /// <param name="fechaCaducidad"></param>
        /// <param name="enviadoA"></param>
        public void CrearPdf(string titulo, string pdf,
            DateTime fechaPublicacion, DateTime fechaCaducidad, EnviadoA enviadoA)
        {
            // Validaciones
            ValidarFechas(fechaCaducidad, fechaPublicacion);
            ValidarPdf(pdf);

            var mensaje = new mensaje()
            {
                Titulo = titulo.Trim(),
                Archivo = pdf.Trim(),
                FechaCaducidad = fechaCaducidad,
                FechaPublicacion = fechaPublicacion,
                EnviadoA = GetEnviadoAString(enviadoA)
            };

            _db.mensajes.Add(mensaje);
            _db.SaveChanges();
        }

        public void ActualizarTexto(int id, string titulo, string contenido,
            DateTime fechaPublicacion, DateTime fechaCaducidad, EnviadoA enviadoA)
        {
            // Validaciones
            ValidarFechas(fechaCaducidad, fechaPublicacion);
            ValidarMensajeId(id);
            ValidarContenido(contenido);

            var mensaje = Find(id);

            mensaje.Titulo = titulo.Trim();
            mensaje.FechaPublicacion = fechaPublicacion;
            mensaje.FechaCaducidad = fechaCaducidad;
            mensaje.EnviadoA = GetEnviadoAString(enviadoA);
            mensaje.Contenido = contenido;

            _db.Entry(mensaje).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void ActualizarPdf(int id, string titulo, string pdf,
            DateTime fechaPublicacion, DateTime fechaCaducidad, EnviadoA enviadoA)
        {
            ValidarFechas(fechaCaducidad, fechaPublicacion);
            ValidarMensajeId(id);
            ValidarPdf(pdf);

            var mensaje = Find(id);

            mensaje.Titulo = titulo.Trim();
            mensaje.FechaPublicacion = fechaPublicacion;
            mensaje.FechaCaducidad = fechaCaducidad;
            mensaje.EnviadoA = GetEnviadoAString(enviadoA);
            mensaje.Archivo = pdf;

            _db.Entry(mensaje).State = EntityState.Modified;
            _db.SaveChanges();
        }
        public void Eliminar(int id)
        {
            ValidarMensajeId(id);

            //TODO validar que hacer con los mensajes que ya han sido visualizados SOFT DELETE

            var mensaje = Find(id);

            _db.mensajes.Remove(mensaje);
            _db.SaveChanges();
        }

        public void Visualizar(int cuentaId, int mensajeId, string usuarioId)
        {

            ValidarMensajeId(mensajeId);

            var cuentaMensaje = new cuentasmensaje()
            {
                CuentaId = cuentaId,
                MensajeId = mensajeId,
                FechaVisualizacion = DateTime.Now,
                UsuarioId = usuarioId
            };

            _db.cuentasmensajes.Add(cuentaMensaje);
            _db.SaveChanges();


        }
    }
}