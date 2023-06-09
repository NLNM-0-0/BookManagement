using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class BillDetailCanPropertyChange : BaseViewModel
    {
        private int soLuong;
        public int SoLuong
        {
            get => soLuong;
            set{
                soLuong = value;
                billDetail.SoLuong = soLuong;
                OnPropertyChanged();
            }
        }
        private CHITIETHOADON billDetail;
        public CHITIETHOADON BillDetail
        {
            get => billDetail;
            set
            {
                billDetail = value;
                OnPropertyChanged();
            }
        }
        public BillDetailCanPropertyChange()
        {
            this.BillDetail = new CHITIETHOADON();
            this.soLuong = 0;
        }
    }
}
