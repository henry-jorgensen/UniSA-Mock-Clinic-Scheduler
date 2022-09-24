using System;
using System.ComponentModel.DataAnnotations;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    public class UserModel
    {
        public UserModel(string token, string firstName, string lastName, string? cCCode=null)
        {
            UserToken = token;
            FirstName = firstName;
            LastName = lastName;
            CCCode = cCCode;
        }

        public string UserToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CCCode { get; set; }
    }
}

