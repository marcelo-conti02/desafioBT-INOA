class StockQuoteAlert(APISystem api, NotificationSystem notification, string asset, double minPrice, double maxPrice)
{
    double price;
    public async Task Alert()
    {
        while (true)
        {
            price = await api.CheckPrice(asset);
            Console.WriteLine($"Cotação ativo {asset}: {price}");

            if (price > maxPrice)
            {
                notification.Notify("sell");
            }
            else if (price < minPrice)
            {
                notification.Notify("buy");
            }

            // Aguarda 30s para fazer uma nova requisição
            await Task.Delay(30000);
        }
    }
}

