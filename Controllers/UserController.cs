using Microsoft.AspNetCore.Mvc;
using Mentability.Models;
using System.Text.Json;

namespace Admin.Controllers
{
    // Controller för användare
    public class UserController : Controller
    {
        /* Returnerar inloggningssidan om användaren inte är inloggad.
            Annars skickas användaren till Admin */
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("SignedIn") == "true")
            {
                return RedirectToAction("NewsArticle", "NewsArticle");
                
            } 
            else
            {
                return View();
            }        
        }

        /* Validering (POST). Användaren skickas till Admin vid lyckad validering, 
            eller tillbaka till inloggningssidan */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            // Ny http-klient och URI
            var client = new HttpClient();
            string uri = "https://localhost:7076/api/User";

            // Här lagras befintliga användare
            User users = new User();

            // Hämtar alla befintliga användare
            var response = client.GetAsync(uri);
            response.Wait();

            var result = response.Result;

            // Om anropet lyckades
            if (result.IsSuccessStatusCode)
            {
                // Konverterar svaret från JSON och lagrar användarna i objektet ovan
                var read = result.Content.ReadFromJsonAsync<List<User>>();
                read.Wait();

                users.users = read.Result;
            }

            // Om formuläret är korrekt ifyllt
            if (ModelState.IsValid)
            {
                // Ändras till true om användarnamn och lösenord är korrekta
                bool userExists = false;

                // Loopar igenom alla befintliga användare
                foreach(User element in users.users)
                {
                    /* Om användarnamn och lösenord matchar, lagras status för inloggning i sessionen och 
                        variabelns värde ändras till true */
                    if (element.Username == user.Username & element.Password == user.Password)
                    {
                        HttpContext.Session.SetString("SignedIn", "true");
                        userExists = true;
                    }
                }

                // Status för valideringen lagras i ViewData
                ViewData["UserExists"] = userExists;

                // Om valideringen lyckades, skickas användaren till Admin, annars tillbaka till inloggningssidan
                if (userExists)
                {
                    return RedirectToAction("NewsArticle", "NewsArticle");
                }
                else
                {
                    return View();
                }
            }
            // Returnerar vyn om formuläret inte är korrekt ifyllt
            else
            {
                return View();
            }
        }
    }
}
