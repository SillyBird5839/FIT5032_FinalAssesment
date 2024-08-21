using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ASS2_20240802.Validators
{
    public class CustomEmailValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value as string;

            if (string.IsNullOrEmpty(email))
            {
                return new ValidationResult("Email cannot be null。");
            }

            string pattern = @"^[a-zA-Z0-9]+[a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, pattern))
            {
                return new ValidationResult("Please enter the correct email format!");
            }

            return ValidationResult.Success;
        }
    }
}