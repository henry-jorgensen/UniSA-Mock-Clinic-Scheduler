using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;
using Google.Cloud.Firestore;
using static Google.Rpc.Context.AttributeContext.Types;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        FirebaseSharedFunctions firebase;

        public AccountController(ILogger<HomeController> logger)
        {
            _logger = logger;
            firebase = new FirebaseSharedFunctions();
        }

        public IActionResult Index()
        {
            var _UserToken = HttpContext.Session.GetString("_UserToken");
            ViewBag.CurrentUser = firebase.GetUserModelAsync(_UserToken).Result;

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

        public IActionResult Reset()
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
            try
            {
                var firebaseAuth = await firebase.Auth().SignInWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);
                string token = firebaseAuth.User.LocalId;

                if (token != null)
                {
                    CollectionReference usersRef = firebase.DB().Collection("Users");
                    QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        if (document.Id == token)
                        {
                            Console.WriteLine("User: {0}", document.Id);
                            Dictionary<string, object> documentDictionary = document.ToDictionary();
                            string FirstName = documentDictionary["FirstName"].ToString();
                            string LastName = documentDictionary["LastName"].ToString();
                            string CCCode = documentDictionary["CCCode"].ToString();

                            HttpContext.Session.SetString("_UserToken", token);
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(accountModel);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountModel accountModel)
        {
            try
            {
                await firebase.Auth().CreateUserWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);
                var firebaseAuth = await firebase.Auth().SignInWithEmailAndPasswordAsync(accountModel.Email, accountModel.Password);

                string token = firebaseAuth.User.LocalId;

                if (token != null)
                {
                    //Insert this into cloud firestore database
                    DocumentReference docRef = firebase.DB().Collection("Users").Document(token);
                    Dictionary<string, object> user = new Dictionary<string, object>
                    {
                        { "FirstName", accountModel.FirstName },
                        { "LastName", accountModel.LastName },
                        { "CCCode", accountModel.CCCode }
                    };
                    await docRef.SetAsync(user);

                    HttpContext.Session.SetString("_UserToken", token);
                    return RedirectToAction("Index");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(accountModel);
            }

            return View();
        }

        [HttpPost]
        public IActionResult Reset(ResetModel resetModel)
        {
            firebase.ResetPassword(resetModel.Email);
            resetModel.Response = "Email has been sent";
            ViewBag.ResetModel = resetModel;
            return View();
        }

        public IActionResult DataRequest(string type)
        {
            var _UserToken = HttpContext.Session.GetString("_UserToken");
            firebase.DataRequest(_UserToken, type);
            return RedirectToAction("Index");
        }

        public IActionResult RequestUserData()
        {
            DataRequest("Request");
            return RedirectToAction("Index");
        }

        public IActionResult DeleteUserData()
        {
            DataRequest("Delete");
            return RedirectToAction("Index");
        }

        public IActionResult DeleteUserDataTest()
        {
            var token = HttpContext.Session.GetString("_UserToken");
            firebase.DeleteUserData(token);
            return RedirectToAction("Logout");
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