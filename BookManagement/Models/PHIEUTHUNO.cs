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
    
    public partial class PHIEUTHUNO
    {
        public string MaPhieuThu { get; set; }
        public System.DateTime NgayThu { get; set; }
        public Nullable<decimal> SoTienThu { get; set; }
        public string MaNhanVien { get; set; }
        public string MaKhachHang { get; set; }
    
        public virtual KHACHHANG KHACHHANG { get; set; }
        public virtual NHANVIEN NHANVIEN { get; set; }
    }
}
