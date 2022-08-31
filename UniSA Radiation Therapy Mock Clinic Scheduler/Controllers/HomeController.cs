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
                    return RedirectToAction("Create", "Home");
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

        //Create clinics page
        //Must be logged into course coordinator account to see
        public IActionResult Create()
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
        public IActionResult DoSomethingWithFirebase(string value)
        {
            string response = value + " Success";
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAClass(string name, string studyPeriod, string semester, string year)
        {
            ClassModel classModel = new ClassModel(name, studyPeriod, semester, year);

            var _UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.VerifyLoggedIn(_UserToken).Result == false)
            {
                return Forbid();
            }

            string? success = await firebase.CreateNewClassAsync(_UserToken, classModel);

            if (success != null)
            {
                return Ok(success);
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> LoadAllClasses()
        {
            var _UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.VerifyLoggedIn(_UserToken).Result == false)
            {
                return Forbid();
            }

            List<ClassModel>? success = await firebase.CollectAllClassAsync(_UserToken);

            if (success != null)
            {
                return Ok(success);
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> LoadAClass(string className)
        {
            var _UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.VerifyLoggedIn(_UserToken).Result == false)
            {
                return Forbid();
            }

            ClassModel? success = await firebase.CollectClassAsync(_UserToken, className);

            if (success != null)
            {
                return Ok(success);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> SaveAClassList(string className, string[] studentList)
        {
            var _UserToken = HttpContext.Session.GetString("_UserToken");

            if (firebase.VerifyLoggedIn(_UserToken).Result == false)
            {
                return Forbid();
            }

            bool success = await firebase.SaveAClassListAsync(_UserToken, className, studentList);

            if (success)
            {
                return Ok(success);
            }

            return BadRequest();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}