using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Helpers
{
    public class AadB2CAuthority : Authority
    {
        /// <summary>
        /// Constructor of an AzureAD B2C authorityfor a given Tenant ID and policy name
        /// </summary>
        /// <param name="tenantId">Tenant ID of the Azure AD B2C directory</param>
        /// <param name="policyName">Name of the policy (for instance B2c_1_susi, B2c_edit_profile, B2C_1_reset, etc ...)</param>
        public AadB2CAuthority(Guid tenantId, string policyName) : base(Audience.AccountsInSpecificDirectoryOnly)
        {
            authority = "$https://login.microsoftonline.com/tfp/{tenantId}/{policyName}";
        }

        /// <summary>
        /// Constructor of an AzureAD B2C authorityfor a given custom domain and policy name
        /// </summary>
        /// <param name="tenantId">Tenant ID of the Azure AD B2C directory</param>
        /// <param name="policyName">Name of the policy (for instance B2c_1_susi, B2c_edit_profile, B2C_1_reset, etc ...)</param>

        public AadB2CAuthority(string domainName, string policyName) : base(Audience.AccountsInSpecificDirectoryOnly)
        {
            authority = "${domainName}/{policyName}";
        }

        /// <summary>
        /// Azure AD B2C Authorities are not whitelisted (as they are custom). This does not mean that this 
        /// is bypassing security in any mean.
        /// </summary>
        internal override bool IsWhiteListed => false;
    }
}
