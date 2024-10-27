using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

public class EmailService
{
    private readonly string _sendGridApiKey;

    public EmailService(string sendGridApiKey)
    {
        _sendGridApiKey = sendGridApiKey;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent)
    {
        var client = new SendGridClient(_sendGridApiKey);
        var from = new EmailAddress("zaka013@hotmail.com", "Zakaria Hassan"); // Ensure this email is verified in SendGrid
        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);

        // Check for successful status codes (OK or Accepted)
        if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            // Log the failed attempt
            Console.WriteLine($"Failed to send email to {toEmail}: {response.StatusCode}");
        }
        else
        {
            // Log successful email send
            Console.WriteLine($" [x] Sent email to {toEmail}");
        }
    }

}

