using System;
using System.ComponentModel.DataAnnotations;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    public class AccountModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string CCCode { get; set; }
    }
}

