using Microsoft.AspNetCore.Mvc;
using Mentability.Models;
using System.Text.Json;
using System.Text;

namespace Admin.Controllers
{
    // Controller för redigering och uppdatering av citat
    public class EditQuoteController : Controller
    {
        // Redigering. Hämtar och returnerar citatet vid klick på "Redigera"
        public IActionResult EditQuote(int? Id)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = $"https://localhost:7076/api/Quote/{Id}";

            // Här lagras citatet
            Quote quote = new Quote();

            // Returnerar vyn om inget id finns
            if (Id == null)
            {
                return View();
            }
            else
            {
                // Hämtar citatet
                var response = client.GetAsync(uri);
                response.Wait();

                var result = response.Result;

                // Om anropet lyckades
                if (result.IsSuccessStatusCode)
                {
                    // Konverterar svaret från JSON och lagrar i objektet ovan
                    var read = result.Content.ReadFromJsonAsync<Quote>();
                    read.Wait();

                    quote = read.Result;
                }
            }

            // Returnerar vyn och citatet
            return View(quote);
        }

        // Uppdatering
        [HttpPost]
        public IActionResult UpdateQuote(Quote quote)
        {
            // Lagrar citatets id
            int id = quote.Id;

            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = $"https://localhost:7076/api/Quote/{id}";

            // Om formuläret är korrekt ifyllt
            if (ModelState.IsValid)
            {
                // Uppdaterar datumet för citatet
                quote.Date = DateTime.Today;

                // Konverterar citatet till JSON och skapar en body
                var json = JsonSerializer.Serialize(quote);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                // Uppdaterar citatet
                var response = client.PutAsync(uri, body);
                response.Wait();

                var result = response.Result;

                // True eller false lagras i TempData beroende på om det gick att uppdatera citatet eller inte
                if (result.IsSuccessStatusCode)
                {
                    TempData["Updated"] = true;
                }
                else
                {
                    TempData["Updated"] = false;
                }
            }
            // Returnerar vyn för redigering av citat om formuläret inte är korrekt ifyllt
            else
            {
                return RedirectToAction("EditQuote", quote);
            }

            // Returnerar vyn för citat
            return RedirectToAction("Quote", "Quote");
        }

        // Utloggning. Tömmer sessionen, ViewBag och skickar användaren till inloggningssidan
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            ViewBag.SignedIn = false;

            return RedirectToAction("Login", "User");
        }
    }
}
