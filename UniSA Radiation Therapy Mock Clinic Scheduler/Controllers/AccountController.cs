using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using System.Threading.Tasks;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        FirebaseAuthProvider auth;

        public AccountController(ILogger<HomeController> logger)
        {
            _logger = logger;

            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyByVk8XwhbQGoeeqkcxr5vRJWhOjep5Ulc"));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            var token = HttpContext.Session.GetString("_UserToken");
            if (token == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult Register()
        {
            var token = HttpContext.Session.GetString("_UserToken");
            if (token == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountModel accountModel)
        {
            var firebaseAuth = await auth.SignInWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);
            string token = firebaseAuth.FirebaseToken;


            if (token != null)
            {
                HttpContext.Session.SetString("_UserToken", token);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountModel accountModel)
        {
            await auth.CreateUserWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);
            var firebaseAuth = await auth.SignInWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);

            string token = firebaseAuth.FirebaseToken;


            if (token != null)
            {
                HttpContext.Session.SetString("_UserToken", token);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("_UserToken");
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}