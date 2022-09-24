using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;
using Google.Cloud.Firestore;
//using static Google.Rpc.Context.AttributeContext.Types;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        FirebaseSharedFunctions firebase;

        public AccountController(ILogger<HomeController> logger)
        {
            _logger = logger;
            firebase = new FirebaseSharedFunctions(HttpContext);
        }

        public IActionResult Index()
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                ViewBag.CurrentUser = firebase.GenerateUserModel(HttpContext).Result;
                return View();
            }

            return RedirectToAction("Login");
        }

        //Login to account page
        //Cannot visit this page while logged in
        public IActionResult Login()
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                return RedirectToAction("Redirect", "Home");
            }

            return View();
        }

        //Reset account password page
        //Cannot visit this page while logged in
        public IActionResult Reset()
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                return RedirectToAction("Redirect", "Home");
            }

            return View();
        }

        //Set a student password for a new account page
        //Cannot visit this page while logged in
        public IActionResult Set()
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                return RedirectToAction("Redirect", "Home");
            }

            return View();
        }

        //Register a new account page
        //Cannot visit this page while logged in
        public IActionResult Register()
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                return RedirectToAction("Redirect", "Home");
            }

            return View();
        }

        //Logout of account page
        //Will clear user token whether present or not
        public IActionResult Logout()
        {            
            //Delete cookies and session
            HttpContext.Response.Cookies.Delete("VerificationToken");
            HttpContext.Session.Remove("VerificationToken");
            return RedirectToAction("Login");
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
                    //Course Coordinators
                    DocumentReference courseRef = firebase.DB().Collection("Users").Document(token);
                    DocumentSnapshot courseSnapshot = await courseRef.GetSnapshotAsync();

                    if (courseSnapshot.Exists)
                    {
                        firebase.SetVerificationToken(HttpContext, token);
                        return RedirectToAction("Redirect", "Home");
                    }

                    //Students
                    DocumentReference studentRef = firebase.DB().Collection("Students").Document(token);
                    DocumentSnapshot studentSnapshot = await studentRef.GetSnapshotAsync();

                    if (studentSnapshot.Exists)
                    {
                        firebase.SetVerificationToken(HttpContext, token);
                        return RedirectToAction("Redirect", "Home");
                    }
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                if (firebaseEx == null)
                {
                    ModelState.AddModelError(String.Empty, "Unknown Firebase Error has occurred");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                }
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
                    string? CCCode = (firebase.VerifyCoordinatorCode(accountModel.CCCode).Result) ? accountModel.CCCode : "";

                    if (CCCode == null || accountModel.FirstName == null || accountModel.LastName == null) return View();

                    //Insert this into cloud firestore database
                    DocumentReference docRef = firebase.DB().Collection("Users").Document(token);
                    Dictionary<string, object> user = new Dictionary<string, object>
                    {
                        { "FirstName", accountModel.FirstName },
                        { "LastName", accountModel.LastName },
                        { "CCCode", CCCode }
                    };
                    await docRef.SetAsync(user);

                    //Set the session token and redirect away from Register.
                    firebase.SetVerificationToken(HttpContext, token);
                    return RedirectToAction("Redirect", "Home");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                if (firebaseEx == null)
                {
                    ModelState.AddModelError(String.Empty, "Unknown Firebase Error has occurred");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                }
                return View(accountModel);
            }

            return View();
        }

        [HttpPost]
        public IActionResult Reset(ResetModel resetModel)
        {
            if(resetModel.Email == null) return View();

            firebase.ResetPassword(resetModel.Email);
            resetModel.Response = "Email has been sent";
            ViewBag.ResetModel = resetModel;
            return View();
        }

        public IActionResult DataRequest(string type)
        {
            firebase.DataRequest(type, HttpContext);
            return RedirectToAction("Index");
        }

        public IActionResult RequestUserData()
        {
            Debug.WriteLine("HIT");
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
            firebase.DeleteUserData(HttpContext);
            return RedirectToAction("Logout");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}