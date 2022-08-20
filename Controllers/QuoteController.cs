using Microsoft.AspNetCore.Mvc;
using Mentability.Models;
using System.Text.Json;
using System.Text;

namespace Mentability.Controllers
{
    // Controller för hämtning, lagring och borttagning av citat
    public class QuoteController : Controller
    {
        /* Returnerar "standardvyn" och listan på citat. Längden på listan varierar beroende 
            på det totala antalet citat och om användaren har valt att visa fler än tre citat */
        public IActionResult Quote(int? Id)
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
                string uri = "https://localhost:7076/api/Quote";

                // Här lagras citaten som returneras
                Quote quotes = new Quote();

                // Hämtar alla befintliga citat
                var response = client.GetAsync(uri);
                response.Wait();

                var result = response.Result;

                // Om anropet lyckades
                if (result.IsSuccessStatusCode)
                {
                    // Konverterar svaret från JSON
                    var read = result.Content.ReadFromJsonAsync<List<Quote>>();
                    read.Wait();

                    // Om inget id har skickats med (användaren har inte valt att visa fler än tre citat)
                    if (Id == null)
                    {
                        // Lagrar högst tre citat i listan ovan
                        if (read.Result.Count < 3)
                        {
                            foreach (Quote quote in read.Result)
                            {
                                quotes.quotes.Add(quote);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                quotes.quotes.Add(read.Result[i]);
                            }
                        }
                    }
                    // Om ett id har skickats med (användaren har valt att visa fler än tre aktiviteter)
                    else
                    {
                        /* Lagrar antalet citat som ska returneras i sessionen och i ViewBag. Sex stycken om sessionen är null
                            (första gången användaren klickar på "Visa fler"), annars ytterligare tre citat */
                        if (HttpContext.Session.GetInt32("NumberOfQuotes") == null)
                        {
                            HttpContext.Session.SetInt32("NumberOfQuotes", 6);
                            ViewBag.NumberOfQuotes = 6;
                        }
                        else
                        {
                            int numberOfQuotes = (int)HttpContext.Session.GetInt32("NumberOfQuotes") + 3;
                            HttpContext.Session.SetInt32("NumberOfQuotes", numberOfQuotes);
                            ViewBag.NumberOfQuotes = HttpContext.Session.GetInt32("NumberOfQuotes");
                        }

                        /* Här beräknas och lagras antalet rader (varför valde jag kolumner i namnet?) i ViewBag.
                            Tre citat per rad. Om antalet citat kan delas med tre utan rest, så används det värdet.
                            Annars avrundas resultatet nedåt */
                        if (read.Result.Count % 3 != 0)
                        {
                            float foo = read.Result.Count / 3;
                            int bar = read.Result.Count % 3;

                            ViewBag.NumberOfColumnsQuote = Math.Floor(foo) + bar;
                        }
                        else
                        {
                            ViewBag.NumberOfColumnsQuote = read.Result.Count / 3;
                        }

                        /* Här läggs antalet citat som ska returneras till i listan i objektet.
                            Om antalet citat som finns underskrider värdet som finns i ViewBag,
                            läggs alla befintliga citat till i listan. Annars läggs det antal
                            citat till som motsvarar värdet i ViewBag */
                        if (read.Result.Count < (int)ViewBag.NumberOfQuotes)
                        {
                            foreach (Quote quote in read.Result)
                            {
                                quotes.quotes.Add(quote);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < (int)ViewBag.NumberOfQuotes; i++)
                            {
                                quotes.quotes.Add(read.Result[i]);
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

                // Returnerar vyn och listan på citat
                return View(quotes);
            }
        }

        // Lägger till ett citat
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Quote quote)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = "https://localhost:7076/api/Quote";

            // Om formuläret är korrekt ifyllt
            if (ModelState.IsValid)
            {
                // Genererar datumet för citatet
                quote.Date = DateTime.Today;

                // Konverterar citatet till JSON och skapar en body
                var json = JsonSerializer.Serialize(quote);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                // Lägger till citatet
                var response = client.PostAsync(uri, body);
                response.Wait();

                var result = response.Result;

                // True eller false lagras i TempData beroende på om det gick att lägga till citatet eller inte
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadFromJsonAsync<Quote>();
                    read.Wait();

                    TempData["Created"] = true;

                }
                else
                {
                    TempData["Created"] = false;
                }
            }

            // Skickar tillbaka användaren till vyn för citat
            return RedirectToAction("Quote");
        }

        // Raderar ett citat
        public IActionResult Delete(int? Id)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = $"https://localhost:7076/api/Quote/{Id}";

            // Returnerar vyn om inget id finns
            if (Id == null)
            {
                return RedirectToAction("Quote");
            }
            else
            {
                // Raderar citatet
                var response = client.DeleteAsync(uri);
                response.Wait();

                var result = response.Result;

                // True eller false lagras i TempData beroende på om det gick att radera citatet eller inte
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
            return RedirectToAction("Quote");
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
