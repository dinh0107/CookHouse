using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CookHouse.Filters
{
    public class SecureEmailAttribute : ValidationAttribute
    {
        private const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Email không được để trống.");
            }

            string email = value.ToString();

            // Kiểm tra định dạng email
            if (!Regex.IsMatch(email, EmailPattern))
            {
                return new ValidationResult("Email không hợp lệ.");
            }

            // Kiểm tra SQL Injection
            if (ContainsSqlInjection(email))
            {
                return new ValidationResult("Email chứa ký tự không hợp lệ.");
            }

            return ValidationResult.Success;
        }

        private bool ContainsSqlInjection(string input)
        {
            string pattern = @"(--|;|'|\b(select|insert|update|delete|drop|alter|exec|execute|declare|xp_)\b)";
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }
    }
}