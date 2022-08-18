using System;
namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase
{
    public class FirebaseError
    {
        public Error error { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<Error> errors { get; set; }
    }
}

