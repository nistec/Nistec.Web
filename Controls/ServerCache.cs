using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nistec.Channels.RemoteCache;
using Nistec.Data.Entities.Config;

namespace Nistec.Web.Controls
{
    public class ServerSyncCache
    {
        private static RemoteCacheApi.SyncApi Current
        {
            get { return RemoteCacheApi.Sync(RemoteCacheSettings.Protocol); }
        }

        public static T Get<T>(string entityName, string[] keys)
        {
            if (entityName==null || keys == null)
                return default(T);
            return Current.GetEntity<T>(entityName, keys);
        }
    }
    public class ServerCache
    {
      
        private static RemoteCacheApi.CacheApi Current
        {
            get { return RemoteCacheApi.Get(RemoteCacheSettings.Protocol); }
        }

        public static bool EnableCache
        {
            get
            {
                return EntityConfig.Settings.EntityCache.Enable;
            }
        }
        public static string CacheProtocol
        {
            get
            {
                
                return EntityConfig.Settings.EntityCache.Protocol;
            }
        }
        static int _CacheTimeout;
        public static int CacheTimeout
        {
            get
            {

                if (_CacheTimeout == 0)
                {
                    _CacheTimeout = EntityConfig.Settings.EntityCache.Timeout;
                }
                return _CacheTimeout;
            }
        }

        #region cache methods
        public static string GetJson(string key)
        {
            if (key == null)
                return null;
            string o = Current.GetJson(key, Serialization.JsonFormat.None);
            return o;
        }

        public static T Get<T>(string key)
        {
            if (key == null)
                return default(T);
            return Current.Get<T>(key);
        }

        public static T GetOrCreate<T>(string key, Func<T> function, int expirationMinutes = 0)
        {
            T instance = default(T);
            if (key == null)
                return instance;
            if (EnableCache)
            {
                instance = Get<T>(key);
                if (instance == null)
                {
                    instance = function();

                    if (instance != null)
                    {
                        Insert(key, instance, expirationMinutes);
                    }
                }
                else
                {
                    return instance;
                }
            }
            else
            {
                instance = function();
            }
            return instance;
        }
        public static IList<T> GetOrCreateList<T>(string key, Func<IList<T>> function, int expirationMinutes = 0)
        {
            IList<T> instance = null;
            if (key == null)
                return instance;
            if (EnableCache)
            {
                instance = Get<IList<T>>(key);
                if (instance == null)
                {
                    instance = function();

                    if (instance != null && instance.Count > 0)
                    {
                        Insert(key, instance, expirationMinutes);
                    }
                }
                else
                {
                    return instance;
                }
            }
            else
            {
                instance = function();
            }
            return instance;
        }

        public static void Insert(string key, object value, int expirationMinutes = 0)
        {
            if (key != null)
            {
                if (expirationMinutes <= 0)
                    Current.Add(key, value, CacheTimeout);
                else
                    Current.Add(key, value, expirationMinutes);
            }
        }

    
        public static void Remove(string key)
        {
            if (key != null)
            {
                Current.Remove(key);
            }
        }
        public static void Remove(IList<string> keys)
        {
            var cur = Current;
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    cur.Remove(key);
                }
            }
        }

        #endregion
    }
}
