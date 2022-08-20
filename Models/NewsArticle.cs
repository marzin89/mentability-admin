// Imports
using System.ComponentModel.DataAnnotations;

namespace Mentability.Models
{
    // Klass som hanterar nyheter
    public class NewsArticle
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

        // Innehåll, obligatorisk uppgift. Minst 300 och max 1000 tecken
        [Display(Name = "Innehåll *")]
        [Required]
        [MinLength(300)]
        [MaxLength(1000)]
        public string? Content { get; set; }

        // Sökväg till bild
        public string? ImageUrl { get; set; }

        // Datum för publicering/uppdatering
        public DateTime Date { get; set; }

        // Författare, obligatorisk uppgift.Max 50 tecken
        [Display(Name = "Författare *")]
        [Required]
        [MaxLength(50)]
        public string? Author { get; set; }

        // Listan används vid utskrift av befintliga nyheter
        public List<NewsArticle>? articles { get; set; } = new List<NewsArticle>();

        // Tom konstruktor
        public NewsArticle() { }
    }
}

