using Nistec.Data;
using Nistec.Data.Entities;
using Nistec.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nistec.Web.Cms
{

    [Entity("CmsSite", EntityMode.Config)]
    public class CmsSiteContext : EntityContext<CmsSite>
    {
        #region ctor


        public CmsSiteContext()
        {

        }

        public CmsSiteContext(int SiteId)
            : base(SiteId)
        {

        }

        #endregion
    }

    public class CmsSiteView : IEntityItem
    {
        public const string MappingName = "web_Cms_Sites";

        public static IList<CmsSiteView> View()
        {
            IList<CmsSiteView> sites = null;
            using (CmsSiteContext context = new CmsSiteContext())
            {
                sites = context.EntityDb.EntityList<CmsSiteView>("select SiteId,SiteName from " + MappingName, null, System.Data.CommandType.Text);

            }
            if (sites == null)
            {
                throw new Exception("CmsSiteView, Site not found");
            }

            return sites;
        }
         [EntityProperty(EntityPropertyType.Identity)]
        public int SiteId { get; set; }
        [EntityProperty]
        public string SiteName { get; set; }
    }

    public class CmsSite : IEntityItem
    {
        public const string MappingName = "web_Cms_Sites";

        public static IList<CmsItem> ViewPage(int PageId)
        {
            if (PageId <= 0)
            {
                throw new Exception("ViewPage error,null PageId");
            }
            IList<CmsItem> page = null;
            using (CmsSiteContext context = new CmsSiteContext())
            {
                page = context.EntityDb.EntityList<CmsItem>("select * from web_Cms_Items where PageId=@PageId", DataParameter.GetSql("PageId", PageId), System.Data.CommandType.Text);

            }
            if (page == null)
            {
                throw new Exception("ViewPage, Site or page not found");
            }

            return page;
        }

        public static IList<CmsSite> ViewAll()
        {
            IList<CmsSite> sites = null;
            using (CmsSiteContext context = new CmsSiteContext())
            {
                object[] prm = null;
                sites = context.EntityList(prm);
            }
            if (sites == null)
            {
                throw new Exception("CmsSite, Site not found");
            }

            return sites;
        }

        #region static get

  
        public static CmsSite GetBySiteName(string SiteName)
        {
            if (string.IsNullOrEmpty(SiteName))
            {
                throw new Exception("CmsSite error,null SiteName");
            }
            CmsSite site = null;
            using (CmsSiteContext context = new CmsSiteContext())
            {
                site = context.EntityDb.QuerySingle<CmsSite>("SiteName", SiteName);
            }
            if (site == null)
            {
                throw new Exception("CmsSite, SiteName not found");
            }

            return site;
        }
        public static CmsSite Get(int SiteId)
        {
            if (SiteId <= 0)
            {
                throw new Exception("CmsSite error,null SiteId");
            }
            CmsSite site = null;
            using (CmsSiteContext context = new CmsSiteContext(SiteId))
            {
                site = context.Entity;
            }
            if (site == null)
            {
                throw new Exception("CmsSite, Site not found");
            }

            return site;
        }
        #endregion

        public int Update(CmsSite newItem)
        {
            int res = 0;
            try
            {
                using (CmsSiteContext context = new CmsSiteContext())
                {
                    context.Set(this);
                    res = context.SaveChanges(newItem, UpdateCommandType.Update);
                    return res;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int Delete()
        {
            int res = 0;
            try
            {
                using (CmsSiteContext context = new CmsSiteContext())
                {

                    res = context.EntityDb.DoCommand<int>("delete from " + MappingName + " where SiteId=@SiteId", DataParameter.GetSql("SiteId", SiteId), System.Data.CommandType.Text);
                    return res;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int Insert()
        {
            using (CmsSiteContext context = new CmsSiteContext())
            {
                return context.SaveChanges(this, UpdateCommandType.Insert);
            }
        }

        
        [EntityProperty(EntityPropertyType.Identity)]
        public int SiteId { get; set; }
        [EntityProperty]
        public string SiteName { get; set; }
        [EntityProperty]
        public int SiteCategory { get; set; }
        [EntityProperty]
        public int AccountId { get; set; }
        [EntityProperty]
        public int Platform { get; set; }
        [EntityProperty]
        public string Header { get; set; }
        [EntityProperty]
        public string Footer { get; set; }
        [EntityProperty]
        public string SiteTitle { get; set; }
        [EntityProperty]
        public string Description { get; set; }
        [EntityProperty]
        public string Keywords { get; set; }
        [EntityProperty]
        public DateTime Expiration { get; set; }
        [EntityProperty]
        public DateTime Creation { get; set; }
        [EntityProperty]
        public bool RemovePowerBy { get; set; }

    }
}
