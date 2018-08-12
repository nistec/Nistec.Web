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

    [Entity("CmsMenu", "web_Cms_Menu", "CmsSite", Data.Entities.EntitySourceType.Table, "SiteId,MenuId")]
    public class CmsMenuContext : EntityContext<CmsMenu>
    {
        #region ctor


        public CmsMenuContext()
        {

        }

        public CmsMenuContext(int SiteId, string MenuId)
            : base(SiteId,MenuId)
        {

        }

        #endregion
    }
    
    public class CmsMenu : IEntityItem
    {
        public const string MappingName = "web_Cms_Menu";

        public static List<CmsMenu> View(int SiteId)
        {
            if (SiteId <= 0)
            {
                throw new Exception("ViewPage error,null SiteId");
            }
            List<CmsMenu> list = null;
            using (CmsMenuContext context = new CmsMenuContext())
            {
                list = context.EntityList(DataFilter.GetSql("SiteId=@SiteId", SiteId));

            }
            //if (list == null)
            //{
            //    throw new Exception("ViewPage, Site or page not found");
            //}

            return list;
        }

        public static IList<CmsMenu> View(int SiteId, string MenuType)
        {
            if (SiteId <= 0 || string.IsNullOrEmpty(MenuType))
            {
                throw new Exception("ViewPage error,null SiteId or MenuType");
            }
            IList<CmsMenu> list = null;
            using (CmsMenuContext context = new CmsMenuContext())
            {
                list = context.EntityDb.Context().Query<CmsMenu>("select * from web_Cms_Menu where SiteId=@SiteId and MenuType=@MenuType", "SiteId", SiteId, "MenuType", MenuType);
            }
            //if (page == null)
            //{
            //    throw new Exception("ViewPage, Site or page not found");
            //}

            return list;
        }

        #region static get

        public static CmsMenu Get(int SiteId,string MenuId)
        {
            if (MenuId == null || MenuId=="")
            {
                throw new Exception("CmsMenu error,null MenuId");
            }
            if (SiteId <=0)
            {
                throw new Exception("CmsMenu error,zero SiteId");
            }
            CmsMenu sec = null;
            using (CmsMenuContext context = new CmsMenuContext(SiteId,MenuId))
            {
                sec = context.Entity;
            }
            if (sec == null)
            {
                throw new Exception("CmsMenu, Menu not found");
            }

            return sec;
        }
        public static int Update(int SiteId, string MenuId, string MenuTitle, string MenuType)
        {
            int res = 0;
            try
            {
                using (CmsMenuContext context = new CmsMenuContext(SiteId,MenuId))
                {
                    context.Entity.MenuTitle = MenuTitle;
                    context.Entity.MenuType = MenuType;
                    res = context.SaveChanges(UpdateCommandType.Update);
                    return res;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
        #endregion
       

        public int Update(CmsMenu newItem)
        {
            int res = 0;
            try
            {
                using (CmsMenuContext context = new CmsMenuContext())
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
                using (CmsMenuContext context = new CmsMenuContext())
                {

                    res = context.EntityDb.DoCommand<int>("delete from " + MappingName + " where SiteId=@SiteId and MenuId=@MenuId", DataParameter.GetSql("SiteId",SiteId,"MenuId", MenuId), System.Data.CommandType.Text);
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
            using (CmsMenuContext context = new CmsMenuContext())
            {
                return context.SaveChanges(this, UpdateCommandType.Insert);
            }
        }


        [EntityProperty(EntityPropertyType.Key)]
        public int SiteId { get; set; }
        [EntityProperty(EntityPropertyType.Key)]
        public string MenuId { get; set; }
        [EntityProperty]
        public string MenuType { get; set; }
        [EntityProperty]
        public string MenuTitle { get; set; }
       

    }
}
