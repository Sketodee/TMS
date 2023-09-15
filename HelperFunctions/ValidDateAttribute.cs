using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace TMS.HelperFunctions
{
    public class ValidDateAttribute : ValidationAttribute
    {
        public ValidDateAttribute()
        {
            ErrorMessage = "Date must be a future date and in dd/MMM/yyyy format"; 
        }
        public override bool IsValid(object value)
        {
            if (value == null || !(value is string))
            {
                // If the value is not a string or is null, it's considered invalid.
                return false;
            }

            string appointmentDateStr = (string)value;

            // Specify the expected date format
            string expectedFormat = "dd/MMM/yyyy";

            if (DateTime.TryParseExact(appointmentDateStr, expectedFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                //check if date is in the past 
                if(date < DateTime.Today)
                {
                    return false;
                }
                // Parsing was successful, it's a valid date format
                return true;
            }

            // Parsing failed, it's an invalid date format
            return false;
        }
    }
}
