using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;
using System.Runtime.Intrinsics.Arm;
using Microsoft.Extensions.Logging;

//TESTING
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Notification;

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
            if (firebase.VerifyLoggedinSession(HttpContext).Result == false)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
                {
                    return RedirectToAction("Create", "Coordinator");
                }
                else
                {
                    return RedirectToAction("Clinics", "Home");
                }
            }
        }

        public IActionResult Index()
        {
            if (firebase.VerifyLoggedinSession(HttpContext).Result == false)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
                {
                    return RedirectToAction("CreateClass", "Coordinator");
                }
                else
                {
                    return RedirectToAction("Clinics", "Home");
                }
            }
        }

        //Create clinics page
        //Must be logged into course coordinator account to see
        public IActionResult CreateSchedule()
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
            var _UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.LoggedInAsCoordinator(_UserToken).Result == true)
            {
                List<AppointmentModel> appointments = await firebase.CollectAllAppointmentsAsync(_UserToken);
                ViewBag.Appointments = appointments;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateASchedule(string className, string name, string date, string startTime, string appointmentDuration, string locations, string schedule)
        {
            ScheduleModel scheduleModel = new ScheduleModel(name, date, startTime, appointmentDuration, locations, schedule);

            var _UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.VerifyLoggedIn(_UserToken).Result == false)
            {
                return Forbid();
            }

            string? success = await firebase.CreateNewScheduleAsync(_UserToken, className, scheduleModel);

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
            var _UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.LoggedInAsCoordinator(_UserToken).Result == true)
            {
                ViewBag.CurrentUser = firebase.GetUserModelAsync(_UserToken).Result;
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
            var _UserToken = firebase.VerifyVerificationToken(HttpContext);
            string? success = await firebase.CreateNewClassAsync(_UserToken, classModel);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> LoadAllClasses()
        {
            //TESTING
            //var success = await SendGridMessenger.SendEmail();

            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            var _UserToken = firebase.VerifyVerificationToken(HttpContext);
            List<ClassModel>? success = await firebase.CollectAllClassAsync(_UserToken);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> LoadAClass(string className)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            var _UserToken = firebase.VerifyVerificationToken(HttpContext);
            ClassModel? success = await firebase.CollectClassAsync(_UserToken, className);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> SaveAClassList(string classCode, string className, string[] studentList)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            var _UserToken = firebase.VerifyVerificationToken(HttpContext);
            bool success = await firebase.SaveAClassListAsync(_UserToken, className, studentList);

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

