using Mentability.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Mentability.Controllers
{
    // Controller för hämtning, lagring och borttagning av nyheter
    public class NewsArticleController : Controller
    {
        /* Returnerar "standardvyn" och listan på nyheter. Längden på listan varierar beroende 
            på det totala antalet nyheter och om användaren har valt att visa fler än tre nyheter */
        public IActionResult NewsArticle(int? Id)
        {
            // Skickar användaren till Logga in om användaren inte är inloggad
            if (HttpContext.Session.GetString("SignedIn") == null)
            {
                return RedirectToAction("Login", "User");

            }
            // Om användaren är inloggad
            else
            {
                // Ny http-klient och URI
                var client = new HttpClient();
                string uri = "https://localhost:7076/api/NewsArticle";

                // Här lagras nyheterna som returneras
                NewsArticle articles = new NewsArticle();

                // Hämtar alla befintliga nyheter
                var response = client.GetAsync(uri);
                response.Wait();

                var result = response.Result;

                // Om anropet lyckades
                if (result.IsSuccessStatusCode)
                {
                    // Konverterar svaret från JSON
                    var read = result.Content.ReadFromJsonAsync<List<NewsArticle>>();
                    read.Wait();

                    // Om inget id har skickats med (användaren har inte valt att visa fler än tre nyheter)
                    if (Id == null)
                    {
                        // Lagrar högst tre nyheter i listan ovan
                        if (read.Result.Count < 3)
                        {
                            foreach (NewsArticle article in read.Result)
                            {
                                articles.articles.Add(article);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                articles.articles.Add(read.Result[i]);
                            }
                        }
                    }
                    // Om ett id har skickats med (användaren har valt att visa fler än tre aktiviteter)
                    else
                    {
                        /* Lagrar antalet nyheter som ska returneras i sessionen och i ViewBag. Sex stycken om sessionen är null
                            (första gången användaren klickar på "Visa fler"), annars ytterligare tre nyheter */
                        if (HttpContext.Session.GetInt32("NumberOfArticles") == null)
                        {
                            HttpContext.Session.SetInt32("NumberOfArticles", 6);
                            ViewBag.NumberOfArticles = 6;
                        }
                        else
                        {
                            int numberOfArticles = (int)HttpContext.Session.GetInt32("NumberOfArticles") + 3;
                            HttpContext.Session.SetInt32("NumberOfArticles", numberOfArticles);
                            ViewBag.NumberOfArticles = HttpContext.Session.GetInt32("NumberOfArticles");
                        }

                        /* Här beräknas och lagras antalet rader (varför valde jag kolumner i namnet?) i ViewBag.
                            Tre nyheter per rad. Om antalet nyheter kan delas med tre utan rest, så används det värdet.
                            Annars avrundas resultatet nedåt */
                        if (read.Result.Count % 3 != 0)
                        {
                            float foo = read.Result.Count / 3;
                            int bar = read.Result.Count % 3;

                            ViewBag.NumberOfColumnsArticle = Math.Floor(foo) + bar;
                        }
                        else
                        {
                            ViewBag.NumberOfColumnsArticle = read.Result.Count / 3;
                        }

                        /* Här läggs antalet nyheter som ska returneras till i listan i objektet.
                            Om antalet nyheter som finns underskrider värdet som finns i ViewBag,
                            läggs alla befintliga nyheter till i listan. Annars läggs det antal
                            nyheter till som motsvarar värdet i ViewBag */
                        if (read.Result.Count < (int)ViewBag.NumberOfArticles)
                        {
                            foreach (NewsArticle article in read.Result)
                            {
                                articles.articles.Add(article);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < (int)ViewBag.NumberOfArticles; i++)
                            {
                                articles.articles.Add(read.Result[i]);
                            }
                        }
                    }
                }

                /* Här lagras status för lagring, uppdatering och borttagning i ViewBag.
                    TempData används eftersom dessa metoder har andra namn och RedirectToAction används. 
                    ViewBag och ViewData kan då inte användas */
                if (TempData["Created"] != null)
                {
                    ViewBag.Created = TempData["Created"];
                }

                if (TempData["Updated"] != null)
                {
                    ViewBag.Updated = TempData["Updated"];
                }

                if (TempData["Deleted"] != null)
                {
                    ViewBag.Deleted = TempData["Deleted"];
                }

                // Returnerar vyn och listan på nyheter
                return View(articles);
            }
        }

        // Lägger till en nyhet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NewsArticle article)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = "https://localhost:7076/api/NewsArticle";

            // Om formuläret är korrekt ifyllt
            if (ModelState.IsValid)
            {
                // Genererar datumet för nyheten
                article.Date = DateTime.Today;

                // Konverterar nyheten till JSON och skapar en body
                var json = JsonSerializer.Serialize(article);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                // Lägger till nyheten
                var response = client.PostAsync(uri, body);
                response.Wait();

                var result = response.Result;

                // True eller false lagras i TempData beroende på om det gick att lägga till nyheten eller inte
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadFromJsonAsync<NewsArticle>();
                    read.Wait();

                    TempData["Created"] = true;
                }
                else
                {
                    TempData["Created"] = false;
                }
            }

            // Skickar tillbaka användaren till vyn för nyheter
            return RedirectToAction("NewsArticle");
        }

        // Raderar en nyhet
        public IActionResult Delete(int? Id)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = $"https://localhost:7076/api/NewsArticle/{Id}";

            // Returnerar vyn om inget id finns
            if (Id == null)
            {
                return RedirectToAction("NewsArticle");
            }
            else
            {
                // Raderar nyheten
                var response = client.DeleteAsync(uri);
                response.Wait();

                var result = response.Result;

                // True eller false lagras i TempData beroende på om det gick att radera nyheten eller inte
                if (result.IsSuccessStatusCode)
                {
                    TempData["Deleted"] = true;
                }
                else
                {
                    TempData["Deleted"] = false;
                }
            }

            // Returnerar vyn
            return RedirectToAction("NewsArticle");
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