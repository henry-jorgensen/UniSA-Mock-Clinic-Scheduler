namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Notification
{
    public class SendGridSettings
    {
        //TODO this needs to be protected with as an ENV variable
        public static string APIKEY = GetEnvironmentVariable("SEND_GRID_API_KEY");

        private static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }   
}
