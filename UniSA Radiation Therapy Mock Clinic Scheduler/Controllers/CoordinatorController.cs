using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;
//using System.Runtime.Intrinsics.Arm;
//using Microsoft.Extensions.Logging;

////TESTING
//using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Notification;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Controllers
{
    public class CoordinatorController : Controller
    {
        FirebaseSharedFunctions firebase;
        private readonly ILogger<CoordinatorController> _logger;

        public CoordinatorController(ILogger<CoordinatorController> logger)
        {
            _logger = logger;
            firebase = new FirebaseSharedFunctions(HttpContext);
        }

        public IActionResult Redirect()
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result == false)
            {
                return RedirectToAction("Login", "Account");
            }

            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                return RedirectToAction("Create", "Coordinator");
            }
            else
            {
                return RedirectToAction("Clinics", "Home");
            }
        }

        public IActionResult Index()
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

        public IActionResult Classes()
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        //Create clinics page
        //Must be logged into course coordinator account to see
        public IActionResult CreateClinic()
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                ViewBag.CurrentUser = firebase.GenerateUserModel(HttpContext).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> Clinics()
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == true)
            {
                List<AppointmentModel>? appointments = await firebase.CollectAllAppointmentsAsync(HttpContext);
                ViewBag.Appointments = appointments;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> EditAppointment(string id)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                AppointmentModel? appointment = await firebase.GetSingleAppointmentAsync(id);
                ViewBag.Appointment = appointment;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        public IActionResult EditAppointmentPost(string time, string date, string patient, string rt1, string rt2, string infect, string room, string site)
        {
            Debug.WriteLine(time + " ADN SONE MORE" + date + "LLL " + infect);
            return RedirectToAction("EditAppointment");
        }

        [HttpPost]
        public async Task<IActionResult> SaveAClinic(string className, string name, string date, string startTime, string appointmentDuration, string locations, string schedule)
        {
            ScheduleModel scheduleModel = new ScheduleModel(name, date, startTime, appointmentDuration, locations, schedule);

            if (firebase.VerifyLoggedInSession(HttpContext).Result == false)
            {
                return Forbid();
            }

            string? success = await firebase.CreateNewClinicAsync(HttpContext, className, scheduleModel);

            if (success != null)
            {
                return Ok(success);
            }

            return BadRequest();
        }

        //Create clinics page
        //Must be logged into course coordinator account to see
        public IActionResult CreateClass()
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == true)
            {
                ViewBag.CurrentUser = firebase.GenerateUserModel(HttpContext).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAClass(string name, string studyPeriod, string semester, string year)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            ClassModel classModel = new ClassModel(name, studyPeriod, semester, year);

            string? success = await firebase.CreateNewClassAsync(HttpContext, classModel);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> LoadAllClasses()
        {
            //TESTING
            //var success = await SendGridMessenger.SendEmail();

            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            List<ClassModel>? success = await firebase.CollectAllClassAsync(HttpContext);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> LoadAClass(string className)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            ClassModel? success = await firebase.CollectClassAsync(HttpContext, className);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> SaveAClassList(string classCode, string[] studentList)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            bool success = await firebase.SaveAClassListAsync(HttpContext, classCode, studentList);

            if (success) return Ok(success);

            return BadRequest();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

