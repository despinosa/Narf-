//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Narf.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Transition
    {
        public int Id { get; set; }
        public int Time { get; set; }
        public Zone From { get; set; }
        public Zone To { get; set; }
    
        public virtual Case Case { get; set; }
    }
}
