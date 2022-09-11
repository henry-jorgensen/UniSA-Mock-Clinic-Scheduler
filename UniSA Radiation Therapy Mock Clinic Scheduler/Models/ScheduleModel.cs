using Google.Cloud.Firestore;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    [FirestoreData]
    public class ScheduleModel
    {
        private ScheduleModel()
        {
        }

        public ScheduleModel(string Name, string Date, string StartTime, string AppointmentDuration, string Locations, string Schedule)
        {
            this.Name = Name;
            this.Date = Date;
            this.StartTime = StartTime;
            this.AppointmentDuration = AppointmentDuration;
            this.Locations = Locations;
            this.Schedule = Schedule;
            ScheduleCode = Guid.NewGuid().ToString(); //Generate a unique class code to link in students
        }

        [FirestoreProperty]
        public string? Name { get; set; }

        [FirestoreProperty]
        public string? Date { get; set; }

        [FirestoreProperty]
        public string? StartTime { get; set; }

        /// <summary>
        /// Hold long, in minutes, each appointment will be. Determines the
        /// spacing of the schedule timetable.
        /// </summary>
        [FirestoreProperty]
        public string? AppointmentDuration { get; set; }

        /// <summary>
        /// Building locations that the Clinic will take place in.
        /// </summary>
        [FirestoreProperty]
        public string? Locations { get; set; }

        /// <summary>
        /// A JSON stringified that details the entire schedule.
        /// </summary>
        [FirestoreProperty]
        public string? Schedule { get; set; }

        /// <summary>
        /// Used to link course coordinators & students to their related schedules
        /// </summary>
        [FirestoreProperty]
        public string ScheduleCode { get; set; }
    }
}
