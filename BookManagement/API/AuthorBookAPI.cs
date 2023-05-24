using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class AuthorDetailAPI
    {
        public static async Task<bool> Add(string authorId, string bookHeaderID)
        {
            bool res = true;
            await Task.Run(() => {
                using (var context = new WPFBookManagementEntities())
                {
                    var sql = $"insert into dbo.CHITIETTACGIA values ('{authorId}', '{bookHeaderID}')";
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
