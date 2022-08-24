using System;
using System.ComponentModel.DataAnnotations;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    public class ResetModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
