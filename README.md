# msal-and-msgraph-dotnet-helper
Experimental libraries aiming at helping developers getting up to speed quicker at using MSAL.NET in some simple scenarios. This repo contains two libraries:

- [Microsoft.Graph.Helper](#Microsoft_Graph_Helper) which is a helper library for the MIcrosoft Graph .NET SDK
- [Microsoft.Identity.Client.Helper](Microsoft_Identity_Client_Helper) which is a helper library for MSAL.NET.

## Microsoft Graph Helper

### The issue

The Microsoft Graph SDK enables developers to quickly access data from many Microsoft services through one unified API. However, to use the SDK
you still need to understand the authentication part.

This library abstracts about authentication for public client applications (Desktop / Mobile applications) which sign-in only one user.

### How can I try it?

To try the library you need to:

1. Add the Microsoft.Graph.helpers nuget package
1. Register your application with the Azure portal (as you would do anyway)
1. Instanciate a `SingleUserPublicClientGraphApplication`passing the application Id (client Id) of your application. Let's assure that this instance is in a variable named `graphServiceClient`
1. Each time you want to call the Graph, add the scope you need by 

   ```CSharp 
   graphServiceClient.Scopes.Add("scope");
   ```

1. Use the `graphServiceClient` as you would use the `GraphServiceClient` instance. When interaction is needed with the user to have it authenticated, this will happen. The tokens are cached so next time the users run tha zpplication, they won't need to re-sign-in.

The following code snipet shows how to get the display name of the authenticated user.

```CSharp
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
```

### More variations

It's also possible to let the application developer control when the interactions happen with the user. For this, the application developer needs to subscribe to the 

```CSharp
            graphServiceClient.InteractionRequired += GraphServiceClient_InteractionRequired;
```

the `GraphServiceClient_InteractionRequired` will need to update the UI to tell the user that some interaction is required (such as a need to sign-in).


## The authentication helper

This repository also contains a by-product which is an MSAL helper, helping developers of desktop/mobile applications to use MSAL quicker.

To use it:

1. Add the Microsoft.Identity.Client.Helpers NuGet pacakge to your project
1. Register an application with the Azure Portal
1. Instanciate a `SingleUserPublicClientApplication`

   ```CSharp
   SingleUserPublicClientApplication application = new SingleUserPublicClientApplication(clientId);
   ```

1. When you need to call a resource, you can:
   - add the scopes you need

     ```CSharp
     application.Scopes.Add("User.Read");
     ```

   - Authenticate the HttpClient or the HttpRequest you want to use to call the web API:

     ```CSharp
     HttpClient client = new HttpClient();
     await application.AuthenticateClientAsync(client);
     ```

    - call your Web API using the HttpClient, or HttpRequest:
      ```CSharp
      string json = await client.GetStringAsync("https://graph.microsoft.com/v1.0/me");
      ```

Here is the code, which, in addition, shows the Displayable Id of the authenticated user.

```CSharp
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
```

## To learn more

### Samples

This repository contains 4 samples:
Sample | Description
------ | ------------
auth\ConsoleApp-with-interactive-authentication | Simple console application which queries the Microsoft Graph through its REST API. This illustrates the MSAL helper (Microsoft.Identity.Client.Helpers), and is easily transposable to call your own Web API.
auth\ConsoleAppUsingGraphSdkWithAuthentication | Similar sample, but this time using the .NET Microsoft Graph SDK. This illustrates the notion of `SingleUserPublicClientApplicationAuthenticationProvider` which manages the authentication part. Note unless you really want to understand about authentication, you might rather look at the two following samples (which leverage the Microsoft.Graph.Helper and abstract out authentication)
graph\ConsoleApp-querying-msgraph-with-graphsdk | Simple console application using the `SingleUserPublicClientApplication` to query the graph (Me endpoint, calendar and last email)
graph\WpfAppCallingGraph | WPF application letting the developer control when sign-in is required. An icon with a key is displayed when a user interaction is required.

### Conceptual documentation

TBD

## To build this repo

run `build.cmd` in a Visual Studio 2017 developer console.
The nuget packages are produced part of the build and are located, for each helper library, under bin\Debug or bin\Release.
