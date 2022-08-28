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
            this.Students = null;
        }

        [FirestoreProperty]
        public string? Name { get; set; }

        [FirestoreProperty]
        public string? StudyPeriod { get; set; }

        [FirestoreProperty]
        public string? Semester { get; set; }

        [FirestoreProperty]
        public string? Year { get; set; }

        public string? Students { get; set; }
    }
}
