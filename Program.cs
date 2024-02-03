using System.Globalization;
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

            if (!sucessMin || !sucessMax)
            {
                Console.WriteLine("Valores de referência precisam ser números.");
                return;
            }
            else if (minPrice >= maxPrice)
            {
                Console.WriteLine("Valor de referência para compra precisa ser menor que o de venda.");
                return;
            }
        }

        string asset = args[0];
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("C:\\Users\\marce\\source\\repos\\desafioBT-INOA\\AppConfig.json");
        IConfigurationRoot appConfig = builder.Build();

        APIBrapi apiBrapi = new APIBrapi(appConfig["apiKey"]);
        EmailNotification emailNotification = new EmailNotification(appConfig["emailSender"], appConfig["emailRecipient"], appConfig["emailPassword"], appConfig["smtpClient"]);
        StockQuoteAlert stockQuoteAlert = new StockQuoteAlert(apiBrapi, emailNotification, asset, minPrice, maxPrice, int.Parse(appConfig["delay"]));

        stockQuoteAlert.Alert().Wait();
    }
}