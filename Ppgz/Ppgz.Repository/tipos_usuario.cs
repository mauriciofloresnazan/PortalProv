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
    
    public partial class tipos_usuario
    {
        public tipos_usuario()
        {
            this.usuarios = new HashSet<usuario>();
        }
    
        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
    
        public virtual ICollection<usuario> usuarios { get; set; }
    }
}