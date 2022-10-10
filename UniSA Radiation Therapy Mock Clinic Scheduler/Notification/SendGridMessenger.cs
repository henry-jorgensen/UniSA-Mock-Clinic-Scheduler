using SendGrid;
using SendGrid.Helpers.Mail;


namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Notification
{
    public class SendGridMessenger
    {
        public static async Task<string?> SendEmail(string name, string date, string complication, string receiverEmail)
        {
            var apiKey = SendGridSettings.APIKEY;
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("unisamockclinic@gmail.com", "Unisa Mock Clinic");
            var to = new EmailAddress(receiverEmail);
            
            //Subject
            var subject = "Unisa Mock Clinic";

            //Initial text line
            var plainTextContent = "Patient Complication";

            //Email content
            var htmlContent = $@"
                To {name},\n
                \n
                For the UniSA Radiation Therapy Mock Clinic scheduled for the {date}, you are to act out the follow.\n
                {complication}\n
                \n
                Kind Regards, \n
                UniSA Mock Clinic
            ";
            
            //MailHelper.CreateMultipleEmailsToMultipleRecipients(from, to, subject, plainTextContent, htmlContent);
            
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.Body.ReadAsStringAsync().Result);
            Console.WriteLine(response.Headers.ToString());
            return response.ToString();
        }
    }
}
