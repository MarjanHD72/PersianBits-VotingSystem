using System.Net;
using System.Net.Mail;

namespace VotingSystem.Domain;

public class EmailService
{
    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = "";
    public string SmtpPass { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string AdminEmail { get; set; } = "";

    public async Task SendAdminRequestAsync(string fullName, string email, string dob, string reason)
    {
        if (string.IsNullOrEmpty(SmtpUser) || string.IsNullOrEmpty(AdminEmail)) return;

        var subject = $"[PersianBits] Admin Request from {fullName}";
        var body = $@"New admin request submitted:

Name: {fullName}
Email: {email}
Date of Birth: {dob}
Reason: {reason}

To approve, change their role to 'Admin' in the database Users table.";

        try
        {
            using var client = new SmtpClient(SmtpHost, SmtpPort)
            {
                Credentials = new NetworkCredential(SmtpUser, SmtpPass),
                EnableSsl = true
            };

            var from = string.IsNullOrEmpty(FromEmail) ? SmtpUser : FromEmail;
            var msg = new MailMessage(from, AdminEmail, subject, body);
            await client.SendMailAsync(msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email send failed: {ex.Message}");
        }
    }
}
