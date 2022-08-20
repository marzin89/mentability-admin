using Microsoft.AspNetCore.Mvc;
using Mentability.Models;
using System.Text.Json;
using System.Text;

namespace Admin.Controllers
{
    // Controller för redigering och uppdatering av nyheter
    public class EditNewsArticleController : Controller
    {
        // Redigering. Hämtar och returnerar nyheten vid klick på "Redigera"
        public IActionResult EditNewsArticle(int? Id)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = $"https://localhost:7076/api/NewsArticle/{Id}";

            // Här lagras nyheten
            NewsArticle article = new NewsArticle();

            // Returnerar vyn om inget id finns
            if (Id == null)
            {
                return View();
            }
            else
            {
                // Hämtar nyheten
                var response = client.GetAsync(uri);
                response.Wait();

                var result = response.Result;

                // Om anropet lyckades
                if (result.IsSuccessStatusCode)
                {
                    // Konverterar svaret från JSON och lagrar i objektet ovan
                    var read = result.Content.ReadFromJsonAsync<NewsArticle>();
                    read.Wait();

                    article = read.Result;
                }
            }

            // Returnerar vyn och nyheten
            return View(article);
        }

        // Uppdatering
        [HttpPost]
        public IActionResult UpdateNewsArticle(NewsArticle article)
        {
            // Lagrar nyhetens id
            int id = article.Id;

            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = $"https://localhost:7076/api/NewsArticle/{id}";

            // Om formuläret är korrekt ifyllt
            if (ModelState.IsValid)
            {
                // Uppdaterar datumet för nyheten
                article.Date = DateTime.Today;

                // Konverterar nyheten till JSON och skapar en body
                var json = JsonSerializer.Serialize(article);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                // Uppdaterar nyheten
                var response = client.PutAsync(uri, body);
                response.Wait();

                var result = response.Result;

                // True eller false lagras i TempData beroende på om det gick att uppdatera nyheten eller inte
                if (result.IsSuccessStatusCode)
                {
                    TempData["Updated"] = true;
                }
                else
                {
                    TempData["Updated"] = false;
                }
            }
            // Returnerar vyn för redigering av nyheter om formuläret inte är korrekt ifyllt
            else
            {
                return RedirectToAction("EditNewsArticle", article);
            }

            // Returnerar vyn för nyheter
            return RedirectToAction("NewsArticle", "NewsArticle");
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
