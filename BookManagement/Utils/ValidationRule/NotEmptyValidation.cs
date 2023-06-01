using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BookManagement
{
    public class NotEmptyValidationRule : ValidationRule
    {
        private bool isNotCheckFirstTime = true;
        public bool IsNotCheckFirstTime
        {
            get => isNotCheckFirstTime;
            set
            {
                isNotCheckFirstTime = value;
            }
        }
        private string errorMessage = "Trường này không thể để trống";
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
            }
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (IsNotCheckFirstTime)
            {
                return string.IsNullOrWhiteSpace((value ?? "").ToString())
                    ? new ValidationResult(false, ErrorMessage)
                    : ValidationResult.ValidResult;
            }
            else
            {
                IsNotCheckFirstTime = true;
                return ValidationResult.ValidResult;
            }
        }
    }
}
