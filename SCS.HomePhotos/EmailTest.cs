using SendGrid;
using SendGrid.Helpers.Mail;

using System;
using System.Threading.Tasks;

namespace SCS.HomePhotos
{
    internal class EmailTest
    {
        private static void Main()
        {
            Execute().Wait();
        }

        static async Task Execute()
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@billandtanja.com", "Bill Davidsen");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("wdavidsen@outlook.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
