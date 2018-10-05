using Microsoft.Graph;
using Microsoft.Graph.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp_querying_msgraph_with_graphsdk
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayIdCardAsync().Wait();
        }

        private static async Task DisplayIdCardAsync()
        {
            // Instantiate the service for a public client application
            string clientId = "145cec56-05b2-4764-a41c-b77466387462";
            SingleUserPublicClientGraphApplication graphServiceClient = new SingleUserPublicClientGraphApplication(clientId);

            try
            {
                // Read the me endpoint
                graphServiceClient.Scopes.Add("User.Read");
                User me = await graphServiceClient.Me.Request().GetAsync();
                Console.WriteLine($"{me.DisplayName}");

                // Now read the calendar
                graphServiceClient.Scopes.Add("Calendars.Read");
                var calendars = await graphServiceClient.Me.Calendars.Request().GetAsync();
                Console.WriteLine("Your calendars:");
                foreach (var calendar in calendars)
                {
                    Console.WriteLine($"- {calendar.Name}");
                }

                graphServiceClient.Scopes.Add("Mail.Read");
                var messages = await graphServiceClient.Me.MailFolders.Inbox.Messages.Request().GetAsync();
                Console.WriteLine("Your last email:");
                Console.WriteLine($"{messages.FirstOrDefault().Subject}");
                await graphServiceClient.SignOut();
            }

            // Handles the exception not handled by the wrapper (for instance in the case the user cancels the authentication dialog)
            // If the developer wants more details s/he can add the Msal NuGet package, but otherwise this is not needed
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }

        }
    }
}
