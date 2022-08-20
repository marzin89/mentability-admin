// Imports
using System.ComponentModel.DataAnnotations;

namespace Mentability.Models
{
    // Klass som hanterar användare
    public class User
    {
        // Properties med anpassade fältetiketter och felmeddelanden
        // Id
        public int Id { get; set; }

        // Användarnamn, obligatorisk uppgift
        [Display(Name = "Användarnamn *")]
        [Required(ErrorMessage = "Du måste ange ditt användarnamn")]
        public string? Username { get; set; }

        // Lösenord, obligatorisk uppgift
        [Display(Name = "Lösenord *")]
        [Required(ErrorMessage = "Du måste ange ditt lösenord")]
        public string? Password { get; set; }

        // Förnamn
        public string? FirstName { get; set; }

        // Efternamn
        public string? LastName { get; set; }

        // E-post
        public string? Email { get; set; }

        // Listan används vid validering i controllern
        public IEnumerable<User>? users { get; set; }

        // Tom konstruktor
        public User() { }
    }
}

