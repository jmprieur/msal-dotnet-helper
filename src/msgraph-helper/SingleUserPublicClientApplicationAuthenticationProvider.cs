using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client.Helpers;

namespace Microsoft.Graph.Helpers
{
    public class SingleUserPublicClientApplicationAuthenticationProvider : SingleUserPublicClientApplication, IAuthenticationProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clientId">Client ID (application ID) of the application</param>
        /// <param name="authority">Authority. Optional. you might want to set it if you want to restrict users signing-in to the 
        /// <remarks>A cache is token cache is provided</remarks>
        public SingleUserPublicClientApplicationAuthenticationProvider(string clientId, Authority authority = null) : base(clientId, authority) { }

        Task IAuthenticationProvider.AuthenticateRequestAsync(HttpRequestMessage request)
        {
            Task result = base.AuthenticateRequestAsync(request, !InteractionRequiredAsSubscribers || AcceptInteraction != AcceptInteraction.None);
            if (AcceptInteraction == AcceptInteraction.Once)
            {
                AcceptInteraction = AcceptInteraction.None;
            }
            return result;
        }

        public AcceptInteraction AcceptInteraction { get; set; }
    }
}
