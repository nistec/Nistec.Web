using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Nistec.Channels;

namespace Nistec.Web.Cms
{

   
    public class CmsConfig : Nistec.Generic.NetConfigSection
    {
        public static NetProtocol CacheProtocol = NetProtocol.Tcp;

        private static CmsConfig settings;

        ///// <summary>
        ///// Get the <see cref="CmsConfig"/>
        ///// </summary>
        //public static CmsConfig Settings
        //{
        //    get
        //    {
        //        if (settings == null)
        //        {
        //            settings = (CmsConfig)ConfigurationManager.GetSection("CmsConfig") ?? new CmsConfig();
        //        }
        //        return settings;
        //    }
        //}

        public static CmsConfig GetConfig()
        {
            if (settings == null)
            {
                settings = (CmsConfig)ConfigurationManager.GetSection("CmsConfig") ?? new CmsConfig();
            }
            return settings;
        }


        [System.Configuration.ConfigurationProperty("CmsSettings")]
        public CmsConfigItem CmsSettings
        {
            get
            {
                object o = this["CmsSettings"];
                return o as CmsConfigItem;
            }
        }

    }

    /// <summary>
    /// Represents a entity cache section  settings within a configuration file.
    /// </summary>
    public class CmsConfigItem : System.Configuration.ConfigurationElement
    {
        /// <summary>Get indicate if cahce enabled</summary>
        [ConfigurationProperty("CacheEnable", DefaultValue = false, IsRequired = false)]
        public bool CacheEnable
        {
            get { return Types.ToBool(this["CacheEnable"], false); }
        }

        /// <summary>Get the cache timeout</summary>
        [ConfigurationProperty("CacheTimeout", DefaultValue = 30, IsRequired = false)]
        public int CacheTimeout
        {
            get { return Types.ToInt(this["CacheTimeout"], 30); }
        }

        /// <summary>Get the network protocol</summary>
        [ConfigurationProperty("CacheProtocol", DefaultValue = "tcp", IsRequired = false)]
        public string CacheProtocol
        {
            get { return (string)this["CacheProtocol"]; }
        }

        /// <summary>Get the connection key</summary>
        [ConfigurationProperty("ConnectionKey", DefaultValue = "Default", IsRequired = false)]
        public string ConnectionKey
        {
            get { return (string)this["ConnectionKey"]; }
        }

    }
}
