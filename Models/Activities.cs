// Imports
using System.ComponentModel.DataAnnotations;
using Admin.Annotations;

namespace Mentability.Models
{
    // Klass som hanterar aktiviteter
    public class Activities
    {
        // Properties med anpassade fältetiketter
        // Id
        public int Id { get; set; }

        // Titel, obligatorisk uppgift. Minst fem och max 100 tecken
        [Display(Name = "Titel *")]
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string? Title { get; set; }

        // Beskrivning, obligatorisk uppgift. Minst 100 och max 1000 tecken
        [Display(Name = "Beskrivning *")]
        [Required]
        [MinLength(100)]
        [MaxLength(1000)]
        public string? Content { get; set; }

        // Sökväg till bild
        public string? ImageUrl { get; set; }

        // Startdatum, obligatorisk uppgift. Måste vara en dag senare än dagens datum
        [Display(Name = "Startdatum*")]
        [Required]
        [StartDateIsGreaterThanToday]
        public DateTime StartDate { get; set; }

        // Slutdatum. Måste vara en dag senare än startdatumet (kontrolleras med JavaScript samt i controllern)
        [Display(Name = "Slutdatum")]
        public DateTime? EndDate { get; set; }

        // Datum för publicering/uppdatering
        public DateTime Date { get; set; }

        // Författare, obligatorisk uppgift. Max 50 tecken
        [Display(Name = "Författare *")]
        [Required]
        [MaxLength(50)]
        public string? Author { get; set; }

        // Listan används vid utskrift av befintliga aktiviteter
        public List<Activities> activities { get; set; } = new List<Activities>();

        // Tom konstruktor
        public Activities() { }
    }
}

