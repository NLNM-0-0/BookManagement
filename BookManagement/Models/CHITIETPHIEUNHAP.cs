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
    
    public partial class CHITIETPHIEUNHAP
    {
        public string MaSach { get; set; }
        public string MaPhieuNhap { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
    
        public virtual PHIEUNHAPSACH PHIEUNHAPSACH { get; set; }
        public virtual SACH SACH { get; set; }
    }
}
