using System;
using System.ComponentModel.DataAnnotations;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    public class AppointmentModel
    {
        public AppointmentModel(string Date, string Patient, string RadiationTherapist1, string RadiationTherapist2, string Room, string Site)
        {
            this.Date = Date;
            this.Patient = Patient;
            this.RadiationTherapist1 = RadiationTherapist1;
            this.RadiationTherapist2 = RadiationTherapist2;
            this.Room = Room;
            this.Site = Site;
        }

        public string Date { get; set; }
        public string Patient { get; set; }
        public string RadiationTherapist1 { get; set; }
        public string RadiationTherapist2 { get; set; }
        public string Room { get; set; }
        public string Site { get; set; }
    }
}

