using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

abstract class NotificationSystem
{
    public abstract void Notify(string warning);
}

class EmailNotification : NotificationSystem
{
    public override void Notify(string warning)
    {
        // Adiciona um arquivo JSON de configuração
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("C:\\Users\\marce\\source\\repos\\desafioBT-INOA\\configuration.json");
        IConfigurationRoot configuration = builder.Build();

        string sender = configuration["emailSender"];
        string recipient = configuration["emailRecipient"];
        string password = configuration["emailPassword"];
        string smtpClient = configuration["smtpClient"];

        MailMessage message;

        if (warning == "sell")
        {
            message = new MailMessage(sender, recipient, "Cotação do ativo da B3", "Prezados, a cotação do ativo está acima do nível de referência para venda. Recomendo a venda.");
        }
        else
        {
            message = new MailMessage(sender, recipient, "Cotação do ativo da B3", "Prezados, a cotação do ativo está abaixo do nível de referência para compra. Recomendo a compra.");
        }

        SmtpClient client = new SmtpClient(smtpClient);
        client.Port = 587;
        client.Credentials = new NetworkCredential(sender, password);
        client.EnableSsl = true;

        client.Send(message);
        Console.WriteLine("E-mail enviado.");
    }
}
