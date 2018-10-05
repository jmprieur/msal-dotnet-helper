using Microsoft.Graph;
using Microsoft.Graph.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppUsingGraphSdkWithAuthentication
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayInformationAsync().Wait();
        }

        private static async Task DisplayInformationAsync()
        {
            // Instanciate the service for a public client application
            string clientId = "145cec56-05b2-4764-a41c-b77466387462";
            SingleUserPublicClientApplicationAuthenticationProvider authenticationProvider = new SingleUserPublicClientApplicationAuthenticationProvider(clientId);

            GraphServiceClient graphServiceClient = new GraphServiceClient(authenticationProvider);
            // Read the me endpoint
            authenticationProvider.Scopes.Add("User.Read");
            User me = await graphServiceClient.Me.Request().GetAsync();
            Console.WriteLine($"{me.DisplayName}");

            // Now read the calendar
            authenticationProvider.Scopes.Add("Calendars.Read");
            var calendars = await graphServiceClient.Me.Calendars.Request().GetAsync();
            Console.WriteLine("Your calendars:");
            foreach (var calendar in calendars)
            {
                Console.WriteLine($"- {calendar.Name}");
            }

            authenticationProvider.Scopes.Add("Mail.Read");
            var messages = await graphServiceClient.Me.MailFolders.Inbox.Messages.Request().GetAsync();
            Console.WriteLine("Your last email:");
            Console.WriteLine($"{messages.FirstOrDefault().Subject}");
            await authenticationProvider.SignOut();
        }
    }
}
