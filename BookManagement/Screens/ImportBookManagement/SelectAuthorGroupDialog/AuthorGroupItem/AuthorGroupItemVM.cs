using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class AuthorGroupItemVM : BaseViewModel
    {
        #region Properties
        private int groupIndex;
        public int GroupIndex
        {
            get => groupIndex;
            set
            {
                this.groupIndex = value;
                OnPropertyChanged(nameof(GroupName));
            }
        }
        public String GroupName
        {
            get
            {
                return $"Nhóm {groupIndex}";
            }
        }
        private List<TACGIA> authors;
        public List<TACGIA> Authors
        {
            get=> authors;
            set
            {
                authors = value;
                OnPropertyChanged();
            }    
        }
        #endregion

        #region Constructor
        public AuthorGroupItemVM(int index, List<TACGIA> authors) 
        {
            GroupIndex = index;
            Authors = authors;
        }
        #endregion
    }
}
