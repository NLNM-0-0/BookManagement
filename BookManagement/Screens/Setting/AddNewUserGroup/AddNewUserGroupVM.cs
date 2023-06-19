using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class AddNewUserGroupVM : BaseViewModel
    {
        #region Action
        public Action<NHOMNGUOIDUNG> AddNewUserGroupSuccess { get; set; }
        #endregion
        #region GenericDataRepository
        private GenericDataRepository<NHOMNGUOIDUNG> userGroupRepo = new GenericDataRepository<NHOMNGUOIDUNG> ();
        #endregion

        #region Properties
        private System.Windows.Controls.UserControl PreviousItem;
        public string UserGroupName { get; set; }
        #endregion

        #region Command
        public ICommand SaveCommand { get; set; }
        #endregion

        #region Constructor
        public AddNewUserGroupVM()
        {
            SaveCommand = new RelayCommand<object>(p => { return !String.IsNullOrEmpty(UserGroupName); }, async p =>
            {
                NHOMNGUOIDUNG temp = await userGroupRepo.GetSingleAsync(u => u.TenNhomNguoiDung.ToLower() == UserGroupName.Trim().ToLower());
                if(temp == null)
                {
                    NHOMNGUOIDUNG userGroup = new NHOMNGUOIDUNG();
                    userGroup.MaNhomNguoiDung = await GenerateId.Gen(typeof(NHOMNGUOIDUNG), "MaNhomNguoiDung");
                    userGroup.TenNhomNguoiDung = UserGroupName;
                    await userGroupRepo.Add(userGroup);
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    AddNewUserGroupSuccess?.Invoke(userGroup);
                    var dl = new ConfirmDialog()
                    {
                        Header = "Thông báo",
                        ContentString = "Đã thêm vào thành công"
                    };
                    await DialogHost.Show(dl, "Main");
                }    
                else
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");
                    var dl = new ConfirmDialog()
                    {
                        Header = "Lỗi",
                        ContentString = "Đã tồn tại nhóm người dùng có tên này trong hệ thống",
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
        }
        #endregion
    }
}
