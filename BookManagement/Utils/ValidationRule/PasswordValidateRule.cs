using System.Globalization;
using System.Windows.Controls;

namespace BookManagement {
    public class PasswordValidateRule : ValidationRule {
        public bool CanRefresh { get; set; } = true;
        public bool IsFirstTime { get; set; } = true;
        public bool CheckPrevPass { get; set; } = false;
        public Wrapper Wrapper { get; set; }

        public static bool Validate(string value) {
            var str = value;
            if(str == null ||
                (str.Length < 6 && str.Length != 0)) {
                return false;
            }
            return true;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            var str = value as string;
            if(string.IsNullOrEmpty(str)) {
                if(!IsFirstTime)
                    return new ValidationResult(false, "Trường này không thể để trống");
                else {
                    if(CanRefresh)
                        IsFirstTime = false;
                    return new ValidationResult(true, null);
                }
            }
            if(str.Length < 6) {
                return new ValidationResult(false, "*Mật khẩu có độ dài lớn hơn 6 kí tự");
            }
            if(CheckPrevPass) {
                if(str == Wrapper.PrevPassword) return new ValidationResult(true, null);
                else return new ValidationResult(false, "Mật khẩu không khớp xin hãy nhập lại");
            }

            return new ValidationResult(true, "None");
        }
    }
}
