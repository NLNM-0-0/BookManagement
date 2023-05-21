using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class UserGroupAccessAPI
    {
        public static async Task<bool> Add(string userGroup, string accessID)
        {
            bool res = true;
            await Task.Run(() => {
                using (var context = new WPFBookManagementEntities())
                {
                    var sql = $"insert into dbo.PHANQUYEN values ('{userGroup}', '{accessID}')";
                    try
                    {
                        context.Database.ExecuteSqlCommand(sql);
                    }
                    catch
                    {
                        res = false;
                    }
                }
            });
            return res;
        }

        public static async Task<bool> Delete(string userGroup, string accessID)
        {
            bool res = true;
            await Task.Run(() => {
                using (var context = new WPFBookManagementEntities())
                {
                    var sql = $"delete from dbo.PHANQUYEN where MaNhomNguoiDung = '{userGroup}' and MaChucNang = '{accessID}'";
                    try
                    {
                        context.Database.ExecuteSqlCommand(sql);
                    }
                    catch
                    {
                        res = false;
                    }
                }
            });
            return res;
        }
    }
}
