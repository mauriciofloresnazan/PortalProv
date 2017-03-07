﻿using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.MySqlClient;
using Ppgz.Repository;
using Ppgz.Web.Models;
using log4net;
using System;
using System.Text;

namespace Ppgz.Web.Infrastructure
{
	/// <summary> Indica los tipos de mensajes manejados para la aplicación.</summary>
	public enum TipoMensaje
	{
		Informativo,
		Advertencia,
		Error
	}

	public class CommonManager
	{
		/// <summary> Log para almacenar errores de negocio.</summary>
		public static readonly ILog businessLog = LogManager.GetLogger(@"BusinessLog");

		/// <summary> Log para almacenar errores de aplicación.</summary>
		public static readonly ILog errorAppLog = LogManager.GetLogger(@"ErrorAppLog");

		/// <summary> Log para ser visualizado en el navegador.</summary>
		public static readonly ILog traceView = LogManager.GetLogger(@"TraceView");

		private readonly UserManager<ApplicationUser> _applicationUserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
		private readonly Entities _db = new Entities();

		/// <summary> Etiqueta un mensaje de traza de log.</summary>
		/// <param name="tipoMensaje">Indica el tipo de mensaje a etiquetar. (Informativo, Advertencia y Error)</param>
		/// <param name="controller">Nombre del controlador.</param>
		/// <param name="action"> Nombre de la acción llamada.</param>
		/// <param name="parametros"> Petición realizada.</param>
		/// <param name="mensaje">Mensaje previo a etiquetar. </param>
		/// <returns>Mensaje de error etiquetado.</returns>
		public static string BuildMessageLog(TipoMensaje tipoMensaje, string controller, string action, string mensaje, HttpRequestBase parametros = null)
		{
			StringBuilder respuesta = new StringBuilder();
			StringBuilder valores = new StringBuilder();
			//Se construye el mensaje de la traza.
			respuesta.Append(string.Empty + Environment.NewLine);
			respuesta.Append(@"Nombre del Controlador = " + controller + Environment.NewLine);
			respuesta.Append(@"Nombre del método o acción solicitada = " + action + Environment.NewLine);
			if (parametros != null)
			{
				//Detectar donde fué enviado los valores.
				if (parametros.Form.Count > 0)
				{
					for (int i = 1; i < parametros.Form.Count; i++)
					{
						if (i == parametros.Form.Count - 1)
						{
							valores.Append(parametros.Form.Keys[i] + @" = " + parametros.Form[i]);
							break;
						}
						valores.Append(parametros.Form.Keys[i] + @" = " + parametros.Form[i] + @", ");
					}
				}
				if (parametros.QueryString.Count > 0)
				{
					for (int i = 1; i < parametros.QueryString.Count; i++)
					{
						if (i == parametros.Form.Count - 1)
						{
							valores.Append(parametros.QueryString.Keys[i] + @" = " + parametros.QueryString[i]);
							break;
						}
						valores.Append(parametros.QueryString.Keys[i] + @" = " + parametros.QueryString[i] + @", ");
					}
				}
				respuesta.Append(@"Conjunto de parámetros enviados = " + valores.ToString() + Environment.NewLine);
			}
			switch (tipoMensaje)
			{
				case TipoMensaje.Informativo:
					respuesta.Append(@"Tipo de mensaje = Informativo." + Environment.NewLine);
					break;
				case TipoMensaje.Advertencia:
					respuesta.Append(@"Tipo de mensaje = Advertencia." + Environment.NewLine);
					break;
				case TipoMensaje.Error:
					respuesta.Append(@"Tipo de mensaje = Error." + Environment.NewLine);
					break;
			}
			return respuesta.Append(@"Traza detallada: " + Environment.NewLine + mensaje).ToString();
		}

		/// <summary> Escribe en el log de error de la aplicación y en el visor web. </summary>
		/// <param name="mensaje">Mensaje completo del error.</param>
		public static void WriteAppLog(string mensaje, TipoMensaje tipo)
		{
			switch (tipo)
			{
				case TipoMensaje.Informativo:
					errorAppLog.Info(mensaje);
					traceView.Info(mensaje);
					break;
				case TipoMensaje.Advertencia:
					errorAppLog.Warn(mensaje);
					traceView.Warn(mensaje);
					break;
				case TipoMensaje.Error:
					errorAppLog.Error(mensaje);
					traceView.Error(mensaje);
					break;
			}
		}

		/// <summary> Escribe en el log de errores de negocio y en el visor web. </summary>
		/// <param name="mensaje">Mensaje completo del error.</param>
		public static void WriteBusinessLog(string mensaje, TipoMensaje tipo)
		{
			switch (tipo)
			{
				case TipoMensaje.Informativo:
					businessLog.Info(mensaje);
					traceView.Info(mensaje);
					break;
				case TipoMensaje.Advertencia:
					businessLog.Warn(mensaje);
					traceView.Warn(mensaje);
					break;
				case TipoMensaje.Error:
					businessLog.Error(mensaje);
					traceView.Error(mensaje);
					break;
			}
		}

		public cuenta GetCuentaUsuarioAutenticado()
		{
			var usuarioAutenticado = GetUsuarioAutenticado();

			return _db.Database.SqlQuery<cuenta>(@"SELECT * FROM cuentas WHERE   id IN (SELECT CuentaId FROM cuentasusuarios WHERE UsuarioId = {0})", usuarioAutenticado.Id).FirstOrDefault();

		}

		public aspnetuser GetUsuarioAutenticado()
		{
			var userName = HttpContext.Current.User.Identity.GetUserName();
			var usuarioAutenticado = _db.aspnetusers.Single(u => u.UserName == userName);
			return usuarioAutenticado;
		}
		public string HashPassword(string password)
		{
			return _applicationUserManager.PasswordHasher.HashPassword(password);
		}

		public DataTable QueryToTable(string queryText, SqlParameter[] parametes = null)
		{
			using (DbDataAdapter adapter = new MySqlDataAdapter())
			{
				adapter.SelectCommand = _db.Database.Connection.CreateCommand();
				adapter.SelectCommand.CommandText = queryText;
				if (parametes != null)
					adapter.SelectCommand.Parameters.AddRange(parametes);
				var table = new DataTable();
				adapter.Fill(table);
				return table;
			}
		}

	}
}