using Nistec;
using Nistec.Channels;
//using Nistec.Channels.RemoteCache;
using Nistec.Data;
using Nistec.Data.Entities;
using Nistec.Data.Entities.Config;
using Nistec.Generic;
using Nistec.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;


namespace Nistec.Web.Controls
{
    public class WebCache
    {
        #region fields
        internal class Fields
        {
            public const string LibName = "lib";
            public const string GroupName = "group";
            public const string AccountId = "aid";
            public const string UserId = "uid";
            public const string EntityName = "mapping";
            public const string Args = "arg";
        }
        #endregion

        #region key formats
        public static string GetSession(string LibName, int AccountId)
        {
            return string.Format("{0}={1}_{2}={3}", Fields.LibName, LibName, Fields.AccountId, AccountId).ToLower();
        }

        public static string GetKey(string LibName, string GroupName, int AccountId, int UserId, string EntityName)
        {
            return string.Format("{0}={1}_{2}={3}_{4}={5}_{6}={7}_{8}={9}", Fields.LibName, LibName, Fields.GroupName, GroupName, Fields.AccountId, AccountId, Fields.UserId, UserId, Fields.EntityName, EntityName).ToLower();
        }
        public static string GetKey(string LibName, string GroupName, int AccountId, int UserId, string EntityName, string Args)
        {
            return string.Format("{0}={1}_{2}={3}_{4}={5}_{6}={7}_{8}={9}_{10}={11}", Fields.LibName, LibName, Fields.GroupName, GroupName, Fields.AccountId, AccountId, Fields.UserId, UserId, Fields.EntityName, EntityName, Fields.Args, Args).ToLower();
        }
        public static string GetKey(string LibName, string GroupName, int AccountId, string EntityName)
        {
            return string.Format("{0}={1}_{2}={3}_{4}={5}_{6}={7}", Fields.LibName, LibName, Fields.GroupName, GroupName, Fields.AccountId, AccountId, Fields.EntityName, EntityName).ToLower();
        }
        public static string GetKey(string LibName, string GroupName, string EntityName, string Args)
        {
            return string.Format("{0}={1}_{2}={3}_{4}={5}_{6}={7}", Fields.LibName, LibName, Fields.GroupName, GroupName, Fields.EntityName, EntityName, Fields.Args, Args).ToLower();
        }
        public static string GetGroupKey(string LibName, string GroupName, int AccountId)
        {
            return string.Format("{0}={1}_{2}={3}_{4}={5}_", Fields.LibName, LibName, Fields.GroupName, GroupName, Fields.AccountId, AccountId).ToLower();
        }
        public static string GetGroupKey(string LibName, string GroupName, string Args)
        {
            return string.Format("{0}={1}_{2}={3}_{4}={5}_", Fields.LibName, LibName, Fields.GroupName, GroupName, Fields.Args, Args).ToLower();
        }
        public static string GetGroupKey(string LibName, string GroupName)
        {
            return string.Format("{0}={1}_{2}={3}_", Fields.LibName, LibName, Fields.GroupName, GroupName).ToLower();
        }
        public static string GetGroupKey(string LibName)
        {
            return string.Format("{0}={1}_", Fields.LibName, LibName).ToLower();
        }
        #endregion

        #region cache methods
        public static string GetJson(string key)
        {
            if (key == null)
                return null;
            object o = HttpContext.Current.Cache.Get(key);
            return o == null ? null : Nistec.Serialization.JsonSerializer.Serialize(o, true);
        }

        public static object Get(string key)
        {
            if (key == null)
                return null;
            return HttpContext.Current.Cache.Get(key);
        }
        public static T Get<T>(string key)
        {
            if (key == null)
                return default(T);
            object o = HttpContext.Current.Cache.Get(key);
            if (o == null)
                return GenericTypes.Default<T>();
            return GenericTypes.Cast<T>(o);
        }

        public static string GetOrCreateJson(string key, Func<string> function, int expirationMinutes = 0)
        {
            string instance = null;

            if (key == null)
                return function();
            if (EnableCache)
            {
                instance = GetJson(key);
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

                    if (instance != null && instance.Count>0)
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
                    HttpContext.Current.Cache.Insert(key, value, null, DateTime.Now.AddMinutes(CacheTimeout), TimeSpan.Zero, CacheItemPriority.Default, null);
                else
                    HttpContext.Current.Cache.Insert(key, value, null, DateTime.Now.AddMinutes(expirationMinutes), TimeSpan.Zero, CacheItemPriority.Default, null);
            }
        }

        //public static void Insert(string key, object value)
        //{
        //    HttpContext.Current.Cache.Insert(key, value, null, DateTime.Now.AddMinutes(CacheTimeout), TimeSpan.Zero, CacheItemPriority.Default, null);
        //}
        public static void Remove(string key)
        {
            if (key != null)
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }
        public static void Remove(IList<string> keys)
        {
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    HttpContext.Current.Cache.Remove(key);
                }
            }
        }

        public static void Remove(string LibName, string GroupName, int AccountId = 0, int UserId = 0, string EntityName = null, string Args = null)
        {
            IList<string> keys = FindKeys(LibName, GroupName, AccountId, UserId ,  EntityName , Args);
            Remove(keys);
        }

        public static int Count()
        {
            return HttpContext.Current.Cache.Count;
        }
        #endregion

        public static IList<string> FindKeys(string LibName, string GroupName, int AccountId = 0, int UserId = 0, string EntityName = null, string Args = null)
        {
            string keylib = string.Format("{0}={1}", Fields.LibName, LibName);
            string keygroup = (GroupName != null) ? string.Format("{0}={1}", Fields.GroupName, GroupName) : null;
            string keyaid = (AccountId > 0) ? string.Format("{0}={1}", Fields.AccountId, AccountId) : null;
            string keyuid = (UserId > 0) ? string.Format("{0}={1}", Fields.UserId, UserId) : null;
            string keymap = (EntityName != null) ? string.Format("{0}={1}", Fields.EntityName, EntityName) : null;
            string keyarg = (Args != null) ? string.Format("{0}={1}", Fields.Args, Args) : null;
            IList<string> keys;

            keys = (from System.Collections.DictionaryEntry dict in HttpContext.Current.Cache
                    let key = dict.Key.ToString()
                    where key.Contains(keylib)
                        && (keygroup != null) ? key.Contains(keygroup) : true
                        && (keyaid != null) ? key.Contains(keyaid) : true
                        && (keyuid != null) ? key.Contains(keyuid) : true
                        && (keymap != null) ? key.Contains(keymap) : true
                    select key).ToList();
            return keys;
        }
        public static void ClearKeys(string LibName, string GroupName, int AccountId, int UserId, string EntityName, string Args)
        {
            IList<string> keys = FindKeys(LibName, GroupName, AccountId, UserId, EntityName, Args);
            foreach (var key in keys)
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        //public static IList<string> FindKeys(string searchValue, bool searchStartsWith)
        //{
        //    IList<string> keys;

        //    if (searchStartsWith)
        //        keys = (from System.Collections.DictionaryEntry dict in HttpContext.Current.Cache
        //                let key = dict.Key.ToString()
        //                where key.StartsWith(searchValue)
        //                select key).ToList();
        //    else
        //        keys = (from System.Collections.DictionaryEntry dict in HttpContext.Current.Cache
        //                let key = dict.Key.ToString()
        //                where key.Contains(searchValue)
        //                select key).ToList();
        //    return keys;
        //}

        //public static void ClearKeys(string searchValue, bool searchStartsWith)
        //{
        //    IList<string> keys = FindKeys(searchValue, searchStartsWith);
        //    foreach (var key in keys)
        //    {
        //        HttpContext.Current.Cache.Remove(key);
        //    }
        //}

        public static bool EnableCache
        {
            get
            {
                return EntityConfig.Settings.EntityCache.Enable;
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

    }
}