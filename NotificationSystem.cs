using System.Net.Mail;
using System.Net;

abstract class NotificationSystem
{
    public abstract void Notify(string subject, string message);
}

class EmailNotification(string sender, string recipient, string password, string smtpClient) : NotificationSystem
{
    public override void Notify(string subject, string message)
    {
        MailMessage emailMessage = new MailMessage(sender, recipient, subject, message);
        SmtpClient client = new SmtpClient(smtpClient);
        client.Port = 587;
        client.Credentials = new NetworkCredential(sender, password);
        client.EnableSsl = true;

        client.Send(emailMessage);
        Console.WriteLine("E-mail enviado.");
    }
}