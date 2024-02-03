class StockQuoteAlert(APISystem api, NotificationSystem notification, string asset, double minPrice, double maxPrice, int delay)
{
    public async Task Alert()
    {
        double price;

        string subject = "Cotação do ativo da B3";
        string sellMessage = $"Prezados, a cotação do ativo {asset} está acima do nível de referência para a venda. Recomendo a venda.";
        string buyMessage = $"Prezados, a cotação do ativo {asset} está abaixo do nível de referência para a compra. Recomendo a compra.";

        while (true)
        {
            price = await api.CheckPrice(asset);
            Console.WriteLine($"Cotação do ativo {asset}: {price}");

            if (price > maxPrice)
            {
                notification.Notify(subject, sellMessage);
            }
            else if (price < minPrice)
            {
                notification.Notify(subject, buyMessage);
            }

            // Aguarda para fazer uma nova requisição
            await Task.Delay(delay);
        }
    }
}

