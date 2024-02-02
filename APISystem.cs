using Newtonsoft.Json.Linq;
using System.Net;

public abstract class APISystem
{
    public abstract Task<double> CheckPrice(string asset);
}

class APIBrapi : APISystem
{
    public override async Task<double> CheckPrice(string asset)
    {
        string apiKey = "";
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
