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
    
    public partial class entregacita
    {
        public int Id { get; set; }
        public int HorarioId { get; set; }
        public int AlmacenRielId { get; set; }
        public int CitaId { get; set; }
    
        public virtual cita cita { get; set; }
        public virtual riele riele { get; set; }
        public virtual horario horario { get; set; }
    }
}
