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
    
    public partial class ordencompra
    {
        public ordencompra()
        {
            this.ordencompradetalles = new HashSet<ordencompradetalle>();
            this.ordencompradetalles1 = new HashSet<ordencompradetalle>();
        }
    
        public int Id { get; set; }
        public string NumeroDocumento { get; set; }
        public string Sociedad { get; set; }
        public string TipoDocumento { get; set; }
        public string ClaseDocumento { get; set; }
        public string IndicadorControl { get; set; }
        public string IndicadorBorrado { get; set; }
        public string EstatusDocumento { get; set; }
        public string FechaCreacionSap { get; set; }
        public Nullable<int> IntervaloPosicion { get; set; }
        public Nullable<int> UltimoNumeroPos { get; set; }
        public string NumeroProveedor { get; set; }
        public string ClavePago { get; set; }
        public string OrganizacionCompra { get; set; }
        public string GrupoCompra { get; set; }
        public string Moneda { get; set; }
        public Nullable<int> TipoCambio { get; set; }
        public string IndTipoCambio { get; set; }
        public string FechaDocCompra { get; set; }
        public System.DateTime FechaCargaPortal { get; set; }
        public Nullable<int> ProveedorId { get; set; }
        public Nullable<System.DateTime> FechaVisualizado { get; set; }
        public Nullable<int> CitaId { get; set; }
    
        public virtual proveedore proveedore { get; set; }
        public virtual ICollection<ordencompradetalle> ordencompradetalles { get; set; }
        public virtual ICollection<ordencompradetalle> ordencompradetalles1 { get; set; }
    }
}
