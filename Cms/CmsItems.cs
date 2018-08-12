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

    [Entity("CmsItem", "web_Cms_Items", "CmsSite", Data.Entities.EntitySourceType.Table, "PageId,ItemId")]
    public class CmsItemContext : EntityContext<CmsItem>
    {
        #region ctor


        public CmsItemContext()
        {

        }

        public CmsItemContext(int PageId,string ItemId)
            : base(PageId,ItemId)
        {

        }

        #endregion
    }
    
    public class CmsItem : IEntityItem
    {
        public const string MappingName = "web_Cms_Items";

        //public static List<CmsItem> ViewPage(int PageId)
        //{
        //    if (PageId <= 0)
        //    {
        //        throw new Exception("ViewPage error,null PageId");
        //    }
        //    List<CmsItem> page = null;
        //    using (CmsItemContext context = new CmsItemContext())
        //    {
        //        page = context.EntityList(DataFilter.GetSql("SiteId=@SiteId", PageId));

        //    }
        //    if (page == null)
        //    {
        //        throw new Exception("ViewPage, Site or page not found");
        //    }

        //    return page;
        //}

        public static IList<CmsItem> ViewPage(int SiteId, int PageId)
        {
            if (SiteId <= 0 || PageId <= 0)
            {
                throw new Exception("ViewPage error,null SiteId or PageId");
            }
            IList<CmsItem> page = null;
            using (CmsItemContext context = new CmsItemContext())
            {
                page = context.EntityDb.Context().Query<CmsItem>("select * from webvw_Cms_PageItems where SiteId=@SiteId and PageId=@PageId", "SiteId", SiteId, "PageId", PageId);
            }
            //if (page == null)
            //{
            //    throw new Exception("ViewPage, Site or page not found");
            //}

            return page;
        }

        public static IList<CmsItem> ViewPage(int SiteId, string PageName)
        {
            if (SiteId <= 0 || string.IsNullOrEmpty(PageName))
            {
                throw new Exception("ViewPage error,null SiteId or PageName");
            }
            IList<CmsItem> page = null;
            using (CmsItemContext context = new CmsItemContext())
            {
                page = context.EntityDb.Context().Query<CmsItem>("select * from webvw_Cms_PageItems where SiteId=@SiteId and PageName=@PageName", "SiteId", SiteId, "PageName", PageName);
            }
            //if (page == null)
            //{
            //    throw new Exception("ViewPage, Site or page not found");
            //}

            return page;
        }

        public static IList<string> ViewSectionsGroup(int PageId)
        {
            if (PageId <= 0)
            {
                throw new Exception("ViewPage error,null PageId");
            }
            IList<string> list = null;
            using (CmsItemContext context = new CmsItemContext())
            {
                list = context.EntityDb.Context().Query<string>("select section from web_Cms_Items where PageId=@PageId group by section", "PageId", PageId);
            }
            list.Add("-All-");
            return list;
        }

        #region static get

        public static CmsItem Get(int PageId,string ItemId)
        {
            if (ItemId == null || ItemId=="")
            {
                throw new Exception("CmsItem error,null ItemId");
            }
            if (PageId <=0)
            {
                throw new Exception("CmsItem error,zero PageId");
            }
            CmsItem sec = null;
            using (CmsItemContext context = new CmsItemContext(PageId,ItemId))
            {
                sec = context.Entity;
            }
            if (sec == null)
            {
                throw new Exception("CmsItem, Item not found");
            }

            return sec;
        }
        public static int Update(int PageId, string ItemId, string Section,string ItemContent, string ItemType, string ItemAttr)
        {
            int res = 0;
            try
            {
                using (CmsItemContext context = new CmsItemContext(PageId,ItemId))
                {
                    context.Entity.ItemContent = ItemContent;
                    context.Entity.ItemAttr = ItemAttr;
                    //context.Entity.ItemType = ItemType;

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
       

        public int Update(CmsItem newItem)
        {
            int res = 0;
            try
            {
                using (CmsItemContext context = new CmsItemContext())
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
                using (CmsItemContext context = new CmsItemContext())
                {

                    res = context.EntityDb.DoCommand<int>("delete from " + MappingName + " where PageId=@PageId and ItemId=@ItemId", DataParameter.GetSql("PageId",PageId,"ItemId", ItemId), System.Data.CommandType.Text);
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
            using (CmsItemContext context = new CmsItemContext())
            {
                return context.SaveChanges(this, UpdateCommandType.Insert);
            }
        }


        [EntityProperty(EntityPropertyType.Key)]
        public string ItemId { get; set; }
        [EntityProperty(EntityPropertyType.Key)]
        public int PageId { get; set; }
        [EntityProperty]
        public string Section { get; set; }
        [EntityProperty]//html|json|link|image|text|input|style
        public string ItemType { get; set; }
        [EntityProperty]
        public string ItemAttr { get; set; }
        [EntityProperty]
        public string ItemContent { get; set; }

        public static CmsItem CreateItem(int PageId, string key)
        {
            //cms_#-Section-ItemType-tagType
            string itemId, itemType, section, attr;
            string[] args = key.Split('-');
            int length = args.Length;
            itemId = key;
            section = length > 1 ? args[1] : "";
            itemType = length > 2 ? args[2] : "html";
            //tag = length > 3 ? args[3] : "";
            attr = null;

            //if (tag == "a")
            //{
            //    attr = "#";
            //}

            CmsItem item = new CmsItem()
            {
                ItemId = itemId,
                ItemType = itemType,
                PageId = PageId,
                Section = section,
                ItemAttr = attr
            };
            return item;
        }

        public static void DoCreateItems(int PageId, string filename)
        {
            Console.WriteLine("Items preaper to create");
            Dictionary<string, CmsItem> Items = new Dictionary<string, CmsItem>();

            string Text = Nistec.IoHelper.ReadFileStream(filename);
            int count = 0;
            string[] list = Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in list)
            {
                string[] wordlist = s.Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in wordlist)
                {
                    if (word.Length > 4 && word.Substring(0, 4) == "cms_")
                    {
                        var item=CreateItem(PageId,word);
                        Items.Add(word,item);
                        Console.WriteLine(word);
                        count++;
                    }
                }
            }
            Console.WriteLine("Items count {0}", count);

            if (Items.Count > 0)
            {
                Console.WriteLine("Items preaper to insert");

                foreach (var item in Items.Values)
                {
                    item.Insert();
                }
                Console.WriteLine("Items inserted");
            }

        }

    }
}
