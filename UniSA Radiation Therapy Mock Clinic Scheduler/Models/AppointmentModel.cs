using System;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    [FirestoreData]
    public class AppointmentModel
    {
        private AppointmentModel() 
        {}
        public AppointmentModel(DateTime date, string room, string patient, string radiationTherapist1, string radiationTherapist2, string site)
        {
            Date = date;
            Room = room;
            Patient = patient;
            RadiationTherapist1 = radiationTherapist1;
            RadiationTherapist2 = radiationTherapist2;
            Site = site;
        }

        [FirestoreProperty]
        public DateTime? Date { get; set; }

        [FirestoreProperty]
        public string? Patient { get; set; }

        [FirestoreProperty]
        public string? Room { get; set; }

        [FirestoreProperty]
        public string? RadiationTherapist1 { get; set; }

        [FirestoreProperty]
        public string? RadiationTherapist2 { get; set; }

        [FirestoreProperty]
        public string? Site { get; set; }
    }
}
