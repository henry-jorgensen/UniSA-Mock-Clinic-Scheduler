using Google.Cloud.Firestore;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    [FirestoreData]
    public class AppointmentModel
    {
        private AppointmentModel() 
        {}
        public AppointmentModel(string date, string time, string room, string patient, string infectious, string radiationTherapist1, string radiationTherapist2, string site, string complication, string? ID=null, string? scheduleCode=null)
        {
            Date = date;
            Time = time;
            Room = room;
            Patient = patient;
            Infectious = infectious;
            RadiationTherapist1 = radiationTherapist1;
            RadiationTherapist2 = radiationTherapist2;
            Site = site;
            Complication = complication;
            AppointmentID = ID;
            ScheduleCode = scheduleCode;
            Emailed = false;
            Status = "Available";
            AppointmentRef = null;
            InfectiousDetails = "";
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
        public string? Complication { get; set; }

        [FirestoreProperty]
        public string? AppointmentID { get; set; }

        [FirestoreProperty]
        public string? ScheduleCode { get; set; }

        [FirestoreProperty]
        public bool Emailed { get; set; }

        [FirestoreProperty]
        public string? Status { get; set; }

        [FirestoreProperty]
        public string? InfectiousDetails { get; set; }

        public string? PatientName { get; set; }

        public string? RadiationTherapist1Name { get; set; }
        public string? RadiationTherapist2Name { get; set; }

        public string? AppointmentRef { get; set; }
    }
}
