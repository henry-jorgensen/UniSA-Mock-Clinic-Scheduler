using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;
//using System.Runtime.Intrinsics.Arm;

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
            if (firebase.VerifyLoggedInSession(HttpContext).Result == false)
            {
                return RedirectToAction("Login", "Account");
            }

            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                return RedirectToAction("CreateClass", "Coordinator");
            }
            else
            {
                return RedirectToAction("Clinics", "Home");
            }
        }

        //Basic home page
        public IActionResult Index()
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                return RedirectToAction("Clinics", "Home");
            }

            return RedirectToAction("Login", "Account");
        }

        //Scheduled clinics page
        //Must be logged into a valid account to see
        public async Task<IActionResult> Clinics()
        {            
            //Need to organise in time/data order
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                SortedList<string, List<AppointmentModel>>? appointments = await firebase.CollectAllAppointmentsAsync(HttpContext, false);
                ViewBag.currentUser = "Course"; //hardcoded as not needed for course coordinator
                ViewBag.Appointments = appointments;
                ViewBag.AllAppointments = null;
                return View();
            }
            else if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                Dictionary<string, SortedList<string, List<AppointmentModel>>>? appointments = await firebase.CollectStudentsAppointmentsAsync(HttpContext, false);
                var first = appointments.First();
                ViewBag.currentUser = first.Key;
                ViewBag.Appointments = first.Value;

                //Need to get all the appointments that a student may be in...
                SortedList<string, List<AppointmentModel>> second = appointments["ALL"];
                ViewBag.AllAppointments = second;
                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        public IActionResult ClinicDay()
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == true)
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
        public async Task<IActionResult> History()
        {
            //Need to organise in time/data order
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                SortedList<string, List<AppointmentModel>>? appointments = await firebase.CollectAllAppointmentsAsync(HttpContext, true);
                ViewBag.currentUser = "Course"; //hardcoded as not needed for course coordinator
                ViewBag.Appointments = appointments;
                return View();
            }
            else if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                Dictionary<string, SortedList<string, List<AppointmentModel>>>? appointments = await firebase.CollectStudentsAppointmentsAsync(HttpContext, true);
                var first = appointments.First();
                ViewBag.currentUser = first.Key;
                ViewBag.Appointments = first.Value;
                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// A basic treatment page that relates to the appointment that has been selected
        /// </summary>
        /// <returns>A redirect action dependent on whether a user is logged in</returns>
        public IActionResult Treatment(string date, string time, string room, string patient, string infectious, string rt1, string rt2, string site, string ID, string complication)
        {                                    
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                AppointmentModel appointment = new AppointmentModel(date, time, room, patient, infectious, rt1, rt2, site, complication, ID);
                ViewBag.Appointment = appointment;

                return View();
            }
            
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPDF(FileModel file)
        {
            if (!ModelState.IsValid) return BadRequest();

            if (firebase.VerifyLoggedInSession(HttpContext).Result == false) return Forbid();

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string success = await firebase.UploadPDF(file);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> RetrievePDF(FileModel file)
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result == false) return Forbid();

            string link = await firebase.RetrievePDF(file);

            if (link != null) return Ok(link);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePDF(FileModel file)
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result == false) return Forbid();

            bool success = await firebase.DeletePDF(file);

            if (success) return Ok();

            return BadRequest();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}