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
    [Entity("CmsPage", "web_Cms_Pages", "CmsSite", Data.Entities.EntitySourceType.Table, "PageId")]
    public class CmsPageContext : EntityContext<CmsPage>
    {
        #region ctor


        public CmsPageContext()
        {

        }

        public CmsPageContext(int PageId)
            : base(PageId)
        {

        }

        #endregion
    }
    
    public class CmsPage : IEntityItem
    {
        public const string MappingName = "web_Cms_Pages";

        #region static get

        public static List<CmsPage> ViewPages(int SiteId)
        {
            if (SiteId <= 0)
            {
                throw new Exception("ViewPages error,null SiteId");
            }
            List<CmsPage> pages = null;
            using (CmsPageContext context = new CmsPageContext())
            {
                pages = context.EntityList(DataFilter.GetSql("SiteId=@SiteId", SiteId));

            }
            if (pages == null)
            {
                throw new Exception("ViewPage, Site or page not found");
            }

            return pages;
        }

        public static CmsPage View(int SiteId,int PageId)
        {
            if (PageId <= 0)
            {
                throw new Exception("CmsPage error,null PageId");
            }
            CmsPage page = null;
            using (CmsPageContext context = new CmsPageContext())
            {
                page = context.EntityDb.EntityFilter<CmsPage>(DataFilter.GetSql("SiteId=@SiteId and PageId=@PageId", SiteId,PageId));
            }
            //if (page == null)
            //{
            //    throw new Exception("CmsPage, Site or page not found");
            //}

            return page;
        }

        public static CmsPage Get(int PageId)
        {
            if (PageId <= 0)
            {
                throw new Exception("CmsPage error,null PageId");
            }
            CmsPage page = null;
            using (CmsPageContext context = new CmsPageContext())
            {
                page = context.EntityDb.EntityFilter<CmsPage>(DataFilter.GetSql("PageId=@PageId", PageId));
            }
            if (page == null)
            {
                throw new Exception("CmsPage, Site or page not found");
            }

            return page;
        }

        public static string LookupPageName(int PageId)
        {
            if (PageId <= 0)
            {
                return "";
            }
            using (CmsSiteContext context = new CmsSiteContext())
            {
                return context.EntityDb.Context().QueryScalar<string>("select PageName from " + MappingName + " where PageId=@PageId", "", "PageId", PageId);
            }
        }

        public static int DeletePage(int SiteId,int PageId)
        {
            int res = 0;
            try
            {
                using (CmsPageContext context = new CmsPageContext())
                {
                    res = context.EntityDb.DoCommand<int>("delete from " + MappingName + " where SiteId=@SiteId and PageId=@PageId", DataParameter.GetSql("SiteId",SiteId,"PageId", PageId), System.Data.CommandType.Text);
                    return res;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        public int Update(CmsPage item)
        {
            int res = 0;
            try
            {
                using (CmsPageContext context = new CmsPageContext())
                {
                    context.Set(this);
                    res = context.SaveChanges(item, UpdateCommandType.Update);
                    return res;
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return -1;
            }
        }

        public int Delete()
        {
            int res = 0;
            try
            {
                using (CmsPageContext context = new CmsPageContext())
                {

                    res = context.EntityDb.DoCommand<int>("delete from " + MappingName + " where PageId=@PageId", DataParameter.GetSql("PageId", PageId), System.Data.CommandType.Text);
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
            using (CmsPageContext context = new CmsPageContext())
            {
                return context.SaveChanges(this, UpdateCommandType.Insert);
            }
        }


        [EntityProperty(EntityPropertyType.Identity)]
        public int PageId { get; set; }
        [EntityProperty]
        public int SiteId { get; set; }
        [EntityProperty]
        public string PageName { get; set; }
        [EntityProperty]
        public int PageCategory { get; set; }

        [EntityProperty]
        public string PageTitle { get; set; }
        [EntityProperty]
        public int PageState { get; set; }
        [EntityProperty]
        public string PageContent { get; set; }
        [EntityProperty]
        public string TopMenu { get; set; }
        [EntityProperty]
        public string FooterMenu { get; set; }


    }
}
