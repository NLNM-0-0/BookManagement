using BookManagement.Models;
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
    public class SelectAuthorGroupDialogVM:BaseViewModel
    {
        #region Action
        public Action<List<TACGIA>> ChooseAuthorSuccess;
        #endregion
        #region GenericDataRepository
        private GenericDataRepository<DAUSACH> bookHeaderRepo = new GenericDataRepository<DAUSACH>(); 
        #endregion

        #region Properties
        private ObservableCollection<AuthorGroupItemVM> authorGroupItemVMs;
        public ObservableCollection<AuthorGroupItemVM> AuthorGroupItemVMs
        {
            get=>authorGroupItemVMs;
            set
            {
                authorGroupItemVMs = value;
                OnPropertyChanged();
            }
        }
        private String bookName;
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        #region Command
        public ICommand CloseDialogCommand { get; set; }
        public ICommand ChooseAuthorGroupCommand { get; set; }
        public ICommand AddNewCustomerGroupCommand { get; set; }
        
        #endregion

        #region Constuctor
        public SelectAuthorGroupDialogVM(String bookName)
        {
            this.bookName = bookName;
            Task.Run(async () =>
            {
                await Load();
                ChooseAuthorGroupCommand = new RelayCommand<List<TACGIA>>(p => p!=null, p =>
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    ChooseAuthorSuccess?.Invoke(p);
                });
                AddNewCustomerGroupCommand = new RelayCommandWithNoParameter(() =>
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");
                    AddAuthorGroupDialog addAuthorGroupDialog = new AddAuthorGroupDialog();
                    AddAuthorGroupDialogVM addAuthorGroupDialogVM = new AddAuthorGroupDialogVM(AuthorGroupItemVMs);
                    addAuthorGroupDialogVM.CloseDialogCommand = new RelayCommandWithNoParameter(() =>
                    {
                        DialogHost.CloseDialogCommand.Execute(null, null);
                        if (PreviousItem != null)
                        {
                            DialogHost.Show(PreviousItem, "Main");
                        }
                    });
                    addAuthorGroupDialogVM.EditAuthorSuccess += OnEditAuthorSuccess;
                    addAuthorGroupDialog.DataContext = addAuthorGroupDialogVM;
                    DialogHost.Show(addAuthorGroupDialog, "Main");
                });
            });
            
        }
        #endregion

        #region Function
        public void OnEditAuthorSuccess(ObservableCollection<TACGIA> authorGroups)
        {
            List<String> authorNames = authorGroups.Select(a => a.TenTacGia).ToList();
            if (AuthorGroupItemVMs.Any(p =>
            {
                List<String> tempAuthorNames = p.Authors.Select(a => a.TenTacGia).ToList();
                if (tempAuthorNames.Count == authorNames.Count)
                {
                    if(tempAuthorNames.Intersect(authorNames).ToList().Count == authorNames.Count)
                    {
                        return true;
                    }    
                }    
                return false;
            }))
            {
                var dl = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Bạn đã thêm vào 1 nhóm tác giả đã có",
                    CM = new RelayCommandWithNoParameter(()=>{
                        if (PreviousItem != null)
                        {
                            DialogHost.Show(PreviousItem, "Main");
                        }
                    })
                };
                DialogHost.Show(dl, "Main");
            }
            else
            {
                if (PreviousItem != null)
                {
                    DialogHost.Show(PreviousItem, "Main");
                }
                AuthorGroupItemVMs.Add(new AuthorGroupItemVM(AuthorGroupItemVMs.Count + 1, authorGroups.ToList()));
            }   
        }
        public async Task Load()
        {
            ICollection<DAUSACH> bookHeaders = await bookHeaderRepo.GetListAsync(p => p.TenSach == bookName, p=>p.TACGIAs);
            ObservableCollection<AuthorGroupItemVM> tempAuthorGroups = new ObservableCollection<AuthorGroupItemVM>();
            for(int i = 0; i < bookHeaders.Count; i++)
            {
                tempAuthorGroups.Add(new AuthorGroupItemVM(i + 1, bookHeaders.ElementAt(i).TACGIAs.ToList()));
            }
            AuthorGroupItemVMs = new ObservableCollection<AuthorGroupItemVM>(tempAuthorGroups);
        }
        #endregion
    }
}
