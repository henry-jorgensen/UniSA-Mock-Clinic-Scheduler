namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Models
{
    public class FileModel
    {
        public string? ID { get; set; } // if you wish to associate these files with some parent record
        public IFormFile? Document { get; set; }
    }
}
