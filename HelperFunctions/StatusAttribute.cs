using System.ComponentModel.DataAnnotations;

namespace TMS.HelperFunctions
{
    public class StatusAttribute : ValidationAttribute
    {
        private readonly string[] _statusTypes = new[]
     {
            "Pending",
            "In-Progress",
            "Completed"
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string strValue = value.ToString();

            if (!_statusTypes.Contains(strValue))
            {
                string allowedTypes = string.Join(", ", _statusTypes);
                return new ValidationResult($"The value '{strValue}' is not allowed. Allowed types are: {allowedTypes}.");
            }

            return ValidationResult.Success;
        }
    }
}
