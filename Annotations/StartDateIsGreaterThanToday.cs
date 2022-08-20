using System.ComponentModel.DataAnnotations;

namespace Admin.Annotations
{
    // Data annotation för aktiviteter som kontrollerar att startdatumet är senare än dagens datum
    public class StartDateIsGreaterThanToday : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Returnerar ett felmeddelande om startdatumet är tidigare eller samma dag som dagens datum
            if ((DateTime) value <= DateTime.Today)
            {
                string errorMessage = "Startdatumet måste vara minst en dag senare än dagens datum";
                return new ValidationResult(errorMessage);
            }
            // Annars är datumet giltigt
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
