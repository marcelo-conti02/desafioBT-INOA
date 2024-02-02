using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

abstract class NotificationSystem
{
    public abstract void Notify(string subject, string message);
}

class EmailNotification : NotificationSystem
{
    public override void Notify(string subject, string message)
    {
        // Adiciona um arquivo JSON de configuração
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("C:\\Users\\marce\\source\\repos\\desafioBT-INOA\\emailConfig.json");
        IConfigurationRoot emailConfig = builder.Build();

        string sender = emailConfig["emailSender"];
        string recipient = emailConfig["emailRecipient"];
        string password = emailConfig["emailPassword"];
        string smtpClient = emailConfig["smtpClient"];

        MailMessage emailMessage = new MailMessage(sender, recipient, subject, message);
        SmtpClient client = new SmtpClient(smtpClient);
        client.Port = 587;
        client.Credentials = new NetworkCredential(sender, password);
        client.EnableSsl = true;

        client.Send(emailMessage);
        Console.WriteLine("E-mail enviado.");
    }
}
