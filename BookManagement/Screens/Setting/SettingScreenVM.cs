using BookManagement.Models;
using DocumentFormat.OpenXml.Presentation;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace BookManagement
{
    internal class SettingScreenVM : BaseViewModel
    {
        #region Access property
        public bool IsAllowChangeRule =>
            AccountStore.instance.CurrentAccount.NHOMNGUOIDUNG.CHUCNANGs.Any(p => p.MaChucNang == AppEnum.ThayDoiQuiDinh);
        public bool IsAllowDecentralization =>
            AccountStore.instance.CurrentAccount.NHOMNGUOIDUNG.CHUCNANGs.Any(p => p.MaChucNang == AppEnum.PhanQuyen);
        #endregion

        #region GenericDataRepository
        private readonly GenericDataRepository<NHOMNGUOIDUNG> userGroupRepo = new GenericDataRepository<NHOMNGUOIDUNG> { };
        private readonly GenericDataRepository<CHUCNANG> accesssRepo = new GenericDataRepository<CHUCNANG> { };
        private readonly GenericDataRepository<THAMSO> ruleRepo = new GenericDataRepository<THAMSO> { };
        #endregion

        #region Properties
        private String minImport;
        public String MinImport
        {
            get => minImport;
            set
            {
                minImport = value;
                OnPropertyChanged();
            }
        }
        private String maxInStockWhenImport;
        public String MaxInStockWhenImport
        {
            get => maxInStockWhenImport;
            set
            {
                maxInStockWhenImport = value;
                OnPropertyChanged();
            }
        }
        private String maxDept;
        public String MaxDept
        {
            get => maxDept;
            set
            {
                maxDept = value;
                OnPropertyChanged();
            }
        }
        private String maxInStockWhenSell;
        public String MaxInStockWhenSell
        {
            get => maxInStockWhenSell;
            set
            {
                maxInStockWhenSell = value;
                OnPropertyChanged();
            }
        }
        private bool isCheckCollectedMoney;
        public bool IsCheckCollectedMoney
        {
            get => isCheckCollectedMoney;
            set
            {
                isCheckCollectedMoney = value;
                OnPropertyChanged();
            }
        }   
        private NHOMNGUOIDUNG userGroup;
        public NHOMNGUOIDUNG UserGroup
        {
            get => userGroup;
            set
            {
                userGroup = value;
                if(userGroup != null)
                {
                    InitAccessList = updateAccessList();
                    AccessList.Clear();
                    foreach (AccessItem item in InitAccessList)
                    {
                        AccessList.Add((AccessItem)item.Clone());
                    }
                }
                OnPropertyChanged();
            }
        }

        private ObservableCollection<NHOMNGUOIDUNG> userGroupList;
        public ObservableCollection<NHOMNGUOIDUNG> UserGroupList
        {
            get => userGroupList;
            set
            {
                userGroupList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AccessItem> accessList = new ObservableCollection<AccessItem>();
        public ObservableCollection<AccessItem> AccessList
        {
            get => accessList;
            set
            {
                accessList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AccessItem> initAccessList;
        public ObservableCollection<AccessItem> InitAccessList
        {
            get => initAccessList;
            set
            {
                initAccessList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CHUCNANG> allAccessList;
        public ObservableCollection<CHUCNANG> AllAccessList
        {
            get => allAccessList;
            set
            {
                allAccessList = value;
                OnPropertyChanged();
            }
        }

        private int selectedPage;
        public int SelectedPage
        {
            get => selectedPage;
            set
            {
                selectedPage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Command
        public ICommand SaveRuleCommand { get; set; }
        public ICommand BackToPreviousRuleCommand { get; set; }
        public ICommand SaveAccessCommand { get; set; }
        public ICommand BackToPreviousAccessCommand { get; set; }
        #endregion

        public SettingScreenVM()
        {
            MainViewModel.IsLoading = true;
            AccountStore.instance.AccountReLoad += Instance_AccountReLoad;
            updateRuleProperty();
            Task.Run(async () =>
            {
                await Load();
                MainViewModel.IsLoading = false;

                SaveRuleCommand = new RelayCommandWithNoParameter(async() =>
                {
                    MainViewModel.SetLoading(true);
                    for(int i = 0; i < RuleStore.instance.CurrentRules.Count(); i++)
                    {
                        THAMSO rule = RuleStore.instance.CurrentRules[i];
                        if(rule.TenThamSo == AppEnum.LuongSachNhapItNhat)
                        {
                            rule.GiaTri = int.Parse(MinImport);
                        } 
                        else if (rule.TenThamSo == AppEnum.LuongTonToiDaKhiNhap)
                        {
                            rule.GiaTri = int.Parse(MaxInStockWhenImport);
                        }
                        else if (rule.TenThamSo == AppEnum.TienNoToiDaKhiBan)
                        {
                            rule.GiaTri = int.Parse(MaxDept);
                        }
                        else if (rule.TenThamSo == AppEnum.LuongTonToiDaKhiBan)
                        {
                            rule.GiaTri = int.Parse(MaxInStockWhenSell);
                        }
                        else if(rule.TenThamSo == AppEnum.ApDungQDKiemTraTienThu)
                        {
                            rule.GiaTri = IsCheckCollectedMoney ? 1 : 0;
                        }    
                        await ruleRepo.Update(rule);
                    }
                    MainViewModel.SetLoading(false);
                });
                BackToPreviousRuleCommand = new RelayCommandWithNoParameter(() =>
                {
                    MainViewModel.SetLoading(true);
                    updateRuleProperty();
                    MainViewModel.SetLoading(false);
                });
                SaveAccessCommand = new RelayCommandWithNoParameter(async () =>
                {
                    MainViewModel.SetLoading(true);
                    if (initAccessList[0].IsAllowed == true && accessList[0].IsAllowed == false)
                    {
                        bool result = await CheckOtherAccessDecentralization();
                        if(result == false) 
                        {
                            var dl = new ConfirmDialog()
                            {
                                Header = "Oops",
                                ContentString = $"Không thể thực hiện chức năng này. Ngoài nhóm người dùng {userGroup.TenNhomNguoiDung}, không nhóm người dùng nào khác còn có thể phân quyền.",
                            };
                            MainViewModel.SetLoading(false);
                            await DialogHost.Show(dl, "Main");
                            return;
                        }
                        else
                        {
                            await UserGroupAccessAPI.Delete(userGroup.MaNhomNguoiDung, initAccessList[0].Access.MaChucNang);
                        }    
                    }
                    else if(initAccessList[0].IsAllowed == false && accessList[0].IsAllowed == true)
                    {
                        await UserGroupAccessAPI.Add(userGroup.MaNhomNguoiDung, initAccessList[0].Access.MaChucNang);
                    }    
                    int i = 1;
                    for(;i < initAccessList.Count;i++)
                    {
                        AccessItem initAccess = initAccessList.ElementAt(i);
                        AccessItem access = accessList.ElementAt(i);
                        if(initAccess.IsAllowed == true && access.IsAllowed == false)
                        {
                            await UserGroupAccessAPI.Delete(userGroup.MaNhomNguoiDung, initAccess.Access.MaChucNang);
                        }
                        else if(initAccess.IsAllowed == false && access.IsAllowed == true)
                        {
                            await UserGroupAccessAPI.Add(userGroup.MaNhomNguoiDung, initAccess.Access.MaChucNang);
                        }
                    }
                    await Load();
                    if (AccountStore.instance.CurrentAccount.MaNhomNguoiDung == userGroup.MaNhomNguoiDung)
                    {
                        await AccountStore.instance.Reload();
                    }    
                    MainViewModel.SetLoading(false);
                });
                BackToPreviousAccessCommand = new RelayCommandWithNoParameter(() =>
                {
                    MainViewModel.SetLoading(true);
                    AccessList.Clear();
                    foreach (AccessItem item in InitAccessList)
                    {
                        AccessList.Add((AccessItem)item.Clone());
                    }
                    MainViewModel.SetLoading(false);
                });
            });
        }
        public override void Dispose()
        {
            AccountStore.instance.AccountReLoad -= Instance_AccountReLoad;
            base.Dispose();
        }
        private void updateRuleProperty()
        {
            MinImport =
                RuleStore.instance.getValue(AppEnum.LuongSachNhapItNhat).ToString();
            MaxInStockWhenImport =
                RuleStore.instance.getValue(AppEnum.LuongTonToiDaKhiNhap).ToString();
            MaxDept =
                RuleStore.instance.getValue(AppEnum.TienNoToiDaKhiBan).ToString();
            MaxInStockWhenSell =
                RuleStore.instance.getValue(AppEnum.LuongTonToiDaKhiBan).ToString();
            IsCheckCollectedMoney =
                RuleStore.instance.getValue(AppEnum.ApDungQDKiemTraTienThu) == 1;
        }
        private void Instance_AccountReLoad()
        {
            OnPropertyChanged(nameof(IsAllowChangeRule));
            OnPropertyChanged(nameof(IsAllowDecentralization));
            SelectedPage = 0;
        }

        private ObservableCollection<AccessItem> updateAccessList()
        {
            ObservableCollection<AccessItem> newAccessList = new ObservableCollection<AccessItem>();
            for (int i = 0; i < allAccessList.Count; i++)
            {
                bool flag = false;
                for (int j = 0; j < userGroup.CHUCNANGs.Count; j++)
                {
                    if (allAccessList.ElementAt(i).MaChucNang == userGroup.CHUCNANGs.ElementAt(j).MaChucNang)
                    {
                        flag = true;
                        break;
                    }
                }
                newAccessList.Add(new AccessItem(i + 1, allAccessList.ElementAt(i), flag));
            }
            return newAccessList;
        }    
        private async Task Load()
        {
            UserGroupList = new ObservableCollection<NHOMNGUOIDUNG>(await userGroupRepo.GetListAsync(p=>true, p=>p.CHUCNANGs));
            AllAccessList = await LoadAllAccess();
            UserGroup = UserGroupList.FirstOrDefault();
        }

        private async Task<ObservableCollection<CHUCNANG>> LoadAllAccess()
        {
            return new ObservableCollection<CHUCNANG>(await accesssRepo.GetListAsync(p => true));
        }

        //Kiểm tra xem coi có nhóm người dùng nào ngoài nhóm người dùng userGroup được phân quyền
        private async Task<bool> CheckOtherAccessDecentralization()
        {
            List<NHOMNGUOIDUNG> userGroupList = (await userGroupRepo.GetListAsync(p => p.MaNhomNguoiDung != userGroup.MaNhomNguoiDung, p=>p.CHUCNANGs)).ToList();
            for(int i = 0; i < userGroupList.Count; i++)
            {
                ICollection<CHUCNANG> tempAccessList = userGroupList[i].CHUCNANGs;
                if (tempAccessList.Any(p => p.MaChucNang == AppEnum.PhanQuyen))
                {
                    return true;
                }    
            }
            return false;
        }
    }
}
