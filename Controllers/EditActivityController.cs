using Microsoft.AspNetCore.Mvc;
using Mentability.Models;
using System.Text.Json;
using System.Text;

namespace Admin.Controllers
{
    // Controller för redigering och uppdatering av aktiviteter
    public class EditActivityController : Controller
    {
        // Redigering. Hämtar och returnerar aktiviteten vid klick på "Redigera"
        public IActionResult EditActivity(int? Id)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = $"https://localhost:7076/api/Activity/{Id}";

            // Här lagras aktiviteten
            Activities activity = new Activities();

            // Returnerar vyn om inget id finns
            if (Id == null)
            {
                return View();
            }
            else
            {
                // Hämtar aktiviteten
                var response = client.GetAsync(uri);
                response.Wait();

                var result = response.Result;

                // Om anropet lyckades
                if (result.IsSuccessStatusCode)
                {
                    // Konverterar svaret från JSON och lagrar i objektet ovan
                    var read = result.Content.ReadFromJsonAsync<Activities>();
                    read.Wait();

                    activity = read.Result;
                }
            }

            // Returnerar vyn och aktiviteten
            return View(activity);
        }

        // Uppdatering
        public IActionResult UpdateActivity(Activities activity)
        {
            // Lagrar aktivitetens id
            int id = activity.Id;

            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = $"https://localhost:7076/api/Activity/{id}";

            // Om formuläret är korrekt ifyllt
            if (ModelState.IsValid)
            {
                // Uppdaterar datumet för aktiviteten
                activity.Date = DateTime.Today;

                /* Om användaren har angett ett slutdatum, och detta datum är tidigare eller samma dag som startdatumet,
                    skickas användaren tillbaka till vyn för redigering av aktiviteter */
                if (activity.EndDate != null)
                {
                    if (activity.EndDate <= activity.StartDate)
                    {
                        return RedirectToAction("EditActivity", activity);
                    }
                }

                // Konverterar aktiviteten till JSON och skapar en body
                var json = JsonSerializer.Serialize(activity);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                // Uppdaterar aktiviteten
                var response = client.PutAsync(uri, body);
                response.Wait();

                var result = response.Result;

                // True eller false lagras i TempData beroende på om det gick att uppdatera aktiviteten eller inte
                if (result.IsSuccessStatusCode)
                {
                    TempData["Updated"] = true;
                }
                else
                {
                    TempData["Updated"] = false;
                }
            }
            // Returnerar vyn för redigering av aktiviteter om formuläret inte är korrekt ifyllt
            else
            {
                return RedirectToAction("EditActivity", activity);
            }

            // Returnerar vyn för aktiviteter
            return RedirectToAction("Activity", "Activity");
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
