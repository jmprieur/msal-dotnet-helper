using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Helpers;
using System;
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
        public SingleUserPublicClientGraphApplication(string clientId, Authority authority = null, string msGraphBaseUrl = "https://graph.microsoft.com/v1.0") 
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
        public event EventHandler InteractionRequired
        {
            add { authenticationProvider.InteractionRequired += value;  }
            remove { authenticationProvider.InteractionRequired -= value; }
        }

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

        public AcceptInteraction AcceptInteraction
        {
            get
            {
                return acceptInteraction;
            }

            set
            {
                acceptInteraction = value;
                authenticationProvider.AcceptInteraction = value;
            }
        }
        private AcceptInteraction acceptInteraction;


    }
}
