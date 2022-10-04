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

        public IActionResult EditClinic()
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
                Dictionary<string, Dictionary<ScheduleModel, List<AppointmentModel>>>? classes = await firebase.CollectClassSchedules(HttpContext);
                if (classes != null)
                {
                    ViewBag.Classes = classes;
                }

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

                Dictionary<string, Array>? studentInformation = await firebase.GetStudentsAsync();
                Dictionary<string, string> studentList = new Dictionary<string, string>();
                
                ViewBag.StudentList = studentList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        [HttpPost]
        public IActionResult EditAppointmentPost(string apptId, string time, string date, string patient, string rt1, string rt2, string infect, string room, string site)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                firebase.EditAppointmentAsync(apptId, time, date, patient, rt1, rt2, infect, room, site);
                return RedirectToAction("EditAppointment", new { id = apptId });
            } else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadASchedule(string scheduleId)
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result == false)
            {
                return Forbid();
            }

            ScheduleModel? response = await firebase.CollectASchedule(HttpContext, scheduleId);

            if (response != null)
            {
                Console.WriteLine(response.ToString());
                return Ok(response);
            }

            return BadRequest();
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

        public async Task<IActionResult> Classes()
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                //TODO GET ALL CLASSES AND STUDENTS
                Dictionary<string, ClassModel>? classes = await firebase.GetAllClassesAndStudents(HttpContext);
                ViewBag.Classes = classes;
                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        //Create clinics page
        //Must be logged into course coordinator account to see
        public IActionResult CreateClass(string name, string code)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == true)
            {
                ViewBag.CurrentUser = firebase.GenerateUserModel(HttpContext).Result;
                ViewBag.Name = name;
                ViewBag.Code = code;
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
        public async Task<IActionResult> DeleteAClass(string className, string classCode)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            bool success = await firebase.DeleteClassAsync(HttpContext, className, classCode);

            if (success) return Ok(success);

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> SaveAClassList(string className, string classCode, string[] studentList)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            bool success = await firebase.SaveAClassListAsync(HttpContext, className, classCode, studentList);

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

