using Newtonsoft.Json.Linq;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    public class ClassModel
    {
        public ClassModel(string name, string studyPeriod, string semester, string year)
        {
            Name = name;
            StudyPeriod = studyPeriod;
            Semester = semester;
            Year = year;
            Students = new JArray();
            Schedules = new JArray();
        }

        public string Name { get; set; }
        public string StudyPeriod { get; set; }
        public string Semester { get; set; }
        public string Year { get; set; }
        public JArray Students { get; set; }
        public JArray Schedules { get; set; }
    }
}
