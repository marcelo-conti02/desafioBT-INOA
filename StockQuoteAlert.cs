class StockQuoteAlert(APISystem api, NotificationSystem notification, string asset, double minPrice, double maxPrice, int delay)
{
    string subject = "Cotação do ativo da B3";
    string sellMessage = $"Prezado, a cotação do ativo {asset} está acima do nível de referência para a venda. Recomendo a venda.";
    string buyMessage = $"Prezado, a cotação do ativo {asset} está abaixo do nível de referência para a compra. Recomendo a compra.";

    public async Task Alert()
    {
        double price;

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
            await Task.Delay(delay * 1000);
        }
    }
}

