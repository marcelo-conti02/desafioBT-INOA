using System.Globalization;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

internal class Program
{
    static void Main(string[] args)
    {
        double maxPrice, minPrice;

        // Usuário precisa fornecer 3 parâmetros: ativo, preço referência de compra e preço referência de venda 
        if (args.Length != 3)
        {
            Console.WriteLine("Número incorreto de parametros.");
            return;
        }
        else
        {
            bool sucessMin = double.TryParse(args[1], CultureInfo.InvariantCulture, out minPrice);
            bool sucessMax = double.TryParse(args[2], CultureInfo.InvariantCulture, out maxPrice);

            if(!sucessMin || !sucessMax)
            {
                Console.WriteLine("Valores de referência precisam ser números.");
                return;
            }
            else if(minPrice >= maxPrice)
            {
                Console.WriteLine("Valor de referência para compra precisa ser menor que o de venda.");
                return;
            }
        }

        // Adiciona um arquivo JSON de configuração
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("C:\\Users\\marce\\source\\repos\\desafioBT-INOA\\configuration.json");
        IConfigurationRoot configuration = builder.Build();

        APIBrapi apiBrapi = new APIBrapi();
        EmailNotification emailNotification = new EmailNotification();
        StockQuoteAlert stockQuoteAlert = 
            new StockQuoteAlert(
                apiBrapi, 
                emailNotification, 
                args[0], 
                minPrice, 
                maxPrice,
                configuration
                );
        stockQuoteAlert.Alert().Wait();
    }
}

class StockQuoteAlert(APISystem api, NotificationSystem notification, string asset, double minPrice, double maxPrice, IConfigurationRoot configuration)
{
    double price;
    public async Task Alert()
    {
        while(true)
        {
            price = await api.CheckPrice(asset, configuration);
            Console.WriteLine($"Cotação ativo {asset}: {price}");

            if (price > maxPrice)
            {
                notification.Notification("sell", configuration);
            }
            else if(price < minPrice)
            {
                notification.Notification("buy", configuration);
            }

            // Aguarda 30s para fazer uma nova requisição
            await Task.Delay(10*60000);
        }
    }
}

abstract class APISystem
{
    public abstract Task<double> CheckPrice(string asset, IConfigurationRoot configuration);
}

class APIBrapi : APISystem
{

    public override async Task<double> CheckPrice(string asset, IConfigurationRoot configuration)
    {
        double price = await CallAPI(asset, configuration);
        return price;
    }

    private static async Task<double> CallAPI(string asset, IConfigurationRoot configuration)
    {
        string apiKey = configuration["apiKeyBrapi"];
        string apiUrl = $"https://brapi.dev/api/quote/{asset}?token={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage requisition = await client.GetAsync(apiUrl);

            if (requisition is { StatusCode: HttpStatusCode.OK })
            {
                string apiResponse = await requisition.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(apiResponse);

                // Obtem a cotação do ativo no JObject
                var jsonData = jsonObject["results"][0]["regularMarketPrice"];
                var price = jsonData.ToString();

                return double.Parse(price);
            }
            else
            {
                Console.WriteLine("Erro na chamada à API.");
                Environment.Exit(1);
                return -1;
            }
        }
    }
}

class APIAlphaVantage : APISystem
{
    public override async Task<double> CheckPrice(string asset, IConfigurationRoot configuration)
    {
        double price = await CallAPI(asset, configuration);
        return price;
    }

    private static async Task<double> CallAPI(string asset, IConfigurationRoot configuration)
    {
        string apiKey = configuration["apiKeyAlpha"];
        string apiUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={asset}&interval=1min&apikey={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage requisition = await client.GetAsync(apiUrl);
            
            if (requisition is { StatusCode: HttpStatusCode.OK })
            {
                string apiResponse = await requisition.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<JObject>(apiResponse);

                // Separa os dados que são atualizados a cada 1min
                var timeSeries = data["Time Series (1min)"] as JObject;
                var recentData = timeSeries.First.First;

                // Separa o preço de fechamento do ativo para ser monitorado
                var price = recentData["4. close"].ToString();
                return double.Parse(price, CultureInfo.InvariantCulture);
            }
            else
            {
                Console.WriteLine("Erro na chamada à API.");
                Environment.Exit(1);
                return -1;
            }
        }
    }
}

abstract class NotificationSystem
{
    public abstract void Notification(string warning, IConfigurationRoot configuration);
}

class EmailNotification : NotificationSystem
{
    public override void Notification(string warning, IConfigurationRoot configuration)
    {
        string sender = configuration["emailSender"];
        string recipient = configuration["emailRecipient"];
        string password = configuration["emailPassword"];
        string smtpClient = configuration["smtpClient"];
        MailMessage message;

        if(warning == "sell")
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