//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ppgz.Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            this.AspNetUserClaims = new HashSet<AspNetUserClaim>();
            this.AspNetUserLogins = new HashSet<AspNetUserLogin>();
            this.AspNetUserRoles = new HashSet<AspNetUserRole>();
            this.citas = new HashSet<cita>();
            this.cuentas = new HashSet<cuenta>();
            this.cuentasmensajes = new HashSet<cuentasmensaje>();
            this.mensajes = new HashSet<mensaje>();
            this.proveedores = new HashSet<proveedore>();
            this.cuentas1 = new HashSet<cuenta>();
        }
    
        public string Id { get; set; }
        public string Email { get; set; }
        public Nullable<bool> EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<bool> PhoneNumberConfirmed { get; set; }
        public Nullable<bool> TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public Nullable<bool> LockoutEnabled { get; set; }
        public Nullable<int> AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public string Tipo { get; set; }
        public bool Activo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cargo { get; set; }
        public int PerfilId { get; set; }
        public Nullable<System.DateTime> TerminosCondicionesFecha { get; set; }
        public sbyte Borrado { get; set; }
        public System.DateTime FechaTx { get; set; }
        public string UsuarioIdTx { get; set; }
        public string OperacionTx { get; set; }
    
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual perfile perfile { get; set; }
        public virtual ICollection<cita> citas { get; set; }
        public virtual ICollection<cuenta> cuentas { get; set; }
        public virtual ICollection<cuentasmensaje> cuentasmensajes { get; set; }
        public virtual ICollection<mensaje> mensajes { get; set; }
        public virtual perfile perfile1 { get; set; }
        public virtual ICollection<proveedore> proveedores { get; set; }
        public virtual ICollection<cuenta> cuentas1 { get; set; }
    }
}
