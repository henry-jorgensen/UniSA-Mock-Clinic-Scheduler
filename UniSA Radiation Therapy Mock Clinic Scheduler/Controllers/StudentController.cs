using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Controllers
{
    public class StudentController : Controller
    {
        FirebaseSharedFunctions firebase;
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
            firebase = new FirebaseSharedFunctions();
        }

        public IActionResult Redirect()
        {
            var UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.VerifyLoggedIn(UserToken).Result == false)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (firebase.LoggedInAsStudent(UserToken).Result == true)
                {
                    return RedirectToAction("Appointments", "Student");
                }
                else
                {
                    return RedirectToAction("Clinics", "Home");
                }
            }
        }

        public IActionResult Appointments()
        {
            var _UserToken = HttpContext.Session.GetString("_UserToken");

            //if (firebase.LoggedInAsStudent(_UserToken).Result == true)
            //{
            //    ViewBag.CurrentUser = firebase.GetUserModelAsync(_UserToken).Result;
            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            return null; // delete this later
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
