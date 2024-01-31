using System.Globalization;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



internal class Program
{
    static void Main(string[] args)
    {
        // Usuário precisa fornecer 3 parâmetros: ativo, preço referência de compra e preço referência de venda 
        if (args.Length != 3)
        {
            Console.WriteLine("Número incorreto de parametros.");
            return;
        }
   
        APIAlphaVantage alphaVantage = new APIAlphaVantage();
        EmailNotification emailNotification = new EmailNotification();
        StockQuoteAlert stockQuoteAlert = new StockQuoteAlert(alphaVantage, emailNotification, args[0], double.Parse(args[1], CultureInfo.InvariantCulture), double.Parse(args[2], CultureInfo.InvariantCulture));
        stockQuoteAlert.Alert().Wait();
    }
}

class StockQuoteAlert(APISystem api, NotificationSystem notification, string asset, double minPrice, double maxPrice)
{
    double price;
    public async Task Alert()
    {
        //while(true)
        //{
            price = await api.CheckPrice(asset);

            Console.WriteLine(price);

            if (price > maxPrice)
            {
                notification.Notification("sell");
            }
            else if(price < minPrice)
            {
                notification.Notification("buy");
            }
        //}
    }
}

abstract class APISystem
{
    public abstract Task<double> CheckPrice(string asset);
}

class APIAlphaVantage : APISystem
{
    public override async Task<double> CheckPrice(string asset)
    {
        double price = await CallAPI(asset);
        return price;
    }

    private static async Task<double> CallAPI(string asset)
    {
        string apiKey = "";
        string apiUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={asset}&interval=1min&apikey={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage requisition = await client.GetAsync(apiUrl);
          
            if (requisition.IsSuccessStatusCode)
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
                Console.WriteLine($"Erro: {requisition.StatusCode}");
                return -1;
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
        string stmpClient = "smtp-mail.outlook.com";
        MailMessage message;

        if(warning == "sell")
        {
            message = new MailMessage(sender, recipient, "Cotação do ativo da B3", "Prezados, a cotação do ativo está acima do nível de referência para venda. Recomendo a venda.");
        }
        else
        {
            message = new MailMessage(sender, recipient, "Cotação do ativo da B3", "Prezados, a cotação do ativo está abaixo do nível de referência para compra. Recomendo a compra.");
        }

        SmtpClient client = new SmtpClient(stmpClient);
        client.Port = 587;
        client.Credentials = new NetworkCredential(sender, password);
        client.EnableSsl = true;

        client.Send(message);
        Console.WriteLine("E-mail enviado.");
    }
}