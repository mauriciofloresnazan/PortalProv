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
    
    public partial class aspnetuser
    {
        public aspnetuser()
        {
            this.aspnetuserclaims = new HashSet<aspnetuserclaim>();
            this.aspnetuserlogins = new HashSet<aspnetuserlogin>();
            this.cuentas = new HashSet<cuenta>();
            this.usuario_mensajes = new HashSet<usuario_mensajes>();
            this.aspnetroles = new HashSet<aspnetrole>();
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
    
        public virtual ICollection<aspnetuserclaim> aspnetuserclaims { get; set; }
        public virtual ICollection<aspnetuserlogin> aspnetuserlogins { get; set; }
        public virtual perfile perfile { get; set; }
        public virtual ICollection<cuenta> cuentas { get; set; }
        public virtual ICollection<usuario_mensajes> usuario_mensajes { get; set; }
        public virtual ICollection<aspnetrole> aspnetroles { get; set; }
        public virtual ICollection<cuenta> cuentas1 { get; set; }
    }
}