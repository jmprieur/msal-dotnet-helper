using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Helpers
{
    public class Options
    {
        /// <summary>
        /// Location of the token cache path. By default, the token cache is created in the same folder as the application, and
        /// has the same name as the application postfixed by ".msalcache.bin". This file is crypted
        /// </summary>
        public static string TokenCachePath
        {
            get { return TokenCacheHelper.CacheFilePath; }
            set { TokenCacheHelper.CacheFilePath = value; }
        }
    }
}
