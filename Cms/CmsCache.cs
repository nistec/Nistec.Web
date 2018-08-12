using Nistec.Channels;
using Nistec.Channels.RemoteCache;
using Nistec.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nistec.Web.Cms
{
    public class CmsCache
    {
        //int DefaultExpiration = 120;
                
        public static bool EnableCache
        {
            get 
            { 
                return CmsConfig.GetConfig().CmsSettings.CacheEnable;//.Get<bool>("CmsCacheEnable"); 
            }
        }

        static int _CacheTimeout;
        public static int CacheTimeout
        {
            get 
            {

                if (_CacheTimeout == 0)
                {
                    _CacheTimeout = CmsConfig.GetConfig().CmsSettings.CacheTimeout;// NetConfig.Get<int>("CmsCacheTimeout", 30);
                }
                return _CacheTimeout;
            }
        }

        static NetProtocol _CacheProtocol=0;
        public static NetProtocol CacheProtocol
        {
            get
            {

                if (_CacheProtocol == 0)
                {
                    var protocl = CmsConfig.GetConfig().CmsSettings.CacheProtocol;// NetConfig.Get<string>("CmsCacheProtocol", "");
                    if (protocl == "tcp")
                        _CacheProtocol = NetProtocol.Tcp;
                    else
                        _CacheProtocol = NetProtocol.Pipe;
                }
                return _CacheProtocol;
            }
        }

        public static string KeyPage(int SiteId, int PageId)
        {
            string key = string.Format("Nistec.Web.Cms.Page:{0}_{1}", SiteId, PageId);
            return key;
        }

        public static string KeySiteMenu(int SiteId)
        {
            string key = string.Format("Nistec.Web.Cms.SiteMenu:{0}", SiteId);
            return key;
        }

        #region Cms View

        public static IList<CmsItem> ViewPageItems(int SiteId, int PageId)
        {
            string key = KeyPage(SiteId, PageId);
            IList<CmsItem> list = null;

            if (EnableCache)
                list = (IList<CmsItem>)RemoteCacheApi.Get(CacheProtocol).Get<List<CmsItem>>(key);
            if (list == null)
            {
                list = CmsItem.ViewPage(SiteId, PageId);
                if (EnableCache && list != null)
                {
                    RemoteCacheApi.Get(CacheProtocol).Add(key, list, CacheTimeout);
                }
            }

            return list;
        }

        public static CmsPage ViewPage(int SiteId, int PageId)
        {
            CmsPage item = null;
            string key = KeyPage(SiteId, PageId);

            if (EnableCache)
                item = (CmsPage)RemoteCacheApi.Get(CacheProtocol).Get<CmsPage>(key);
            if (item == null)
            {
                item = Nistec.Web.Cms.CmsPage.View(SiteId, PageId);
                if (EnableCache && item != null)
                {
                    RemoteCacheApi.Get(CacheProtocol).Add(key, item, CacheTimeout);
                }
            }

            return item;
        }

        public static List<CmsPage> ViewCmsMenu(int SiteId)
        {
            List<CmsPage> list = null;

            string key = KeySiteMenu(SiteId);

            if (EnableCache)
                list = (List<CmsPage>)RemoteCacheApi.Get(CacheProtocol).Get<List<CmsPage>>(key);
            if (list == null)
            {
                list = Nistec.Web.Cms.CmsPage.ViewPages(SiteId);
                if (EnableCache && list != null)
                {
                    RemoteCacheApi.Get(CacheProtocol).Add(key, list, CacheTimeout);
                }
            }

            return list;
        }

        #endregion

        #region Cms Update

        public static int CmsItemUpdate(int SiteId, int PageId, string ItemId, string Section, string ItemContent, string ItemType, string ItemAttr)
        {
            int result = 0;
            if (SiteId>0 && PageId > 0)
            {
                string key = KeyPage(SiteId, PageId);
                result = CmsItem.Update(PageId, ItemId, Section, ItemContent, ItemType, ItemAttr);
                RemoteCacheApi.Get(CmsConfig.CacheProtocol).Remove(key);
            }
            return result;
        }

        public static int CmsPageUpdate(int SiteId, int PageId, string PageName, int PageCategory, string TopMenu, string FooterMenu, string PageContent)
        {
            int result = 0;
            if (SiteId > 0 && PageId > 0)
            {
                string key = KeyPage(SiteId, PageId);
                var page = Nistec.Web.Cms.CmsPage.Get(PageId);
                result = page.Update(new Nistec.Web.Cms.CmsPage() { SiteId = SiteId, PageId = PageId, PageContent = PageContent, PageName = PageName, PageCategory = PageCategory, TopMenu = TopMenu, FooterMenu = FooterMenu });
                RemoteCacheApi.Get(CmsConfig.CacheProtocol).Remove(key);
            }
            return result;
        }

        public static int CmsPageAdd(int SiteId, int PageId, string PageName, int PageCategory, string TopMenu, string FooterMenu, string PageContent)
        {
            int result = 0;
            if (SiteId > 0 && PageId > 0)
            {
                string key = KeyPage(SiteId, PageId);
                var page = new Nistec.Web.Cms.CmsPage() { SiteId = SiteId, PageId = PageId, PageContent = PageContent, PageName = PageName, PageCategory = PageCategory, TopMenu = TopMenu, FooterMenu = FooterMenu };
                result = page.Insert();
                string keyMenu = KeySiteMenu(SiteId);
                RemoteCacheApi.Get(CmsConfig.CacheProtocol).Add(key, page, CacheTimeout);
            }
            return result;
        }

        public static int CmsPageRemove(int SiteId, int PageId)
        {
            int result = 0;
            if (SiteId > 0 && PageId > 0)
            {
                string key = KeyPage(SiteId, PageId);
                result = Nistec.Web.Cms.CmsPage.DeletePage(SiteId, PageId);
                string keyMenu = KeySiteMenu(SiteId);
                RemoteCacheApi.Get(CmsConfig.CacheProtocol).Remove(key);
            }
            return result;
        }
        #endregion


        public static T ViewItem<T>(string key, Func<T> action)
        {
            T item = default(T);

            if (EnableCache)
                item = RemoteCacheApi.Get(CacheProtocol).Get<T>(key);
            if (item == null)
            {
                item=action();
                if (EnableCache && item != null)
                {
                    RemoteCacheApi.Get(CacheProtocol).Add(key, item, CacheTimeout);
                }
            }

            return item;
        }

        public static int UpdateItem(string key, Func<int> action)
        {
            int result = 0;
            if (action == null)
                return -1;

            result = action();
            RemoteCacheApi.Get(CacheProtocol).Remove(key);

            return result;
        }

    }
}
