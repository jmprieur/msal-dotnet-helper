
using Microsoft.Identity.Client.Helpers;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp_with_interactive_authentication
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayIdCardAsync().Wait();
        }

        private static async Task DisplayIdCardAsync()
        {
            string clientId = "145cec56-05b2-4764-a41c-b77466387462";
            SingleUserPublicClientApplication application = new SingleUserPublicClientApplication(clientId);

            HttpClient client = new HttpClient();

            application.Scopes.Add("User.Read");
            await application.AuthenticateClientAsync(client);
            Console.WriteLine($"Hello {application.User.DisplayableId}");

            string json = await client.GetStringAsync("https://graph.microsoft.com/v1.0/me");
            dynamic me = JsonConvert.DeserializeObject(json);
            Console.WriteLine(me.displayName);
            Console.WriteLine(me.jobTitle);
            Console.WriteLine($"Phone {me.mobilePhone}");
        }
    }
}
