using Nistec.Data;
using Nistec.Data.Entities;
using Nistec.Web.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Security
{
    public enum PermsValue
    {
        None = 0,           // לא מורשה
        Read = 1,           // קריאה בלבד
        Write = 3,          // קריאה ועריכה
        Add = 5,            // קריאה והוספה
        Modify = 7,         // עריכה מלאה
        FullControl = 15,   // שליטה מלאה
    }

    [Flags]
    public enum PermsLevel
    {
        //DenyAll = 0,
        //ReadOnly = 1,
        //EditOnly = 2,
        //FullControl = 3

        None = 0,   // לא מורשה
        Read = 1,   // קריאה
        Write = 2,  // כתיבה
        Append = 4, // הוספה
        Delete = 8, // מחיקה

        //PermsView	0	None    לא מורשה
        //PermsView	1	Read    קריאה בלבד
        //PermsView	3	ReadWrite   קריאה ועריכה
        //PermsView	5	ReadAdd קריאה והוספה
        //PermsView	7	Modify  עריכה מלאה
        //PermsView	15	Full Control    שליטה מלאה

    }


    [Entity("UserPerms", EntityMode.Config)]
    public class PermsContext : EntityContext<PermsItem>
    {

        #region ctor


        public PermsContext(string connectionName)
            : base()
        {
        }

        public PermsContext(int UserId)
            : base(UserId)
        {

        }

        public PermsContext(PermsItem item)
            : base(item)
        {

        }
        protected PermsContext()
            : base()
        {
        }

        internal static PermsItem Instance
        {
            get { return new PermsItem(); }
        }
        #endregion

        #region binding

        protected override void EntityBind()
        {
            //base.EntityDb = new EntityDb();
            //base.EntityDb.EntityCulture = Nistec.Data.DB.NetcellDB.GetCulture();
            //If EntityAttribute not define you can initilaize the entity here
            //base.InitEntity<AdventureWorks>("Contact", "Person.Contact", EntityKeys.Get("ContactID"));
        }

        #endregion

       public static IEnumerable<PermsItem> GetPerms(int UserId, int AppId)
        {
            using (PermsContext context = new PermsContext())
            {
               return context.EntityDb.Query<PermsItem>("UserId", UserId, "AppId", AppId , "Mode", 0);
            }
        }

        public static IDictionary<string,int> GetPermsDictionary(int UserId, int AppId)
        {
            using (PermsContext context = new PermsContext())
            {
                return context.EntityDb.QueryDictionary<int>("Item", "Perms", "UserId", UserId, "AppId", AppId, "Mode", 1);
            }

        }

        public static IDictionary<string, int> PermsCache(string LibName,int UserId, int AppId, bool EnableCache, int expirationMinutes)
        {
            //bool EnableCache = true;
            IDictionary<string, int> instance = null;
            string key = WebCache.GetKey(LibName, "Perms", 0, UserId, "PermsItem");
            if (key == null)
                return instance;
            if (EnableCache)
            {
                instance = WebCache.Get<Dictionary<string, int>>(key);
                if (instance == null)
                {
                    instance = GetPermsDictionary(UserId, AppId);

                    if (instance != null)
                    {
                        WebCache.Insert(key, instance, expirationMinutes);
                    }
                }
                else
                {
                    return instance;
                }
            }
            else
            {
                instance = GetPermsDictionary(UserId, AppId);
            }
            return instance;
        }

        public static PermsValue LookupPerms(string LibName, int UserId, int AppId, bool EnableCache, int expirationMinutes, string item, string field="*")
        {
            int perms=0;
            IDictionary<string, int> permsItems=null;

            if (EnableCache)
                permsItems = PermsCache(LibName, UserId, AppId, EnableCache, expirationMinutes);

            if (permsItems != null) {
                permsItems.TryGetValue(item + "." + field, out perms);
            }

            return (PermsValue)perms;
        }

        public static int Query(int UserId, string ItemName, string ItemField="*")
        {
            using (PermsContext context = new PermsContext())
            {
                return context.ExecuteCommand<int>("sp_Ad_Perms_Query", DataParameter.GetSql("UserId", UserId, "ItemName", ItemName, "ItemField", ItemField), System.Data.CommandType.StoredProcedure);
            }
        }
    }

    public class PermsItem
    {
        [EntityProperty(EntityPropertyType.Key)]
        public int UserId { get; set; }
        [EntityProperty(EntityPropertyType.Key)]
        public string ItemName { get; set; }
        [EntityProperty(EntityPropertyType.Key)]
        public string ItemField { get; set; }
        public int Perms { get; set; }
    }


    public class Permission
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public IEnumerable<Permission> ImpliedBy { get; set; }
        public bool RequiresOwnership { get; set; }

        public static Permission Named(string name)
        {
            return new Permission { Name = name };
        }
    }
}
