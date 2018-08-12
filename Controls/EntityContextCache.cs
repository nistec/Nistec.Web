using Nistec.Data;
using Nistec.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Controls
{
    public class EntityContextCache<Dbc,T> : EntityContext<Dbc, T> 
        where Dbc : IDbContext
        where T : IEntityItem
    {

        //public string CaheKey{get;protected set;}
        public string EntityCacheGroups{get;set;}
        public string LibName { get; set; }
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string Args { get; set; }

        //public static void Refresh(int AccountId)
        //{
        //    DbContextCache.Remove<T>(Settings.ProjectName, EntityCacheGroups.System, AccountId, 0);
        //}

        public EntityContextCache(string LibName, string GroupName, int AccountId, int UserId)
        {
            this.AccountId = AccountId;
            this.UserId = UserId;
            this.EntityCacheGroups = GroupName;
            this.LibName = LibName;

            string mapping = EntityMappingAttribute.Name<T>();
            if (mapping != null)
            {
                CacheKey = WebCache.GetKey(LibName, GroupName, AccountId, UserId, mapping);
            }
        }
        public EntityContextCache(string LibName, string GroupName, int AccountId, int UserId, string args) 
        {
            this.AccountId = AccountId;
            this.UserId = UserId;
            this.EntityCacheGroups = GroupName;
            this.LibName = LibName;

            string mapping = EntityMappingAttribute.Name<T>();
            if (mapping != null)
            {
                CacheKey=WebCache.GetKey(LibName, GroupName, AccountId, UserId, mapping + args);
            }
        }

        public EntityContextCache()
        {
            //no cache
        }
       
        public IList<T> ExecList(params object[] keyValueParameters)
        {
            return DbContextCache.ExecuteList<Dbc, T>(CacheKey, keyValueParameters);
        }
        public IList<T> GetList(object[] keyValueParameters)
        {
            return DbContextCache.EntityList<Dbc, T>(CacheKey, keyValueParameters);
        }
        public IList<T> GetListByAccount()
        {
            return DbContextCache.EntityList<Dbc, T>(CacheKey, new object[] { "AccountId", AccountId });
        }
        protected override void OnChanged(ProcedureType commandType)
        {
            DbContextCache.Remove(CacheKey);
        }
        public FormResult GetFormResult(EntityCommandResult res, string reason)
        {
            return FormResult.Get(res, this.EntityName, reason);
        }

    }
}
