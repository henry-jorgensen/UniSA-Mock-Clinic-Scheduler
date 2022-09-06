using Google.Cloud.Firestore;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    [FirestoreData]
    public class StudentModel
    {
        private StudentModel()
        {
        }

        public StudentModel(string FirstName, string LastName, string StudentId, string Username)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.StudentId = StudentId;
            this.Username = Username;
            ClassCode = new List<string>();
            ScheduleCode = new List<string>();
        }

        [FirestoreProperty]
        public string? FirstName { get; set; }

        [FirestoreProperty]
        public string? LastName { get; set; }

        [FirestoreProperty]
        public string? StudentId { get; set; }

        [FirestoreProperty]
        public string? Username { get; set; }

        [FirestoreProperty]
        public List<string> ClassCode { get; set; }

        [FirestoreProperty]
        public List<string> ScheduleCode { get; set; }

        //Create an easy to send string of the student
        public string toString()
        {
            return StudentId +
                ":" + FirstName +
                ":" + LastName +
                ":" + Username;
        }
    }
}