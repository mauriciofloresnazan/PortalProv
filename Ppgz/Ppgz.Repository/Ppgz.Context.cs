﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<aspnetrole> aspnetroles { get; set; }
        public virtual DbSet<aspnetuserclaim> aspnetuserclaims { get; set; }
        public virtual DbSet<aspnetuserlogin> aspnetuserlogins { get; set; }
        public virtual DbSet<aspnetuser> aspnetusers { get; set; }
        public virtual DbSet<cuentaproveedore> cuentaproveedores { get; set; }
        public virtual DbSet<cuenta> cuentas { get; set; }
        public virtual DbSet<cuentasmensaje> cuentasmensajes { get; set; }
        public virtual DbSet<fakedataproveedor> fakedataproveedors { get; set; }
        public virtual DbSet<mensaje> mensajes { get; set; }
        public virtual DbSet<perfile> perfiles { get; set; }
        public virtual DbSet<vwmensaje> vwmensajes { get; set; }
    }
}
