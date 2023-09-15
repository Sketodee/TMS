using System.ComponentModel.DataAnnotations;

namespace TMS.HelperFunctions
{
    public class PriorityAttribute : ValidationAttribute
    {
        private readonly string[] _priorityTypes = new[]
      {
            "Low",
            "Medium", 
            "High"
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string strValue = value.ToString();

            if (!_priorityTypes.Contains(strValue))
            {
                string allowedPriorityTypes = string.Join(", ", _priorityTypes);
                return new ValidationResult($"The value '{strValue}' is not allowed. Allowed types are: {allowedPriorityTypes}.");
            }

            return ValidationResult.Success;
        }
    }
}
