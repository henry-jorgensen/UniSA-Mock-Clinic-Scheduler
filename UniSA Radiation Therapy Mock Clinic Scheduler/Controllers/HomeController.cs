using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;

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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var _UserToken = HttpContext.Session.GetString("_UserToken");
            ViewBag.CurrentUser = firebase.GetUserModelAsync(_UserToken).Result;

            return View();
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

            if (_UserToken == null)
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

            if (_UserToken == null)
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

            if (_UserToken == null)
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

            if (_UserToken == null)
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

        public IActionResult Clinics()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}