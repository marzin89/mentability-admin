// Imports
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mentability.Models
{
    // Klass som hanterar citat
    public class Quote
    {
        // Properties med anpassade fältetiketter
        // Id
        public int Id { get; set; }

        // Citat, obligatorisk uppgift. Minst 20 och max 100 tecken
        [Required]
        [Display(Name = "Citat *")]
        [MinLength(20)]
        [MaxLength(100)]
        public string? Content { get; set; }

        // Upphovsperson, obligatorisk uppgift.Max 50 tecken
        [Required]
        [Display(Name = "Upphovsperson *")]
        [MaxLength(50)]
        public string? Author { get; set; }

        // Användare
        public string? User { get; set; }

        // Datum för publicering/uppdatering
        public DateTime Date { get; set; }

        // Listan används vid utskrift av befintliga citat
        public List<Quote>? quotes { get; set; } = new List<Quote>();

        // Tom konstruktor
        public Quote() { }
    }
}

