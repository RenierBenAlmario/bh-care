using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace YourNamespace.Models
{
    public class SignupViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z0-9_]{3,}$", 
            ErrorMessage = "Username must be at least 3 characters and can only contain letters, numbers, and underscores")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [RegularExpression(@"^[\w.-]+@[\w.-]+\.\w{2,}$", 
            ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[A-Za-z\s\-']+$", 
            ErrorMessage = "First name can only contain letters, spaces, hyphens, and apostrophes")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[A-Za-z\s\-']+$", 
            ErrorMessage = "Middle name can only contain letters, spaces, hyphens, and apostrophes")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[A-Za-z\s\-']+$", 
            ErrorMessage = "Last name can only contain letters, spaces, hyphens, and apostrophes")]
        public string LastName { get; set; }

        [RegularExpression(@"^[A-Za-z\s\-']+$", 
            ErrorMessage = "Suffix can only contain letters, spaces, hyphens, and apostrophes")]
        public string Suffix { get; set; }

        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"^(09\d{9}|\+639\d{9})$", 
            ErrorMessage = "Contact number must be in format 09XXXXXXXXX or +639XXXXXXXXX")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Birth date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [FutureDateValidation(ErrorMessage = "Birth date cannot be in the future")]
        public DateTime BirthDate { get; set; }

        // Guardian information (conditionally required)
        public string GuardianName { get; set; }
        
        public string GuardianContactNumber { get; set; }
        
        public string GuardianRelationship { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;""'<>,.?/~\\|-]).{8,}$", 
            ErrorMessage = "Password must include at least one uppercase letter, one lowercase letter, one number, and one special character")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "You must agree to the terms and conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and conditions")]
        public bool TermsAgreement { get; set; }

        // Custom validation for guardian information based on age
        public bool IsUnder18 => BirthDate > DateTime.Now.AddYears(-18);

        public bool Validate(out string errorMessage)
        {
            errorMessage = null;

            // Validate future dates
            if (BirthDate > DateTime.Now)
            {
                errorMessage = "Birth date cannot be in the future";
                return false;
            }

            // Validate guardian info for minors
            if (IsUnder18)
            {
                if (string.IsNullOrWhiteSpace(GuardianName))
                {
                    errorMessage = "Guardian name is required for users under 18";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(GuardianContactNumber))
                {
                    errorMessage = "Guardian contact number is required for users under 18";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(GuardianRelationship))
                {
                    errorMessage = "Guardian relationship is required for users under 18";
                    return false;
                }

                // Validate guardian contact number format
                if (!Regex.IsMatch(GuardianContactNumber, @"^(09\d{9}|\+639\d{9})$"))
                {
                    errorMessage = "Guardian contact number must be in format 09XXXXXXXXX or +639XXXXXXXXX";
                    return false;
                }
            }

            return true;
        }
    }

    // Custom validation attribute for future date validation
    public class FutureDateValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            DateTime date = (DateTime)value;
            return date <= DateTime.Now;
        }
    }
} 