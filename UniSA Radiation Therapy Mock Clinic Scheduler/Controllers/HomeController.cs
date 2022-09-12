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
            firebase = new FirebaseSharedFunctions(HttpContext);
        }

        //Basic home page
        public IActionResult Redirect()
        {
            var UserName = HttpContext.Session.GetString("_UserName");
            
            if (firebase.VerifyLoggedinSession(HttpContext).Result == false && firebase.VerifyAnonymousLoggedIn(UserName).Result == false)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (firebase.VerifyLoggedinSession(HttpContext).Result)
                {
                    return RedirectToAction("CreateClass", "Coordinator");
                }
                else
                {
                    return RedirectToAction("Clinics", "Home");
                }
            }
        }

        //Basic home page
        public IActionResult Index()
        {
            var UserName = HttpContext.Session.GetString("_UserName");

            if (firebase.VerifyLoggedinSession(HttpContext).Result == false && firebase.VerifyAnonymousLoggedIn(UserName).Result == false)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Clinics", "Home");
            }
        }

        //Scheduled clinics page
        //Must be logged into a valid account to see
        public async Task<IActionResult> Clinics()
        {
            var UserName = HttpContext.Session.GetString("_UserName");
            
            if (firebase.VerifyLoggedinSession(HttpContext).Result == false && firebase.VerifyAnonymousLoggedIn(UserName).Result == false)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                List<AppointmentModel> appointments = await firebase.CollectStudentsAppointmentsAsync(_UserToken);
                ViewBag.Appointments = appointments;
                return View();
            }
        }

        //Historical clinics page
        //Must be logged into a valid account to see
        public IActionResult History()
        {
            if (firebase.VerifyLoggedinSession(HttpContext).Result)
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