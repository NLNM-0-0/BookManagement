using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class AccountStore
    {
        public static AccountStore instance;
        public event Action AccountChanged;
        public event Action AccountUpdated;
        public event Action AccountReLoad;
        private NHANVIEN currentAccount;
        private bool notInvoke { get; set; } = false;
        public NHANVIEN CurrentAccount
        {
            get { return currentAccount; }
            set
            {
                currentAccount = value;
                if (!notInvoke)
                    OnCurrentAccountChange();
            }
        }

        private GenericDataRepository<NHANVIEN> userRepo = new GenericDataRepository<NHANVIEN>();

        public async Task Reload()
        {
            string userId = AccountStore.instance.CurrentAccount.MaNhanVien;
            NHANVIEN user = await userRepo.GetSingleAsync(u=>u.MaNhanVien == userId, u=>u.NHOMNGUOIDUNG, u=>u.NHOMNGUOIDUNG.CHUCNANGs);
            CurrentAccount = user;
            AccountReLoad?.Invoke();
        }
        public async Task Update(NHANVIEN user)
        {
            notInvoke = true;
            CurrentAccount = user;
            await userRepo.Update(CurrentAccount);
            notInvoke = false;
            AccountUpdated?.Invoke();
        }
        private void OnCurrentAccountChange()
        {
            AccountChanged?.Invoke();
            var nav = NavigationStore.instance.stackScreen;
            var temp = nav[nav.Count - 1];
            nav.Clear();
            nav.Add(temp);
        }
    }
}
