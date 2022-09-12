using SendGrid;
using SendGrid.Helpers.Mail;


namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Notification
{
    public class SendGridMessenger
    {
        public static async Task<string?> SendEmail(string receiver)
        {
            var apiKey = SendGridSettings.APIKEY;
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("unisamockclinic@gmail.com", "Unisa Mock Clinic");
            var to = new EmailAddress(receiver);
            
            var subject = "Unisa Mock Clinic";
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.Body.ReadAsStringAsync().Result);
            Console.WriteLine(response.Headers.ToString());
            return response.ToString();
        }
    }
}
