using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;
using System.Runtime.Intrinsics.Arm;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        FirebaseSharedFunctions firebase;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            firebase = new FirebaseSharedFunctions();
        }

        //Basic home page
        public IActionResult Index()
        {
            var UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.VerifyLoggedIn(UserToken).Result == false)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (firebase.LoggedInAsCoordinator(UserToken).Result == true)
                {
                    return RedirectToAction("Create", "Coordinator");
                }   
                else
                {
                    return RedirectToAction("Clinics", "Home");
                }
            }
        }

        //Scheduled clinics page
        //Must be logged into a valid account to see
        public IActionResult Clinics()
        {
            var UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.VerifyLoggedIn(UserToken).Result == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //Historical clinics page
        //Must be logged into a valid account to see
        public IActionResult History()
        {
            var UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.VerifyLoggedIn(UserToken).Result == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}