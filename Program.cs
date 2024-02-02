using System.Globalization;

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

        APIBrapi apiBrapi = new APIBrapi();
        EmailNotification emailNotification = new EmailNotification();
        StockQuoteAlert stockQuoteAlert = new StockQuoteAlert(apiBrapi, emailNotification, args[0], minPrice, maxPrice);

        stockQuoteAlert.Alert().Wait();
    }
}