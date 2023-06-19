using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class BookDetailVM : BaseViewModel
    {
        #region Properties
        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        private string category;
        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                OnPropertyChanged();
            }
        }
        private string authors;
        public string Authors
        {
            get { return authors; }
            set
            {
                authors = value;
                OnPropertyChanged();
            }
        }
        private string nxb;
        public string NXB
        {
            get { return nxb; }
            set
            {
                nxb = value;
                OnPropertyChanged();
            }
        }
        private string timeRepublish;
        public string TimeRepublish
        {
            get { return timeRepublish; }
            set
            {
                timeRepublish = value;
                OnPropertyChanged();
            }
        }
        private string price;
        public string Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }
        private string amount;
        public string Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public BookDetailVM(SACH book)
        {
            Id = book.MaSach;
            Name = book.DAUSACH.TenSach;
            Category = book.DAUSACH.THELOAI.TenTheLoai;
            Authors = string.Join(", ", book.DAUSACH.TACGIAs.Select(p=>p.TenTacGia));
            NXB = book.NhaXuatBan;
            TimeRepublish = book.LanTaiBan.ToString();
            Price = (book.DonGiaNhapMoiNhat * RuleStore.instance.getValue(AppEnum.TiLeTinhDonGiaXuat) / 100).ToString("G29");
            Amount = book.SoLuong.ToString();
        }
        #endregion

    }
}
