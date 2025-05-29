using System.ComponentModel.DataAnnotations;

namespace Barangay.Models.Validation
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly object _desiredValue;

        public RequiredIfAttribute(string propertyName, object desiredValue)
        {
            _propertyName = propertyName;
            _desiredValue = desiredValue;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var propertyValue = instance.GetType().GetProperty(_propertyName)?.GetValue(instance, null);

            if (propertyValue?.ToString() == _desiredValue.ToString())
            {
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
} 