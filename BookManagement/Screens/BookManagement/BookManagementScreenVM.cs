using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    internal class BookManagementScreenVM : BaseViewModel
    {
        #region Public Properties
        public ObservableCollection<BookModel> BookList { get; set; }
        public List<string> SearchByOptions { get; set; }

        #endregion


        #region Commands

        #endregion

        #region Constructor
        public BookManagementScreenVM()
        {
            BookList = new ObservableCollection<BookModel> { new BookModel(), new BookModel(), new BookModel(),
            new BookModel(), new BookModel(), new BookModel(),
            new BookModel(), new BookModel(), new BookModel(),
            new BookModel(), new BookModel(), new BookModel(),
            new BookModel(), new BookModel(), new BookModel(),
            new BookModel(), new BookModel(), new BookModel(),
            new BookModel(), new BookModel(), new BookModel(),
            new BookModel(), new BookModel(), new BookModel(),
            new BookModel(), new BookModel(), new BookModel()};

            SearchByOptions = new List<string> { "Title", "Author" };

        }

        //private async Task Load()
        //{

        //}
        #endregion

        #region Command Methods




        #endregion
    }

    public class BookModel
    {
        public static int id = 0;
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set;  }
        public double Price { get; set; }
        public string Category { get; set; }
        public BookModel()
        {
            this.Id = id++.ToString();
            this.Title = "My title";
            this.Author = "My Author";
            this.Quantity = 578;
            this.Price = 20000;
            this.Category = "Comic";
        }
    }
}
