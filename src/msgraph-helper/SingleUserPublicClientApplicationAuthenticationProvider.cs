using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client.Helpers;

namespace Microsoft.Graph.Helpers
{
    // [bogavril]: I particularly like inheritance here, it's confusing to users what they should use - the derived class or the base class?
    // Instead, I would use composition - rename the SingleUserPublicClientApplicationAuthenticationProvider to SingleUserPublicClientApplicationAuthenticationAdapter
    // and make it clear what you wish to expose to users. 
    // Note: this is not very important as you just want to prototype the API.
    public class SingleUserPublicClientApplicationAuthenticationProvider : SingleUserPublicClientApplication, IAuthenticationProvider
    {
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
        public SingleUserPublicClientApplicationAuthenticationProvider(string clientId, string authority = null) : base(clientId, authority) { }

        Task IAuthenticationProvider.AuthenticateRequestAsync(HttpRequestMessage request)
        {
            Task result = base.AuthenticateRequestAsync(request, !InteractionRequiredAsSubscribers || AcceptInteraction != AcceptInteraction.None);
            if (AcceptInteraction == AcceptInteraction.Once)
            {
                // [bogavril] I would not 
                AcceptInteraction = AcceptInteraction.None;
            }
            return result;
        }

        // I would not allow a setter here. As a developer, I would not expect "settings" to change. Use a counter to count the 
        // actual number of interactions.
        public AcceptInteraction AcceptInteraction { get; set; }
    }
}
