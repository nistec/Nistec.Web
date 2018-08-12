using Nistec.Data;
using Nistec.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Controls
{
    public class DbContextCache
    {
        public static string GetKey<T>(string LibName, string GroupName, int AccountId, int UserId) where T : IEntityItem
        {
            string mapping = EntityMappingAttribute.Name<T>();
            if (mapping == null)
            {
                return null;
            }
            return WebCache.GetKey(LibName, GroupName, AccountId, UserId, mapping);
        }
        public static string GetKey<T>(string LibName, string GroupName, int AccountId, int UserId, string args) where T : IEntityItem
        {
            string mapping = EntityMappingAttribute.Name<T>();
            if (mapping == null)
            {
                return null;
            }
            return WebCache.GetKey(LibName, GroupName, AccountId, UserId, mapping + args);
        }
        public static string GetKey<T>(string LibName, string GroupName, int UserId, object[] keyValueParameters, string excludeKeys) where T : IEntityItem
        {
            string args = DataParameter.ToQueryString(keyValueParameters, excludeKeys);
            string mapping = EntityMappingAttribute.Name<T>();
            if (mapping == null)
            {
                return null;
            }
            return WebCache.GetKey(LibName, GroupName, 0, UserId, mapping, args);
        }
        //public static string GetKey<T>(string LibName, string GroupName, int AccountId, int UserId, string EntityName)
        //{
        //    return string.Format("lib={0}_group={1}_accountid={2}_userid={3}_mapping={4}", LibName, GroupName, AccountId, UserId, EntityName).ToLower();
        //}
        public static void Remove<T>(string LibName, string GroupName, int AccountId=0, int UserId=0) where T : IEntityItem
        {
            string mapping = EntityMappingAttribute.Name<T>();
            if (mapping != null)
            {
                IList<string> keys = WebCache.FindKeys(LibName, GroupName, AccountId, UserId, mapping);
                if (keys != null)
                    WebCache.Remove(keys);
            }
        }
      
        public static void Remove(string key)
        {
            if (key != null)
                WebCache.Remove(key);
        }

        public static IList<T> ExecuteList<Dbc, T>(string key, object[] keyValueParameters)
            where Dbc : IDbContext
            where T : IEntityItem
        {
            var procName = EntityMappingAttribute.Proc<T>(ProcedureType.GetList);
            if (key == null)
                return DbContext.ExecuteList<Dbc, T>(procName, keyValueParameters);
            else
            {
                var map = EntityMappingAttribute.Get<T>();
                if (map == null || map.EnableCache == false || map.CacheTtl <= 0)
                    return DbContext.ExecuteList<Dbc, T>(procName,keyValueParameters);
                return WebCache.GetOrCreateList(key, () => DbContext.ExecuteList<Dbc, T>(procName,keyValueParameters), map.CacheTtl);
            }
        }
        public static IList<T> ExecuteList<Dbc, T>(string key, int ttl, object[] keyValueParameters)
            where Dbc : IDbContext
            where T : IEntityItem
        {
            var procName = EntityMappingAttribute.Proc<T>(ProcedureType.GetList);
            if (key == null)
                return DbContext.ExecuteList<Dbc, T>(procName, keyValueParameters);
            else
            {
                var map = EntityMappingAttribute.Get<T>();
                if (map == null || map.EnableCache == false || map.CacheTtl <= 0)
                    return DbContext.ExecuteList<Dbc, T>(procName, keyValueParameters);
                return WebCache.GetOrCreateList(key, () => DbContext.ExecuteList<Dbc, T>(procName, keyValueParameters), map.CacheTtl);
            }
        }

        public static IList<T> ExecOrViewList<Dbc, T>(string key, object[] keyValueParameters)
            where Dbc : IDbContext
            where T : IEntityItem
        {
            bool hasMap = true;
            string procName = null;
            var map = EntityMappingAttribute.Get<T>();

            if (map == null || map.EnableCache == false || map.CacheTtl <= 0)
                hasMap = false;
            else
                procName = map.ProcListView;

 
            if (key != null && hasMap)
            {
                if (procName != null)
                    return WebCache.GetOrCreateList(key, () => DbContext.ExecuteList<Dbc, T>(procName, keyValueParameters), map.CacheTtl);
                else
                    return WebCache.GetOrCreateList(key, () => DbContext.EntityList<Dbc, T>(keyValueParameters), map.CacheTtl);
            }
            else
            {
                if (procName != null)
                    return DbContext.ExecuteList<Dbc, T>(procName, keyValueParameters);
                else
                    return DbContext.EntityList<Dbc, T>(keyValueParameters);
            }
        }

        public static IList<T> EntityList<Dbc, T>(string key, object[] keyValueParameters)
            where Dbc : IDbContext
            where T : IEntityItem
        {
            if (key == null)
                return DbContext.EntityList<Dbc, T>(keyValueParameters);
            else
            {
                var map = EntityMappingAttribute.Get<T>();
                if (map == null || map.EnableCache == false || map.CacheTtl <= 0)
                    return DbContext.EntityList<Dbc, T>(keyValueParameters);
                return WebCache.GetOrCreateList(key, () => DbContext.EntityList<Dbc, T>(keyValueParameters), map.CacheTtl);
            }
        }

        public static IList<T> EntityList<Dbc, T>(string key, int ttl, object[] keyValueParameters)
            where Dbc : IDbContext
            where T : IEntityItem
        {
            if (key == null)
                return DbContext.EntityList<Dbc, T>(keyValueParameters);
            else
            {
                if (ttl < 0)
                    ttl = WebCache.CacheTimeout;
                return WebCache.GetOrCreateList(key, () => DbContext.EntityList<Dbc, T>(keyValueParameters), ttl);
            }
        }
        public static T EntityGet<Dbc, T>(string key, int ttl, object[] keyValueParameters)
            where Dbc : IDbContext
            where T : IEntityItem
        {
            if (key == null)
                return DbContext.EntityGet<Dbc, T>(keyValueParameters);
            else
            {
                if (ttl < 0)
                    ttl = WebCache.CacheTimeout;
                return WebCache.GetOrCreate(key, () => DbContext.EntityGet<Dbc, T>(keyValueParameters), ttl);
            }
        }
        public static T EntityGet<Dbc, T>(string key, object[] keyValueParameters)
            where Dbc : IDbContext
            where T : IEntityItem
        {
            if (key == null)
                return DbContext.EntityGet<Dbc, T>(keyValueParameters);
            else
            {
                var map = EntityMappingAttribute.Get<T>();
                if (map == null || map.EnableCache == false || map.CacheTtl <= 0)
                    return DbContext.EntityGet<Dbc, T>(keyValueParameters);
                return WebCache.GetOrCreate(key, () => DbContext.EntityGet<Dbc, T>(keyValueParameters), map.CacheTtl);
            }
        }
    }
}
