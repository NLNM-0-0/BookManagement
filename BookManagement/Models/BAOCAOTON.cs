//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BAOCAOTON
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BAOCAOTON()
        {
            this.CHITIETBAOCAOTONs = new HashSet<CHITIETBAOCAOTON>();
        }
    
        public string MaBaoCaoTon { get; set; }
        public System.DateTime Thang { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETBAOCAOTON> CHITIETBAOCAOTONs { get; set; }
    }
}
