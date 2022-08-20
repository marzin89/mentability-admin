using Microsoft.AspNetCore.Mvc;
using Mentability.Models;
using System.Text.Json;
using System.Text;

namespace Mentability.Controllers
{
    // Controller för hämtning, lagring och borttagning av aktiviteter
    public class ActivityController : Controller
    {
        /* Returnerar "standardvyn" och listan på aktiviteter. Längden på listan varierar beroende 
            på det totala antalet aktiviteter och om användaren har valt att visa fler än tre aktiviteter */
        public IActionResult Activity(int? Id)
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
                string uri = "https://localhost:7076/api/Activity";

                // Här lagras aktiviteterna som returneras
                Activities activities = new Activities();

                // Hämtar alla befintliga aktiviteter
                var response = client.GetAsync(uri);
                response.Wait();

                var result = response.Result;

                // Om anropet lyckades
                if (result.IsSuccessStatusCode)
                {
                    // Konverterar svaret från JSON
                    var read = result.Content.ReadFromJsonAsync<List<Activities>>();
                    read.Wait();

                    // Om inget id har skickats med (användaren har inte valt att visa fler än tre aktiviteter)
                    if (Id == null)
                    {
                        // Lagrar högst tre aktiviteter i listan ovan 
                        if (read.Result.Count < 3)
                        {
                            foreach (Activities activity in read.Result)
                            {
                                activities.activities.Add(activity);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                activities.activities.Add(read.Result[i]);
                            }
                        }
                    }
                    // Om ett id har skickats med (användaren har valt att visa fler än tre aktiviteter)
                    else
                    {
                        /* Lagrar antalet aktiviteter som ska returneras i sessionen och i ViewBag. Sex stycken om sessionen är null
                            (första gången användaren klickar på "Visa fler"), annars ytterligare tre aktiviteter */
                        if (HttpContext.Session.GetInt32("NumberOfActivities") == null)
                        {
                            HttpContext.Session.SetInt32("NumberOfActivities", 6);
                            ViewBag.NumberOfActivities = 6;
                        }
                        else
                        {
                            int numberOfActivities = (int)HttpContext.Session.GetInt32("NumberOfActivities") + 3;
                            HttpContext.Session.SetInt32("NumberOfActivities", numberOfActivities);
                            ViewBag.NumberOfActivities = HttpContext.Session.GetInt32("NumberOfActivities");
                        }

                        /* Här beräknas och lagras antalet rader (varför valde jag kolumner i namnet?) i ViewBag.
                            Tre aktiviteter per rad. Om antalet aktiviteter kan delas med tre utan rest, så används det värdet.
                            Annars avrundas resultatet nedåt */
                        if (read.Result.Count % 3 != 0)
                        {
                            float foo = read.Result.Count / 3;
                            int bar = read.Result.Count % 3;

                            ViewBag.NumberOfColumnsActivity = Math.Floor(foo) + bar;
                        }
                        else
                        {
                            ViewBag.NumberOfColumnsActivity = read.Result.Count / 3;
                        }

                        /* Här läggs antalet aktiviteter som ska returneras till i listan i objektet.
                            Om antalet aktiviteter som finns underskrider värdet som finns i ViewBag,
                            läggs alla befintliga aktiviteter till i listan. Annars läggs det antal
                            aktiviteter till som motsvarar värdet i ViewBag */
                        if (read.Result.Count < (int)ViewBag.NumberOfActivities)
                        {
                            foreach (Activities activity in read.Result)
                            {
                                activities.activities.Add(activity);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < (int)ViewBag.NumberOfActivities; i++)
                            {
                                activities.activities.Add(read.Result[i]);
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

                // Returnerar vyn och listan på aktiviteter
                return View(activities);
            }
        }

        // Lägger till en aktivitet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Activities activity)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = "https://localhost:7076/api/Activity";

            // Om formuläret är korrekt ifyllt
            if (ModelState.IsValid)
            {
                // Genererar datumet för aktiviteten
                activity.Date = DateTime.Today;

                /* Om användaren har angett ett slutdatum, och detta datum är tidigare eller samma dag som startdatumet,
                    skickas användaren tillbaka till vyn för aktiviteter */
                if (activity.EndDate != null)
                {
                    if (activity.EndDate <= activity.StartDate)
                    {
                        return RedirectToAction("Activity");
                    }
                }

                // Konverterar aktiviteten till JSON och skapar en body
                var json = JsonSerializer.Serialize(activity);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                // Lägger till aktiviteten
                var response = client.PostAsync(uri, body);
                response.Wait();

                var result = response.Result;

                // True eller false lagras i TempData beroende på om det gick att lägga till aktiviteten eller inte
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadFromJsonAsync<Activities>();
                    read.Wait();

                    TempData["Created"] = true;
                }
                else
                {
                    TempData["Created"] = false;
                }
            }

            // Skickar tillbaka användaren till vyn för aktiviteter
            return RedirectToAction("Activity");
        }

        // Raderar en aktivitet
        public IActionResult Delete(int? Id)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = $"https://localhost:7076/api/Activity/{Id}";

            // Returnerar vyn om inget id finns
            if (Id == null)
            {
                return RedirectToAction("Activity");
            }
            else
            {
                // Raderar aktiviteten
                var response = client.DeleteAsync(uri);
                response.Wait();

                var result = response.Result;

                // True eller false lagras i TempData beroende på om det gick att radera aktiviteten eller inte
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
            return RedirectToAction("Activity");
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

