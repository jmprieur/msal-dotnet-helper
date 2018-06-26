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
            return base.AuthenticateRequestAsync(request);
        }
    }
}
