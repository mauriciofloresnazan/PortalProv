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
    
    public partial class cuentasxpagar
    {
        public int Id { get; set; }
        public string NumeroCompensacion { get; set; }
        public string Referencia { get; set; }
        public Nullable<System.DateTime> FechaDePago { get; set; }
        public Nullable<System.DateTime> FechaBase { get; set; }
        public string Importe { get; set; }
        public string Ml { get; set; }
        public string TipoMovimiento { get; set; }
        public int ProveedoresId { get; set; }
        public Nullable<int> FacturaId { get; set; }
    }
}
