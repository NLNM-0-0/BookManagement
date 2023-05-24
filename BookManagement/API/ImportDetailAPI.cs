using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class ImportDetailAPI
    {
        public static async Task<bool> Add(string bookId, string importId, int number)
        {
            bool res = true;
            await Task.Run(() => {
                using (var context = new WPFBookManagementEntities())
                {
                    var sql = $"insert into dbo.CHITIETPHIEUNHAP values ('{bookId}', '{importId}', {number})";
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
