using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class AddAuthorGroupDialogVM : BaseViewModel 
    {
        #region Action
        public Action<ObservableCollection<TACGIA>> EditAuthorSuccess;
        #endregion
        #region GenericDataRepository
        private GenericDataRepository<TACGIA> authorRepo = new GenericDataRepository<TACGIA>();
        #endregion
        #region Command
        public ICommand CloseDialogCommand { get; set; }
        public ICommand RemoveAuthorCommand { get; set; }
        public ICommand AddAuthorCommand { get; set; }
        public ICommand AddAuthorGroupSuccessCommand { get; set; }

        #endregion

        #region Properties
        private System.Windows.Controls.UserControl PreviousItem;
        private ObservableCollection<TACGIA> filterAuthors;
        public ObservableCollection<TACGIA> FilterAuthors
        {
            get { return filterAuthors; }
            set {
                filterAuthors = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<TACGIA> allAuthors;
        public ObservableCollection<TACGIA> AllAuthors
        {
            get { return allAuthors; }
            set
            {
                allAuthors = value;
                OnPropertyChanged();
            }
        }
        private TACGIA selectedAuthor;
        public TACGIA SelectedAuthor
        {
            get { return selectedAuthor; }
            set
            {
                selectedAuthor = value;
                OnPropertyChanged();
            }
        }
        private string selectedAuthorString;
        public string SelectedAuthorString
        {
            get { return selectedAuthorString; }
            set
            {
                selectedAuthorString = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<AuthorGroupItemVM> authorGroupItems;
        #endregion 
        public AddAuthorGroupDialogVM(ObservableCollection<AuthorGroupItemVM> authorGroupItems)
        {
            this.authorGroupItems = authorGroupItems; 
            Task.Run(async () =>
            {
                await Load();
                FilterAuthors = new ObservableCollection<TACGIA>();
                RemoveAuthorCommand = new RelayCommand<TACGIA>((p) => true, p =>
                {
                    FilterAuthors.Remove(p);
                });
                AddAuthorCommand = new RelayCommand<object>(p => !String.IsNullOrEmpty(SelectedAuthorString), async (p) =>
                {
                    if (!FilterAuthors.Any(a => a.TenTacGia == SelectedAuthorString))
                    {
                        if (SelectedAuthor != null && SelectedAuthor.TenTacGia == SelectedAuthorString)
                        {
                            FilterAuthors.Insert(0, SelectedAuthor);
                        }
                        else
                        {
                            FilterAuthors.Insert(0, new TACGIA() { TenTacGia = SelectedAuthorString.Trim() });
                        }
                    }
                    else
                    {
                        PreviousItem = MainViewModel.UpdateDialog("Main");
                        var dl = new ConfirmDialog()
                        {
                            ContentString = "Tác giả này đã được được chọn. Hãy chọn tác giả khác!",
                            Header = "Oops",
                            CM = new RelayCommandWithNoParameter(() =>
                            {
                                DialogHost.CloseDialogCommand.Execute(null, null);
                                if (PreviousItem != null)
                                {
                                    DialogHost.Show(PreviousItem, "Main");
                                }
                            })
                        };
                        await DialogHost.Show(dl, "Main");
                    }
                });
                AddAuthorGroupSuccessCommand = new RelayCommandWithNoParameter(() =>
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    EditAuthorSuccess?.Invoke(FilterAuthors);
                });
            });
        }

        private async Task Load()
        {
            AllAuthors = new ObservableCollection<TACGIA>(await authorRepo.GetAllAsync());
        }
    }
}
