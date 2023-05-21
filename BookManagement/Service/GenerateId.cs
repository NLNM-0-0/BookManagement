using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class GenerateId
    {
        static char reVal(long num)
        {
            if (num > -1 && num < 5)
                return (char)(num + '5');
            else if (num > 4 && num < 10)
                return (char)(num - 5 + '0');
            else if (num > 9 && num < 36)
                return (char)(num - 10 + 'A');
            else if (num > 35 && num < 62)
                return (char)(num - 36 + 'a');
            else if (num == 62) return '=';
            else if (num == 63) return '-';
            return '?';
        }
        static string encode(long inputNum)
        {
            inputNum += 261816;
            inputNum = (long)(inputNum * 1.62);
            long index = 0;
            string res = "";
            while (inputNum > 0)
            {
                res += reVal(inputNum % 64);
                index++;
                inputNum /= 64;
            }
            return res;
        }

        static public async Task<string> Gen(Type type, string checkProperty = "Id")
        {
            long res = 0;
            await Task.Run(() => {
                using (var context = new WPFBookManagementEntities())
                {
                    string t = type.Name;
                    var sql = $"SELECT COUNT(*) FROM dbo.{t}";
                    res = context.Database.SqlQuery<int>(sql).Single();
                }
            });

            #region Fake ID
            //string id = type.Name.Substring(0, 4);
            //id = id + (res + 50).ToString();
            //50 thay bằng các số bắt đầu của mỗi người
            #endregion

            #region Real ID
            bool check = false;
            string id = "";
            while (!check)
            {
                id = encode(res);
                if (id.Length < 6)
                    for (int i = 0; i <= 6 - id.Length; i++)
                        id = "#" + id;
                char r1 = (char)(new Random().Next(0, 10) + '0');
                char r2 = (char)(new Random().Next(36, 62) - 36 + 'a');
                id = r1 + id;
                id += r2;

                await Task.Run(() => {
                    using (var context = new WPFBookManagementEntities())
                    {
                        string t = type.Name;
                        var sql = $"SELECT COUNT(1) FROM dbo.{t} WHERE {checkProperty} = '{id}'";
                        check = context.Database.SqlQuery<int>(sql).Single() == 0 ? true : false;
                    }
                });
                if (!check) res = (long)(res * 1.5);
            }
            #endregion

            return id;
        }
    }
}
