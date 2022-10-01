﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models;
using UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase;
//using System.Runtime.Intrinsics.Arm;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        FirebaseSharedFunctions firebase;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            firebase = new FirebaseSharedFunctions(HttpContext);
        }

        //Basic home page
        public IActionResult Redirect()
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

        //Basic home page
        public IActionResult Index()
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                return RedirectToAction("Clinics", "Home");
            }

            return RedirectToAction("Login", "Account");
        }

        //Scheduled clinics page
        //Must be logged into a valid account to see
        public async Task<IActionResult> Clinics()
        {            
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                List<AppointmentModel>? appointments = await firebase.CollectStudentsAppointmentsAsync(HttpContext);
                ViewBag.Appointments = appointments;
                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        //Historical clinics page
        //Must be logged into a valid account to see
        public IActionResult History()
        {
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                return View();
            }

            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// A basic treatment page that relates to the appointment that has been selected
        /// </summary>
        /// <returns>A redirect action dependent on whether a user is logged in</returns>
        public IActionResult Treatment(string date, string time, string room, string patient, string infectious, string rt1, string rt2, string site, string ID)
        {                                    
            if (firebase.VerifyLoggedInSession(HttpContext).Result)
            {
                AppointmentModel appointment = new AppointmentModel(date, time, room, patient, infectious, rt1, rt2, site, ID);
                ViewBag.Appointment = appointment;

                return View();
            }
            
            return RedirectToAction("Login", "Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}