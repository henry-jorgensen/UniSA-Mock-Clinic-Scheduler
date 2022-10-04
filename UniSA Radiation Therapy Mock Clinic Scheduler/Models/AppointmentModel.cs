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
        public AppointmentModel(string date, string time, string room, string patient, string infectious, string radiationTherapist1, string radiationTherapist2, string site, string? ID=null)
        {
            Date = date;
            Time = time;
            Room = room;
            Patient = patient;
            Infectious = infectious;
            RadiationTherapist1 = radiationTherapist1;
            RadiationTherapist2 = radiationTherapist2;
            Site = site;
            if (ID != null)
            {
                AppointmentID = ID;
            }
            
        }

        [FirestoreProperty]
        public string? Date { get; set; }

        [FirestoreProperty]
        public string? Time { get; set; }

        [FirestoreProperty]
        public string? Patient { get; set; }

        [FirestoreProperty]
        public string? Infectious { get; set; }

        [FirestoreProperty]
        public string? Room { get; set; }

        [FirestoreProperty]
        public string? RadiationTherapist1 { get; set; }

        [FirestoreProperty]
        public string? RadiationTherapist2 { get; set; }

        [FirestoreProperty]
        public string? Site { get; set; }

        [FirestoreProperty]
        public string? AppointmentID { get; set; }

        public string? PatientName { get; set; }

        public string? RadiationTherapist1Name { get; set; }

        public string? RadiationTherapist2Name { get; set; }

        //public string toParameters()
        //{
        //    return $"date={Date}&room";
        //}
    }
}
