using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Helpers
{
    public class SingleUserPublicClientApplication
    {
        /// <summary>
        /// List of Scopes that the application requests 
        /// </summary>
        public List<string> Scopes = new List<string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clientId">Client ID (application ID) of the application</param>
        /// <param name="authority">Authority. Optional. you might want to set it if you want to restrict signing to: 
        /// <list type="bullet">
        /// <item>only one tenant (in which case use https://login.microsoftonline.com/tenantId/ of
        /// https://login.microsoftonline.com/domainName/</item>
        /// <item>Work and School accounts only (in which case, use https://login.microsoftonline.com/organizations/ )</item>
        /// <item>Microsoft personal accounts only (in which case, use https://login.microsoftonline.com/consumers/ )</item>
        /// </list>
        /// </param>
        /// <remarks>A cache is token cache is provided</remarks>
        public SingleUserPublicClientApplication(string clientId, string authority = null)
        {
            if (authority != null)
            {
                app = new PublicClientApplication(clientId, authority, tokenCache);
            }
            else
            {
                app = new PublicClientApplication(clientId, "https://login.microsoftonline.com/common/", tokenCache);
            }
        }

        /// <summary>
        /// Signs the user out
        /// </summary>
        public void SignOut()
        {
            while (app.Users.Any())
            {
                SignOut(app.Users.FirstOrDefault());
            }
        }

        /// <summary>
        /// Signs out a given user
        /// </summary>
        /// <param name="user"></param>
        private void SignOut(IUser user)
        {
            app.Remove(user);
        }

        /// <summary>
        /// User of the application
        /// </summary>
        public IUser User
        {
            get
            {
                return app.Users.FirstOrDefault();
            }
        }

        /// <summary>
        /// Adds a token to the HttpRequestMessage. Interaction happens if needed
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AuthenticateRequestAsync(HttpRequestMessage request)
        {
            AuthenticationResult result = await AcquireTokenForScopesAsync();

            if (result != null)
            {
                request.Headers.Add("Authorization", $"bearer {result.AccessToken}");
            }
            return result;
        }

        /// <summary>
        /// Adds a token to the HttpRequestMessage. Interaction happens if needed
        /// </summary>
        /// <param name="client">Http client</param>
        /// <returns>The authentication result</returns>
        public async Task<AuthenticationResult> AuthenticateClientAsync(HttpClient client)
        {
            AuthenticationResult result = await AcquireTokenForScopesAsync();

            if (result != null)
            {
                client.DefaultRequestHeaders.Add("Authorization", $"bearer {result.AccessToken}");
            }
            return result;
        }

        /// <summary>
        /// Acquires a token for the given scopes. This tries:
        /// <list type="bullent">
        /// <item>To get a token silently first</item>
        /// <item>managing incremental consents</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        private async Task<AuthenticationResult> AcquireTokenForScopesAsync()
        {
            bool needInteractionAPriori = !app.Users.Any();
            AuthenticationResult result = null;

            if (!needInteractionAPriori)
            {
                try
                {
                    result = await app.AcquireTokenSilentAsync(Scopes, app.Users.FirstOrDefault());
                }
                catch (MsalServiceException msalServiceException)
                {
                    result = await AcquireTokenForClaimChallengeAsync(msalServiceException);
                }
                catch (MsalException msalException)
                {
                    if (msalException is MsalUiRequiredException)
                    {
                        try
                        {
                            result = await app.AcquireTokenAsync(Scopes);

                        }
                        catch (Exception ex2)
                        {
                            string message = ex2.Message;
                            throw;
                        }
                    }
                }
            }
            else
            {
                result = await app.AcquireTokenAsync(Scopes);
            }

            return result;
        }

        private async Task<AuthenticationResult> AcquireTokenForClaimChallengeAsync(MsalServiceException msalServiceException)
        {
            AuthenticationResult result = null;
            string extraQueryParameters = string.Empty;
            if (!string.IsNullOrEmpty(msalServiceException.Claims))
            {
                extraQueryParameters = $"claims={msalServiceException.Claims}";
            }
            result = await app.AcquireTokenAsync(Scopes,
                                                 app.Users.FirstOrDefault(),
                                                 new UIBehavior(),
                                                 extraQueryParameters);


            return result;
        }

        /// <summary>
        /// This is for public client applications (Desktop / Mobile apps)
        /// </summary>
        PublicClientApplication app;

        // Make sure that we add it only in the case of .NET FW and .NET Core, not Xamarin / UWP
        TokenCache tokenCache = TokenCacheHelper.GetUserCache();

    }
}
