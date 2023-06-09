using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BookManagement
{
    public class EmailValidateRule : ValidationRule
    {

        public bool CanRefresh { get; set; } = true;
        public bool IsFirstTime { get; set; } = true;
        public Wrapper Wrapper { get; set; }

        public static bool Validate(string value, bool IsAdmin = false)
        {
            var str = value;

            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            if (IsAdmin) return true;
            if (!ValidateRegex.Email.IsMatch(str))
            {
                return false;
            }

            return true;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;

            if (string.IsNullOrEmpty(str))
            {
                if (!IsFirstTime)
                    return new ValidationResult(false, "Trường này không thể để trống");
                else
                {
                    if (CanRefresh)
                        IsFirstTime = false;
                    return ValidationResult.ValidResult;
                }
            }

            if (Wrapper != null && Wrapper.AdminAccess) return new ValidationResult(true, null);

            if (!ValidateRegex.Email.IsMatch(str))
            {
                return new ValidationResult(false, "Email đang ở sai định dạng");
            }

            return new ValidationResult(true, null);
        }
    }
}
