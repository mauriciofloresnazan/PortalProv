
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
    
public partial class perfile
{

    public perfile()
    {

        this.AspNetUsers = new HashSet<AspNetUser>();

        this.AspNetRoles = new HashSet<AspNetRole>();

    }


    public int Id { get; set; }

    public string Nombre { get; set; }

    public string Tipo { get; set; }

    public Nullable<int> CuentaId { get; set; }



    public virtual ICollection<AspNetUser> AspNetUsers { get; set; }

    public virtual cuenta cuenta { get; set; }

    public virtual ICollection<AspNetRole> AspNetRoles { get; set; }

}

}
