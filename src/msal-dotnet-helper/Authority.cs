using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Helpers
{
    /// <summary>
    /// Audience of users authorized to sign-in to the application
    /// </summary>
    public enum Audience
    {
        /// <summary>
        /// Only users from one specific directory (Contoso for instance) can sign-in to the application
        /// </summary>
        AccountsInSpecificDirectoryOnly = 1,

        /// <summary>
        /// Users from any Azure Active Directory can sign-in to the application
        /// </summary>
        AcountsInAnyAzureAdDirectory = 2,

        /// <summary>
        /// Only users having a Microsoft Personal account (e.g. Skype, Xbox, Outlook.com) can sign-in 
        /// to the application
        /// </summary>
        MicrosoftPersonalAcountsOnly = 4,

        /// <summary>
        /// Any Microsoft identity (any Azure AD or Personal account) can sign-in to the application
        /// </summary>
        AccountsInAnyAzureAdDirectoryAndPersonalMicrosoftsAccounts = 6,
    }

    /// <summary>
    /// Authority (Security Token server) authenticating users
    /// </summary>
    public class Authority
    {
        /// <summary>
        /// Constructor of the authority letting users of a given Azure AD tenant specified by its domain name, sign-in to the application
        /// </summary>
        /// <param name="tenantDomainName">Of of the domain names associated with the Azure Active Directory tenants</param>
        /// <remarks>This authority is created in the Microsoft Azure public cloud (See <see cref="AzurePublicCloudUrl"/>) )</remarks>
        public Authority(string tenantDomainName) : this(tenantDomainName, MicrosoftAzurePublicCloudUrl)
        {
        }

        /// <summary>
        /// Constructor of the authority letting users of a given Azure AD tenant specified by its tenant ID, sign-in to the application
        /// </summary>
        /// <param name="tenantId">Tenant ID (also named directory ID) of the Azure AD directory</param>
        /// <remarks>This authority is created in the Microsoft Azure public cloud (See <see cref="AzurePublicCloudUrl"/>) )</remarks>
        public Authority(Guid tenantId) : this(tenantId, MicrosoftAzurePublicCloudUrl)
        {
        }

        /// <summary>
        /// Constructor of the authority letting users of a given Azure AD tenant specified by its tenant ID in a given cloud, sign-in to the application
        /// </summary>
        /// <param name="tenantId">Tenant ID (also named directory ID) of the Azure AD directory</param>
        /// <param name="cloudUrl">Url of the specific cloud</param>
        public Authority(string tenantDomainName, string cloudUrl)
        {
            if (tenantDomainName == null)
            {
                throw new ArgumentNullException("tenantDomainName");
            }
            this.CloudUrl = cloudUrl;
            this.Audience = Audience.AccountsInSpecificDirectoryOnly;
            authority = GetAuthority(Audience, tenantDomainName, cloudUrl);
        }

        /// <summary>
        /// Constructor of the authority letting users of a given Azure AD tenant specified by its tenant ID in a given cloud, sign-in to the application
        /// </summary>
        /// <param name="tenantId">Tenant ID (also named directory ID) of the Azure AD directory</param>
        /// <param name="cloudUrl">Url of the specific cloud</param>
        public Authority(Guid tenantId, string cloudUrl)
        {
            if (tenantId == null)
            {
                throw new ArgumentNullException("tenantId");
            }
            this.CloudUrl = cloudUrl;
            this.Audience = Audience.AccountsInSpecificDirectoryOnly;
            authority = GetAuthority(Audience, tenantId.ToString(), cloudUrl);
        }


        /// <summary>
        /// Constructor of an authority letting users of a specified audience of the Microsoft public cloud 
        /// sign-in to the application
        /// </summary>
        /// <param name="audience">Audience</param>
        public Authority(Audience audience) : this(audience, MicrosoftAzurePublicCloudUrl)
        {
        }

        /// <summary>
        /// Constructor of an authority letting users of a specified audience sign-in to the application
        /// </summary>
        /// <param name="audience">Audience</param>
        /// <param name="cloudUrl">Url for the targeted cloud.</param> is <c>AccountsInSpecificDirectoryOnly</c>
        public Authority(Audience audience, string cloudUrl)
        {
            if (audience == Audience.AccountsInSpecificDirectoryOnly)
            {
                throw new ArgumentException("to create an audience for a specific azure AD tenant, please use another constructor", "audience");
            }
            this.Audience = audience;
            this.CloudUrl = cloudUrl;
            authority = GetAuthority(audience, cloudUrl, null);
        }

        /// <summary>
        /// Url of the Cloud
        /// </summary>
        public string CloudUrl
        {
            get; private set;
        }

        /// <summary>
        /// Audience (kind of users authorized to sign-in)
        /// </summary>
        public Audience Audience
        {
            get; private set;
        }

        /// <summary>
        /// Url of the Microsoft Azure public cloud
        /// </summary>
        public const string MicrosoftAzurePublicCloudUrl = "https://login.microsoftonline.com";

        /// <summary>
        /// Url of the Microsoft Azure Chenese sovereign cloud
        /// </summary>
        public const string MicrosoftAzureChinaCloudUrl = "https://login.chinacloudapi.cn";

        /// <summary>
        /// Url of the Microsoft Azure German sovereign cloud
        /// </summary>
        public const string MicrosoftAzureGernmanyCloudUrl = "https://login.microsoftonline.de";

        /// <summary>
        /// Default authority
        /// </summary>
        public static Authority Default
        {
            get
            {
                if (defaultAuthority == null)
                {
                    defaultAuthority = new Authority(Audience.AccountsInAnyAzureAdDirectoryAndPersonalMicrosoftsAccounts, MicrosoftAzurePublicCloudUrl);
                }
                return defaultAuthority;
            }
        }
        private static Authority defaultAuthority;

        protected string authority { get; set; }

        /// <summary>
        /// Get the format of the 
        /// </summary>
        /// <param name="audience"></param>
        /// <param name="cloudUrl"></param>
        /// <param name="tenantInformation"></param>
        /// <returns></returns>
        private string GetAuthority(Audience audience, string cloudUrl, string tenantInformation)
        {
            switch (audience)
            {
                case Audience.AccountsInSpecificDirectoryOnly:
                    return $"{cloudUrl}/{tenantInformation}/";

                case Audience.AcountsInAnyAzureAdDirectory:
                    return $"{cloudUrl}/organizations/";

                case Audience.MicrosoftPersonalAcountsOnly:
                    return $"{cloudUrl}/consumers/";

                case Audience.AccountsInAnyAzureAdDirectoryAndPersonalMicrosoftsAccounts:
                    return $"{cloudUrl}/common/";

                default:
                    throw new ArgumentException("Unsupported tenantInformation");
            }
        }

        /// <summary>
        /// String representation of the authority
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return authority;
        }

        /// <summary>
        /// Is the authority whitelisted by the authentication library
        /// </summary>
        internal virtual bool IsWhiteListed { get { return true; } }
    }
}
