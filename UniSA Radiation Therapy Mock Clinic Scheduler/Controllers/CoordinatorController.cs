using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Notification;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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

        public IActionResult EditClass()
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
        public async Task<IActionResult> UpdateAppointments(List<string> appointmentDetails)
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result == false)
            {
                return Forbid();
            }

            bool? success = await firebase.UpdateAppointmentsAsync(HttpContext, appointmentDetails);

            if (success != null)
            {
                return Ok(success);
            }

            return BadRequest();
        }

        [HttpPost]
        public IActionResult EditAppointmentPost(string apptId, string schedulecode, string time, string date, string patient, string rt1, string rt2, string infect, string room, string site, string complication)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result)
            {
                firebase.EditAppointmentAsync(apptId, schedulecode, time, date, patient, rt1, rt2, infect, room, site, complication);
                return RedirectToAction("Clinics");
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

        [HttpPost]
        public async Task<IActionResult> EditAClinic(string code, string name, string date, string startTime, string appointmentDuration, string locations, string schedule)
        {
            //Create a new schedule model but with the supplied code.
            ScheduleModel scheduleModel = new ScheduleModel(name, date, startTime, appointmentDuration, locations, schedule, code);

            if (firebase.VerifyLoggedInSession(HttpContext).Result == false)
            {
                return Forbid();
            }

            ScheduleModel? success = await firebase.EditClinicAsync(HttpContext, scheduleModel);

            if (success != null)
            {
                return Ok(success);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteASchedule(string scheduleCode, string className)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            string? success = await firebase.DeleteScheduleAsync(HttpContext, scheduleCode, className);

            if (success != null) return Ok(success);

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

        [HttpPost]
        public async Task<IActionResult> EditAClass(string oldName, string name, string studyPeriod, string semester, string year, string code)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            ClassModel classModel = new ClassModel(name, studyPeriod, semester, year);
            classModel.ClassCode = code; //override the auto-generated one

            string? success = await firebase.EditClassAsync(HttpContext, oldName, classModel);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> LoadAllClasses()
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            List<ClassModel>? success = await firebase.CollectAllClassAsync(HttpContext);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> LoadAllClinics(string className)
        {
            Console.WriteLine(className);

            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            var success = await firebase.CollectAllClinicAsync(HttpContext, className);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        /// <summary>
        /// Load a group of schedules for the Clinic Day - only available for course coordinators
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LoadScheduleAppointments(string scheduleId)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            var success = await firebase.CollectScheduleAppointmentsAsync(HttpContext, scheduleId);

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
        public async Task<IActionResult> DeleteAClass(string className)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            string? success = await firebase.DeleteClassAsync(HttpContext, className);

            if (success != null) return Ok(success);

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

        //TODO FINISH THIS AREA OFF
        //============================================
        //EMAIL AREA
        //============================================
        [HttpPost]
        public async Task<IActionResult> EmailAPatient(string date, string username, string name, string complication, string appointmentRef)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            string email = username + "@mymail.unisa.edu.au";

            var success = true;
            //UNCOMMENT TO ENABLE EMAILS
            //var success = await SendGridMessenger.SendEmail(date, name, complication, email);

            //Update appointment emailed status
            await firebase.UpdateEmailAppointmentAsync(appointmentRef, true);

            if (success != null) return Ok(success);

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> EmailAllPatients(string patientList, string scheduleCode)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            try
            {
                JObject? patientJSON = (JObject)JsonConvert.DeserializeObject(patientList);

                foreach (var patient in patientJSON)
                {
                    string date = patient.Value.Value<string>("date");
                    string username = patient.Value.Value<string>("username");
                    string name = patient.Value.Value<string>("name");
                    string complication = patient.Value.Value<string>("complication");
                    string email = username + "@mymail.unisa.edu.au";

                    //UNCOMMENT TO ENABLE EMAILS
                    //await SendGridMessenger.SendEmail(date, name, complication, email);

                    //Update appointment emailed status
                    //await firebase.UpdateEmailAppointmentAsync(patient.Value.Value<string>("appointmentRef"), true);
                }
            } catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok();
        }

        //============================================
        //CLINIC SITES AREA
        //============================================
        [HttpGet]
        public async Task<IActionResult> CollectSites()
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            List<string>? sites = await firebase.CollectSiteList(HttpContext);

            if (sites != null) return Ok(sites);

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSites(string newList)
        {
            if (firebase.VerifyLoggedInCoordinator(HttpContext).Result == false) return Forbid();

            bool? success = await firebase.UpdateSiteList(HttpContext, newList);

            if (success != null) return Ok(success);

            return BadRequest();
        }


        //============================================
        //ERROR AREA
        //============================================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

