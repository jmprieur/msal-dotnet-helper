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
        /// Constructor.
        /// </summary>
        /// <param name="clientId">This is the string representation of a GUID which is the client ID (also named application ID)
        /// of the application that you would have registered in the Azure portal</param>
        /// <param name="authority">Optional. You might want to specify: which clouds will authenticate your users, or restrict the audience
        /// of your application to a specific organization, any Azure AD directory, or Microsoft personal accounts only
        /// <remarks>A cache is token cache is provided</remarks>
        public SingleUserPublicClientApplication(string clientId, Authority authority = null)
        {
            if (authority != null)
            {
                app = new PublicClientApplication(clientId, authority.ToString(), tokenCache);
            }
            else
            {
                app = new PublicClientApplication(clientId, Authority.Default.ToString(), tokenCache);
            }
        }

        /// <summary>
        /// Options to control the behavior of the helper
        /// </summary>
        public Options Options { get; } = new Options();

        /// <summary>
        /// List of Scopes that the application requests 
        /// </summary>
        public List<string> Scopes { get; } = new List<string>();

        /// <summary>
        /// Event firing when interaction is required and the app does not allow the authentication
        /// dialog to popup when it decides
        /// </summary>
        /// <remarks>
        /// If, as an app developer you mind about having authentication dialogs firing
        /// when the application requires it, you can subscribe to the InteractionRequired
        /// event, and update the UI to notify the user that interaction is required (for instance
        /// by displaying the icon of a key). Then, the user can trigger the authentication when s/he
        /// wishes and your application will call <see cref="AuthenticateRequestAsync"/> 
        /// or <see cref="AuthenticateClientAsync"/> accepting the interaction (passing a boolean set to <c>true</c>
        /// as the second parameter
        /// </remarks>
        public event EventHandler InteractionRequired;

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
        /// Adds a token to the HttpRequestMessage. Interaction happens if needed except if the application
        /// has subscribed to the InteractionRequired event
        /// </summary>
        /// <param name="request">Http request message on which to setup the authorization header</param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AuthenticateRequestAsync(HttpRequestMessage request, bool? doInteraction = null)
        {
            bool canDoInteraction = CanDoInteraction(doInteraction);
            AuthenticationResult result = await AcquireTokenForScopesAsync(canDoInteraction);

            if (result != null)
            {
                request.Headers.Add("Authorization", $"bearer {result.AccessToken}");
            }
            return result;
        }

        /// <summary>
        /// Is the framework authorized to do a user interaction right now.
        /// </summary>
        /// <param name="doInteraction"></param>
        /// <returns></returns>
        private bool CanDoInteraction(bool? doInteraction)
        {
            return !doInteraction.HasValue && !InteractionRequiredAsSubscribers
                                     || doInteraction.HasValue && doInteraction.Value;
        }

        protected bool InteractionRequiredAsSubscribers
        {
            get
            {
                return InteractionRequired!=null && InteractionRequired.GetInvocationList().Any();
            }
        }

        /// <summary>
        /// Adds a token to the HttpRequestMessage. Interaction happens if needed except if the application
        /// has subscribed to the InteractionRequired event
        /// </summary>
        /// <param name="client">Http client on which to setup the default authorization header</param>
        /// <returns>The authentication result</returns>
        public async Task<AuthenticationResult> AuthenticateClientAsync(HttpClient client, bool? doInteraction = null)
        {
            bool canDoInteraction = CanDoInteraction(doInteraction);
            AuthenticationResult result = await AcquireTokenForScopesAsync(canDoInteraction);

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
        private async Task<AuthenticationResult> AcquireTokenForScopesAsync(bool acceptAuthentication)
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
                    if (acceptAuthentication)
                    {
                        result = await AcquireTokenForClaimChallengeAsync(msalServiceException);
                    }
                    else
                    {
                        InteractionRequired(this, EventArgs.Empty);
                    }
                }
                catch (MsalException msalException)
                {
                    if (msalException is MsalUiRequiredException)
                    {
                        try
                        {
                            result = await InteractWithUsersOrNotifyInteractionRequiredAsync(acceptAuthentication);
                        }
                        catch (Exception ex)
                        {
                            string message = ex.Message;
                            throw;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    result = await InteractWithUsersOrNotifyInteractionRequiredAsync(acceptAuthentication);
                }
                catch (Exception ex2)
                {
                    string message = ex2.Message;
                    throw;
                }
            }

            return result;
        }

        private async Task<AuthenticationResult> InteractWithUsersOrNotifyInteractionRequiredAsync(bool acceptAuthentication)
        {
            AuthenticationResult result = null;
            if (acceptAuthentication)
            {
                result = await app.AcquireTokenAsync(Scopes);
            }
            else
            {
                InteractionRequired(this, EventArgs.Empty);
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
