using System.Net;
using System.Net.Mail;

internal class Program
{
    static void Main()
    {
        APIAlphaVantage alphaVantage = new APIAlphaVantage();
        EmailNotification emailNotification = new EmailNotification();

        alphaVantage.APIcall();
        emailNotification.Notification();
    }
}

abstract class APISystem
{
    public abstract void APIcall();
}

class APIAlphaVantage : APISystem
{
    public override void APIcall()
    {
        // Aguarda a conclusão da chamada à API
        Call().Wait();
    }

    private static async Task Call()
    {
        // Chave de acesso da API e ativo da B3 para ser monitorado
        string apiKey = "";
        string asset = "AAPL";

        string apiUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={asset}&interval=1min&apikey={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            // Envia uma requisição para a API
            HttpResponseMessage requisition = await client.GetAsync(apiUrl);

            if (requisition.IsSuccessStatusCode)
            {
                string reqResponse = await requisition.Content.ReadAsStringAsync();
                Console.WriteLine(reqResponse);
            }
            else
            {
                Console.WriteLine($"Erro: {requisition.StatusCode}");
            }
        }
    }
}

abstract class NotificationSystem
{
    public abstract void Notification();
}

class EmailNotification : NotificationSystem
{
    public override void Notification()
    {
        string sender = "";
        string recipient = "";
        string password = "";

        // Mensagem do e-mail
        MailMessage message = new MailMessage(sender, recipient, "teste", "testando");

        // Configurações do cliente SMTP
        SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
        client.Port = 587;
        client.Credentials = new NetworkCredential(sender, password);
        client.EnableSsl = true;

        // Envia o e-mail
        client.Send(message);
        Console.WriteLine("E-mail enviado.");
    }
}