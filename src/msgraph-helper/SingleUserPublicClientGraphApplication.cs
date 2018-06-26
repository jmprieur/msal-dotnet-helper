using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace Microsoft.Graph.Helpers
{
    public class SingleUserPublicClientGraphApplication : GraphServiceClient
    {
        //
        // Summary:
        //     Instantiates a new SingleUserPublicClientGraphApplication.
        //
        // Parameters:
        //   httpProvider:
        //     The Microsoft.Graph.IHttpProvider for sending requests.

        /// <summary>
        /// Constructor of a Single User Public client application (i.e. for Desktop/Mobile) Leveraging Microsoft Graph
        /// </summary>
        /// <param name="clientId">ClientID (also named Application ID) as registered in the Azure portal (https://portal.azure.com)</param>
        /// <param name="authority">Optional cloud authority used for the authentication. this can be null, in which case the authority is
        /// set to https://login.microsoftonline.com, which is the global Microsoft Cloud. You might want to use another authority in the case
        /// you want to target national or a sovereign cloud (See https://aka.ms/authorities) </param>
        /// <param name="msGraphBaseUrl">Optional base URL for the Microsoft Graph service. If you don't specify such a URL, the
        /// Microsoft Graph V1.0 will be used (https://graph.microsoft.com/v1.0). You might want to use this parameter to use another
        /// version of Microsoft graph (such as https://graph.microsoft.com/beta) </param>
        public SingleUserPublicClientGraphApplication(string clientId, string authority = null, string msGraphBaseUrl = "https://graph.microsoft.com/v1.0") 
            : this(msGraphBaseUrl, new SingleUserPublicClientApplicationAuthenticationProvider(clientId, authority), null)
        {

        }

        /// <summary>
        /// Constructor of a Single User Public client application (i.e. for Desktop/Mobile) targetting a specific URL of Microsoft Graph
        /// </summary>
        /// <param name="baseUrl">The base service URL for Microsoft Graph. For example, "https://graph.microsoft.com/v1.0".</param>
        /// <param name="authenticationProvider">The authentication provider</param>
        /// <param name="httpProvider">An optional Http Provider</param>
        private SingleUserPublicClientGraphApplication(string baseUrl, SingleUserPublicClientApplicationAuthenticationProvider authenticationProvider, IHttpProvider httpProvider)
            : base(baseUrl, authenticationProvider, httpProvider)
        {
            this.authenticationProvider = authenticationProvider;
        }

        /// <summary>
        /// Authentication provider
        /// </summary>
        private SingleUserPublicClientApplicationAuthenticationProvider authenticationProvider;

        /// <summary>
        /// Sign out the user
        /// </summary>
        public void SignOut()
        {
            authenticationProvider.SignOut();
        }

        /// <summary>
        /// User signed-in with the application
        /// </summary>
        public IUser User
        {
            get
            {
                return authenticationProvider.User;
            }
        }

        public IList<string> Scopes
        {
            get
            {
                return authenticationProvider.Scopes;
            }
        }
    }
}
