﻿using Firebase.Auth;
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
            firebase = new FirebaseSharedFunctions(HttpContext);
        }

        public IActionResult Index()
        {
            if (firebase.VerifyLoggedinSession(HttpContext).Result)
            {
                ViewBag.CurrentUser = firebase.GenerateUserModel(HttpContext).Result;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //Login to account page
        //Cannot visit this page while logged in
        public IActionResult Login()
        {
            if (firebase.VerifyLoggedinSession(HttpContext).Result == false)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Redirect", "Home");
            }
        }

        //Reset account password page
        //Cannot visit this page while logged in
        public IActionResult Reset()
        {
            if (firebase.VerifyLoggedinSession(HttpContext).Result == false)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Redirect", "Home");
            }
        }

        //Register a new account page
        //Cannot visit this page while logged in
        public IActionResult Register()
        {
            if (firebase.VerifyLoggedinSession(HttpContext).Result == false)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Redirect", "Home");
            }
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
                    CollectionReference usersRef = firebase.DB().Collection("Users");
                    QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        if (document.Id == token)
                        {
                            Dictionary<string, object> documentDictionary = document.ToDictionary();
                            string FirstName = documentDictionary["FirstName"].ToString();
                            string LastName = documentDictionary["LastName"].ToString();
                            string CCCode = documentDictionary["CCCode"].ToString();

                            //Set the session token and redirect away from Login.
                            firebase.SetVerificationToken(HttpContext, token, accountModel.RememberLogin);
                            return RedirectToAction("Redirect", "Home");
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
                    string CCCode = (firebase.VerifyCoordinatorCode(accountModel.CCCode).Result) ? accountModel.CCCode : "";

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
            firebase.DataRequest(type, HttpContext);
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