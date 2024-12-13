using System;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _sendGridApiKey;
    private static readonly HttpClient client = new HttpClient();
    private readonly string _cloudFunctionUrl = "https://europe-west4-nice-compass-442412-d2.cloudfunctions.net/sendEmail"; // Je Cloud Function URL

    public EmailService(string sendGridApiKey)
    {
        _sendGridApiKey = sendGridApiKey;
    }

    // Nieuwe methode om de Cloud Function aan te roepen
    public async Task CallCloudFunctionAsync(string taskName, string email)
    {
        var json = $"{{ \"taskName\": \"{taskName}\", \"email\": \"{email}\" }}";
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(_cloudFunctionUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Successfully called Cloud Function for task: {taskName}");
            }
            else
            {
                Console.WriteLine($"Failed to call Cloud Function. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling Cloud Function: {ex.Message}");
        }
    }


    //public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent)
    //{
    //    var client = new SendGridClient(_sendGridApiKey);
    //    var from = new EmailAddress("zaka013@hotmail.com", "Zakaria Hassan"); // Ensure this email is verified in SendGrid
    //    var to = new EmailAddress(toEmail);

    //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
    //    var response = await client.SendEmailAsync(msg);

    //    // Check for successful status codes (OK or Accepted)
    //    if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
    //    {
    //        // Log the failed attempt
    //        Console.WriteLine($"Failed to send email to {toEmail}: {response.StatusCode}");
    //    }
    //    else
    //    {
    //        // Log successful email send
    //        Console.WriteLine($" [x] Sent email to {toEmail}");
    //    }
    //}

}

