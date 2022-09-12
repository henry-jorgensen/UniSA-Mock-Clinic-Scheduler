using Google.Cloud.Firestore;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    [FirestoreData]
    public class ClassModel
    {
        private ClassModel()
        {
        }

        public ClassModel(string Name, string StudyPeriod, string Semester, string Year)
        {
            this.Name = Name;
            this.StudyPeriod = StudyPeriod;
            this.Semester = Semester;
            this.Year = Year;
            Students = null;
            ClassCode = Guid.NewGuid().ToString(); //Generate a unique class code to link in students
            ScheduleCode = new List<string>(); //A list containing the unique schedule codes used to link in assoicated schedules
        }

        [FirestoreProperty]
        public string? Name { get; set; }

        [FirestoreProperty]
        public string? StudyPeriod { get; set; }

        [FirestoreProperty]
        public string? Semester { get; set; }

        [FirestoreProperty]
        public string? Year { get; set; }

        [FirestoreProperty]
        public string? ClassCode { get; set; }

        [FirestoreProperty]
        public List<string>? ScheduleCode { get; set; }

        public string? Students { get; set; }
    }
}
