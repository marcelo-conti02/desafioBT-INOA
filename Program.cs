using System.Globalization;
using System.Net;
using System.Net.Mail;

internal class Program
{
    static void Main(string[] args)
    {
        // Verifica se o número de parâmetros está correto
        if (args.Length != 3)
        {
            Console.WriteLine("Número incorreto de parametros.");
            return;
        }

        APIAlphaVantage alphaVantage = new APIAlphaVantage();
        EmailNotification emailNotification = new EmailNotification();

        StockQuoteAlert stockQuoteAlert = new StockQuoteAlert(alphaVantage, emailNotification, args[0], double.Parse(args[1], CultureInfo.InvariantCulture), double.Parse(args[2], CultureInfo.InvariantCulture));
        stockQuoteAlert.Alert();
    }
}

class StockQuoteAlert(APISystem api, NotificationSystem notification, string asset, double minPrice, double maxPrice)
{
    bool loop = true;
    double price;
    public void Alert()
    {
        // Loop para verificar o ativo e tomar uma decisão
        while(loop)
        {
        price = api.CheckPrice(asset);
            if(price > maxPrice)
            {
                notification.Notification("sell");
            }
            else if(price < minPrice)
            {
                notification.Notification("buy");
            }
        }
    }
}

abstract class APISystem
{
    public abstract double CheckPrice(string asset);
}

class APIAlphaVantage : APISystem
{
    public override double CheckPrice(string asset)
    {
        // Aguarda a conclusão da chamada à API
        CallAPI(asset).Wait();
        return 40.4;
    }

    private static async Task CallAPI(string asset)
    {
        // Chave de acesso da API
        string apiKey = "";

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
            message = new MailMessage(sender, recipient, "Cotação do ativo da B3", "Prezados, a cotação do ativo está abaixo do nível de referência para compra. Recomendo a compra.");
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