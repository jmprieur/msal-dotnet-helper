# Helpers for MSAL.NET and Microsoft Graph .NET SDK

Experimental libraries aiming at helping developers getting up to speed quicker at using MSAL.NET in some simple scenarios. This repo contains two libraries:

- [Microsoft.Graph.Helper](#microsoft-graph-helper) which is a helper library for the MIcrosoft Graph .NET SDK
- [Microsoft.Identity.Client.Helper](#the-authentication-helper) which is a helper library for MSAL.NET.

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
1. Each time you want to call the Graph, add the scope you need by:

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

It's also possible to let the application developer control when the interactions happen with the user. For this, the application developer needs to subscribe to the `InteractionRequired` event on the `SingleUserPublicClientGraphApplication` instance:

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

Sample                                          | Description
----------------------------------------------- | -----------
auth\ConsoleApp-with-interactive-authentication | Simple console application which queries the Microsoft Graph through its REST API. This illustrates the MSAL helper (Microsoft.Identity.Client.Helpers), and is easily transposable to call your own Web API.
auth\ConsoleAppUsingGraphSdkWithAuthentication | Similar sample, but this time using the .NET Microsoft Graph SDK. This illustrates the notion of `SingleUserPublicClientApplicationAuthenticationProvider` which manages the authentication part. Note unless you really want to understand about authentication, you might rather look at the two following samples (which leverage the Microsoft.Graph.Helper and abstract out authentication)
graph\ConsoleApp-querying-msgraph-with-graphsdk | Simple console application using the `SingleUserPublicClientApplication` to query the graph (Me endpoint, calendar and last email)
graph\WpfAppCallingGraph | WPF application letting the developer control when sign-in is required. An icon with a key is displayed when a user interaction is required.

### Conceptual documentation of the authentication helper

#### SingleUserPublicClientApplication class
The authentication helper is really one public class: `SingleUserPublicClientApplication` which takes care of everything.

![class](https://user-images.githubusercontent.com/13203188/43017323-f8a2d8c4-8c55-11e8-9b01-8071ee4c4320.png)

Its members are the following:

##### The constructor
the constructor which takes two arguments:
- `clientId` is the string representation of a GUID which is the client ID (also named application ID) of the application that you would have registered in the Azure portal
- `authority` is an optional argument that can be set if you want to restrict users that sign-in to the application to belong to certain clouds, or audiences (specific Azure AD directories, any Azure AD directories, personal accounts or not, Azure AD B2C directories)
   
##### Properties
- `Scopes` is used to add or remove permissions scopes to get consent to call an API
- `User` is the signed-in user

##### Authenticating an HttpClient or and HttpRequest to call a protected API

Before calling a protected API, you need to authenticate the HttpClient or HttpRequest that will be used to make the call. To do this
you can use one of the following methods that will manage for you the security tokens and the user interaction:
- `AuthenticateClientAsync(HttpClient)`applies to an HttpClient
- `AuthenticateRequestAsync(HttpClient)`applies to an HttpRequest

##### Signing the user out

The authentication helper maintains a token cache for the public client application, which will avoid forcing the user to
resign-in next time s/he uses the application.
You can sign the user out by calling `SignOut()`

##### Controlling the user interaction

Optionally if you really want to control the user interaction, you can subscribe to the `InteractionRequired` event. you will be notified when a user interaction needs to happen to sign-in the user, or present a consent

#### Advanced scenarios: restricting the users that sign-in

The classes named `Authority` and `AzureADB2CAuthority` enable you to restrict the users that sign-in to your application.
A number of constructors enble to specify:
- the cloud to use (by default the Microsoft Azure public cloud)
- the audience among: users of a specific Azure AD directory, users of any directory, Microsoft personal Accounts only, or any Microsoft identity

![image](https://user-images.githubusercontent.com/13203188/43077229-306d53f0-8e87-11e8-882b-ec20fd7fd92b.png)

The constructor of `AzureADB2CAuthority` also enables you to specify the AzureAD B2C policy to apply

## To build this repo

run `build.cmd` in a Visual Studio 2017 developer console.
The nuget packages are produced part of the build and are located, for each helper library, under bin\Debug or bin\Release.
