using System.Net;
using System.Net.Mail;

internal class Program
{
    static void Main()
    {
        APIAlphaVantage alphaVantage = new APIAlphaVantage();
        EmailNotification emailNotification = new EmailNotification();

        StockQuoteAlert stockQuoteAlert = new StockQuoteAlert(alphaVantage, emailNotification);
        stockQuoteAlert.Alert();
    }
}

class StockQuoteAlert(APISystem api, NotificationSystem notification)
{
    bool loop = true;
    float price;
    public void Alert()
    {
        // Loop para verificar o ativo e tomar uma decisão
        while(loop){
            price = api.CheckPrice();
            if(price > max)
            {
                notification.Notification("sell");
            }
            else if(price < min)
            {
                notification.Notification("buy");
            }
        }
    }
}

abstract class APISystem
{
    public abstract void CheckPrice();
}

class APIAlphaVantage : APISystem
{
    public override void CheckPrice()
    {
        // Aguarda a conclusão da chamada à API
        CallAPI().Wait();
    }

    private static async Task CallAPI()
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
    public abstract void Notification(string warning);
}

class EmailNotification : NotificationSystem
{
    public override void Notification(string warning)
    {
        string sender = "";
        string recipient = "";
        string password = "";
        MailMessage message;

        // Mensagem do e-mail
        if(warning == "sell")
        {
            message = new MailMessage(sender, recipient, "Cotação do ativo da B3", "Prezados, a cotação do ativo está acima do nível de referência para venda. Recomendo a venda.");
        }
        else
        {
            message = new MailMessage(sender, recipient, "Cotação do ativo da B3", "Prezados, a cotação do ativo está acima do nível de referência para compra. Recomendo a compra.");
        }

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