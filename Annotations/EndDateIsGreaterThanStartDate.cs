using System.ComponentModel.DataAnnotations;

namespace Admin.Annotations
{
    public class EndDateIsGreaterThanStartDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime) value <= DateTime.Today)
            {
                string errorMessage = "Slutdatumet måste vara minst en dag senare än startdatumet";
                return new ValidationResult(errorMessage);
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
