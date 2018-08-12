using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Nistec.Generic;
using System.Web;
using Nistec;
using Nistec.Runtime;

namespace Nistec.Web.Asp
{
    public class CookieItem
    {
        const string TagCulture = "Culture";
 
        public static string CookieName { get { return NetConfig.AppSettings["CookieName"]; } }
        public static string DefaultCulture { get { return Types.NzOr(NetConfig.AppSettings["DefaultCulture"], "en"); } }

        public CookieItem()
        {
            Culture = DefaultCulture;
        }
 
        public string Culture { get; set; }

        public bool IsRtl 
        { 
            get
            {
                switch (Culture)
                {
                    case "he":
                        return true;
                    default:
                        return false;
                }
            } 
        }

        public static CookieItem Get(Page p)
        {
            if (p.Request.Browser.Cookies)
            {
                if (p.Request.Cookies[CookieItem.CookieName] != null)
                {
                    return VerifyCookie(p.Request.Cookies[CookieItem.CookieName]);
                }
            }
            return new CookieItem();
        }

        public static CookieItem Get(HttpRequest request)
        {
            if (request.Browser.Cookies)
            {
                if (request.Cookies[CookieItem.CookieName] != null)
                {
                    return VerifyCookie(request.Cookies[CookieItem.CookieName]);
                }
            }
            return new CookieItem();
        }

        public static void Set(Page p, CookieItem item)
        {
           
            HttpCookie cooki = new HttpCookie(CookieName);

            cooki[TagCulture] = item.Culture;
            cooki.Expires = DateTime.Now.AddDays(60);
            p.Response.Cookies.Add(cooki);
        }


        public static CookieItem VerifyCookie(HttpCookie cookies)
        {
            if (cookies == null || cookies.Values.Count == 0)
                return null;
            //if (cookies.Expires < DateTime.Now)
            //    return false;
            string culture = Encryption.DecryptPass(string.Format("{0}", cookies[TagCulture]));

            return new CookieItem() { Culture=culture };
         }

    }
}
