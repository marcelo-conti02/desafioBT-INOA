using System.Net;
using System.Net.Mail;

internal class Program
{
    static void Main()
    {
        APIAlphaVantage alphaVantage = new APIAlphaVantage();

        alphaVantage.APIcall();
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