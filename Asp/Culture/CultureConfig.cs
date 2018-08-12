using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nistec.Generic;
using Nistec;

namespace Nistec.Web.Asp
{

    public enum Cultures
    {
        en,
        he
    }

    public class CultureConfig
    {
        public const string CultureCacheKey = "Nistec.Web.Culture.";
        
        //public static string CultureCacheKey { get { return Types.NzOr(NetConfig.AppSettings["CultureCacheKey"], "Nistec.Web.Culture"); } }

        public static string DefaultCulture { get { return Types.NzOr(NetConfig.AppSettings["DefaultCulture"], "en"); } }

        public static string SiteName { get { return Types.NzOr(NetConfig.AppSettings["SiteName"], "Default"); } }

    }
}
