using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Text.Json;

namespace IdentityModelApp
{
    class Program
    {
        static  void Main(string[] args)
        {
            GetXBankData();
            Console.ReadLine();
        }

        public static async void GetXBankData()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:1000");

            var tokenRequest = new ClientCredentialsTokenRequest()
            {
                ClientId = "XBank",
                ClientSecret = "xbank",
                Address = disco.TokenEndpoint,
            };

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:2000/api/XBank/Bakiye?musteriId=3000");
            if (response.IsSuccessStatusCode)
            {
                int bakiye = JsonSerializer.Deserialize<int>(await response.Content.ReadAsStringAsync());
                Console.WriteLine($"Hesap bakiyeniz : {bakiye}");
            }

            Console.WriteLine("Hello World!");
        }
    }
}
