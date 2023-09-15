using System.ComponentModel.DataAnnotations;

namespace TMS.HelperFunctions
{
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StatusAndPriorityAttribute : ValidationAttribute
    {
        private readonly string[] allowedStatusOptions = { "Pending", "In-Progress" , "Completed"};
        private readonly string[] allowedPriorityOptions = { "Low", "Medium", "High" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                // Handle the case where the value is null (customize as needed)
                return ValidationResult.Success;
            }

            var propertyValue = value.ToString();

            if (allowedStatusOptions.Contains(propertyValue) || allowedPriorityOptions.Contains(propertyValue))
            {
                return ValidationResult.Success;
            }

            // If the value is not in the allowed options for both Status and Priority, return an error message
            return new ValidationResult(ErrorMessage);
        }
    }
}
